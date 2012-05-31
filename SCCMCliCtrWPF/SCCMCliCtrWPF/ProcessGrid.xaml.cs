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
    /// Interaction logic for ProcessGrid.xaml
    /// </summary>
    public partial class ProcessGrid : UserControl
    {
        private SCCMAgent oAgent;
        public MyTraceListener Listener;

        public ProcessGrid()
        {
            InitializeComponent();
        }

        internal List<sccmclictr.automation.functions.Win32_Process> iProcesses;
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
                        iProcesses = oAgent.Client.Process.Win32_Processes.Where(t=>t.ProcessId > 0).OrderBy(t => t.Name).ToList();
                        dataGrid1.BeginInit();
                        dataGrid1.ItemsSource = iProcesses;
                        dataGrid1.EndInit();
                    }
                    catch { }
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
        }
    }
}
