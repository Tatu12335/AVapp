using AVcore.classes.abuse.ch_client;
using System.Security.Cryptography;

namespace AVcore.classes
{
    public class hasher
    {
        public static hasher GetHasher { get; } = new hasher();

        public async Task asyncHash(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" No filepath provided to asyncHash function");
                Console.ResetColor();
                return;
            }

            var curUser = Environment.UserName;
            var fileInfo = new FileInfo(file);

            if (!fileInfo.Exists)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($" File <{fileInfo.FullName}> not found");
                Console.ResetColor();
                return;
            }

            try
            {
                using var sha256 = SHA256.Create();

                using var fS = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                const int maxRetries = 10;
                var attempt = 0;
                while (true)
                {
                    attempt++;
                    if (fS.CanSeek)
                    {


                        fS.Position = 0;
                        byte[] hashvalue = await sha256.ComputeHashAsync(fS).ConfigureAwait(false);

                        string hashhex = Convert.ToHexString(hashvalue).ToLowerInvariant();

                        try
                        {

                            await MalwareBazaarClient.GetClient.CheckHashStatus(hashhex).ConfigureAwait(false);

                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(" Error in calling the api | ERROR :  " + ex.Message);
                            Console.ResetColor();
                        }
                        break;

                    }
                    else if (attempt >= maxRetries)
                    {
                        throw new Exception(" Max-retries is 10, could not openfile");
                    }
                }

            }
            catch (IOException ioex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" IO Exception is asyncHash() | ERROR : {ioex.Message} ");
                Console.ResetColor();
            }
            catch (UnauthorizedAccessException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" User : \"{curUser}\" | Is not allowed to open file : \"{fileInfo.FullName}\"");
                Console.WriteLine(" ****** Try running the application with elevated permissions ****** ");
                Console.ResetColor();
            }
            catch (ArgumentNullException anex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Argument error: " + anex.Message);
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" Unexpected error occured | ERROR : {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
