using Antivirus.core.Classes.logs;

namespace AVcore.classes
{
    public class FileScanner
    {
        public static FileScanner FileScannerInstance { get; } = new FileScanner();
        public static bool IsDirectory = false;

        // FOR storing file extensions, i plan on making a file that checks the magic bytes to determine the extension in the future!
        public static List<string> RiskyExtensions = new List<string>()
        {
            "*.exe", "*.com", ".*scr", "*.dll", "*.ocx", "*.sys", "*.msi", "*.cab", "*.appx", "*.bat", "*.ps1", "*.vbs", "*.js", "*.docm", "*.xlsm"
        };

        public async Task<string> ProcessFileAsync(string? filepath)
        {
            while (true)
            {
                Thread.Sleep(300);
                if (File.Exists(filepath))
                {


                    try
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($" File <{filepath}> exists");
                        await ScanFileAsync(filepath);
                        Console.ResetColor();

                        return filepath;

                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"\n Unexpected error occured in ProcessFileAsync | ERROR : {ex.Message}\n");
                        Console.ResetColor();
                    }
                }
                else if (Directory.Exists(filepath))
                {

                    Console.ForegroundColor = ConsoleColor.Green;
                    try
                    {
                        var Files = Directory.EnumerateFiles(filepath, "*", SearchOption.AllDirectories);


                        foreach (string file in Files)
                        {
                            Thread.Sleep(300);
                            try
                            {
                                await ScanFileAsync(file);
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($" Unexpected error occured in ProcessFileAsync | ERROR : {ex.Message}");
                                Console.ResetColor();
                            }
                        }

                    }
                    catch(UnauthorizedAccessException uaex)
                    {
                        Console.ForegroundColor= ConsoleColor.Red;
                        Console.WriteLine($" Unauthorized to the file/directory <{filepath}> Try running the app with elevated privilages!");
                        Console.WriteLine($" | ERROR : {uaex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($" Unexpeted error occured trying to enumerate files | ERROR : {ex.Message}");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($" File/Directory <{filepath}>, not found");
                    Console.ResetColor();
                }
                Thread.Sleep(300);
                return filepath;

            }
        }
        public async Task ScanFileAsync(string? file)
        {
            logmsg.Instance.Log($"\n Proceeding with scanning <{file}>");
            
            Console.ForegroundColor= ConsoleColor.Green;

            try
            {
                await hasher.GetHasher.asyncHash(file);
                Console.WriteLine("Hashed sending to the api");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" Unexpeted error occured calling the hash function | ERROR : {ex.Message}");
                Console.ResetColor();
            }

            Console.ResetColor();

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
    }
}
