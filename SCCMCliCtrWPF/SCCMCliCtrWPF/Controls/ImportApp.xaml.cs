using sccmclictr.automation.functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for ImportApp.xaml
    /// </summary>
    public partial class ImportApp : Window
    {
        public ImportApp()
        {
            InitializeComponent();
        }

        private void bt_Done_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
        private void cb_Apps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbAppID.Text = cb_Apps.SelectedValue as string;
        }

        private void tbAppID_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(string.IsNullOrEmpty(tbAppID.Text))
            {
                bt_Done.IsEnabled = false;
            }
            else
            {
                bt_Done.IsEnabled = true;
            }
        }
    }
}
