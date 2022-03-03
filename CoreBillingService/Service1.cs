using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Management;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Web.Script.Serialization;



namespace CoreBillingService
{
   

    public partial class Service1 : ServiceBase
    { 
        private string deviceName;
        private string coreApiUrl;
        private string coreAuthKey;
        private string coreDeviceId;
        private string session_page = "api/v1/index.php";
        private string noun = "session";
        private string fullCoreWebUrl;
        private System.Timers.Timer timer;
        private System.Diagnostics.EventLog log;
        private string releaseId;
        private string defaultUser = "SYSTEM";
        public Service1()
        {
            InitializeComponent();
            this.AutoLog = false;
            log = new System.Diagnostics.EventLog();
            
            if (!System.Diagnostics.EventLog.SourceExists("CoreBillingService"))
            {
                System.Diagnostics.EventLog.CreateEventSource("CoreBillingService", "Application");
            }
            log.Source = "CoreBillingService";
            log.Log = "Application";
            releaseId = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "").ToString();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                log.WriteEntry("Starting Service");
                deviceName = System.Environment.MachineName;
                Microsoft.Win32.RegistryKey regKey = Registry.LocalMachine;
                regKey = regKey.OpenSubKey("SOFTWARE\\CoreBillingService");
                coreApiUrl = (string)regKey.GetValue("CoreApiUrl");
                coreAuthKey = (string)regKey.GetValue("CoreAuthKey");
                coreDeviceId = (string)regKey.GetValue("CoreDeviceId");
                if (coreApiUrl == "" || coreApiUrl == null || coreAuthKey == "" || coreAuthKey == null || coreDeviceId == "" || coreDeviceId == null)
                {
                    log.WriteEntry("CoreApiUrl, CoreAuthKey, or CoreDeviceId registry keys are not set.  The registry location is at HKLM\\Software\\CoreBillingService.", EventLogEntryType.Error);

                }

                if (coreApiUrl.EndsWith("/"))
                {
                    fullCoreWebUrl = coreApiUrl + session_page + "/" + noun + "/" + coreDeviceId;

                }
                else
                {
                    fullCoreWebUrl = coreApiUrl + "/" + session_page + "/" + noun + "/" + coreDeviceId;
                }

