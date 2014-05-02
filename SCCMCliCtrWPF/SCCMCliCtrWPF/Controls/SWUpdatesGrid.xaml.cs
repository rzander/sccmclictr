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
    /// Interaction logic for SWUpdatesGrid.xaml
    /// </summary>
    public partial class SWUpdatesGrid : UserControl
    {
        private SCCMAgent oAgent;
        public MyTraceListener Listener;
        internal List<sccmclictr.automation.functions.softwareupdates.CCM_SoftwareUpdate> iUpdates;

        public SWUpdatesGrid()
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

                        iUpdates = oAgent.Client.SoftwareUpdates.SoftwareUpdate.OrderBy(t => t.ArticleID).ToList();
                        dataGrid1.BeginInit();
                        dataGrid1.ItemsSource = iUpdates;
                        dataGrid1.EndInit();
                    }
                    catch { }
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
        }

        private void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            softwareupdates.CCM_SoftwareUpdate item = e.Row.Item as softwareupdates.CCM_SoftwareUpdate;
            if (item != null)
            {
            }
        }

        private void miInstallUpdate_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                //Install all selected Updates;
                List<softwareupdates.CCM_SoftwareUpdate> lUpdates = new List<softwareupdates.CCM_SoftwareUpdate>();
                foreach (softwareupdates.CCM_SoftwareUpdate cUpdate in dataGrid1.SelectedItems)
                {
                    lUpdates.Add(cUpdate);
                }
                oAgent.Client.SoftwareUpdates.InstallUpdates(lUpdates);
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_InstallAll_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                oAgent.Client.SoftwareUpdates.InstallAllRequiredUpdates();
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_Reload_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                this.iUpdates = oAgent.Client.SoftwareUpdates.GetSoftwareUpdate(true).OrderBy(t => t.ArticleID).ToList();
                dataGrid1.BeginInit();
                dataGrid1.ItemsSource = iUpdates;
                dataGrid1.EndInit();
            }
            catch(Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_InstallAllApproved_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                oAgent.Client.SoftwareUpdates.InstallAllApprovedUpdates();
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_OpenUpdateLog_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                Process Explorer = new Process();
                Explorer.StartInfo.FileName = "Explorer.exe";

                //Connect IPC$ if not already connected (not needed with integrated authentication)
                if (!oAgent.ConnectIPC)
                    oAgent.ConnectIPC = true;

                Explorer.StartInfo.Arguments = @"\\" + oAgent.TargetHostname + @"\admin$\WindowsUpdate.log";


                Explorer.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                Explorer.Start();
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }
    }
}
