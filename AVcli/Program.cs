// Time wasted on both refactoring the prototype and writting and debbuging : 27hrs 00mins


using Antivirus.core.Classes.logs;
using AVcore.classes;


class Program1
{
    public static Program1 MainInstance { get; } = new Program1();

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
        // I plan on getting the extensions from the magic bytes have not implemented it yet tho, this will do for now.
        path = Path.GetFullPath(path);
        Stack<string> dirs = new Stack<string>();
        dirs.Push(path);

        while (dirs.Count > 0)
        {

            var curDir = dirs.Pop();



            try
            {

                var directories = Directory.EnumerateDirectories(curDir, "*");
                //var files2 = Directory.EnumerateFiles(curDir,"*");

                /*foreach(var file in files2)
                {
                    var extensions = Path.GetExtension(file).ToLower();

                    if (extensions == ".zip")
                    {
                        await FileScanner.FileScannerInstance.IsZip(file);
                        return;
                    }
                    else
                    {
                        await FileScanner.FileScannerInstance.ScanFileAsync(file);
                    }
                }*/

                foreach (var d in directories)
                {
                    //var directories2 = Directory.EnumerateDirectories(d,"*");
                    var files = Directory.EnumerateFiles(d, "*");


                    foreach (var file in files)
                    {

                        var extension = Path.GetExtension(file).ToLower();

                        if (extension == ".zip")
                        {
                            await FileScanner.FileScannerInstance.IsZip(file);
                        }
                        else
                        {
                            await FileScanner.FileScannerInstance.ScanFileAsync(file);

                        }

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

