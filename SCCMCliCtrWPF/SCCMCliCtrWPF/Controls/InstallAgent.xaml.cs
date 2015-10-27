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
using System.Windows.Shapes;

namespace ClientCenter
{
    /// <summary>
    /// Interaction logic for InstallAgent.xaml
    /// </summary>
    public partial class InstallAgent : Window
    {
        public InstallAgent()
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(Properties.Settings.Default.AgentInstallSiteCode))
            {
                tb_SiteCode.Text = Properties.Settings.Default.AgentInstallSiteCode;
            }

            if (!string.IsNullOrEmpty(Properties.Settings.Default.AgentInstallMP))
            {
                tb_MPName.Text = Properties.Settings.Default.AgentInstallMP;
            }

            tbInstallPS.Text = Properties.Settings.Default.AgentInstallPS;

            tb_SiteCode_LostFocus(this, null);
            tb_MPName_LostFocus(this, null);

            
            sc1.Height = (7 * tbInstallPS.Text.Split('\n').Count()) ;
            if (sc1.Height > 100)
                sc1.Height = sc1.Height - 30;
            
        }

        public void RefreshMPandSiteCode()
        {
            tb_SiteCode_LostFocus(this, null);
            tb_MPName_LostFocus(this, null);
        }

        private void tb_SiteCode_LostFocus(object sender, RoutedEventArgs e)
        {
            int iStart = tbInstallPS.Text.IndexOf("$CMSiteCode");
            if (iStart >= 0)
            {
                int iEnd = tbInstallPS.Text.IndexOf("\n", iStart) + 1;

                tbInstallPS.Text = tbInstallPS.Text.Remove(iStart, iEnd - iStart);
                tbInstallPS.Text = tbInstallPS.Text.Insert(iStart, "$CMSiteCode='" + tb_SiteCode.Text + "'\n");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(tb_SiteCode.Text) & !string.IsNullOrEmpty(tb_MPName.Text))
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("SiteCode and ManagementPoint cannot be empty !", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void tb_MPName_LostFocus(object sender, RoutedEventArgs e)
        {
            int iStart = tbInstallPS.Text.IndexOf("$CMMP");
            if (iStart >= 0)
            {
                int iEnd = tbInstallPS.Text.IndexOf("\n", iStart) + 1;

                tbInstallPS.Text = tbInstallPS.Text.Remove(iStart, iEnd - iStart);
                tbInstallPS.Text = tbInstallPS.Text.Insert(iStart, "$CMMP='" + tb_MPName.Text + "'\n");
            }
        }

    }
}
