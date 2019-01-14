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

        private void rb_DEMO_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Type t = System.Reflection.Assembly.GetEntryAssembly().GetType("ClientCenter.Common", false, true);
                //Get the Hostname
                System.Reflection.PropertyInfo pInfo = t.GetProperty("Hostname");

                //System.Reflection.PropertyInfo pInfo = t.GetProperty("Agent");
                //SCCMAgent oAgent = (SCCMAgent)pInfo.GetValue(null, null);
                //string sHost = oAgent.TargetHostname;

                string sHost = (string)pInfo.GetValue(null, null);

                //Custom Code here...
                
                MessageBox.Show(sHost);
                
                //...
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
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
