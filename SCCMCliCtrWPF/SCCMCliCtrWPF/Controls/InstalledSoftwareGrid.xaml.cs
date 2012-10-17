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
                        oAgent = value;
                        iInstalledSW = oAgent.Client.Inventory.InstalledSoftware.OrderBy(t => t.ProductName).ToList();

                        dataGrid1.BeginInit();
                        dataGrid1.ItemsSource = iInstalledSW;
                        dataGrid1.EndInit();
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

        private void miInstallApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Install all selected Updates;
                List<softwaredistribution.CCM_Application> lApps = new List<softwaredistribution.CCM_Application>();
                foreach (softwaredistribution.CCM_Application cApp in dataGrid1.SelectedItems)
                {
                    lApps.Add(cApp);
                    cApp.Install(softwaredistribution.AppPriority.Normal, false);
                }
                //oAgent.Client.SoftwareUpdates.InstallUpdates(lUpdates);
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
                //Install all selected Updates;
                List<softwaredistribution.CCM_Application> lApps = new List<softwaredistribution.CCM_Application>();
                foreach (softwaredistribution.CCM_Application cApp in dataGrid1.SelectedItems)
                {
                    lApps.Add(cApp);
                    cApp.Uninstall(softwaredistribution.AppPriority.Normal, false);
                }
                //oAgent.Client.SoftwareUpdates.InstallUpdates(lUpdates);
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
                    cmSWInstall.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    cmSWInstall.Visibility = System.Windows.Visibility.Visible;
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
