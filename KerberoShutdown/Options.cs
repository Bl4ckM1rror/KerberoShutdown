using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace KerberoShutdown
{
    class Options
    {
        public static Options Instance { get; set; }

        [Option("GetWritableFiles", Default = false, HelpText = "Enumerate all writable files on the target host")]
        public bool GetWritableFiles { get; set; }

        [Option("FindUnquotedsvc", Default = false, HelpText = "Enumerate all unquoted services on the target host")]
        public bool FindUnquotedsvc { get; set; }

        [Option("DCSync", Default = false, HelpText = "Enumerate all possible DCSync accounts")]
        public bool DCSync { get; set; }

        [Option("GetASREPRoastable", Default = false, HelpText = "Enumerate all AS-REP Roastable users")]
        public bool GetASREPRoastable { get; set; }

        [Option("GetAllMembers", Default = false, HelpText = "Enumerate all users (also within nested groups)")]
        public bool GetAllMembers { get; set; }

        [Option("Root", Default = @"C:\", HelpText = "Root folder")]
        public string Root { get; set; }

        [Option("groupName", Default = "Domain Admins", HelpText = "Local Group Name for Local Group Member Enumeration")]
        public string groupName { get; set; }

        [Option("domainName", Default = "pentest.local", HelpText = "Domain to enumerate")]
        public string domainName { get; set; }

        [Option("FileFormat", Default = "*", HelpText = "File format to search")]
        public string FileFormat { get; set; }


        public static void GetHelp()
        {
            var help = @"
  --domainName        (Default: pentest.local) Domain to enumerate
  --Root              (Default: C:\) Root folder
  --groupName         (Default: Domain Admins) Local Group Name for Local Group Member Enumeration
  --GetWritableFiles  Enumerate all writable files on the target host
  --FindUnquotedsvc   Enumerate all unquoted services on the target host
  --GetASREPRoastable Enumerate all AS-REP Roastable users 
  --GetAllMembers     Enumerate all users (also within nested groups)
  --DCSync            Enumerate all possible DCSync accounts
  --help              Display this help screen

Example: .\KerberoShutdown.exe --help
         .\KerberoShutdown.exe --domainName domain.local 
";
            System.Console.WriteLine(help);

        }
    }
}