                FastUserSwitchingEnabled();
                // Set up a timer to trigger every minute.
                timer = new System.Timers.Timer();
                timer.Interval = 60000; // 60 seconds
                timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
                timer.Start();
            }
            catch (Exception ex)
            {
                log.WriteEntry(ex.Message, EventLogEntryType.Error);
            }

        }

        protected override void OnStop()
        {
            if (timer != null && timer.Enabled)
            {
                timer.Stop();
                timer.Dispose();
                log.WriteEntry("Stopping Service");
            }
           
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            string userName = getUsername();
            OutputJson outputJson_obj = new OutputJson {
                key = coreAuthKey,
                username = userName,
                os = getWindowsVersion()
            };
            
            outputJson_obj.version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            outputJson_obj.computer_name = System.Environment.MachineName;

            outputJson_obj.user_switching = FastUserSwitchingEnabled();
            string ipaddress = getIPAddress();
            outputJson_obj.ipaddress = ipaddress;
            outputJson_obj.hard_drives = getHardDrives();

            string json_serialized = new JavaScriptSerializer().Serialize(outputJson_obj);
            
            


            //Send to REST API

            if ((fullCoreWebUrl != "") && (coreAuthKey != "")) {
                String results = HttpPost(fullCoreWebUrl, json_serialized);
            

                //If Result is not null
                if (results != null) {

                    ReceivedJson jsonResult = new JavaScriptSerializer().Deserialize<ReceivedJson>(results);
 
                    if (userName != defaultUser)
                    {
                        
                        if (jsonResult.result)
                        {
                            log.WriteEntry(jsonResult.message);
                        }
                        else if (!jsonResult.result)
                        {
                            log.WriteEntry(jsonResult.message, EventLogEntryType.Error);
                        }
                        
                    }
                    
                }
                else {
                    log.WriteEntry("Not able to contact web service at " + coreApiUrl, EventLogEntryType.Error);
                }
            }
            else {
                log.WriteEntry("CoreApiUrl, CoreAuthKey, or CoreDeviceID registry keys are not set.  The registry location is at HKLM\\Software\\CoreBillingService.", EventLogEntryType.Error);
            }

 
        }
      
        private String HttpPost(string url,string json_data)
        {
            HttpWebRequest req = WebRequest.Create(new Uri(url))
                                 as HttpWebRequest;
            req.Method = "POST";
            req.ContentType = "application/json; charset=UTF-8";
            req.Accept = "application/json";

            string encoded_auth = System.Convert.ToBase64String(Encoding.Default.GetBytes(coreDeviceId + ":" + coreAuthKey));
           
            req.Headers.Add("Authorization", "Basic " + encoded_auth);
            

            // Encode the parameters as form data:
            byte[] postBytes = UTF8Encoding.UTF8.GetBytes(json_data);
            req.ContentLength = postBytes.Length;

            // Send the request:
            using (Stream post = req.GetRequestStream())
            {
                post.Write(postBytes, 0, postBytes.Length);
            }

            

            // Pick up the response:
            string result = null;
            try
            {
                using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(resp.GetResponseStream());
                    result = reader.ReadToEnd();
                    if (resp.StatusCode != HttpStatusCode.OK)
                    {
                        log.WriteEntry("HTTP Code: " + (int)resp.StatusCode + " for " + fullCoreWebUrl, EventLogEntryType.Error);
                        return null;
                    }

                }
            }
            catch (WebException we)
            {
                log.WriteEntry("HTTP Code: " + (int)((HttpWebResponse)we.Response).StatusCode + " for " + fullCoreWebUrl, EventLogEntryType.Error);
                return null;
            }
            return result;

        }

        private string getUsername()
        {
            return getUserNameByProcess();   
        }
        private String getLocalUsername()
        {
            string username = null;
            try
            {
                // Define WMI scope to look for the Win32_ComputerSystem object
                //ManagementScope ms = new ManagementScope("\\\\.\\root\\cimv2");
                ManagementScope ms = new ManagementScope(@"\\.\root\cimv2");
                ms.Connect();

                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_ComputerSystem");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(ms, query);

                
                // This loop will only run at most once.
                foreach (ManagementObject mo in searcher.Get())
                {
                    
                    // Extract the username
                    username = mo["UserName"].ToString();
                    // Remove the domain part from the username
                    string[] usernameParts = username.Split('\\');
                    // The username is contained in the last string portion.
                    username = usernameParts[usernameParts.Length - 1];
                    
                    
                }
            }
            catch (ManagementException ex)
            {
                log.WriteEntry("Error" + ex.StackTrace.ToString(), EventLogEntryType.Error);
                log.WriteEntry("Error: " + ex.Message, EventLogEntryType.Error);
            }
            catch (Exception)
            {
                // The system currently has no users who are logged on
                // Set the username to "SYSTEM" to denote that
                username = defaultUser;
                
            }
            
            return username;
        }

        private String getUserNameByProcess()
        {
            try
            {
                SelectQuery query = new SelectQuery(@"Select * from Win32_Process");
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (System.Management.ManagementObject Process in searcher.Get())
                    {
                        
                        
                        if (Process.GetPropertyValue("Name") != null &&
                            string.Equals(Path.GetFileName(Process.GetPropertyValue("Name").ToString()), "explorer.exe", StringComparison.OrdinalIgnoreCase))
                        {
                            
                            string[] OwnerInfo = new string[] { string.Empty, string.Empty };
                            int returnVal = Convert.ToInt32(Process.InvokeMethod("GetOwner", (object[])OwnerInfo));
                            if (returnVal == 0)
                            {
                                return OwnerInfo[0];
                            }
 
                        }
                    }
                }
                return defaultUser;
            }
            catch (Exception ex)
            {
                log.WriteEntry(ex.Message, EventLogEntryType.Error);
                return defaultUser;
            }


        }
        private void InitializeComponent()
        {
            // 
            // Service1
            // 
            this.ServiceName = "CoreBillingService";

        }

        private string getIPAddress()
        {

            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            IPAddress local_host = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            return local_host.ToString();

        }
        

        private Boolean FastUserSwitchingEnabled()
        {
            try
            {

                string value = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "HideFastUserSwitching", "0").ToString();
                
                if ((value == null) || (Convert.ToInt32(value) == 0))
                {
                    log.WriteEntry("Fast User switching is enabled.  Please disable to properly track users.",EventLogEntryType.Warning);
                    return true;

                }
                return false;
            }
            catch (Exception)
            {
                log.WriteEntry("Fast User switching is enabled or not detected.  Please disable to properly track users.", EventLogEntryType.Warning);
                return true;
            }
        }

        private string getWindowsVersion()
        {
            
            string windows_version = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "").ToString();
            
            //For Windows 7 - gets service pack
            if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion", null) != null) 
            {
                windows_version += " " + Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion", "").ToString();

            }
            //For Windows 10 - gets Windows 10 version ie 1909, 2004,21H2, etc
            else if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "DisplayVersion", null) != null)
            {
                windows_version += " " + Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "DisplayVersion", "").ToString();
            }

            if (Environment.Is64BitOperatingSystem)
            {
                windows_version += " x64";
            }
            else
            {
                windows_version += " x32";
            }

            return windows_version;
        }
        private List<HardDrive> getHardDrives()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            List<HardDrive> HardDrives = new List<HardDrive>();

            foreach (DriveInfo d in allDrives)
            {
                if(d.DriveType.ToString() == "Fixed")
                {
                    
                    HardDrive Drive = new HardDrive
                    {
                        
                        volume = d.Name.Substring(0,1),
                        size = d.TotalSize,
                        free = d.TotalFreeSpace
                    };
                    HardDrives.Add(Drive);

                }

            }
            return HardDrives;
        }
    }

    public class HardDrive
    {
        public string volume { get; set; }
        public long size { get; set; }
        public long free { get; set; }


    }
    public class OutputJson
    {
        public string key { get; set; }
        public string username { get; set; }
        public string os { get; set; }
        public string version { get; set; }
        public string computer_name { get; set; }
        public bool user_switching { get; set; }
        public string ipaddress { get; set; }
        public List<HardDrive> hard_drives { get; set; }

    }

    public class ReceivedJson
    {
        public bool result { get; set; }
        public string message { get; set; }

    }
}
