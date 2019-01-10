using System;
using System.Windows;
using System.Diagnostics;

using System.Runtime.InteropServices;

using sccmclictr.automation;
using Microsoft.Win32;
using System.IO;
using System.Management;

namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AgentActionTool_CMRemote : System.Windows.Controls.UserControl
    {
        public AgentActionTool_CMRemote()
        {
            InitializeComponent();
            btOpenCMResExplorer.IsEnabled = SCCMCliCtr.Customization.CheckLicense();
            if (!btOpenCMResExplorer.IsEnabled)
            {
                btOpenCMResExplorer.ToolTip = "Please make a donation to get access to this feature !";
            }

            if (!File.Exists(System.IO.Path.Combine(SCCMConsolePath, @"bin\ResourceExplorer.exe")))
            {
                btOpenCMResExplorer.IsEnabled = false;
                btOpenCMResExplorer.ToolTip = "ConfigMgr Admin Console is missing !";
            }
        }

        public string SCCMConsolePath
        {
            get
            {
                try
                {
                    //RegistryKey rAdminUI = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\ConfigMgr\Setup");
                    string sArchitecture = System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE").ToLower();
                    RegistryKey rAdminUI = null;
                    switch (sArchitecture)
                    {
                        case "x86":
                            rAdminUI = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\ConfigMgr10\Setup");
                            break;
                        case "amd64":
                            rAdminUI = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\ConfigMgr10\Setup");
                            break;
                        case "ia64":
                            rAdminUI = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\ConfigMgr10\Setup");
                            break;
                    }
                    if (rAdminUI != null)
                    {
                        string sUIPath = rAdminUI.GetValue("UI Installation Directory", "").ToString();
                        if (Directory.Exists(sUIPath))
                        {
                            return sUIPath;
                        }
                    }
                }
                catch { }
                return "";
            }
        }

        [DllImport("User32.dll", SetLastError = true)]
        public static extern int SetForegroundWindow(IntPtr hwnd);

        public string SCCMServer
        {
            get
            {
                try
                {
                    string sArchitecture = System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE").ToLower();
                    RegistryKey rAdminCon = null;
                    switch (sArchitecture)
                    {
                        case "x86":
                            rAdminCon = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\ConfigMgr10\AdminUI\Connection");
                            break;
                        case "amd64":
                            rAdminCon = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\ConfigMgr10\AdminUI\Connection");
                            break;
                        case "ia64":
                            rAdminCon = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\ConfigMgr10\AdminUI\Connection");
                            break;
                    }
                    if (rAdminCon != null)
                    {
                        string sServer = rAdminCon.GetValue("Server", "").ToString();
                        return sServer;
                    }
                }
                catch { }
                return "";
            }
        }

        public string SCCMSiteCode(string sServer)
        {
            try
            {
                ManagementObjectSearcher MOS = new ManagementObjectSearcher(string.Format(@"\\{0}\root\sms", sServer), "SELECT * FROM SMS_ProviderLocation");
                foreach (ManagementObject MO in MOS.Get())
                {
                    return MO["SiteCode"].ToString();
                }
            }
            catch { }
            return "";
        }

        private void btOpenCMResExplorer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Type t = System.Reflection.Assembly.GetEntryAssembly().GetType("ClientCenter.Common", false, true);
                System.Reflection.PropertyInfo pInfo = t.GetProperty("Agent");
                SCCMAgent oAgent = (SCCMAgent)pInfo.GetValue(null, null);
                string sHost = oAgent.TargetHostname.Split('.')[0];

                Process pRemote = new Process();

                string sServer = SCCMServer.Replace(@"\", "");
                string sSiteCode = SCCMSiteCode(sServer);

                pRemote.StartInfo.FileName = Properties.Settings.Default.RemoteCommand;
                if (Properties.Settings.Default.RequireAdminConsole)
                    pRemote.StartInfo.WorkingDirectory = System.IO.Path.Combine(SCCMConsolePath, @"bin\");
                pRemote.StartInfo.Arguments = string.Format(Properties.Settings.Default.RemoteArgument, sHost, sServer, sSiteCode);
                pRemote.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                pRemote.Start();

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
