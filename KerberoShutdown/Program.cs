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
        }

        public static void ChooseOption(Options options)
        {
            if (options.FindUnquotedsvc)
            {
                Kerbreak.InitializeSearch();
                Kerbreak.FindUnquotedsvc();
                DisplayUtil.Done();
            }
            else if (options.GetWritableFiles)
            {
                Kerbreak.InitializeSearch();
                Kerbreak.GetWritableFiles(options.root, options.fileFormat);
                DisplayUtil.Done();
            }
            else if (options.GetASREPRoastable)
            {
                Kerbreak.InitializeSearch();
                try
                {
                    Kerbreak.GetASREPRoastable();
                }
                catch { }
                DisplayUtil.Done();
            }
            else if (options.GetAllMembers)
            {
                Kerbreak.InitializeSearch();
                Kerbreak.GetAllMembers(options.groupName, options.domainName);
                DisplayUtil.Done();
            }
            else if (options.DCSync)
            {
                Kerbreak.InitializeSearch();
                Kerbreak.DCSync();
                DisplayUtil.Done();
            }
            else
            {
                Options.GetHelp();
            }
        }
    }
}
