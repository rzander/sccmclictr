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
using System.ComponentModel;

namespace ClientCenter.Controls
{
    /// <summary>
    /// Interaction logic for ApplicationGrid.xaml
    /// </summary>
    public partial class ApplicationGrid : UserControl
    {
        private SCCMAgent oAgent;
        public MyTraceListener Listener;
        internal List<sccmclictr.automation.functions.softwaredistribution.CCM_Application> iApplications;

        public ApplicationGrid()
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

                        if (oAgent != value)
                        {
                            oAgent = value;
                            try
                            {
                                if (Properties.Settings.Default.HideNonUserUIExperienceApplicattions)
                                {
                                    List<softwaredistribution.CCM_Application> oList = oAgent.Client.SoftwareDistribution.Applications_(false).Where(t => t.UserUIExperience == true).ToList();
                                    iApplications = oList.GroupBy(t => t.Id).Select(grp => grp.FirstOrDefault()).OrderBy(o => o.FullName).ToList();
                                }
                                else
                                {
                                    List<softwaredistribution.CCM_Application> oList = oAgent.Client.SoftwareDistribution.Applications_(false).ToList();
                                    iApplications = oList.GroupBy(t => t.Id).Select(grp => grp.FirstOrDefault()).OrderBy(o => o.FullName).ToList();
                                }
                                //TEST.Source = BitMapConvert.ToBitmapImage(iApplications[0].IconAsImage);

                                dataGrid1.BeginInit();
                                dataGrid1.ItemsSource = iApplications;
                                dataGrid1.EndInit();
                            }
                            catch { }
                        }
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
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                List<SortDescription> ssort = GetSortInfo(dataGrid1);

                if (Properties.Settings.Default.HideNonUserUIExperienceApplicattions)
                {
                    List<softwaredistribution.CCM_Application> oList = oAgent.Client.SoftwareDistribution.Applications_(true).Where(t => t.UserUIExperience == true).ToList();
                    iApplications = oList.GroupBy(t => t.Id).Select(grp => grp.FirstOrDefault()).OrderBy(o => o.FullName).ToList();
                }
                else
                {
                    List<softwaredistribution.CCM_Application> oList = oAgent.Client.SoftwareDistribution.Applications_(true).ToList();
                    iApplications = oList.GroupBy(t => t.Id).Select(grp => grp.FirstOrDefault()).OrderBy(o => o.FullName).ToList();
                }

                dataGrid1.BeginInit();
                dataGrid1.ItemsSource = iApplications;
                dataGrid1.EndInit();

                SetSortInfo(dataGrid1, ssort);
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void miInstallApp_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                //Install all selected Updates;
                List<softwaredistribution.CCM_Application> lApps = new List<softwaredistribution.CCM_Application>();
                foreach (softwaredistribution.CCM_Application cApp in dataGrid1.SelectedItems)
                {
                    lApps.Add(cApp);
                    cApp.Install(softwaredistribution.AppPriority.Normal, false);
                }
                //oAgent.Client.SoftwareUpdates.InstallUpdates(lUpdates);
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void MiRepairApp_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                //Install all selected Updates;
                List<softwaredistribution.CCM_Application> lApps = new List<softwaredistribution.CCM_Application>();
                foreach (softwaredistribution.CCM_Application cApp in dataGrid1.SelectedItems)
                {
                    lApps.Add(cApp);
                    cApp.Repair(softwaredistribution.AppPriority.Normal, false);
                }
                //oAgent.Client.SoftwareUpdates.InstallUpdates(lUpdates);
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void miUnInstallApp_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                //Install all selected Updates;
                List<softwaredistribution.CCM_Application> lApps = new List<softwaredistribution.CCM_Application>();
                foreach (softwaredistribution.CCM_Application cApp in dataGrid1.SelectedItems)
                {
                    lApps.Add(cApp);
                    cApp.Uninstall(softwaredistribution.AppPriority.Normal, false);
                }
                //oAgent.Client.SoftwareUpdates.InstallUpdates(lUpdates);
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        //Does not work currently; Cancel command must be triggered from local System account and even then the App does not stop.
        private void miCancelApp_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                //Cancel all selected app;
                List<softwaredistribution.CCM_Application> lApps = new List<softwaredistribution.CCM_Application>();
                foreach (softwaredistribution.CCM_Application cApp in dataGrid1.SelectedItems)
                {
                    lApps.Add(cApp);
                    cApp.Cancel();
                }
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void miDownloadContent_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                //Cancel all selected app;
                List<softwaredistribution.CCM_Application> lApps = new List<softwaredistribution.CCM_Application>();
                foreach (softwaredistribution.CCM_Application cApp in dataGrid1.SelectedItems)
                {
                    lApps.Add(cApp);
                    cApp.DownloadContents();
                }
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
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

        private void bt_ImportApp_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                ImportApp oSWForm = new ImportApp();
                List<ComboboxItem> cbItems = new List<ComboboxItem>();
                foreach (var oApp in oAgent.Client.SoftwareDistribution.ApplicationCatalog("", ""))
                {
                    ComboboxItem cbItem = new ComboboxItem();
                    cbItem.DisplayValue = oApp.Name;
                    cbItem.InternalValue = oApp.ApplicationId;

                    cbItems.Add(cbItem);
                }

                oSWForm.cb_Apps.ItemsSource = cbItems;
                oSWForm.cb_Apps.DisplayMemberPath = "DisplayValue";
                oSWForm.cb_Apps.SelectedValuePath = "InternalValue";

                oSWForm.ShowDialog();

                if (oSWForm.DialogResult.HasValue && oSWForm.DialogResult.Value)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    string sAppID = oSWForm.tbAppID.Text;
                    oAgent.Client.AgentActions.ImportApplicationPolicy(sAppID);
                }
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        public class ComboboxItem
        {
            public string DisplayValue { get; set; }
            public string InternalValue { get; set; }
        }


    }
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (!string.IsNullOrEmpty(value as string))
                {
                    switch (value as string)
                    {
                        case "Updates":
                            return new BitmapImage(new Uri("pack://application:,,,/SCCMCliCtrWPF;component/Images/Computer_protection.ico"));
                        default:
                            return BitMapConvert.ToBitmapImage(common.Base64ToImage(value as string) as System.Drawing.Image) as BitmapImage;
                    }
                }
                else
                {
                    return new BitmapImage(new Uri("pack://application:,,,/SCCMCliCtrWPF;component/Images/Icon065.ico"));
                }
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
