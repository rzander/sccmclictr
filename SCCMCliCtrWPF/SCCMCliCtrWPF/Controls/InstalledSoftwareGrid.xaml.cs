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
using sccmclictr.automation.functions;
using System.IO;
using System.Globalization;

namespace ClientCenter.Controls
{
    /// <summary>
    /// Interaction logic for InstalledSoftwareGrid.xaml
    /// </summary>
    public partial class InstalledSoftwareGrid : UserControl
    {
        private SCCMAgent oAgent;
        public MyTraceListener Listener;
        internal List<sccmclictr.automation.functions.inventory.AI_InstalledSoftwareCache> iInstalledSW;

        public InstalledSoftwareGrid()
        {
            InitializeComponent();
        }

        public SCCMAgent SCCMAgentConnection
        {
            get
            {
                return oAgent;
            }
            set
            {
                if (value.isConnected)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    try
                    {
                        if (oAgent != value)
                        {
                            oAgent = value;
                            try
                            {
                                iInstalledSW = oAgent.Client.Inventory.InstalledSoftware.OrderBy(t => t.ProductName).ToList();

                                dataGrid1.BeginInit();
                                dataGrid1.ItemsSource = iInstalledSW;
                                dataGrid1.EndInit();
                            }
                            catch { }
                        }
                    }
                    catch(Exception ex)
                    {
                        ex.Message.ToString();
                    }
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
        }


        private void bt_Reload_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                iInstalledSW = oAgent.Client.Inventory.InstalledSoftware.OrderBy(t => t.ProductName).ToList();

                dataGrid1.BeginInit();
                dataGrid1.ItemsSource = iInstalledSW;
                dataGrid1.EndInit();
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void miRepairApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (inventory.AI_InstalledSoftwareCache cApp in dataGrid1.SelectedItems)
                {
                    string sResult = cApp.Repair();
                    sResult.ToString();
                }
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
        }

        private void miUnInstallApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (inventory.AI_InstalledSoftwareCache cApp in dataGrid1.SelectedItems)
                {
                    string sResult = cApp.Uninstall(); 
                    sResult.ToString();
                }
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            sccmclictr.automation.functions.inventory.AI_InstalledSoftwareCache oSW = dataGrid1.SelectedItem as sccmclictr.automation.functions.inventory.AI_InstalledSoftwareCache;
            if (oSW != null)
            {
                if (!oSW.SoftwareCode.StartsWith("{"))
                {
                    cmSWInstall.IsOpen = false;
                }
                else
                {
                }
            }
        }

        private void dataGrid1_KeyDown(object sender, KeyEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid.Items.Count == 0 || e.Key < Key.A || e.Key > Key.Z)
            {
                return;
            }

            foreach(sccmclictr.automation.functions.inventory.AI_InstalledSoftwareCache oItem in dataGrid.Items)
            {
                if (oItem.ARPDisplayName.StartsWith(e.Key.ToString(), StringComparison.CurrentCultureIgnoreCase))
                {
                    dataGrid.SelectedItem = oItem;
                    dataGrid.ScrollIntoView(dataGrid.SelectedItem);
                    return;
                }
            }
        }
    }

    public class SWListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                BitmapImage logo = new BitmapImage();

                if ((value as string).StartsWith("{") & (value as string).Length == 38)
                {
                    logo.BeginInit();
                    logo.UriSource = new Uri("pack://application:,,,/SCCMCliCtrWPF;component/Images/msi.ico");
                    logo.EndInit();
                    return logo;
                }
                else
                {
                    logo.BeginInit();
                    logo.UriSource = new Uri("pack://application:,,,/SCCMCliCtrWPF;component/Images/setupold.ico");
                    logo.EndInit();
                    return logo;
                }


            }
            catch { }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }


}
