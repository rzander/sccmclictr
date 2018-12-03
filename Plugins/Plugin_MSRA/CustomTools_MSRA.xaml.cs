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

using System.Runtime.InteropServices;
using System.Windows.Forms;
using sccmclictr.automation;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;

namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for CustomTools__CMRemote.xaml
    /// </summary>
    public partial class CustomTools_MSRA : System.Windows.Controls.UserControl
    {
        public CustomTools_MSRA()
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
                //Get the Hostname
                System.Reflection.PropertyInfo pInfo = t.GetProperty("Hostname");
                string sHost = (string)pInfo.GetValue(null, null);

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
