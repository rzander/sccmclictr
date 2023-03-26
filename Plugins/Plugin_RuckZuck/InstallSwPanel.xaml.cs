using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

using System.Diagnostics;
using System.Threading;
using System.Management.Automation;
using System.Globalization;
using System.Collections.ObjectModel;
using sccmclictr.automation;
using System.Threading.Tasks;
using RuckZuck.Base;

namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for InstallSwPanel.xaml
    /// </summary>
    public partial class InstallSwPanel : UserControl
    {
        public SCCMAgent oAgent;
        public string sAuthToken;
        public string sInternalURL;
        public List<GetSoftware> lAllSoftware;
        public List<AddSoftware> lSoftware = new List<AddSoftware>();
        public System.Timers.Timer tSearch = new System.Timers.Timer(1000);
        delegate void AnonymousDelegate();
        public event EventHandler OnSWUpdated = delegate { };

        public delegate void ChangedEventHandler(object sender, EventArgs e);
        //public event ChangedEventHandler onEdit;

        public InstallSwPanel()
        {
            InitializeComponent();
            tSearch.Elapsed += TSearch_Elapsed;
            tSearch.Enabled = false;
            tSearch.AutoReset = false;

            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                RZScan oSCAN = new RZScan(false, false);
                
                Task.Run(() => oSCAN.GetSWRepositoryAsync(new CancellationTokenSource(15000).Token)).Wait();

                lAllSoftware = oSCAN.SoftwareRepository;

                List<GetSoftware> oDBCat = new List<GetSoftware>();
                PropertyGroupDescription PGD = new PropertyGroupDescription("", new ShortnameToCategory());

                foreach (GetSoftware oSW in oSCAN.SoftwareRepository)
                {
                    try
                    {
                        if (oSW.Categories.Count > 1)
                        {
                            foreach (string sCAT in oSW.Categories)
                            {
                                try
                                {

                                    //Check if SW is already installed
                                    if (lSoftware.FirstOrDefault(t => t.ProductName == oSW.ProductName & t.ProductVersion == oSW.ProductVersion) != null)
                                    {
                                        GetSoftware oNew = new GetSoftware() { Categories = new List<string> { sCAT }, Description = oSW.Description, Downloads = oSW.Downloads, IconHash = oSW.IconHash, SWId = oSW.SWId, Manufacturer = oSW.Manufacturer, ProductName = oSW.ProductName, ProductURL = oSW.ProductURL, ProductVersion = oSW.ProductVersion,  ShortName = oSW.ShortName, isInstalled = true };
                                        oDBCat.Add(oNew);
                                    }
                                    else
                                    {
                                        GetSoftware oNew = new GetSoftware() { Categories = new List<string> { sCAT }, Description = oSW.Description, Downloads = oSW.Downloads, IconHash = oSW.IconHash, SWId = oSW.SWId, Manufacturer = oSW.Manufacturer, ProductName = oSW.ProductName, ProductURL = oSW.ProductURL, ProductVersion = oSW.ProductVersion, ShortName = oSW.ShortName, isInstalled = false };
                                        oDBCat.Add(oNew);
                                    }
                                }
                                catch { }
                            }
                        }
                        else
                        {
                            //Check if SW is already installed
                            if (lSoftware.FirstOrDefault(t => t.ProductName == oSW.ProductName & t.ProductVersion == oSW.ProductVersion) != null)
                            {
                                oDBCat.Add(new GetSoftware() { Categories = oSW.Categories, Description = oSW.Description, Downloads = oSW.Downloads, IconHash = oSW.IconHash, SWId = oSW.SWId, Manufacturer = oSW.Manufacturer, ProductName = oSW.ProductName, ProductURL = oSW.ProductURL, ProductVersion = oSW.ProductVersion, ShortName = oSW.ShortName, isInstalled = true });
                            }
                            else
                            {
                                oDBCat.Add(new GetSoftware() { Categories = oSW.Categories, Description = oSW.Description, Downloads = oSW.Downloads, IconHash = oSW.IconHash, SWId = oSW.SWId, Manufacturer = oSW.Manufacturer, ProductName = oSW.ProductName, ProductURL = oSW.ProductURL, ProductVersion = oSW.ProductVersion, ShortName = oSW.ShortName, isInstalled = false });
                            }
                        }
                    }
                    catch { }
                }

                ListCollectionView lcv = new ListCollectionView(oDBCat.ToList());

                foreach (var o in RZRestAPIv2.GetCategories(oSCAN.SoftwareRepository))
                {
                    PGD.GroupNames.Add(o);
                }

                lcv.GroupDescriptions.Add(PGD);

                lvSW.ItemsSource = lcv;

            }
            catch { }
            Mouse.OverrideCursor = null;
        }

        private void OSCAN_OnSWRepoLoaded(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TSearch_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            AnonymousDelegate update = delegate ()
            {
                tbSearch_LostFocus(sender, null);
            };
            Dispatcher.Invoke(update);

        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }
            catch { }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private string GetTimeToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(time.Concat(key).ToArray());
        }

        private void tbSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            tbSearch.Foreground = new SolidColorBrush(Colors.Black);
            if (tbSearch.Tag != null)
                tbSearch.Text = "";
        }

        private void tbSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (string.IsNullOrEmpty(tbSearch.Text))
            {
                tbSearch.Foreground = new SolidColorBrush(Colors.LightGray);
                tbSearch.Tag = "Search";
                tbSearch.Text = "Search...";
            }
            else
            {
                tbSearch.Foreground = new SolidColorBrush(Colors.Black);
                tbSearch.Tag = null;

                try
                {
                    var vResult = lAllSoftware.FindAll(t => t.ShortName.IndexOf(tbSearch.Text, 0, StringComparison.InvariantCultureIgnoreCase) >= 0).ToList();
                    vResult.AddRange(lAllSoftware.FindAll(t => t.ProductName.IndexOf(tbSearch.Text, 0, StringComparison.InvariantCultureIgnoreCase) >= 0).ToList());
                    vResult.AddRange(lAllSoftware.FindAll(t => t.Manufacturer.IndexOf(tbSearch.Text, 0, StringComparison.InvariantCultureIgnoreCase) >= 0).ToList());
                    if (vResult.Count <= 15)
                    {
                        vResult.AddRange(lAllSoftware.FindAll(t => (t.Description ?? "").IndexOf(tbSearch.Text, 0, StringComparison.InvariantCultureIgnoreCase) >= 0).ToList());
                    }

                    lvSW.ItemsSource = vResult.Distinct().OrderBy(t => t.ShortName).ThenByDescending(t => t.ProductVersion).ThenByDescending(t => t.ProductName);
                }
                catch { }
            }
            Mouse.OverrideCursor = null;

            //Mouse.OverrideCursor = Cursors.Wait;
            //if (string.IsNullOrEmpty(tbSearch.Text))
            //{
            //    tbSearch.Foreground = new SolidColorBrush(Colors.LightGray);
            //    tbSearch.Tag = "Search";
            //    tbSearch.Text = "Search...";


            //    ListCollectionView lcv = new ListCollectionView(lAllSoftware.Distinct().OrderBy(t => t.ShortName).ThenByDescending(t => t.ProductVersion).ThenByDescending(t => t.ProductName).ToList());

            //    //ListCollectionView lcv = new ListCollectionView(oAPI.SWResults("", "").Distinct().OrderBy(t => t.Shortname).ThenByDescending(t => t.ProductVersion).ThenByDescending(t => t.ProductName).ToList());
            //    PropertyGroupDescription PGD = new PropertyGroupDescription("", new ShortnameToCategory());

            //    //PGD.GroupNames.Add(RZRestAPI.GetCategories(lAllSoftware));
            //    foreach (var o in RZRestAPIv2.GetCategories(lAllSoftware))
            //    {
            //        PGD.GroupNames.Add(o);
            //    }


            //    lcv.GroupDescriptions.Add(PGD);

            //    lvSW.ItemsSource = lcv;
            //}
            //else
            //{
            //    tbSearch.Foreground = new SolidColorBrush(Colors.Black);
            //    tbSearch.Tag = null;

            //    try
            //    {
            //        var vResult = lAllSoftware.FindAll(t => t.ShortName.IndexOf(tbSearch.Text, 0, StringComparison.InvariantCultureIgnoreCase) >= 0).ToList();
            //        vResult.AddRange(lAllSoftware.FindAll(t => t.ProductName.IndexOf(tbSearch.Text, 0, StringComparison.InvariantCultureIgnoreCase) >= 0).ToList());
            //        vResult.AddRange(lAllSoftware.FindAll(t => t.Manufacturer.IndexOf(tbSearch.Text, 0, StringComparison.InvariantCultureIgnoreCase) >= 0).ToList());
            //        if (vResult.Count <= 15)
            //        {
            //            vResult.AddRange(lAllSoftware.FindAll(t => (t.Description ?? "").IndexOf(tbSearch.Text, 0, StringComparison.InvariantCultureIgnoreCase) >= 0).ToList());
            //        }

            //        lvSW.ItemsSource = vResult.Distinct().OrderBy(t => t.ShortName).ThenByDescending(t => t.ProductVersion).ThenByDescending(t => t.ProductName);
            //    }
            //    catch { }
            //}
            //Mouse.OverrideCursor = null;
        }

        public class ShortnameToCategory : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value.GetType() == typeof(GetSoftware))
                {
                    GetSoftware oSW = (GetSoftware)value;

                    return oSW.Categories[0];
                }

                return null;

            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private void lvSW_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvSW.SelectedItems.Count > 0)
            {
                btInstall.IsEnabled = true;
            }
            else
            {
                btInstall.IsEnabled = false;
            }
        }

        private void btInstall_Click(object sender, RoutedEventArgs e)
        {
            if (lvSW.SelectedItem != null)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    foreach (var oItem in lvSW.SelectedItems)
                    {
                        try
                        {
                            //string sPS = "";
                            string sProdName = "";
                            string sProdVersion = "";
                            string sManuf = "";

                            if (oItem.GetType() == typeof(GetSoftware))
                            {
                                GetSoftware dgr = oItem as GetSoftware;
                                sProdName = dgr.ProductName;
                                sProdVersion = dgr.ProductVersion;
                                sManuf = dgr.Manufacturer;
                            }

                            bool bInstalled = false;
                            foreach (var DT in RZRestAPIv2.GetSoftwares(sProdName, sProdVersion, sManuf))
                            {
                                //Check PreReqs
                                try
                                {
                                    if (!string.IsNullOrEmpty(DT.PSPreReq) & !bInstalled)
                                    {
                                        if (!(bool)oAgent.Client.GetObjectsFromPS(DT.PSPreReq, true, new TimeSpan(0, 0, 0))[0].BaseObject)
                                        {
                                            //PreReq not match
                                            continue;
                                        }
                                        else
                                        {
                                            //Check if already installed
                                            if ((bool)oAgent.Client.GetObjectsFromPS(DT.PSDetection, true, new TimeSpan(0, 0, 0))[0].BaseObject)
                                            {
                                                MessageBox.Show("Software is already installed.", "Installation Status:", MessageBoxButton.OK, MessageBoxImage.Information);
                                                bInstalled = true;
                                                continue;
                                            }

                                            //Create target Folder
                                            oAgent.Client.GetStringFromPS("$Folder =  join-path $env:TEMP '" + DT.ContentID + "'; New-Item -ItemType Directory -Force -Path $Folder -ErrorAction SilentlyContinue | Out-Null", true);

                                            //Download Files
                                            foreach (var File in DT.Files)
                                            {
                                                if (!File.URL.StartsWith("http", StringComparison.InvariantCultureIgnoreCase) & !File.URL.StartsWith("ftp", StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    try
                                                    {
                                                        File.URL = oAgent.Client.GetStringFromPS(File.URL);
                                                    }
                                                    catch { }
                                                }
                                                oAgent.Client.GetStringFromPS("$Folder =  join-path $env:TEMP '" + DT.ContentID + "'; $Target = join-path $Folder '" + File.FileName + "' ;Invoke-WebRequest '" + File.URL + "' -MaximumRedirection 2 -OutFile $Target -UserAgent 'chocolatey command line' ", true);
                                            }

                                            //Install
                                            string sInst = "Set-Location -Path $Folder -ErrorAction SilentlyContinue; ";
                                            if (!string.IsNullOrEmpty(DT.PSPreInstall))
                                                sInst = sInst + "Invoke-Expression " + DT.PSPreInstall + " | Out-Null; ";
                                            if (!string.IsNullOrEmpty(DT.PSInstall))
                                                sInst = sInst + "Invoke-Expression " + DT.PSInstall + " | Out-Null; ";
                                            if (!string.IsNullOrEmpty(DT.PSPostInstall))
                                                sInst = sInst + "Invoke-Expression " + DT.PSPostInstall + " | Out-Null; ";
                                            string sRes = oAgent.Client.GetStringFromPS(sInst,true);

                                            //Check if installed
                                            if ((bool)oAgent.Client.GetObjectsFromPS(DT.PSDetection, true, new TimeSpan(0, 0, 0))[0].BaseObject)
                                            {
                                                RZRestAPIv2.FeedbackAsync(DT.ProductName, DT.ProductVersion, DT.Manufacturer, DT.Architecture, "true", "SCCMCliCtr", "Ok...").ConfigureAwait(false);
                                                MessageBox.Show("Software installed successfully.", "Installation Status:", MessageBoxButton.OK, MessageBoxImage.Information);
                                                bInstalled = true;
                                                continue;
                                            }
                                            else
                                            {
                                                MessageBox.Show("Software installation failed.. " + sRes, "Installation Status:", MessageBoxButton.OK, MessageBoxImage.Warning);
                                            }
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                }
                catch { }

                Mouse.OverrideCursor = null;
            }
        }

        private void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                lvSW.Focus();
            }
            else
            {
                tSearch.Enabled = true;
                tSearch.Start();
            }
        }

        private void miOpenPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lvSW.SelectedItems.Count > 0)
                {
                    Process.Start(((GetSoftware)lvSW.SelectedItem).ProductURL.ToString());
                }
            }
            catch { }
        }

        private void tbSearch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //lAllSoftware = oAPI.SWResults("", "").Distinct().OrderBy(t => t.Shortname).ThenByDescending(t => t.ProductVersion).ThenByDescending(t => t.ProductName).ToList();
        }

        private void miInstall_Click(object sender, RoutedEventArgs e)
        {
            lvSW.ContextMenu.IsOpen = false;
            Thread.Sleep(200);

            Dispatcher.Invoke(new Action(() => { }), System.Windows.Threading.DispatcherPriority.ContextIdle, null);

            btInstall_Click(sender, e);
        }

        public Collection<PSObject> RunPS(string PSScript)
        {
            PowerShell PowerShellInstance = PowerShell.Create();

            PowerShellInstance.AddScript(PSScript);

            Collection<PSObject> PSOutput = PowerShellInstance.Invoke();

            return PSOutput;

        }
    }
}
