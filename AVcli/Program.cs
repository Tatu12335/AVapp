// Time wasted on both refactoring the prototype and writting and debbuging the prototype it self : 17hrs 00mins


using Antivirus.core.Classes.logs;
using AVcore.classes;

class Program
{


    static async Task Main(string[] args)
    {
        string filepath;
        logmsg.Instance.Log("\n--- av started ---\n");
        Console.Clear();
        Console.WriteLine(" Enter a file/directory to scan, or \"--about\"/\"--help\" for more info");

        Console.Write("\n>");


        try
        {
            if (args.Length > 0)
            {
                filepath = args[0];
                await ProcessUserInput(filepath);
            }
            else
            {
                filepath = Console.ReadLine();
                if (!string.IsNullOrEmpty(filepath))
                {
                    await ProcessUserInput(filepath);

                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" No filepath provided, exitting....");
                    Console.ResetColor();
                    logmsg.Instance.Log("\n No filepath provided, exitting....");
                    Environment.Exit(0);

                }
            }
        }
        catch (Exception ex)
        {
            logmsg.Instance.Log($"\n Unexpected error occured while processing user input | ERROR => {ex.Message}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($" Unexpected error occured while processing user input | ERROR => {ex.Message}");
            Console.ResetColor();
        }
        logmsg.Instance.Dispose();
    }
    public static async Task ProcessUserInput(string filepath)
    {
        char charsToTrimming = '"';
        char trimWhiteSpaces = ' ';

        try
        {
            filepath = filepath.Trim(charsToTrimming, trimWhiteSpaces);

            if (filepath.Equals("--help", StringComparison.OrdinalIgnoreCase))
            {
                FileScanner.FileScannerInstance.Help();
            }
            else if (filepath.Equals("--about", StringComparison.OrdinalIgnoreCase))
            {
                FileScanner.FileScannerInstance.About();
            }
            else
            {
                try
                {
                    await FileScanner.FileScannerInstance.ProcessFileAsync(filepath);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($" Unexpected error occured in ProcessUserInput | ERROR : {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            logmsg.Instance.Log(ex.ToString());
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n Unexpected error occured while processing user input | ERROR => {ex.Message}");
            Console.ResetColor();
        }
    }
}
