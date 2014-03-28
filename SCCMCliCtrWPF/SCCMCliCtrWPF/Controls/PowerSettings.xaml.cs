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
                    try
                    {
                        oAgent = value;
                        iPWR = oAgent.Client.Inventory.PowerSettings(false).OrderBy(p=>p.Name).ToList();

                        dataGrid1.BeginInit();
                        dataGrid1.ItemsSource = iPWR;
                        dataGrid1.EndInit();
                    }
                    catch { }
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
        }

    }
}
