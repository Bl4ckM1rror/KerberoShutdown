using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Reflection.PortableExecutable;
using System.ServiceProcess;
using System.Text;
using System.DirectoryServices.ActiveDirectory;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using Microsoft.AspNetCore.Authentication;
using System.Threading;
using System.Security.AccessControl;
using System.Collections;
using System.Security.Principal;

namespace KerberoShutdown
{
    class Kerbreak
    {
        public static void FindUnquotedsvc()
        {
            ServiceController[] scs = ServiceController.GetServices();
            foreach (ServiceController s in scs)
            {
                RegistryKey rkey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\" + s.ServiceName);
                string path = rkey.GetValue("ImagePath").ToString();


                if (path[0] != '"' && !path.Contains("system32") && !path.Contains("System32"))
                {
                    Console.WriteLine(s.ServiceName);
                    Console.WriteLine(path);
                }
            }
        }

        public string GetDCSyncUsers(string domainDN)
        {
            string result;

            StringWriter sw = new StringWriter();


            Hashtable ht = new Hashtable();
            ht.Add("DS-Replication-Get-Changes", "1131f6aa-9c07-11d1-f79f-00c04fc2dcd2");
            ht.Add("DS-Replication-Get-Changes-All", "1131f6ad-9c07-11d1-f79f-00c04fc2dcd2");
            ht.Add("DS-Replication-Get-Changes-In-Filtered-Set", "89e95b76-444d-4c62-991a-0facbeda640c");
            ht.Add("DS-Replication-Manager-Topology", "1131f6ac-9c07-11d1-f79f-00c04fc2dcd2");
            ht.Add("DS-Replication-Monitor-Topology", "f98340fb-7c5b-4cdb-a00b-2ebdfa115a96");
            ht.Add("DS-Replication-Synchronize", "1131f6ab-9c07-11d1-f79f-00c04fc2dcd2");

            System.DirectoryServices.DirectoryEntry de = new System.DirectoryServices.DirectoryEntry("LDAP://" + domainDN);
            DirectorySearcher ds = new DirectorySearcher();
            ds.SearchRoot = de;

            try
            {
                foreach (SearchResult sr in ds.FindAll())
                {
                    System.DirectoryServices.DirectoryEntry temp = sr.GetDirectoryEntry();
                    ActiveDirectorySecurity adpwn = temp.ObjectSecurity;

                    AuthorizationRuleCollection arc = adpwn.GetAccessRules(true, true, typeof(NTAccount));


                    if (domainDN.Contains(temp.Name))
                    {
                        foreach (ActiveDirectoryAccessRule a in arc)
                        {
                            foreach (DictionaryEntry d in ht)
                            {
                                if (d.Value.ToString() == a.ObjectType.ToString())
                                {
                                    sw.WriteLine(a.IdentityReference);
                                    //sw.WriteLine(a.ObjectType);
                                    sw.WriteLine(d.Key.ToString());
                                    sw.WriteLine(a.ActiveDirectoryRights);
                                }
                            }
                        }
                    }

                }
            }
            catch { }

            result = sw.ToString();
            return result;
        }

        public static void DCSync()
        {
            string cmd = "";
            Forest f = Forest.GetCurrentForest();
            DomainCollection domains = f.Domains;
            foreach (Domain d in domains)
            {

                string domainName = d.Name.ToString();


                string[] dn = domainName.Split('.');
                for (int i = 0; i < dn.Length; i++)
                {
                    dn[i] = "DC=" + dn[i];
                }

                string domainDN = String.Join(",", dn);

                Kerbreak kerbreak = new Kerbreak();
                Thread dcsync = new Thread(() => { cmd += kerbreak.GetDCSyncUsers(domainDN); });
                dcsync.Start();
                dcsync.Join();
            }

            Console.WriteLine(cmd);

        }

        public static void GetWritableFiles(string root, string fileformat)
        {
            var dirs = Directory.EnumerateDirectories(root);
            foreach (string dir in dirs)
            {
                //Console.WriteLine(dir);
                try
                {
                    //Directory.EnumerateFiles(dir, "*.exe", SearchOption.AllDirectories);
                    var files = Directory.EnumerateFiles(dir, fileformat, SearchOption.AllDirectories);
                    foreach (string filename in files)
                    {
                        //Console.WriteLine(filename);
                        try
                        {
                            FileStream fs = File.Open(filename, FileMode.Open, FileAccess.ReadWrite);
                            Console.WriteLine("Write Access on {0} ", filename);
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {
                }
            }

        }

        public static void GetASREPRoastable()
        {
            //Fetching all domains
            Forest woods = Forest.GetCurrentForest();
            DomainCollection domains = woods.Domains;
            foreach (Domain domain in domains)
            {
                //Console.WriteLine(domain.Name);
                //pentest.local --> DC=pentest,DC=local
                string domainName = domain.Name.ToString();
                string[] dn = domainName.Split('.');

                for (int i = 0; i < dn.Length; i++)
                {
                    dn[i] = "DC=" + dn[i];

                }

                //LDAP://DC=pentest,DC=local
                System.DirectoryServices.DirectoryEntry de = new System.DirectoryServices.DirectoryEntry(String.Format("LDAP://{0}", String.Join(",", dn)));
                DirectorySearcher ds = new DirectorySearcher();
                ds.SearchRoot = de;
                ds.Filter = "(&(objectclass=user)(!(objectclass=computer))(useraccountcontrol>=4194304))";
                foreach (SearchResult sr in ds.FindAll())
                {
                    Console.WriteLine("User: {0} from Domain: {1}", sr.Properties["samaccountname"][0], domainName);
                    Console.WriteLine("UserAccountControlsr {0}", sr.Properties["useraccountcontrol"][0]);
                    Console.WriteLine();
                }

            }
        }

        public static void GetAllMembers(string groupName, string domainName)
        {
            PrincipalContext pc = new PrincipalContext(ContextType.Domain, domainName);
            GroupPrincipal gp = GroupPrincipal.FindByIdentity(pc, groupName);
            foreach (Principal group in gp.GetMembers())
            {
                if (group.StructuralObjectClass == "user")
                {
                    Console.WriteLine("User: {0} ", group.Name);
                }
                else if (group.StructuralObjectClass == "group")
                {
                    //Console.WriteLine("Group: {0} is memberOf {1} ", group.Name, groupName);
                    GetAllMembers(group.Name, domainName);
                }
            }
        }
    }
}
