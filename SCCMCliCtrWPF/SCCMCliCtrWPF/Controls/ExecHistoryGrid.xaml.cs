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
    /// Interaction logic for ExecHistoryGrid.xaml
    /// </summary>
    public partial class ExecHistoryGrid : UserControl
    {
        private SCCMAgent oAgent;
        public MyTraceListener Listener;

        public ExecHistoryGrid()
        {
            InitializeComponent();
        }

        internal List<sccmclictr.automation.functions.softwaredistribution.REG_ExecutionHistory> iHistory;
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

                        iHistory = oAgent.Client.SoftwareDistribution.ExecutionHistory.OrderBy(t => t._RunStartTime).ToList();
                        dataGrid1.BeginInit();
                        dataGrid1.ItemsSource = iHistory;
                        dataGrid1.EndInit();
                    }
                    catch { }
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
        }

        private void miDeleteItems_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                foreach (softwaredistribution.REG_ExecutionHistory oReg in dataGrid1.SelectedItems)
                {
                    oReg.Delete();
                    iHistory.Remove(oReg);
                }

                //iHistory = oAgent.Client.SoftwareDistribution.ExecutionHistory.OrderBy(t => t._RunStartTime).ToList();
                dataGrid1.BeginInit();
                dataGrid1.ItemsSource = null;
                dataGrid1.ItemsSource = iHistory;
                dataGrid1.EndInit();
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow; 
        }

        private void miResolveSID_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                foreach (softwaredistribution.REG_ExecutionHistory oReg in dataGrid1.SelectedItems)
                {
                    //oReg.GetUserFromSID();
                    iHistory.Single(t=>t == oReg).GetUserFromSID();
                }


                //iHistory = oAgent.Client.SoftwareDistribution.ExecutionHistory.OrderBy(t => t._RunStartTime).ToList();
                dataGrid1.BeginInit();
                dataGrid1.ItemsSource = null;
                dataGrid1.ItemsSource = iHistory;
                dataGrid1.EndInit();
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow; 
        }
    }
}
