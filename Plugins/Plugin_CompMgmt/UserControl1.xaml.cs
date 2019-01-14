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

namespace WpfControlLibrary1
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        public static SCCMAgent oAgent;

        public object GETControl()
        {
            return btOpenCompMgmt;
        }

        private void btOpenPSConsole_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("TEST");
            Type t = System.Reflection.Assembly.GetEntryAssembly().GetType("SMSCliCtrV2.Common", true, true);
            System.Reflection.PropertyInfo pInfo = t.GetProperty("Hostname");
            string sHost = (string)pInfo.GetValue(null, null);
            
            Process COMPMGMT = new Process();
            COMPMGMT.StartInfo.FileName = Environment.ExpandEnvironmentVariables("compmgmt.msc");
            COMPMGMT.StartInfo.Arguments = string.Format("/computer:{0}", sHost);
            COMPMGMT.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            COMPMGMT.Start();

        }
    }
}
