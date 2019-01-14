using System;
using System.Windows;

using sccmclictr.automation;

namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AgentActionTool_FEP_Enable : System.Windows.Controls.UserControl
    {
        public SCCMAgent oAgent;
        
        public AgentActionTool_FEP_Enable()
        {
            InitializeComponent();
            btQuickScan.IsEnabled = SCCMCliCtr.Customization.CheckLicense();
            if (!btQuickScan.IsEnabled)
            {
                btQuickScan.ToolTip = "Please make a donation to get access to this feature !";
            }
        }

        private void btQuickScan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Type t = System.Reflection.Assembly.GetEntryAssembly().GetType("ClientCenter.Common", false, true);
                System.Reflection.PropertyInfo pInfo = t.GetProperty("Agent");
                oAgent = (SCCMAgent)pInfo.GetValue(null, null);
                string sHost = oAgent.TargetHostname;

                oAgent.Client.GetStringFromPS("Set-MpPreference -DisableRealtimeMonitoring $false");
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
