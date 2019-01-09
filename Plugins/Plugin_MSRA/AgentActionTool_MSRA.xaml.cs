using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

using System.Runtime.InteropServices;
using System.Windows.Forms;

using sccmclictr.automation;
using Microsoft.Win32;
using System.IO;

namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AgentActionTool_MSRA : System.Windows.Controls.UserControl
    {
        public AgentActionTool_MSRA()
        {
            InitializeComponent();
            btOpenMSRA.IsEnabled = SCCMCliCtr.Customization.CheckLicense();
            if (!btOpenMSRA.IsEnabled)
            {
                btOpenMSRA.ToolTip = "Please make a donation to get access to this feature !";
            }
        }

        private void btOpenMSRA_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Type t = System.Reflection.Assembly.GetEntryAssembly().GetType("ClientCenter.Common", false, true);
                System.Reflection.PropertyInfo pInfo = t.GetProperty("Agent");
                SCCMAgent oAgent = (SCCMAgent)pInfo.GetValue(null, null);
                string sHost = oAgent.TargetHostname;

                Process pRemote = new Process();

                pRemote.StartInfo.FileName = Properties.Settings.Default.RemoteCommand;
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
