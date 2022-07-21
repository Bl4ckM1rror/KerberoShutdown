using System;
using System.Collections.Generic;
using CommandLine;

namespace KerberoShutdown
{
    class Program
    {
        static void Main(string[] args)
        {
            DisplayUtil.PrintBanner();

            var parser = new Parser(with =>
            {
                with.CaseInsensitiveEnumValues = true;
                with.CaseSensitive = false;
                with.HelpWriter = null;
            });

            parser.ParseArguments<Options>(args).WithParsed(o => { Options.Instance = o; }).WithNotParsed(error => { });
            parser.Dispose();

            var options = Options.Instance;
            if (options == null) { Options.GetHelp(); return; }

            ChooseOption(options);

            DisplayUtil.Done();
        }

        public static void ChooseOption(Options options)
        {
            if (options.FindUnquotedsvc)
            {
                Kerbreak.FindUnquotedsvc();
            }
            else if (options.GetWritableFiles)
            {
                Kerbreak.GetWritableFiles(options.Root, options.FileFormat);
            }
            else if (options.GetASREPRoastable)
            {
                Kerbreak.GetASREPRoastable();
            }
            else if (options.GetAllMembers)
            {
                Kerbreak.GetAllMembers(options.groupName, options.domainName);
            }
            else if (options.DCSync)
            {
                Kerbreak.DCSync();
            }
        }
    }
}
