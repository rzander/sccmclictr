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
using System.Windows.Shapes;
using sccmclictr.automation;
using System.Windows.Controls.Ribbon;

namespace AgentActionTools
{

    /// <summary>
    /// Interaction logic for AppVForm.xaml
    /// </summary>
    public partial class AppVForm : Window
    {
        public AppVForm()
        {
            InitializeComponent();
            Style s = new Style();
            s.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Collapsed));
            tabNavigationPanels.ItemContainerStyle = s;


        }

        public List<sccmclictr.automation.functions.appv4.Application> lApplications = new List<sccmclictr.automation.functions.appv4.Application>();
        public List<sccmclictr.automation.functions.appv4.Package> lPackages = new List<sccmclictr.automation.functions.appv4.Package>();

        
        public SCCMAgent CMAgent;

        private void bt_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                if (sender.GetType() == typeof(RibbonButton))
                {
                    RibbonButton BT = (RibbonButton)sender;
                    foreach (TabItem ti in tabNavigationPanels.Items)
                    {
                        if (ti.Header.ToString() == BT.Label.ToString())
                        {
                            ti.IsSelected = true;
                            if (ti.Header.ToString() == "Applications")
                            {
                                if (!CMAgent.isConnected)
                                {
                                    CMAgent.connect();
                                }

                                if (lApplications.Count == 0)
                                {
                                    lApplications = CMAgent.Client.AppV4.Appv4Applications;
                                }

                                AppGrid1.BeginInit();
                                AppGrid1.ItemsSource = lApplications;
                                AppGrid1.EndInit();
                            }

                            if (ti.Header.ToString() == "Packages")
                            {
                                if (!CMAgent.isConnected)
                                {
                                    CMAgent.connect();
                                }

                                if (lPackages.Count == 0)
                                {
                                    lPackages = CMAgent.Client.AppV4.Appv4Packages;
                                }

                                PkgGrid1.BeginInit();
                                PkgGrid1.ItemsSource = lPackages;
                                PkgGrid1.EndInit();
                            }

                            if (ti.Header.ToString() == "Cache")
                            {
                                if (!CMAgent.isConnected)
                                {
                                    CMAgent.connect();
                                }

                                try
                                {
                                    if (CMAgent.Client.Inventory.isx64OS)
                                    {
                                        tbCacheSize.Text = CMAgent.Client.GetStringFromPS(@"(get-item hklm:\SOFTWARE\Wow6432Node\Microsoft\SoftGrid\4.5\Client\AppFS).GetValue('FileSize')");
                                        tbDrive.Text = CMAgent.Client.GetStringFromPS(@"(get-item hklm:\SOFTWARE\Wow6432Node\Microsoft\SoftGrid\4.5\Client\AppFS).GetValue('DriveLetter')");
                                    }
                                    else
                                    {
                                        tbCacheSize.Text = CMAgent.Client.GetStringFromPS(@"(get-item hklm:\SOFTWARE\Microsoft\SoftGrid\4.5\Client\AppFS).GetValue('FileSize')");
                                        tbDrive.Text = CMAgent.Client.GetStringFromPS(@"(get-item hklm:\SOFTWARE\Microsoft\SoftGrid\4.5\Client\AppFS).GetValue('DriveLetter')");
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;

        }

        private void bt_AppReload_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                if (!CMAgent.isConnected)
                {
                    CMAgent.connect();
                }

                lApplications = CMAgent.Client.AppV4.Appv4ApplicationsList(true, new TimeSpan(0,0,60));

                AppGrid1.BeginInit();
                AppGrid1.ItemsSource = lApplications;
                AppGrid1.EndInit();
            }
            catch { }

            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_PkgReload_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                if (!CMAgent.isConnected)
                {
                    CMAgent.connect();
                }

                lPackages = CMAgent.Client.AppV4.Appv4PackagesList(true, new TimeSpan(0,0,60));

                PkgGrid1.BeginInit();
                PkgGrid1.ItemsSource = lPackages;
                PkgGrid1.EndInit();
            }
            catch { }

            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void miDeletePackage_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                foreach (sccmclictr.automation.functions.appv4.Package oPkg in PkgGrid1.SelectedItems)
                {
                    string PSCommand = "start-process -wait sftmime.exe -argumentlist \"DELETE PACKAGE:$([char]34){0}$([char]34) /global\"";
                    PSCommand = PSCommand.Replace("{0}", oPkg.Name);
                    CMAgent.Client.GetStringFromPS(PSCommand);
                }

                bt_PkgReload_Click(sender, e);
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void miUnloadPackage_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                foreach (sccmclictr.automation.functions.appv4.Package oPkg in PkgGrid1.SelectedItems)
                {
                    string PSCommand = "start-process -wait sftmime.exe -argumentlist \"UNLOAD PACKAGE:$([char]34){0}$([char]34)\"";
                    PSCommand = PSCommand.Replace("{0}", oPkg.Name);
                    CMAgent.Client.GetStringFromPS(PSCommand);
                }

                bt_PkgReload_Click(sender, e);
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void miLoadPackage_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                foreach (sccmclictr.automation.functions.appv4.Package oPkg in PkgGrid1.SelectedItems)
                {
                    string PSCommand = "start-process -wait sftmime.exe -argumentlist \"LOAD PACKAGEe:$([char]34){0}$([char]34)\"";
                    PSCommand = PSCommand.Replace("{0}", oPkg.Name);
                    CMAgent.Client.GetStringFromPS(PSCommand);
                }

                bt_PkgReload_Click(sender, e);
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void miDeleteApp_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                foreach (sccmclictr.automation.functions.appv4.Application oPkg in AppGrid1.SelectedItems)
                {
                    string PSCommand = "start-process -wait sftmime.exe -argumentlist \"DELETE APP:$([char]34){0}$([char]34)\"";
                    PSCommand = PSCommand.Replace("{0}", oPkg.Name + " " + oPkg.Version);
                    CMAgent.Client.GetStringFromPS(PSCommand);
                }

                bt_PkgReload_Click(sender, e);
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void miUnloadApp_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                foreach (sccmclictr.automation.functions.appv4.Application oPkg in AppGrid1.SelectedItems)
                {
                    string PSCommand = "start-process -wait sftmime.exe -argumentlist \"UNLOAD APP:$([char]34){0}$([char]34)\"";
                    PSCommand = PSCommand.Replace("{0}", oPkg.Name + " " + oPkg.Version);
                    CMAgent.Client.GetStringFromPS(PSCommand);
                }

                bt_PkgReload_Click(sender, e);
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void miLoadApp_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                foreach (sccmclictr.automation.functions.appv4.Application oPkg in AppGrid1.SelectedItems)
                {
                    string PSCommand = "start-process -wait sftmime.exe -argumentlist \"LOAD APP:$([char]34){0}$([char]34)\"";
                    PSCommand = PSCommand.Replace("{0}", oPkg.Name + " " + oPkg.Version);
                    CMAgent.Client.GetStringFromPS(PSCommand);
                }

                bt_PkgReload_Click(sender, e);
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void miRepairApp_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                foreach (sccmclictr.automation.functions.appv4.Application oPkg in AppGrid1.SelectedItems)
                {
                    string PSCommand = "start-process -wait sftmime.exe -argumentlist \"REPAIR APP:$([char]34){0}$([char]34)\"";
                    PSCommand = PSCommand.Replace("{0}", oPkg.Name + " " + oPkg.Version);
                    CMAgent.Client.GetStringFromPS(PSCommand);
                }

                bt_PkgReload_Click(sender, e);
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }
    }
}
