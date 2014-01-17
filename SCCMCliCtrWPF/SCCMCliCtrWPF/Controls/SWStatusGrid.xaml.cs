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
using System.ComponentModel;

namespace ClientCenter.Controls
{
    /// <summary>
    /// Interaction logic for ApplicationGrid.xaml
    /// </summary>
    public partial class SWStatusGrid : UserControl
    {
        private SCCMAgent oAgent;
        public MyTraceListener Listener;
        internal List<sccmclictr.automation.functions.softwaredistribution.SoftwareStatus> iSoftware;

        public SWStatusGrid()
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
                        //SCCMAgent oAgent = new SCCMAgent("localhost");
                        //List<softwaredistribution.CCM_Application> lApps = oAgent.Client.SoftwareDistribution.Applications.OrderBy(t => t.FullName).ToList();
                        //lApps.ToString();

                        oAgent = value;
                        iSoftware = oAgent.Client.SoftwareDistribution.SoftwareSummary.GroupBy(t => t.Name).Select(grp => grp.FirstOrDefault()).Where(p=>p.Name != "").OrderBy(o => o.Name).ToList();
                        //TEST.Source = BitMapConvert.ToBitmapImage(iApplications[0].IconAsImage);

                        dataGrid1.BeginInit();
                        dataGrid1.ItemsSource = iSoftware;
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
            iSoftware = oAgent.Client.SoftwareDistribution.SoftwareSummary_(true).GroupBy(t => t.Name).Select(grp => grp.FirstOrDefault()).Where(p => p.Name != "").OrderBy(o => o.Name).ToList();

            List<SortDescription> ssort = GetSortInfo(dataGrid1);

            dataGrid1.BeginInit();
            dataGrid1.ItemsSource = iSoftware;
            dataGrid1.EndInit();

            SetSortInfo(dataGrid1, ssort);
        }

        private void bt_ReloadActive_Click(object sender, RoutedEventArgs e)
        {
            iSoftware = oAgent.Client.SoftwareDistribution.SoftwareSummary_(true).GroupBy(t => t.Name).Select(grp => grp.FirstOrDefault()).Where(p => p.Name != "").Where(p=>p.Status != "Installed").OrderBy(o => o.Name).ToList();

            List<SortDescription> ssort = GetSortInfo(dataGrid1);

            dataGrid1.BeginInit();
            dataGrid1.ItemsSource = iSoftware;
            dataGrid1.EndInit();

            SetSortInfo(dataGrid1, ssort);
        }

        List<DataGridColumn> GetColumnInfo(DataGrid dg)
        {
            List<DataGridColumn> columnInfos = new List<DataGridColumn>();
            foreach (var column in dg.Columns)
            {
                columnInfos.Add(column);
            }
            return columnInfos;
        }

        List<SortDescription> GetSortInfo(DataGrid dg)
        {
            List<SortDescription> sortInfos = new List<SortDescription>();
            foreach (var sortDescription in dg.Items.SortDescriptions)
            {
                sortInfos.Add(sortDescription);
            }
            return sortInfos;
        }

        void SetColumnInfo(DataGrid dg, List<DataGridColumn> columnInfos)
        {
            columnInfos.Sort((c1, c2) => { return c1.DisplayIndex - c2.DisplayIndex; });
            foreach (var columnInfo in columnInfos)
            {
                var column = dg.Columns.FirstOrDefault(col => col.Header == columnInfo.Header);
                if (column != null)
                {
                    column.SortDirection = columnInfo.SortDirection;
                    column.DisplayIndex = columnInfo.DisplayIndex;
                    column.Visibility = columnInfo.Visibility;
                }
            }
        }

        void SetSortInfo(DataGrid dg, List<SortDescription> sortInfos)
        {
            dg.Items.SortDescriptions.Clear();
            foreach (var sortInfo in sortInfos)
            {
                dg.Items.SortDescriptions.Add(sortInfo);
            }
        }

    }
}
