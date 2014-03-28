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
using System.ComponentModel;

namespace ClientCenter
{
    /// <summary>
    /// Interaction logic for ServicesGrid.xaml
    /// </summary>
    public partial class PowerSettings : UserControl
    {
        private SCCMAgent oAgent;
        public MyTraceListener Listener;

        public Boolean OnlyCMServices = false;

        public PowerSettings()
        {
            InitializeComponent();
        }
        internal List<sccmclictr.automation.functions.inventory.SMS_PowerSettings> iPWR;

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
                    List<SortDescription> ssort = GetSortInfo(dataGrid1);

                    try
                    {
                        oAgent = value;
                        iPWR = oAgent.Client.Inventory.PowerSettings(false).OrderBy(p=>p.Name).ToList();

                        dataGrid1.BeginInit();
                        dataGrid1.ItemsSource = iPWR;
                        dataGrid1.EndInit();
                    }
                    catch { }

                    SetSortInfo(dataGrid1, ssort);
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
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
