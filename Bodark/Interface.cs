using System;
using static Bodark.CryptUtils;
using static Bodark.KeyChain;
using NaCl;
namespace Bodark
{
    class Interface
    {
        static void Main(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

           Console.WriteLine(NaCl.Core.ChaCha20.BLOCK_SIZE_IN_INTS);

            KeyChain secureChain = new KeyChain("securedShit");
            secureChain.addKey("www.netflix.com", "notMyAccount", "notMyPassword");
            Console.WriteLine(secureChain.findKey("www.netflix.com"));
            Console.ReadLine();
            secureChain.deleteKey("www.netflix.com");


            if (args.Length < 1)
            {
                Environment.Exit(1);
            }

            bool keygen = false;
            bool encTool = false;
            bool keychain = false;
            bool safari = false;
            bool diceware = false;
            bool rsa = false;
            int numberOfPasswords = 1;
            int passwordLength = 24;
            int numberOfWords = 5;
            bool aesencryption = false;
            bool aesdecryption = false;
            bool hash = false;

            foreach (string arg in args)
            {
                if (arg == "keygen")
                {
                    keygen = true;
                }
                else if (arg == "safari")
                {
                    safari = true;
                }
                else if (arg == "dice")
                {
                    diceware = true;
                }
                else if (arg == "rsa")
                {
                    rsa = true;
                }
                else if (arg.StartsWith("--count=", StringComparison.Ordinal))
                {
                    numberOfPasswords = Int32.Parse(arg.Split('=')[1]);
                }
                else if (arg.StartsWith("--length=", StringComparison.Ordinal))
                {
                    passwordLength = Int32.Parse(arg.Split('=')[1]);
                }
                else if (arg.StartsWith("--words=", StringComparison.Ordinal))
                {
                    numberOfWords = Int32.Parse(arg.Split('=')[1]);
                }
                /// KEYGEN ENDS
                else if (arg == "enc")
                {
                    encTool = true;
                }
                else if (arg == "aese")
                {
                    aesencryption = true;
                } else if (arg == "aesd")
                {
                    aesdecryption = true;
                } else if (arg == "hash")
                {
                    hash = true;
                }
                else if (arg == "chain")
                {
                    keychain = true;
                }
            }

            Environment.Exit(0);
        }
    }
}

        


        



        