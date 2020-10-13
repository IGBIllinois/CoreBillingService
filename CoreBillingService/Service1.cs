using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Management;
using System.Text.RegularExpressions;

namespace CoreBillingService
{
   
    public partial class Service1 : ServiceBase
    { 
        private string deviceName;
        private string coreWebUrl;
        private string coreAuthKey;
        private string session_page = "session.php";
        private string fullCoreWebUrl;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("In OnStart");
            deviceName = System.Environment.MachineName;

            Microsoft.Win32.RegistryKey regKey = Registry.LocalMachine;
            regKey = regKey.OpenSubKey("SOFTWARE\\IGB");
            coreWebUrl = (string)regKey.GetValue("CoreApiUrl");
            coreAuthKey = (string)regKey.GetValue("CoreAuthKey");
            
            if (coreWebUrl.EndsWith("/"))
            {
                fullCoreWebUrl = coreWebUrl + session_page;

            }
            else
            {
                fullCoreWebUrl = coreWebUrl + "/" + session_page;
            }

            // Set up a timer to trigger every minute.
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000; // 60 seconds
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();

        }

        protected override void OnStop()
        {
            string sEvent = "Service stopped";
            EventLog.WriteEntry("Core Billing Service", sEvent);
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.

            string userName = "";

            /*
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT UserName FROM Win32_ComputerSystem");
            ManagementObjectCollection collection = searcher.Get();
            string userName = (string)collection.Cast<ManagementBaseObject>().First()["UserName"];
            */

            userName = getUsername();

            //Set array of parameters to send to REST API
            string[] paramName = new string[] {"key","username"};
            string[] paramValue = new string[] {coreAuthKey,userName};

            //Send to REST API
          
            
            string results = HttpPost(fullCoreWebUrl, paramName, paramValue);

            //Results has a length of 0 or is not null then log the error
            if(results != null || results.Length>0)
            {
                EventLog.WriteEntry("Core Billing Service", results);
            }
        }
      
        static string HttpPost(string url,
    string[] paramName, string[] paramVal)
        {
            HttpWebRequest req = WebRequest.Create(new Uri(url))
                                 as HttpWebRequest;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            // Build a string with all the params, properly encoded.
            // We assume that the arrays paramName and paramVal are
            // of equal length:
            StringBuilder paramz = new StringBuilder();
            for (int i = 0; i < paramName.Length; i++)
            {
                paramz.Append(paramName[i]);
                paramz.Append("=");          
                paramz.Append(paramVal[i]);
                paramz.Append("&");
            }

            // Encode the parameters as form data:
            byte[] formData =
                UTF8Encoding.UTF8.GetBytes(paramz.ToString());
            req.ContentLength = formData.Length;

            // Send the request:
            using (Stream post = req.GetRequestStream())
            {
                post.Write(formData, 0, formData.Length);
            }

            // Pick up the response:
            string result = null;
            using (HttpWebResponse resp = req.GetResponse()
                                          as HttpWebResponse)
            {
                StreamReader reader =
                    new StreamReader(resp.GetResponseStream());
                result = reader.ReadToEnd();
            }

            return result;
        }

        public String getUsername()
        {
            string username = null;
            try
            {
                // Define WMI scope to look for the Win32_ComputerSystem object
                ManagementScope ms = new ManagementScope("\\\\.\\root\\cimv2");
                ms.Connect();

                ObjectQuery query = new ObjectQuery
                        ("SELECT * FROM Win32_ComputerSystem");
                ManagementObjectSearcher searcher =
                        new ManagementObjectSearcher(ms, query);

                // This loop will only run at most once.
                foreach (ManagementObject mo in searcher.Get())
                {
                    // Extract the username
                    username = mo["UserName"].ToString();
                }
                // Remove the domain part from the username
                string[] usernameParts = username.Split('\\');
                // The username is contained in the last string portion.
                username = usernameParts[usernameParts.Length - 1];
            }
            catch (Exception)
            {
                // The system currently has no users who are logged on
                // Set the username to "SYSTEM" to denote that
                username = "SYSTEM";
            }
            
            return username;
        }

        private void InitializeComponent()
        {
            // 
            // Service1
            // 
            this.ServiceName = "CoreBillingService";

        }
    }
}
