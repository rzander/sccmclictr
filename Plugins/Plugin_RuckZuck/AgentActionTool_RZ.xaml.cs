using System;
using System.Windows;
using System.Windows.Controls;
using sccmclictr.automation;

namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AgentActionTool_RuckZuck : UserControl
    {
        public AgentActionTool_RuckZuck()
        {
            InitializeComponent();

            //For Donators only:
            //rb_DEMO.IsEnabled = SCCMCliCtr.Customization.CheckLicense();
        }

        private void rb_Inst_Click(object sender, RoutedEventArgs e)
        {
            Type t = System.Reflection.Assembly.GetEntryAssembly().GetType("ClientCenter.Common", false, true);
            //Get the Hostname
            //System.Reflection.PropertyInfo pInfo = t.GetProperty("Hostname");

            System.Reflection.PropertyInfo pInfo = t.GetProperty("Agent");
            SCCMAgent oAgent = (SCCMAgent)pInfo.GetValue(null, null);

            Install oIntsWin = new Install(oAgent);
            oIntsWin.ShowDialog();
        }

        private void btRZ_Click(object sender, RoutedEventArgs e)
        {
            rb_Inst_Click(sender, e);
        }
    }
}
