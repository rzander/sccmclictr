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
using System.Management.Automation;

namespace ClientCenter.Controls
{
    /// <summary>
    /// Interaction logic for SettingsMgmt.xaml
    /// </summary>
    public partial class SettingsMgmt : UserControl
    {
        private SCCMAgent oAgent;
        public MyTraceListener Listener;
        internal List<sccmclictr.automation.functions.dcm.SMS_DesiredConfiguration> iBaselines;

        public SettingsMgmt()
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
                                iBaselines = oAgent.Client.DCM.DCMBaselines.OrderBy(t => t.DisplayName).ToList(); //SoftwareDistribution.Applications.GroupBy(t => t.Id).Select(grp => grp.FirstOrDefault()).OrderBy(o => o.FullName).ToList();

                                dataGrid1.BeginInit();
                                dataGrid1.ItemsSource = iBaselines;
                                dataGrid1.EndInit();
                            }
                            catch { }
                        }
                    }
                    catch(Exception ex)
                    {
                        dataGrid1.ItemsSource = null;
                        dataGrid1.Items.Clear();
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
                iBaselines = oAgent.Client.DCM.DCMBaselines.OrderBy(t => t.DisplayName).ToList();

                dataGrid1.BeginInit();
                dataGrid1.ItemsSource = iBaselines;
                dataGrid1.EndInit();
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                dcm.SMS_DesiredConfiguration oDCM = ((DataGrid)e.OriginalSource).SelectedItem as dcm.SMS_DesiredConfiguration;
                List<dcm.SMS_DesiredConfiguration.ConfigItem> oCIs = oDCM.ConfigItems().OrderBy(t => t.CIName).ToList(); ;

                dataGrid2.BeginInit();
                dataGrid2.ItemsSource = oCIs;
                dataGrid2.EndInit();
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void miEvaluateBaseline_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                foreach (dcm.SMS_DesiredConfiguration oBaseline in dataGrid1.SelectedItems)
                {
                    try
                    {
                        string sJob = "";
                        oBaseline.TriggerEvaluation(true, out sJob);
                    }
                    catch { }
                }
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }


    }
}
