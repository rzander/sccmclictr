using System;
using System.Windows;
using System.Diagnostics;

using System.Runtime.InteropServices;
using System.Windows.Forms;

using sccmclictr.automation;

namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AgentActionTool_Regedit : System.Windows.Controls.UserControl
    {
        public AgentActionTool_Regedit()
        {
            InitializeComponent();
            try
            {
                btOpenRegedit.IsEnabled = SCCMCliCtr.Customization.CheckLicense();
                if (!btOpenRegedit.IsEnabled)
                {
                    btOpenRegedit.ToolTip = "Please make a donation to get access to this feature !";
                }
            }
            catch { }
        }

        private void btOpenPSConsole_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Type t = System.Reflection.Assembly.GetEntryAssembly().GetType("ClientCenter.Common", true, true);
                System.Reflection.PropertyInfo pInfo = t.GetProperty("Agent");
                SCCMAgent oAgent = (SCCMAgent)pInfo.GetValue(null, null);
                string sHost = oAgent.TargetHostname;

                Process pRegedit = new Process();
                pRegedit.StartInfo.FileName = "Regedit.exe";
                pRegedit.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                pRegedit.Start();
                int pID = pRegedit.Id;

                System.Threading.Thread.Sleep(1000);
                System.IntPtr MainHandle = Process.GetProcessById(pID).MainWindowHandle;
                SetForegroundWindow(MainHandle);
                System.Threading.Thread.Sleep(100);
                //SendKeys.SendWait("{home}{left}");
                SendKeys.SendWait("%fc");
                System.Threading.Thread.Sleep(600);
                SendKeys.SendWait(string.Format("{0}~", sHost));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        [DllImport("User32.dll", SetLastError = true)]
        public static extern int SetForegroundWindow(IntPtr hwnd);
    }
}
