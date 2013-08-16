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
                        oAgent = value;
                        iBaselines = oAgent.Client.DCM.DCMBaselines.OrderBy(t => t.DisplayName).ToList(); //SoftwareDistribution.Applications.GroupBy(t => t.Id).Select(grp => grp.FirstOrDefault()).OrderBy(o => o.FullName).ToList();

                        dataGrid1.BeginInit();
                        dataGrid1.ItemsSource = iBaselines;
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
            iBaselines = oAgent.Client.DCM.DCMBaselines.OrderBy(t => t.DisplayName).ToList();

            dataGrid1.BeginInit();
            dataGrid1.ItemsSource = iBaselines;
            dataGrid1.EndInit();
        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                dcm.SMS_DesiredConfiguration oDCM = ((DataGrid)e.OriginalSource).SelectedItem as dcm.SMS_DesiredConfiguration;
                List<dcm.SMS_DesiredConfiguration.ConfigItem> oCIs = oDCM.ConfigItems().OrderBy(t => t.CIName).ToList(); ;

                //System.Collections.ObjectModel.ObservableCollection<wmiProp> oColl = new System.Collections.ObjectModel.ObservableCollection<wmiProp>();
                //oDCM._RawObject.Properties.Where(w => w.GetType() == typeof(PSProperty) & !w.Name.StartsWith("_")).ToList().ForEach(x => oColl.Add(new wmiProp(x.Name, x.Value, x.TypeNameOfValue)));


                dataGrid2.BeginInit();
                dataGrid2.ItemsSource = oCIs;
                dataGrid2.EndInit();
            }
            catch { }
        }
        public class wmiProp
        {
            private string _Value;
            public wmiProp(string sKey, string sValue, string oType)
            {
                Property = sKey;
                TypeName = oType.Replace("System.", "").Replace("Deserialized.", "");
                if (!string.IsNullOrEmpty(sValue))
                    Value = sValue;
                else
                    Value = "";
            }

            public wmiProp(string sKey, object oValue, string oType)
            {
                Property = sKey;
                TypeName = oType.Replace("System.", "").Replace("Deserialized.", "");
                if (oValue != null)
                    Value = oValue.ToString();
                else
                    Value = "";
            }
            public string Property { get; set; }
            public string Value
            {
                get
                {
                    return _Value;
                }
                set
                {
                    _Value = value;

                    //Check if it's in a DateTime Format
                    if (value.Length == 25)
                    {
                        if (value[14] == '.')
                        {
                            _Value = System.Management.ManagementDateTimeConverter.ToDateTime(value).ToString();
                        }
                    }
                }
            }
            public string TypeName { get; set; }
        }

        private void miEvaluateBaseline_Click(object sender, RoutedEventArgs e)
        {
            foreach(dcm.SMS_DesiredConfiguration oBaseline in dataGrid1.SelectedItems)
            {
                string sJob = "";
                oBaseline.TriggerEvaluation(true, out sJob);
            }
        }


    }
}
