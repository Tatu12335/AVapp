using Antivirus.core.Classes.logs;
using System.ComponentModel.Design;
using Antivirus.core;
using System.IO.Compression;

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

        public async Task ScanFileAsync(string? file)
        {
            logmsg.Instance.Log($"\n Proceeding with scanning <{file}>");



            try
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(file);
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Green;
                //if (File.Exists(file))
                //{
                    await hasher.GetHasher.asyncHash(file);

                //}
                //else if(Directory.Exists(file))
               


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
        public async Task IsZip(string path)
        {

            try
            {

                if (new FileInfo(path).Length > 100000000)
                {
                    return;
                }

                using (FileStream fS = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (ZipArchive zipArchive = new ZipArchive(fS, ZipArchiveMode.Read))
                {
                    // Protection against Entry Bombs
                    if (zipArchive.Entries.Count > 1000)
                    {
                        logmsg.Instance.Log($"Zip file <{path}> entries is over the limit");
                        return;
                    }

                    long currentTotal = 0;
                    foreach (var entry in zipArchive.Entries)
                    {
                        currentTotal += entry.Length;
                        var Path2 = entry.FullName;
                        // Early Exit, stop counting as soon as we hit the limit
                        if (currentTotal > 100000000)
                        {
                            Console.WriteLine("size is over the limit. Skipping.");
                            return;
                        }
                        foreach (var file in zipArchive.Entries)
                        {

                            await ScanFileAsync(file.FullName);
                        }


                        if (entry.CompressedLength > 0)
                        {
                            double ratio = (double)entry.Length / entry.CompressedLength;
                            if (ratio > 100)
                            { // potential zip bomb 
                                return;
                            }
                            else
                            {

                                await ScanFileAsync(Path2);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error processing zip file: {ex.Message}");
                Console.ResetColor();
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
            Console.WriteLine("This is a virus scanner developed by Tatu1335,");

            Console.ResetColor();
        }
        public void QuarantineFile()
        {
            // Method to quarantine an infected file
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
