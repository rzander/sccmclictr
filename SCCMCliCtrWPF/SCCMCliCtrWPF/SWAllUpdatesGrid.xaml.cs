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
                        oAgent = value;
                       
                        //Remove duplicates
                        iUpdates = oAgent.Client.SoftwareUpdates.UpdateStatus.GroupBy(a => new { a.Article, a.Bulletin, a.ProductID }).Select(y => y.First()).OrderBy(t => t.Article).ToList(); 

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
        }

        private void bt_InstallAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //oAgent.Client.SoftwareUpdates.InstallAllRequiredUpdates();
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
        }

        private void bt_Reload_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                //iUpdates = oAgent.Client.SoftwareUpdates.UpdateStatus.GroupBy(a => new { a.ProductID }).Where(grp => grp.Count() > 1).SelectMany(grp => grp.Select(r => r)).ToList();
                iUpdates = oAgent.Client.SoftwareUpdates.UpdateStatus.GroupBy(a => new { a.Article, a.Bulletin, a.ProductID }).Select(y => y.First()).OrderBy(t=>t.Article).ToList(); 
                //iUpdates = oAgent.Client.SoftwareUpdates.UpdateStatus.OrderBy(t => t.Article).ToList();
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
    }
}
