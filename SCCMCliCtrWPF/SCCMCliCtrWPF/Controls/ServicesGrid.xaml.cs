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

namespace ClientCenter
{
    /// <summary>
    /// Interaction logic for ServicesGrid.xaml
    /// </summary>
    public partial class ServicesGrid : UserControl
    {
        private SCCMAgent oAgent;
        public MyTraceListener Listener;

        public Boolean OnlyCMServices = false;

        public ServicesGrid()
        {
            InitializeComponent();
        }
        internal List<sccmclictr.automation.functions.Win32_Service> iServices;

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
                        iServices = oAgent.Client.Services.Win32_Services.OrderBy(t => t.DisplayName).ToList();
                        if(OnlyCMServices)
                            iServices = iServices.Where(t =>Properties.Settings.Default.ServicesHighlited.Contains(t.Name)).ToList();
                        dataGrid1.BeginInit();
                        dataGrid1.ItemsSource = iServices;
                        dataGrid1.EndInit();
                    }
                    catch { }
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
        }

        private void miStartService_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            foreach (sccmclictr.automation.functions.Win32_Service oService in dataGrid1.SelectedItems)
            {
                try
                {
                    oService.StartService();
                    dataGrid1.Items.Refresh();
                }
                catch (Exception ex)
                {
                    Listener.WriteError(ex.Message);
                }
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void miStopService_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            foreach (sccmclictr.automation.functions.Win32_Service oService in dataGrid1.SelectedItems)
            {
                try
                {
                    oService.StopService();
                    dataGrid1.Items.Refresh();

                }
                catch (Exception ex)
                {
                    Listener.WriteError(ex.Message);
                }
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            sccmclictr.automation.functions.Win32_Service item = e.Row.Item as sccmclictr.automation.functions.Win32_Service;
            if (item != null)
            {

                if (Properties.Settings.Default.ServicesHighlited.Contains(item.Name))
                {
                    if (!OnlyCMServices)
                    {
                        e.Row.Background = new SolidColorBrush(Colors.BlanchedAlmond);
                    }
                }

            }
            

        }

        private void dataGrid1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            foreach (sccmclictr.automation.functions.Win32_Service oService in dataGrid1.SelectedItems)
            {
                try
                {
                    Win32_Service WService = oAgent.Client.Services.GetService(oService.Name, true);
                    iServices[iServices.IndexOf(oService)] = WService;
                    dataGrid1.Items.Refresh();
                    break;

                }
                catch (Exception ex)
                {
                    Listener.WriteError(ex.Message);
                }
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void miSetManual_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            foreach (sccmclictr.automation.functions.Win32_Service oService in dataGrid1.SelectedItems)
            {
                try
                {
                    oService.SetStartup_Manual();
                    oService.StartMode = "Manual";
                    dataGrid1.Items.Refresh();
                }
                catch (Exception ex)
                {
                    Listener.WriteError(ex.Message);
                }
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void miSetDisable_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            foreach (sccmclictr.automation.functions.Win32_Service oService in dataGrid1.SelectedItems)
            {
                try
                {
                    oService.SetStartup_Disabled();
                    oService.StartMode = "Disabled";
                    dataGrid1.Items.Refresh();
                }
                catch (Exception ex)
                {
                    Listener.WriteError(ex.Message);
                }
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void miSetAuto_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            foreach (sccmclictr.automation.functions.Win32_Service oService in dataGrid1.SelectedItems)
            {
                try
                {
                    oService.SetStartup_Auto();
                    oService.StartMode = "Auto";
                    dataGrid1.Items.Refresh();
                }
                catch (Exception ex)
                {
                    Listener.WriteError(ex.Message);
                }
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }
    }
}
