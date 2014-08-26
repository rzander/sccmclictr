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
using System.Reflection;

namespace ClientCenter.Controls
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();

            tbLicense.Text = Properties.Resources.License;
            lVersion.Content = "Version: " + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString(); 
            lAssembly.Content = "Assembly: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=TLTFJHYA69VHU");
                e.Handled = true;
            }
            catch { }
        }

        public bool MSG
        {
            get { return spMSG.IsVisible; }
            set 
            {
                if (value)
                    spMSG.Visibility = System.Windows.Visibility.Visible;
                else
                    spMSG.Visibility = System.Windows.Visibility.Hidden;
            }
            
        }

    }
}
