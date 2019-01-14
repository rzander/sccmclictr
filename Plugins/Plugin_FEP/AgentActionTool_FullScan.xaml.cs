using System;
using System.Windows;

using sccmclictr.automation;

namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AgentActionTool_FEPFullScan : System.Windows.Controls.UserControl
    {
        public SCCMAgent oAgent;
        public AgentActionTool_FEPFullScan()
        {
            InitializeComponent();
            btFullScan.IsEnabled = SCCMCliCtr.Customization.CheckLicense();
            if (!btFullScan.IsEnabled)
            {
                btFullScan.ToolTip = "Please make a donation to get access to this feature !";
            }
        }

        private void btFullScan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Type t = System.Reflection.Assembly.GetEntryAssembly().GetType("ClientCenter.Common", false, true);
                System.Reflection.PropertyInfo pInfo = t.GetProperty("Agent");
                oAgent = (SCCMAgent)pInfo.GetValue(null, null);
                string sHost = oAgent.TargetHostname;

                oAgent.Client.GetStringFromPS("Start-MpScan -ScanType FullScan -AsJob");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
