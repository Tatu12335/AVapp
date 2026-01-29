
using Antivirus.core.Classes.logs;
using AVcore.classes.abuse.ch_client;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace AVcore.classes
{
    public class FileScanner
    {
        public static FileScanner FileScannerInstance { get; } = new FileScanner();
        public bool IsDirectory { get; set; }

        // FOR storing file extensions, i plan on making a file that checks the magic bytes to determine the extension in the future!
        public static List<string> RiskyExtensions = new List<string>()
        {
            "*.exe", "*.com", ".*scr", "*.dll", "*.ocx", "*.sys", "*.msi", "*.cab", "*.appx", "*.bat", "*.ps1", "*.vbs", "*.js", "*.docm", "*.xlsm"
        };

        public async Task ScanFileAsync(string file)
        {
            logmsg.Instance.Log($"\n Proceeding with scanning <{file}>");

            try
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(file);
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Green;

                await hasher.GetHasher.asyncHash(file);

                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" Unexpeted error occured calling the hash function | ERROR : {ex.Message}");
                Console.ResetColor();
            }

            Console.ResetColor();
        }
        // iszip() might or might not be partially developed by github copilot ;), nah but on a more serious note i carefully look it through.
        public async Task IsZip(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            // Normalize and validate input
            path = Path.GetFullPath(path);

            if (!File.Exists(path))
            {
                //do nothing
                return;
            }

            const long MaxTotalUncompressed = 100_000_000; // 100 MB
            const int MaxEntries = 1000;
            const double MaxCompressionRatio = 100.0;

            try
            {
                var fi = new FileInfo(path);
                if (fi.Length > MaxTotalUncompressed)
                {
                    logmsg.Instance.Log($" File <{path}> is larger than allowed limit.");
                    return;
                }

                // Use safe FileStream with read sharing
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var zip = new ZipArchive(fs, ZipArchiveMode.Read, leaveOpen: false);

                if (zip.Entries.Count > MaxEntries)
                {
                    logmsg.Instance.Log($" Zip file <{path}> entries are over the limit ({zip.Entries.Count} > {MaxEntries})");
                    Console.WriteLine($" Zip file <{path}> entries are over the limit");
                    return;
                }

                long currentTotalUncompressed = 0;

                // Create a temporary extraction root for this archive
                var tempRoot = Path.Combine(Path.GetTempPath(), "AVcore", Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(tempRoot);

                try
                {
                    foreach (var entry in zip.Entries)
                    {
                        // Skip directory entries
                        if (string.IsNullOrEmpty(entry.Name) && entry.FullName.EndsWith("/"))
                        {
                            continue;
                        }

                        // Update totals and check limits
                        currentTotalUncompressed += entry.Length;
                        if (currentTotalUncompressed > MaxTotalUncompressed)
                        {
                            Console.WriteLine(" Total uncompressed size is over the limit. Skipping further entries.");
                            logmsg.Instance.Log(" Total uncompressed size is over the limit. Skipping further entries.");
                            return;
                        }

                        // Protection against zip-bomb by compression ratio if compressed length > 0
                        if (entry.CompressedLength > 0)
                        {
                            double ratio = entry.Length / (double)entry.CompressedLength;
                            if (double.IsInfinity(ratio) || double.IsNaN(ratio) || ratio > MaxCompressionRatio)
                            {
                                logmsg.Instance.Log($" Potential zip bomb detected in <{path}> entry <{entry.FullName}> - ratio {ratio:F2}");
                                Console.WriteLine($" Potential zip bomb detected in entry <{entry.FullName}> - skipping archive.");
                                return;
                            }
                        }

                        // Build safe destination path and prevent zip-slip
                        var destinationPath = Path.GetFullPath(Path.Combine(tempRoot, entry.FullName.Replace('/', Path.DirectorySeparatorChar)));
                        if (!destinationPath.StartsWith(Path.GetFullPath(tempRoot), StringComparison.OrdinalIgnoreCase))
                        {
                            logmsg.Instance.Log($" Skipped entry with invalid path (zip slip): {entry.FullName}");
                            continue;
                        }

                        // Ensure directory exists
                        var destDir = Path.GetDirectoryName(destinationPath);
                        if (!string.IsNullOrEmpty(destDir))
                        {
                            Directory.CreateDirectory(destDir);
                        }

                        // Extract entry to temp file and scan it
                        try
                        {
                            using var entryStream = entry.Open();
                            using var destFs = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
                            await entryStream.CopyToAsync(destFs).ConfigureAwait(false);
                        }
                        catch (Exception exEntry)
                        {
                            logmsg.Instance.Log($" Failed to extract entry {entry.FullName}: {exEntry.Message}");
                            continue;
                        }

                        // Now scan the extracted file
                        try
                        {
                            await ScanFileAsync(destinationPath).ConfigureAwait(false);
                            if(MalwareBazaarClient.IsVirus)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"\n Virus detected in extracted file: {entry.FullName} quarantine file;");
                                await QuarantineFileAsync(destinationPath, entry.FullName);
                                Console.ResetColor();
                            }
                        }
                        catch (Exception exScan)
                        {
                            logmsg.Instance.Log($" Failed scanning extracted entry {entry.FullName}: {exScan.Message}");
                        }

                        // Optionally delete the extracted file immediately after scan
                        try
                        {
                            if (File.Exists(destinationPath))
                            {
                                File.Delete(destinationPath);
                            }
                        }
                        catch
                        {
                            // ignore cleanup failures - temp root will remain for later cleanup
                        }
                    }
                }
                finally
                {
                    // Attempt to clean up the temporary extraction folder
                    try
                    {
                        if (Directory.Exists(tempRoot))
                        {
                            Directory.Delete(tempRoot, recursive: true);
                        }
                    }
                    catch
                    {
                        // ignore any cleanup errors - OS will clean temp eventually
                    }
                }
            }
            catch (InvalidDataException)
            {
                // Not a zip archive
                // Do nothing - caller can handle as regular file
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error processing zip file: {ex.Message}");
                Console.ResetColor();
                logmsg.Instance.Log($"Error processing zip file {path}: {ex.Message}");
            }
        }

        public void Help()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(" Type the path to a file or a directory you want to scan.");
            Console.WriteLine(" For information about this virus scanner type --about or see the readme.md in this github repo.");
            Console.ResetColor();
        }

        public void About()
        {
            // Method to display about information
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("This is a virus scanner developed by Tatu12335,");

            Console.ResetColor();
        }
        public async Task QuarantineFileAsync(string filepath,string? entry)
        {
            
            var quarantinefolder = Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tatus-Antivirus", "quarantine"));
            
            if (Directory.Exists(filepath))  return ;
            var filename = entry ?? Path.GetFileName(filepath);
            var destPath = Path.Combine(quarantinefolder.FullName, $"{filename}_{DateTime.Now:yyyyMMddHHmmss}.quar");
            try
            {
                File.Move(filepath, destPath);
                logmsg.Instance.Log($" File <{filepath}> quarantined to <{destPath}>");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" Failed to quarantine file <{filepath}>: {ex.Message}");
                Console.ResetColor();
                logmsg.Instance.Log($" Failed to quarantine file <{filepath}>: {ex.Message}");
            }


        }
        public void DeleteFile()
        {
            // Method to delete an infected file
        }
        public void UpdateVirusDefinitions()
        {
            // Method to update virus definitions
        }
        public void ScheduleScan()
        {
            // Method to schedule a scan
        }
        public void RealTimeProtection()
        {
            // Method to enable real-time protection
        }
        public void ExcludeFile()
        {
            // Method to exclude a file from scanning
        }
        public void IncludeFile()
        {
            // Method to include a file in scanning
        }
        public void RestoreFile()
        {
            // Method to restore a quarantined file
        }
        public void NotifyUser()
        {
            // Method to notify the user about scan results
        }
        public void LogScanActivity()
        {
            // Method to log scan activity
        }
        public void ConfigureSettings()
        {
            // Method to configure scanner settings
        }
        public void CheckForUpdates()
        {
            // Method to check for software updates
        }
        public void OptimizePerformance()
        {
            // Method to optimize scanner performance
        }
    }
}
