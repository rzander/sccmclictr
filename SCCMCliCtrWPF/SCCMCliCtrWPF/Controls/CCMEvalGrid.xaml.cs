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
    public partial class CCMEvalGrid : UserControl
    {
        private SCCMAgent oAgent;
        public MyTraceListener Listener;
        internal List<sccmclictr.automation.functions.health.ccmeval> iCCMEval;

        public CCMEvalGrid()
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

                                List<sccmclictr.automation.functions.health.ccmeval> oList = oAgent.Client.Health.GetCCMEvalStatus();
                                iCCMEval = oList.OrderBy(o => o.Description).ToList();

                                dataGrid1.BeginInit();
                                dataGrid1.ItemsSource = iCCMEval;
                                dataGrid1.EndInit();

                                lLastDate.Content = oAgent.Client.Health.LastCCMEval.ToString();
                            }
                            catch { }
                        }
                    }
                    catch (Exception ex)
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
                List<SortDescription> ssort = GetSortInfo(dataGrid1);

                List<sccmclictr.automation.functions.health.ccmeval> oList = oAgent.Client.Health.GetCCMEvalStatus();
                iCCMEval = oList.OrderBy(o => o.Description).ToList();


                dataGrid1.BeginInit();
                dataGrid1.ItemsSource = iCCMEval;
                dataGrid1.EndInit();

                SetSortInfo(dataGrid1, ssort);

                lLastDate.Content = oAgent.Client.Health.LastCCMEval.ToString();
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
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

        private void bt_ReRun_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                oAgent.Client.Health.RunCCMEval();
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

    }
}
