using System;
using System.Windows;
using System.Diagnostics;
using sccmclictr.automation;

namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AgentActionTool_RDP : System.Windows.Controls.UserControl
    {
        public AgentActionTool_RDP()
        {
            InitializeComponent();
            btOpenRDP.IsEnabled = SCCMCliCtr.Customization.CheckLicense();
            if (!btOpenRDP.IsEnabled)
            {
                btOpenRDP.ToolTip = "Please make a donation to get access to this feature !";
            }
        }

        private void btOpenRDP_Click(object sender, RoutedEventArgs e)
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
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
