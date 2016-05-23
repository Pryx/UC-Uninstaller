using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Web;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.ComponentModel;

namespace Core
{
    public class core
    {

        #region Třídy

        public class updateclass
        {
            public string server_status { get; set; }
            public string download { get; set; }
            public bool update { get; set; }
            public Dictionary<string, string> changelog { get; set; }
            public string updatetype { get; set; }
        }

        [Serializable]
        public class program
        {
            public string icon { get; set; }
            public string name { get; set; }
            public string regkey { get; set; }
            public string fullregkey { get; set; }
            public int id { get; set; }
            public string[] parameters { get; set; }
            public int keyid { get; set; }
            public bool uninstall { get; set; }
            public bool invalid { get; set; }
            public string path { get; set; }
            public string uninstallstring { get; set; }
        }

        #endregion

        #region Kontrola aktualizací

        public static Timer lastcheck = new Timer();

        public static void timerreset(Object myObject,EventArgs myEventArgs)
        {
            lastcheck.Enabled = false;
            lastcheck.Interval = 0;
        }

        public static Task<updateclass> updatecheck(bool asynchron)
        {
           
            return Task.Run(() =>
        {
            if (lastcheck.Enabled == false)
            {
                lastcheck.Interval = 500;
                lastcheck.Enabled = true;

                if (network())
                {
                    string URL = "http://uninstaller.pryx.net/update.php";
                    WebClient webClient = new WebClient();
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        program = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString().Replace(".", ""),
                        database = database_ver.Replace(".", ""),
                        beta = false
                    });
                    NameValueCollection post = new NameValueCollection();
                    post["json"] = json;
                    byte[] responseBytes = webClient.UploadValues(URL, "POST", post);
                    string responsefromserver = Encoding.UTF8.GetString(responseBytes);
                    webClient.Dispose();
                    JavaScriptSerializer ser = new JavaScriptSerializer();
                    updatedata = ser.Deserialize<updateclass>(responsefromserver);
                    return updatedata;
                }
                else { updatedata = new updateclass(); updatedata.server_status = "down"; return updatedata; }
            }
            else { return updatedata; }
        });
        }
        #endregion

        #region Aktualizovat DB

        public static void databaseupdate()
        {
            if (updatedata == null) { updatedata = new updateclass(); updatedata.download = "http://data.pryx.net/UC%20Uninstaller/update/"; }
            sqlite.Shutdown();
            using (WebClient Client = new WebClient())
            {
                if (File.Exists(Application.StartupPath + @"\uninstaller.udb")) { File.Delete(Application.StartupPath + @"\uninstaller.udb"); }
                Client.DownloadFile(updatedata.download + "uninstaller.udb", "uninstaller.udb");
            }
            dbconnect();
            sqlite.Close();
        }

        #endregion

        #region Ověření dostupnosti pryx.net
        

        public static bool network()
        {
            try
            {
                WebRequest request = WebRequest.Create("http://uninstaller.pryx.net");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch { return false; }


        }

        #endregion

        #region Připojení DB

        public static SQLiteConnection sqlite = new SQLiteConnection("Data Source=" + Application.StartupPath + @"\uninstaller.udb;Version=3;");

        public static bool dbconnect()
        {
            if (File.Exists(Application.StartupPath + @"\uninstaller.udb"))
            {
                sqlite.Close();
                sqlite.Open();
                SQLiteCommand cmd = new SQLiteCommand("SELECT version FROM version", sqlite);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) { database_ver = reader.GetString(0); }
                reader.Close();
                cmd.Dispose();
                return true;
            }
            else
            {
                database_ver = "0";
                return false;
            }
        }

        #endregion

        #region Otvírání web stránek

        public static void openwebpage(string url)
        {
            Process.Start(new ProcessStartInfo(url));
        }

        #endregion

        #region Práce s registrem


        public static RegistryKey[] keys = new RegistryKey[] { RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"), RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.CurrentUser, RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"), RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall") };
        public static string[] keywordshigh = new string[] { "toolbar", "casino", "security scan" };
        public static string[] keywordslow = new string[] { "acer", "asus", "dell", "lenovo", "hp", "sony", "samsung", "toshiba", "gigabyte", "fujitsu", "msi", "packard bell" };

        public static List<program> search(bool advanced)
        {
            ignore = get_ignored();
            if (sqlite.State != System.Data.ConnectionState.Open) { dbconnect(); }
            uninstalllist = new List<program>();
            int i = 0;
            int n = 0;
            searchicon();
            foreach (RegistryKey regkey in keys)
            {
                n++;
                if (regkey != null & ((n != 3) | (is64bit)))
                {
                    foreach (string keyname in regkey.GetSubKeyNames())
                    {   
                        i++;
                        using (RegistryKey key = regkey.OpenSubKey(keyname))
                        {
                            if (key.GetValue("UninstallString") != null && key.GetValue("DisplayName") != null)
                            {

                            SQLiteCommand command = new SQLiteCommand("SELECT * FROM uninstall WHERE key='" + keyname.Replace("'","") + "'", sqlite);
                            SQLiteDataReader reader = command.ExecuteReader();
                            bool syscomp = false;
                            if (key.GetValue("SystemComponent") != null) 
                            {
                                try
                                {
                                    if ((int)key.GetValue("SystemComponent") == 0) syscomp = false;
                                    else syscomp = true;
                                }
                                catch { }
                            }

                                if ((reader != null & reader.Read()) | keywordshigh.Any(key.GetValue("DisplayName").ToString().ToLower().Contains))
                                {

                                    program program = new program();
                                    if (reader.Read()) { program.uninstall = (bool)reader["checked"]; }
                                    else { program.uninstall = true; }
                                    program.name = key.GetValue("DisplayName").ToString();
                                    if (key.GetValue("DisplayIcon") != null) { program.icon = key.GetValue("DisplayIcon").ToString(); }
                                    else { var item = iconlist.Find(x => x.name == program.name); if (item != null) { program.icon = item.icon; } }
                                    if (key.GetValue("InstallLocation") != null) { program.path = key.GetValue("InstallLocation").ToString(); }
                                    if (key.GetValue("QuietUninstallString") != null) { program.uninstallstring = key.GetValue("QuietUninstallString").ToString(); }
                                    else { program.uninstallstring = key.GetValue("UninstallString").ToString(); }
                                    program.keyid = n;
                                    program.regkey = key.ToString().Replace(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\", "").Replace(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\", "").Replace(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\", "");
                                    program.id = i;
                                    uninstalllist.Add(smarty(program));
                                }
                                else if (!syscomp)
                                {
                                    if (advanced | keywordslow.Any(key.GetValue("DisplayName").ToString().ToLower().Contains))
                                    {
                                        program program = new program();
                                        program.uninstall = false;
                                        program.name = key.GetValue("DisplayName").ToString();
                                        if (key.GetValue("DisplayIcon") != null) { program.icon = key.GetValue("DisplayIcon").ToString(); }
                                        else { var item = iconlist.Find(x => x.name == program.name); if (item != null) { program.icon = item.icon; } }
                                        if (key.GetValue("InstallLocation") != null) { program.path = key.GetValue("InstallLocation").ToString(); }
                                        if (key.GetValue("QuietUninstallString") != null) { program.uninstallstring = key.GetValue("QuietUninstallString").ToString(); }
                                        else { program.uninstallstring = key.GetValue("UninstallString").ToString(); }
                                        program.keyid = n;
                                        program.regkey = key.ToString().Replace(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\", "").Replace(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\", "").Replace(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\", "");
                                        program.id = i;
                                        uninstalllist.Add(smarty(program));
                                    }
                                }
                                command.Dispose();
                                reader.Dispose();
                            }
                        }
                    }
                }
            }
            core.uninstalllist.Sort((a, b) => String.Compare(a.name, b.name));
            sqlite.Close();
            return uninstalllist;
        }

        public static List<program> searchicon()
        {
            iconlist = new List<program>();
            int i = 0;
            RegistryKey regkey = null;
            if (is64bit) { regkey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Classes\Installer\Products"); }
            else { regkey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Classes\Installer\Products"); }
            if (regkey != null)
            {
                foreach (string keyname in regkey.GetSubKeyNames())
                {
                    i++;
                    using (RegistryKey key = regkey.OpenSubKey(keyname))
                    {
                        if (key.GetValue("ProductName") != null && key.GetValue("ProductIcon") != null)
                        {

                            program program = new program();
                            program.name = key.GetValue("ProductName").ToString();
                            program.icon = key.GetValue("ProductIcon").ToString();
                            program.id = i;
                            iconlist.Add(program);

                        }
                    }
                }
            }
            iconlist = iconlist.ToList();
            return iconlist;
        }

        public static List<program> ignore = new List<program>();

        #endregion

        #region chytre funkce
        public static bool invalidate(string filename)
        {
            if (!Path.GetInvalidFileNameChars().Any(Path.GetFileName(filename).Contains))
            {
                if (File.Exists(filename)) { return false; }
            }
            else { return true; }

            var paths = new[] { Environment.CurrentDirectory }.Concat(Environment.GetEnvironmentVariable("PATH").Split(';'));
            var extensions = new[] { String.Empty }.Concat(Environment.GetEnvironmentVariable("PATHEXT").Split(';').Where(e => e.StartsWith(".")));
            var combinations = paths.SelectMany(x => extensions, (path, extension) => Path.Combine(path, filename + extension));
            var foo = combinations.FirstOrDefault(File.Exists);
            if (foo == null || !foo.Any()) return true;
            else return false;

        }

        public static program smarty(program input)
        {
           input.parameters = smart_split(input.uninstallstring);
           if (invalidate(input.parameters[0])) { input.parameters = basicsplit(input.uninstallstring); }
           else { input.invalid = false; return input; }
           if (ignore.Where(a => a.regkey != input.regkey).Count() == 0) { input.invalid = invalidate(input.parameters[0]); }
           else { input.invalid = false; }
           return input;
        }

        public static string[] basicsplit(string input)
        {
                string[] splitter = { " \"", " /", " -" };
                string[] array = input.Split(splitter, 2, StringSplitOptions.None);
                return array;
        }

        public static string[] smart_split(string input)
        {
            string[] regex =  System.Text.RegularExpressions.Regex.Matches(input, @""".*?""|[^\s]+").Cast<System.Text.RegularExpressions.Match>().Select(m => m.Value).ToArray();
            regex[0] = regex[0].Replace("\"","");
            if (regex.Count() > 2) 
            {
                string firstElem = regex.First();
                string restOfArray = string.Join(" ", regex.Skip(1));
                regex = new string[2];
                regex[0] = firstElem;
                regex[1] = restOfArray;
            }
            return regex;
        }
        #endregion

        #region Sdílené proměnné
        public static updateclass updatedata { get; set; }
        public static string database_ver = "0";
        public static List<program> uninstalllist = new List<program>();
        public static List<program> iconlist = new List<program>();
        public static bool is64bit = Environment.Is64BitOperatingSystem;
        public static bool no_shutdown = false;
        public static bool shutdown = false;

        #endregion

        #region Odesílání na server
        public class send
        {
            public string key { get; set; }
            public string name { get; set; }
        }

        public static void sendtoserver(List<program> tosend)
        {
            List<send> array = new List<send>();
            foreach (program program in tosend)
            {
                send snd = new send();
                snd.key = program.regkey;
                snd.name = program.name;
                array.Add(snd);
            }
            string URL = "http://uninstaller.pryx.net/insert.php";
            WebClient webClient = new WebClient();
            string json = new JavaScriptSerializer().Serialize(new
            {
                array
            });
            NameValueCollection post = new NameValueCollection();
            post["json"] = json;
            webClient.UploadValues(URL, "POST", post);
            webClient.Dispose();
        }

        #endregion

        #region eventy

        public static void fire(System.EventArgs e)
        {
            EventHandler handler = core.keypress;
            handler(null,e);
        }

        public static event EventHandler keypress;
        #endregion

        #region Serializace
        public static string ignorefile = Path.Combine(Application.StartupPath,"ignore.list");

        public static List<program> get_ignored()
        {
            if (File.Exists(ignorefile))
            {
                using (Stream stream = File.Open(ignorefile, FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    return (List<program>)bformatter.Deserialize(stream);
                }
            }
            else return new List<program>();
        }

        class progcomp : IEqualityComparer<program>
        {
            public bool Equals(program p1, program p2)
            {
                return p1.regkey == p2.regkey;
            }

            public int GetHashCode(program p)
            {
                return 1;
            }
        }

        public static void write_ignored(List<program> ignorelist)
        {
           List<program> written = get_ignored();
           List<program> merged = new List<program>();
           merged = (List<program>)ignorelist.Union(written, new progcomp()).ToList();

           File.Delete(ignorefile);
           using (Stream stream = File.Open(ignorefile, FileMode.Create))
           {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                bformatter.Serialize(stream, merged);
            }
        }
        #endregion

    }

}
