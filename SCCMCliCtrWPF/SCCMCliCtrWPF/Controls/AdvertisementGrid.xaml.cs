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
    /// Interaction logic for ApplicationGrid.xaml
    /// </summary>
    public partial class AdvertisementGrid : UserControl
    {
        private SCCMAgent oAgent;
        public MyTraceListener Listener;
        internal List<sccmclictr.automation.functions.softwaredistribution.CCM_SoftwareDistribution> iAdvertisements;
        public static List<sccmclictr.automation.functions.softwaredistribution.REG_ExecutionHistory> lProgs;

        public AdvertisementGrid()
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

                        iAdvertisements = oAgent.Client.SoftwareDistribution.Advertisements.GroupBy(t => t.ADV_AdvertisementID).Select(grp => grp.FirstOrDefault()).OrderBy(o => o.PKG_Name).ThenBy(o => o.PKG_version).ToList();

                        dataGrid1.BeginInit();
                        dataGrid1.ItemsSource = iAdvertisements;
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
            iAdvertisements = oAgent.Client.SoftwareDistribution.Advertisements.GroupBy(t => t.ADV_AdvertisementID).Select(grp => grp.FirstOrDefault()).OrderBy(o => o.PKG_Name).ThenBy(o => o.PKG_version).ToList();

            dataGrid1.BeginInit();
            dataGrid1.ItemsSource = iAdvertisements;
            dataGrid1.EndInit();
        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                softwaredistribution.CCM_SoftwareDistribution oSW = ((DataGrid)e.OriginalSource).SelectedItem as softwaredistribution.CCM_SoftwareDistribution;

                System.Collections.ObjectModel.ObservableCollection<wmiProp> oColl = new System.Collections.ObjectModel.ObservableCollection<wmiProp>();
                oSW._RawObject.Properties.Where(w => w.GetType() == typeof(PSProperty) & !w.Name.StartsWith("_")).ToList().ForEach(x => oColl.Add(new wmiProp(x.Name, x.Value, x.TypeNameOfValue)));
                
                //softwaredistribution.CCM_Scheduler_ScheduledMessage oSchedule = oSW._ScheduledMessage();

                dataGrid2.BeginInit();
                dataGrid2.ItemsSource = oColl;
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

        private void dataGrid2_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

            if (e.EditAction == DataGridEditAction.Commit)
            {
                string sProp = ((wmiProp)e.Row.Item).Property.ToString();
                string sType = ((wmiProp)e.Row.Item).TypeName.ToString();
                string sVal = ((System.Windows.Controls.TextBox)(e.EditingElement)).Text;
                string sRelPath = ((sccmclictr.automation.functions.softwaredistribution.CCM_SoftwareDistribution)(dataGrid1.SelectedItem))._RawObject.Properties["__NAMESPACE"].Value.ToString() + ":" + ((sccmclictr.automation.functions.softwaredistribution.CCM_SoftwareDistribution)(dataGrid1.SelectedItem))._RawObject.Properties["__RELPATH"].Value.ToString().Replace("\"", "'");
                switch (sType.ToLower())
                {
                    case "string":
                        try
                        {
                            DateTime oDate = DateTime.Parse(sVal);
                            string sDate = System.Management.ManagementDateTimeConverter.ToDmtfDateTime(oDate);
                            oAgent.Client.SetProperty(sRelPath, sProp, "'" + sDate + "'");
                        }
                        catch
                        {
                            oAgent.Client.SetProperty(sRelPath, sProp, "'" + sVal + "'");
                        }
                        break;
                    case "boolean":
                        oAgent.Client.SetProperty(sRelPath, sProp, "$" + sVal);
                        break;
                    case "uint32":
                        oAgent.Client.SetProperty(sRelPath, sProp, sVal);
                        break;
                    case "string[]":
                        oAgent.Client.SetProperty(sRelPath, sProp, "@({" + sVal + "})");
                        break;
                }

            }
        }

        private void miInstallApp_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            foreach (softwaredistribution.CCM_SoftwareDistribution oSW in dataGrid1.SelectedItems)
            {
                try
                {
                    //Enforce ReRun Always, otherwise Adv may not start a 2nd time
                    if (!oSW.ADV_RepeatRunBehavior.StartsWith("RerunAlways", StringComparison.CurrentCultureIgnoreCase) | !((oSW.ADV_MandatoryAssignments.HasValue) ? oSW.ADV_MandatoryAssignments.Value : false))
                    {
                        oSW.TriggerSchedule(true);
                        oSW.ADV_RepeatRunBehavior = "RerunAlways";
                        oSW.ADV_MandatoryAssignments = true;
                    }
                    else
                    {
                        oSW.TriggerSchedule(true);
                    }
                }
                catch { }
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_ReloadStatus_Click(object sender, RoutedEventArgs e)
        {
            lProgs = oAgent.Client.SoftwareDistribution.ExecutionHistory;

            dataGrid1.BeginInit();
            dataGrid1.ItemsSource = iAdvertisements;
            dataGrid1.EndInit();
        }

    }

    public class StatusConverter : IMultiValueConverter
    {
        //Error	2	'ClientCenter.Controls.StatusConverter' does not implement interface member 'System.Windows.Data.IMultiValueConverter.ConvertBack(object, System.Type[], object, System.Globalization.CultureInfo)'	D:\SkyDrive\Dokumente\Visual Studio 2010\Project_ClientCenter\SCCMCliCtr_WPF\SCCMCliCtrWPF\Controls\AdvertisementGrid.xaml.cs	203	18	SCCMCliCtr

        public object Convert(object[] value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                //return BitMapConvert.ToBitmapImage(common.Base64ToImage(value as string) as System.Drawing.Image) as BitmapImage;
                string sAdvID = value[2].ToString();
                string sPkgID = value[0].ToString();
                string sProgID = value[1].ToString();

                if (AdvertisementGrid.lProgs != null)
                {
                    if (AdvertisementGrid.lProgs.Count > 0)
                    {
                        string sStatus = AdvertisementGrid.lProgs.Single(t => t.PackageID == sPkgID & t._ProgramID == sProgID)._State;
                        if (sStatus.StartsWith("Success", StringComparison.CurrentCultureIgnoreCase))
                        {
                            BitmapImage logo = new BitmapImage();
                            logo.BeginInit();
                            logo.UriSource = new Uri("pack://application:,,,/SCCMCliCtrWPF;component/Images/Flag 4.ico");
                            logo.EndInit();

                            return logo;

                        }
                        else
                        {
                            BitmapImage logo = new BitmapImage();
                            logo.BeginInit();
                            logo.UriSource = new Uri("pack://application:,,,/SCCMCliCtrWPF;component/Images/Flag.ico");
                            logo.EndInit();

                            return logo;
                        }
                    }
                }


            }
            catch { }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
