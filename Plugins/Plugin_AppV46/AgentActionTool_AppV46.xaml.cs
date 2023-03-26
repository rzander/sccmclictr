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

using System.Windows.Forms;

using sccmclictr.automation;

namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AgentActionTool_AppV46 : System.Windows.Controls.UserControl
    {
        public SCCMAgent oAgent;
        public AgentActionTool_AppV46()
        {
            InitializeComponent();
            //btAppV46.IsEnabled = SCCMCliCtr.Customization.CheckLicense();
        }

        private void btAppV46_Click(object sender, RoutedEventArgs e)
        {
            AppVForm fAppv = new AppVForm();
            Type t = System.Reflection.Assembly.GetEntryAssembly().GetType("ClientCenter.Common", false, true);
            System.Reflection.PropertyInfo pInfo = t.GetProperty("Agent");
            oAgent = (SCCMAgent)pInfo.GetValue(null, null);
            fAppv.CMAgent = oAgent;
            fAppv.Show();
        }


    }
}
