using System;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using sccmclictr.automation;

namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AgentActionTool_CompMgmt : UserControl
    {
        public AgentActionTool_CompMgmt()
        {
            InitializeComponent();
            btOpenCompMgmt.IsEnabled = SCCMCliCtr.Customization.CheckLicense();
            if(!btOpenCompMgmt.IsEnabled)
            {
                btOpenCompMgmt.ToolTip = "Please make a donation to get access to this feature !";
            }
        }

        private void btOpenPSConsole_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Type t = System.Reflection.Assembly.GetEntryAssembly().GetType("ClientCenter.Common", false, true);
                System.Reflection.PropertyInfo pInfo = t.GetProperty("Agent");
                SCCMAgent oAgent = (SCCMAgent)pInfo.GetValue(null, null);
                string sHost = oAgent.TargetHostname;
                Process COMPMGMT = new Process();
                COMPMGMT.StartInfo.FileName = Environment.ExpandEnvironmentVariables("compmgmt.msc");
                COMPMGMT.StartInfo.Arguments = string.Format("/computer:{0}", sHost);
                COMPMGMT.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                COMPMGMT.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
