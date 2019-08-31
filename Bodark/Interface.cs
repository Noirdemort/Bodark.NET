using System;

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

            if (args.Length < 1)
            {
                Environment.Exit(exitCode: 1);
            }

            Environment.Exit(exitCode: 0);
        }


    }
}

        


        



        