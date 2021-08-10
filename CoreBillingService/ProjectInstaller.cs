using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Diagnostics;

namespace CoreBillingService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            //serviceInstaller1.AfterInstall += (sender, args) => new ServiceController(serviceInstaller1.ServiceName).Start();
            

        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            new ServiceController(serviceInstaller1.ServiceName).Start();
            var serviceName = "CoreBillingService";
            var resetAfter = 60000;
            Process.Start("cmd.exe", $"/c sc failure \"{serviceName}\" reset= 0 actions= restart/{resetAfter}/restart/{resetAfter}/restart/{resetAfter}");


        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
