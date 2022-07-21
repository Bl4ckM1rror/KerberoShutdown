using System;
using System.Linq;
using System.Collections.Generic;

namespace KerberoShutdown
{
    public static class DisplayUtil
    {
        public static void PrintBanner()
        {
            Console.WriteLine();
            Console.WriteLine(@"  _  __         _                    _____ _           _      _                      ");
            Console.WriteLine(@" | |/ /        | |                  / ____| |         | |    | |                     ");
            Console.WriteLine(@" | ' / ___ _ __| |__   ___ _ __ ___| (___ | |__  _   _| |_ __| | _____      ___ __   ");
            Console.WriteLine(@" |  < / _ \ '__| '_ \ / _ \ '__/ _ \\___ \| '_ \| | | | __/ _` |/ _ \ \ /\ / / '_ \  ");
            Console.WriteLine(@" | . \  __/ |  | |_) |  __/ | | (_) |___) | | | | |_| | || (_| | (_) \ V  V /| | | | ");
            Console.WriteLine(@" |_|\_\___|_|  |_.__/ \___|_|  \___/_____/|_| |_|\__,_|\__\__,_|\___/ \_/\_/ |_| |_| ");
            Console.WriteLine();
            Console.WriteLine("                           v1.0 Powered by Bl4ckM1rror                                ");
        }


        public static void Print(string output, Enums.PrintColor color)
        {
            switch (color)
            {
                case Enums.PrintColor.YELLOW:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(output);
                    Console.ResetColor();
                    break;
                case Enums.PrintColor.GREEN:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(output);
                    Console.ResetColor();
                    break;
                case Enums.PrintColor.RED:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(output);
                    Console.ResetColor();
                    break;
            }
        }


        public static void Done()
        {
            Print("\n[*] Done!\n", Enums.PrintColor.GREEN);
        }
    }
}