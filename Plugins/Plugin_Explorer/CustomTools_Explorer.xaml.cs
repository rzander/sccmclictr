using System;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls.Ribbon;

namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for CustomTools_Explorer.xaml
    /// </summary>
    public partial class CustomTools_Explorer : System.Windows.Controls.UserControl
    {
        public CustomTools_Explorer()
        {
            InitializeComponent();
        }

        private void btC_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (((RibbonButton)sender).Tag != null)
                {
                    string sTag = ((RibbonButton)sender).Tag.ToString();
                    string sShare = "";
                    switch (sTag)
                    {
                        case "C":
                            sShare = "C$";
                            break;
                        case "Admin":
                            sShare = "Admin$";
                            break;
                        case "WBEM":
                            sShare = @"Admin$\System32\wbem";
                            break;
                        case "ccmsetup":
                            sShare = @"Admin$\ccmsetup\logs";
                            break;
                        case "CCMLOGS":
                            sShare = @"Admin$\ccm\logs";
                            break;
                    }
                    Type t = System.Reflection.Assembly.GetEntryAssembly().GetType("ClientCenter.Common", false, true);
                    //Get the Hostname
                    System.Reflection.PropertyInfo pInfo = t.GetProperty("Hostname");
                    string sHost = (string)pInfo.GetValue(null, null);

                    Process Explorer = new Process();
                    Explorer.StartInfo.FileName = "Explorer.exe";
                    Explorer.StartInfo.Arguments = @"\\" + sHost + @"\" + sShare;
                    Explorer.Start();
                }
            }
            catch { }
        }
    }
}
