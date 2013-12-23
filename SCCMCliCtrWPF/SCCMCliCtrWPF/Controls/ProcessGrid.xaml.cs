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

        internal List<sccmclictr.automation.functions.ExtProcess> iProcesses;
        
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
                        
                        iProcesses = oAgent.Client.Process.ExtProcesses(false).Where(t=>t.ProcessId > 4).OrderBy(t => t.Name).ToList();
                        dataGrid1.BeginInit();
                        dataGrid1.ItemsSource = iProcesses;
                        dataGrid1.EndInit();
                    }
                    catch { }
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
        }

        private void miStartService_Click(object sender, RoutedEventArgs e)
        {
            foreach (ExtProcess oProc in dataGrid1.SelectedItems)
            {
                oProc.Terminate();
                ((DataGridRow)dataGrid1.ItemContainerGenerator.ContainerFromItem(oProc)).Background = new SolidColorBrush(Colors.BlanchedAlmond);
                ((DataGridRow)dataGrid1.ItemContainerGenerator.ContainerFromItem(oProc)).FontStyle = FontStyles.Italic;
                ((DataGridRow)dataGrid1.ItemContainerGenerator.ContainerFromItem(oProc)).Foreground = new SolidColorBrush(Colors.LightGray);

            }
        }

        private void bt_RunCommand_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                UInt32? oResult = oAgent.Client.Process.CreateProcess(tb_Command.Text);
                if (oResult == null)
                    Listener.WriteError("Unable to create process.");
                else
                {
                    Listener.WriteLine("Process started with Id:" + oResult.ToString());
                    sccmclictr.automation.functions.ExtProcess oNewProc = oAgent.Client.Process.GetExtProcess(oResult.ToString());
                    iProcesses.Add(oNewProc);
                    iProcesses.GroupBy(t => t.Name);

                    dataGrid1.Items.Refresh(); 
                }
            }
            catch(Exception ex)
            {
                ex.Message.ToString();
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            sccmclictr.automation.functions.ExtProcess item = e.Row.Item as sccmclictr.automation.functions.ExtProcess;
            if (item != null)
            {
                if (DateTime.Now - item.CreationDate < new TimeSpan(0,0,10))
                {
                    e.Row.Background = new SolidColorBrush(Colors.LightGreen);
                }
            }
        }

        private void bt_Reload_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            iProcesses = oAgent.Client.Process.ExtProcesses(true).Where(t => t.ProcessId > 4).OrderBy(t => t.Name).ToList();
            dataGrid1.BeginInit();
            dataGrid1.ItemsSource = iProcesses;
            dataGrid1.EndInit();
            Mouse.OverrideCursor = Cursors.Arrow;
        }
    }
}
