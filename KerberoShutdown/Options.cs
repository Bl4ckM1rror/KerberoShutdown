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

        [Option("GetUACFlags", Default = false, HelpText = "Enumerate all UAC flags of a specific user")]
        public bool GetUACFlags { get; set; }

        [Option("FindUnquotedsvc", Default = false, HelpText = "Enumerate all unquoted services on the target host")]
        public bool FindUnquotedsvc { get; set; }

        [Option("DCSync", Default = false, HelpText = "Enumerate all possible DCSync accounts")]
        public bool DCSync { get; set; }

        [Option("GetASREPRoastable", Default = false, HelpText = "Enumerate all AS-REP Roastable users")]
        public bool GetASREPRoastable { get; set; }

        [Option("UnconstrainDeleg", Default = false, HelpText = "Enumerate all Unconstrained Delegation accounts")]
        public bool UnconstrainDeleg { get; set; }

        [Option("ConstrainDeleg", Default = false, HelpText = "Enumerate all Constrained Delegation accounts")]
        public bool ConstrainDeleg { get; set; }

        [Option("RBCD", Default = false, HelpText = "Enumerate all Resource-based Constrained Delegation accounts")]
        public bool RBCD { get; set; }

        [Option("GetAllMembers", Default = false, HelpText = "Enumerate all users (also within nested groups)")]
        public bool GetAllMembers { get; set; }

        [Option("root", Default = @"C:\", HelpText = "Root folder")]
        public string root { get; set; }

        [Option("user", Default = "", HelpText = "User Account Name")]
        public string user { get; set; }

        [Option("groupName", Default = "Domain Admins", HelpText = "Local Group Name")]
        public string groupName { get; set; }

        [Option("domainName", Default = "pentest.local", HelpText = "Domain to enumerate")]
        public string domainName { get; set; }

        [Option("fileFormat", Default = @"*.dll", HelpText = "File format to search")]
        public string fileFormat { get; set; }


        public static void GetHelp()
        {
            var help = @"
  --GetWritableFiles  Enumerate all writable files on the target host
  --FindUnquotedsvc   Enumerate all unquoted services on the target host
  --GetAllMembers     Enumerate all users (also within nested groups)
  --GetUACFlags       Enumerate all UAC flags of a specific user
  --GetASREPRoastable Enumerate all AS-REP Roastable users 
  --DCSync            Enumerate all possible DCSync accounts
  --UnconstrainDeleg  Enumerate all Unconstrained Delegation accounts
  --ConstrainDeleg    Enumerate all Constrained Delegation accounts
  --help              Display this help screen

Example: .\KerberoShutdown.exe --help
         .\KerberoShutdown.exe --FindUnquotedsvc
         .\KerberoShutdown.exe --GetWritableFiles --root C:\ --fileFormat *.dll
         .\KerberoShutdown.exe --GetAllMembers --groupName ''Domain Admins'' --domainName <domain_name>
         .\KerberoShutdown.exe --GetUACFlags --user <user_name>
         .\KerberoShutdown.exe --GetASREPRoastable
         .\KerberoShutdown.exe --DCSync
         .\KerberoShutdown.exe --UnconstrainDeleg
         .\KerberoShutdown.exe --ConstrainDeleg

";
            System.Console.WriteLine(help);

        }
    }
}
