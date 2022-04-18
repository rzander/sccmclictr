﻿using System;
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
    public partial class SWAllUpdatesGrid : UserControl
    {
        private SCCMAgent oAgent;
        public MyTraceListener Listener;
        internal List<sccmclictr.automation.functions.softwareupdates.CCM_UpdateStatus> iUpdates;

        public SWAllUpdatesGrid()
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

                                if (cb_Filter.IsChecked == true)
                                {
                                    //Remove duplicates, load only the first time...
                                    iUpdates = oAgent.Client.SoftwareUpdates.UpdateStatus.GroupBy(a => new { a.Article, a.Bulletin, a.ProductID }).Select(y => y.First()).OrderBy(t => t.Article).ToList();
                                }
                                else
                                {
                                    iUpdates = oAgent.Client.SoftwareUpdates.UpdateStatus.OrderBy(t => t.Article).ToList();
                                }
                                dataGrid1.BeginInit();
                                dataGrid1.ItemsSource = iUpdates;
                                dataGrid1.EndInit();
                            }
                            catch { }
                        }
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
                if (cb_Filter.IsChecked == true)
                {
                    iUpdates = oAgent.Client.SoftwareUpdates.GetUpdateStatus(true).GroupBy(a => new { a.Article, a.Bulletin, a.ProductID }).Select(y => y.First()).OrderBy(t => t.Article).ToList();
                }
                else
                {
                    iUpdates = oAgent.Client.SoftwareUpdates.GetUpdateStatus(true).OrderBy(t => t.Article).ToList();
                }

                dataGrid1.BeginInit();
                dataGrid1.ItemsSource = iUpdates;
                dataGrid1.EndInit();
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_ReloadMissing_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                Listener.Filter = new SourceFilter("test");
                //iUpdates = oAgent.Client.SoftwareUpdates.UpdateStatus.GroupBy(a => new { a.Article, a.Bulletin, a.ProductID }).Select(y => y.First()).Where(u => u.Status == "Missing").OrderBy(t => t.Article).ToList();
                if (cb_Filter.IsChecked == true)
                {
                    iUpdates = oAgent.Client.SoftwareUpdates.UpdateStatus.GroupBy(a => new { a.Article, a.Bulletin, a.ProductID }).Select(y => y.First()).Where(u => u.Status == "Missing").OrderBy(t => t.Article).ToList();
                }
                else
                {
                    iUpdates = oAgent.Client.SoftwareUpdates.UpdateStatus.Where(u => u.Status == "Missing").OrderBy(t => t.Article).ToList();
                }
                dataGrid1.BeginInit();
                dataGrid1.ItemsSource = iUpdates;
                dataGrid1.EndInit();
                Listener.Filter = null;
                //Fake message (we use the content from cache :-)
                Listener.WriteLine(" get-wmiobject -query \"SELECT * FROM CCM_UpdateStatus\" -namespace \"root\\ccm\\SoftwareUpdates\\UpdatesStore\" | where {$_.status -eq \"Missing\"}");

            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Listener.Filter = null;
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
                if (!oAgent.ConnectIPC_)
                    oAgent.ConnectIPC_ = true;

                string LogPath = oAgent.Client.AgentProperties.LocalSCCMAgentLogPath.Replace(':', '$');
                Explorer.StartInfo.Arguments = @"\\" + oAgent.TargetHostname + "\\" + LogPath + "\\" + "UpdatesDeployment.log";


                Explorer.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                Explorer.Start();
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;

        }

        private void Bt_OpenWUALog_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                Process Explorer = new Process();
                Explorer.StartInfo.FileName = "Explorer.exe";

                //Connect IPC$ if not already connected (not needed with integrated authentication)
                if (!oAgent.ConnectIPC_)
                    oAgent.ConnectIPC_ = true;

                string LogPath = oAgent.Client.AgentProperties.LocalSCCMAgentLogPath.Replace(':', '$');
                Explorer.StartInfo.Arguments = @"\\" + oAgent.TargetHostname + "\\" + LogPath + "\\" + "WUAHandler.log";


                Explorer.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                Explorer.Start();
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void cb_Filter_Checked(object sender, RoutedEventArgs e)
        {
            if (oAgent != null)
            {
                iUpdates = oAgent.Client.SoftwareUpdates.UpdateStatus.GroupBy(a => new { a.Article, a.Bulletin, a.ProductID }).Select(y => y.First()).OrderBy(t => t.Article).ToList();
                dataGrid1.Columns[8].Visibility = System.Windows.Visibility.Hidden;
                dataGrid1.Columns[9].Visibility = System.Windows.Visibility.Hidden;
                dataGrid1.BeginInit();
                dataGrid1.ItemsSource = iUpdates;
                dataGrid1.EndInit();
            }
        }

        private void cb_Filter_Unchecked(object sender, RoutedEventArgs e)
        {
            if (oAgent != null)
            {

                iUpdates = oAgent.Client.SoftwareUpdates.UpdateStatus.OrderBy(t => t.Article).ToList();
                dataGrid1.Columns[8].Visibility = System.Windows.Visibility.Visible;
                dataGrid1.Columns[9].Visibility = System.Windows.Visibility.Visible;
                dataGrid1.BeginInit();
                dataGrid1.ItemsSource = iUpdates;
                dataGrid1.EndInit();
            }
        }
    }
}
