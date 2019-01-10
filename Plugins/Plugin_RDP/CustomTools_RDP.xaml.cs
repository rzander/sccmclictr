using System;
using System.Windows;
using System.Diagnostics;

namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for CustomTools__CMRemote.xaml
    /// </summary>
    public partial class CustomTools_RDP : System.Windows.Controls.UserControl
    {
        public CustomTools_RDP()
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
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
