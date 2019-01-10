using System;
using System.Windows;
using System.Diagnostics;

using System.Runtime.InteropServices;

using sccmclictr.automation;
using Microsoft.Win32;
using System.IO;

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
            btOpenCMRemote.IsEnabled = SCCMCliCtr.Customization.CheckLicense();
            if (!btOpenCMRemote.IsEnabled)
            {
                btOpenCMRemote.ToolTip = "Please make a donation to get access to this feature !";
            }

            if (!File.Exists(System.IO.Path.Combine(SCCMConsolePath, @"bin\i386\CmRcViewer.exe")))
            {
                btOpenCMRemote.IsEnabled = false;
                btOpenCMRemote.ToolTip = "ConfigMgr Admin Console is not installed !";
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

        private void btOpenCMRemote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Type t = System.Reflection.Assembly.GetEntryAssembly().GetType("ClientCenter.Common", false, true);
                System.Reflection.PropertyInfo pInfo = t.GetProperty("Agent");
                SCCMAgent oAgent = (SCCMAgent)pInfo.GetValue(null, null);
                string sHost = oAgent.TargetHostname;

                Process pRemote = new Process();

                pRemote.StartInfo.FileName = Properties.Settings.Default.RemoteCommand;
                if (Properties.Settings.Default.RequireAdminConsole)
                    pRemote.StartInfo.WorkingDirectory = System.IO.Path.Combine(SCCMConsolePath, @"bin\i386\");
                pRemote.StartInfo.Arguments = string.Format(Properties.Settings.Default.RemoteArgument, sHost);
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
