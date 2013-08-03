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
                        //TEST.Source = BitMapConvert.ToBitmapImage(iApplications[0].IconAsImage);

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
                string sVal = ((System.Windows.Controls.TextBox)(e.EditingElement)).Text;
                string sRelPath = ((sccmclictr.automation.functions.softwaredistribution.CCM_SoftwareDistribution)(dataGrid1.SelectedItem))._RawObject.Properties["__NAMESPACE"].Value.ToString() + ":" + ((sccmclictr.automation.functions.softwaredistribution.CCM_SoftwareDistribution)(dataGrid1.SelectedItem))._RawObject.Properties["__RELPATH"].Value.ToString().Replace("\"", "'");

                oAgent.Client.SetProperty(sRelPath, sProp, "'" + sVal + "'");
            }
        }



    }

    //[ValueConversion(typeof(Image), typeof(BitmapImage))]
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return BitMapConvert.ToBitmapImage(common.Base64ToImage(value as string) as System.Drawing.Image) as BitmapImage;
            }
            catch { }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    static class BitMapConvert
    {
        public static BitmapImage ToBitmapImage(this System.Drawing.Image image)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                image.Save(ms, image.RawFormat);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                return bi;
            }
            catch { }

            return null;
        }
    }


}
