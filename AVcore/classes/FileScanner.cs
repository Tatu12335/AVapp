using Antivirus.core.Classes.logs;

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

                await hasher.GetHasher.asyncHash(file);
                //Console.WriteLine("Hashed sending to the api");
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
