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

        public SWUpdatesGrid()
        {
            InitializeComponent();
        }

        internal List<sccmclictr.automation.functions.softwareupdates.CCM_SoftwareUpdate> iUpdates;
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
    }
}
