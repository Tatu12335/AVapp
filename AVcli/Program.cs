// Time wasted on both refactoring the prototype and writting and debbuging : 23hrs 30mins


using Antivirus.core.Classes.logs;
using AVcore.classes;

class Program
{
    public static bool Isfile = false;

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
                await ProcessFile(filepath);
            }
        }
        catch (Exception ex)
        {
            logmsg.Instance.Log(ex.Message);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n Unexpected error occured while processing user input | ERROR => {ex.Message}");
            Console.ResetColor();
        }
    }
    public static async Task ProcessFile(string path)
    {
        //var dirs = Directory.GetDirectories(path);
        Stack<string> dirs = new Stack<string>();
        dirs.Push(path);
        while (dirs.Count > 0)
        {
            var curDir = dirs.Pop();
            try
            {
                if (Directory.Exists(curDir) || File.Exists(curDir))
                {
                    if (File.Exists(curDir))
                    {
                        await FileScanner.FileScannerInstance.ScanFileAsync(curDir);
                    }
                    else
                    {
                        try
                        {
                            var directories = Directory.EnumerateDirectories(curDir);

                            foreach (var d in directories)
                            {

                                var files = Directory.EnumerateFiles(d);
                                foreach (var file in files)
                                {

                                    Console.WriteLine(file.ToString());
                                    await FileScanner.FileScannerInstance.ScanFileAsync(file.ToString());
                                }



                                dirs.Push(d);

                            }
                        }
                        catch (UnauthorizedAccessException uaex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"ERROR : {uaex.Message} | SKIPPING FILE");
                            Console.ResetColor();
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($" ERROR : {ex.Message}");
                        }
                    }
                }



            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" ERROR : {ex.Message}");
                Console.ResetColor();
            }
        }

    }


}

