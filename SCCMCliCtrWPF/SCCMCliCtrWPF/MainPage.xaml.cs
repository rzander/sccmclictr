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

using Stema.Controls;
using sccmclictr.automation;
using System.Diagnostics;
using System.ComponentModel;
using System.Deployment.Application;
using System.Management;
using System.Security.Principal;
using System.Reflection;
using Microsoft.Win32;
using System.IO;
//using System.Windows.Markup;

namespace ClientCenter
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public SCCMAgent oAgent;
        public MyTraceListener myTrace;
        private bool bPasswordChanged = false;

        public MainPage()
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            InitializeComponent();

            AgentSettingsPane.IsSelected = true;

            try
            {
                this.WindowTitle = SCCMCliCtr.Customization.Title;
                rStatus.AppendText("Client Center for Configuration Manager (c) 2013 by Roger Zander\n");
                rStatus.AppendText("Project-Page: http://sccmclictr.codeplex.com\n");
                rStatus.AppendText("Current Version: " + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString() + "\n");
                rStatus.AppendText("Assembly Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + "\n");


                if (!SCCMCliCtr.Customization.CheckLicense() | SCCMCliCtr.Customization.isOpenSource)
                {
                    this.AgentSettingsPanel.IsSelected = false;
                }
                else
                {
                    this.AgentSettingsPanel.IsSelected = true;
                    this.myAbout.MSG = false;
                }

                if (Properties.Settings.Default.HideShutdownPane)
                {
                    installRepair1.gbRestart.IsEnabled = false;
                    installRepair1.gbRestart.Visibility = System.Windows.Visibility.Hidden;
                }

            }
            catch { }

            myTrace = new MyTraceListener(ref rStatus);
            myTrace.TraceOutputOptions = TraceOptions.None;

            //Load external Agent Action Tool Plugins...
            try
            {
                string sCurrentDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                foreach (string sFile in Directory.GetFiles(sCurrentDir, "Plugin*.dll", SearchOption.TopDirectoryOnly))
                {
                    Assembly asm = Assembly.LoadFile(sFile);
                    Type[] tlist = asm.GetTypes();
                    foreach (Type t in tlist)
                    {
                        try
                        {
                            if (t.Name.StartsWith("AgentActionTool_"))
                            {
                                //Make Tool Group visible
                                rgTools.Visibility = System.Windows.Visibility.Visible;

                                var obj = Activator.CreateInstance(t);
                                btTools.Items.Add(obj);

                            }
                        }
                        catch { }
                    }
                }

            }
            catch { }



            Application.Current.Exit += new ExitEventHandler(Current_Exit);
            ThemeManager.SetActiveTheme(NavigationPaneTheme.WindowsLive);
            Style s = new Style();
            s.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Collapsed));
            tabNavigationPanels.ItemContainerStyle = s;
            tb_wsmanport.Text = Properties.Settings.Default.WinRMPort;
            cb_ssl.IsChecked = Properties.Settings.Default.WinRMSSL;
            wmiBroser.lAdhocQueries = Properties.Settings.Default.AdhocInv.Cast<string>().ToList();
            if (Properties.Settings.Default.showPingButton)
                bt_Ping.Visibility = System.Windows.Visibility.Visible;


            try
            {
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    //Pass Parameter like: http://sccmclictr.codeplex.com/releases/clickonce/SCCMCliCtrWPF.application?Computer2
                    Uri launchUri = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.ActivationUri;
                    tb_TargetComputer.Text = launchUri.Query.Replace("?", "");
                    tb_TargetComputer.Text = tb_TargetComputer.Text.Replace("-debug", "127.0.0.1");
                    tb_TargetComputer.Text = tb_TargetComputer.Text.Replace("&ProjectName=sccmclictr", "");
                    tb_TargetComputer.Text = tb_TargetComputer.Text.Replace("=", "");
                    tb_TargetComputer.Text = tb_TargetComputer.Text.Replace("-Embedding", "");

                    tb_TargetComputer2.Text = tb_TargetComputer.Text;

                    if (!IsRunAsAdministrator() & !System.Windows.Interop.BrowserInteropHelper.IsBrowserHosted)
                    {

                        // It is not possible to launch a ClickOnce app as administrator directly, so instead we launch the
                        // app as administrator in a new process.
                        var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);

                        // The following properties run the new process as administrator
                        processInfo.UseShellExecute = true;
                        processInfo.Verb = "runas";
                        processInfo.Arguments = tb_TargetComputer2.Text;

                        // Start the new process
                        try
                        {
                            System.Diagnostics.Process.Start(processInfo);

                            // Shut down the current process
                            Application.Current.Shutdown();
                        }
                        catch (Exception)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    tb_TargetComputer.Text = Environment.GetCommandLineArgs()[1].Trim();
                    tb_TargetComputer2.Text = Environment.GetCommandLineArgs()[1].Trim();
                    tb_TargetComputer.Text = tb_TargetComputer.Text.Replace("-debug", "127.0.0.1");
                    tb_TargetComputer2.Text = tb_TargetComputer.Text.Replace("-debug", "127.0.0.1");
                    tb_TargetComputer.Text = tb_TargetComputer.Text.Replace("-Embedding", "");
                    tb_TargetComputer2.Text = tb_TargetComputer2.Text.Replace("-Embedding", "");
                }
            }
            catch { }

            pb_Password.Password = Properties.Settings.Default.Password;
            tb_Username.Text = Properties.Settings.Default.Username;

            //Not needed, local admin is only required if connecting the local machien...
            if (!Properties.Settings.Default.NoLocalAdminCheck)
            {
                //Check if App is running as Admin, otherwise restart App as Admin...
                Application_Startup(this, tb_TargetComputer.Text);
            }

            if (Environment.GetCommandLineArgs().Count() > 1)
            {
                if (!string.IsNullOrEmpty(tb_TargetComputer.Text))
                {
                    bt_Connect_Click(this, null);
                }
            }

        }

        public void PageReset()
        {
            AgentSettingsPane.IsSelected = true;

            if (!SCCMCliCtr.Customization.CheckLicense() | SCCMCliCtr.Customization.isOpenSource)
            {
                this.AgentSettingsPanel.IsSelected = false;
            }
            else
            {
                this.AgentSettingsPanel.IsSelected = true;
                this.myAbout.MSG = false;
            }

            AgentSettingsPane.IsSelected = true;
            tviAgentSettings.IsSelected = true;

        }



        //Code from http://antscode.blogspot.com/2011/02/running-clickonce-application-as.html
        private bool IsRunAsAdministrator()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);

            return wp.IsInRole(WindowsBuiltInRole.Administrator);
        }

        //Code from http://antscode.blogspot.com/2011/02/running-clickonce-application-as.html
        private void Application_Startup(object sender, string parameter)
        {
            if (!IsRunAsAdministrator())
            {
                if (System.Windows.Interop.BrowserInteropHelper.IsBrowserHosted)
                {
                    MessageBox.Show("Sorry, this application must be run as Administrator to connect the local machine. Please restart your browser as Administrator.", "Administrative rights required", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    // It is not possible to launch a ClickOnce app as administrator directly, so instead we launch the
                    // app as administrator in a new process.
                    var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);

                    // The following properties run the new process as administrator
                    processInfo.UseShellExecute = true;
                    processInfo.Verb = "runas";
                    processInfo.Arguments = parameter;

                    // Start the new process
                    try
                    {
                        System.Diagnostics.Process.Start(processInfo);
                    }
                    catch (Exception)
                    {
                        // The user did not allow the application to run as administrator
                        MessageBox.Show("Sorry, this application must be run as Administrator.");
                    }

                    // Shut down the current process
                    Application.Current.Shutdown();
                }
            }
            else
            {
                // We are running as administrator

                // Do normal startup stuff...
            }
        }

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.ToString();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.ToString();
        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                oAgent.Client.Monitoring.AsynchronousScript.Close();
            }
            catch { }
            try
            {
                oAgent.disconnect();
            }
            catch { }
        }

        private void bt_Connect_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            if (oAgent != null)
            {
                this.AgentSettingsPanel.IsSelected = true;

                if (oAgent.isConnected)
                {
                    try
                    {
                        eventMonitoring1.bt_StopMonitoring_Click(sender, e);

                        oAgent.Client.Monitoring.AsynchronousScript.Close();
                        oAgent.disconnect();
                    }
                    catch { }
                }

                oAgent.Dispose();
                PageReset();

            }

            if (bPasswordChanged)
            {
                Properties.Settings.Default.Password = common.Encrypt(pb_Password.Password, Application.ResourceAssembly.ManifestModule.Name);
                Properties.Settings.Default.Save();
                pb_Password.Password = Properties.Settings.Default.Password;
                bPasswordChanged = false;
            }

            tb_TargetComputer.Text = tb_TargetComputer.Text.Trim();
            tb_TargetComputer2.Text = tb_TargetComputer2.Text.Trim();

            string sTarget = tb_TargetComputer.Text;
            try
            {

                if (sender == bt_Connect2)
                    sTarget = tb_TargetComputer2.Text;

                tb_TargetComputer.Text = sTarget;
                tb_TargetComputer2.Text = sTarget;

                if (sTarget.ToLower() == "localhost" | sTarget == "127.0.0.1")
                {
                    if (!IsRunAsAdministrator())
                    {
                        MessageBox.Show("Sorry, connecting the local machine requires administrative permissions. Please start the Tool as Administrator.");
                    }
                }

                if (System.Text.RegularExpressions.Regex.Match(sTarget, "^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}$", System.Text.RegularExpressions.RegexOptions.CultureInvariant).Success)
                {
                    if (sTarget != "127.0.0.1")
                    {
                        if (string.IsNullOrEmpty(tb_Username.Text) | string.IsNullOrEmpty(pb_Password.Password))
                        {
                            MessageBox.Show("connecting an IP Address requires Username and Password", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }

                rStatus.Document = new FlowDocument();
                myTrace = new MyTraceListener(ref rStatus);
                myTrace.TraceOutputOptions = TraceOptions.None;

                if (string.IsNullOrEmpty(tb_Username.Text))
                {
                    oAgent = new SCCMAgent(sTarget, null, null, int.Parse(tb_wsmanport.Text), false, cb_ssl.IsChecked ?? false);
                }
                else
                {
                    if (!tb_Username.Text.Contains(@"\"))
                    {
                        tb_Username.Text = Environment.UserDomainName + @"\" + tb_Username.Text;
                    }
                    string sPW = common.Decrypt(pb_Password.Password, Application.ResourceAssembly.ManifestModule.Name);
                    oAgent = new SCCMAgent(sTarget, tb_Username.Text, sPW, int.Parse(tb_wsmanport.Text), false, cb_ssl.IsChecked ?? false);
                }

                oAgent.PSCode.Listeners.Add(myTrace);
                oAgent.connect();

                //Store Agent settings in Common class for Plugin access.
                Common.Agent = oAgent;

                try
                {
                    //remove existing entry
                    if (Properties.Settings.Default.recentlyUsedComputers.Contains(sTarget))
                        Properties.Settings.Default.recentlyUsedComputers.Remove(sTarget);

                    //add on first position
                    Properties.Settings.Default.recentlyUsedComputers.Insert(0, sTarget);

                    if (Properties.Settings.Default.recentlyUsedComputers.Count > 10)
                    {
                        Properties.Settings.Default.recentlyUsedComputers.RemoveAt(10);
                    }

                    Properties.Settings.Default.Save();
                }
                catch { }

                agentSettingItem1.SCCMAgentConnection = oAgent;
                agentSettingItem1.Listener = myTrace;
                cacheGrid1.Listener = myTrace;
                servicesGrid1.Listener = myTrace;
                processGrid1.Listener = myTrace;
                sWUpdatesGrid1.Listener = myTrace;
                execHistoryGrid1.Listener = myTrace;
                sWAllUpdatesGrid1.Listener = myTrace;
                installRepair1.Listener = myTrace;
                applicationGrid1.Listener = myTrace;
                eventMonitoring1.Listener = myTrace;
                invInstalledSWGrid.Listener = myTrace;
                serviceWindowGrid1.Listener = myTrace;
                CollectionVariablesGrid1.Listener = myTrace;
                SettingsMgmtGrid.Listener = myTrace;
                SWDistSummaryGrid1.Listener = myTrace;
                LogViewPane.Listener = myTrace;

                navigationPane1.IsEnabled = true;
                ribAgenTActions.IsEnabled = true;

                ConnectionDock.Visibility = System.Windows.Visibility.Collapsed;
                ribbon1.IsEnabled = true;
                agentSettingItem1.IsEnabled = true;

                this.WindowTitle = sTarget;

            }
            catch (Exception ex)
            {
                //ribbon1.IsEnabled = false;
                navigationPane1.IsEnabled = false;
                agentSettingItem1.IsEnabled = false;
                myTrace.WriteError("Unable to connect: " + sTarget);
                myTrace.WriteError("Error: " + ex.Message);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                ribAgenTActions.IsEnabled = false;
                ConnectionDock.Visibility = System.Windows.Visibility.Visible;
                bt_Ping.Visibility = System.Windows.Visibility.Visible;
            }

            Mouse.OverrideCursor = Cursors.Arrow;

        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((System.Windows.Controls.HeaderedItemsControl)(e.Source)).Tag != null)
                {
                    foreach (TabItem iTab in tabNavigationPanels.Items)
                    {
                        try
                        {
                            if (iTab.Tag.ToString() == ((System.Windows.Controls.HeaderedItemsControl)(e.Source)).Tag.ToString())
                            {
                                iTab.IsSelected = true;
                                switch (iTab.Tag.ToString())
                                {
                                    case "Components":
                                        agentComponents1.SCCMAgentConnection = oAgent;
                                        break;
                                    case "Cache":
                                        cacheGrid1.SCCMAgentConnection = oAgent;
                                        break;
                                    case "Services":
                                        servicesGrid1.OnlyCMServices = false;
                                        if (((System.Windows.Controls.HeaderedItemsControl)(e.Source)).Name == "CMServices")
                                            servicesGrid1.OnlyCMServices = true;
                                        servicesGrid1.SCCMAgentConnection = oAgent;
                                        break;
                                    case "Process":
                                        processGrid1.SCCMAgentConnection = oAgent;
                                        break;
                                    case "SWUpdates":
                                        sWUpdatesGrid1.SCCMAgentConnection = oAgent;
                                        break;
                                    case "SWAllUpdates":
                                        sWAllUpdatesGrid1.SCCMAgentConnection = oAgent;
                                        break;
                                    case "ExecHistory":
                                        execHistoryGrid1.SCCMAgentConnection = oAgent;
                                        break;
                                    case "InstallRepair":
                                        installRepair1.SCCMAgentConnection = oAgent;
                                        break;
                                    case "SWDistApps":
                                        applicationGrid1.SCCMAgentConnection = oAgent;
                                        break;
                                    case "EventMonitoring":
                                        eventMonitoring1.SCCMAgentConnection = oAgent;
                                        break;
                                    case "InvInstalledSW":
                                        invInstalledSWGrid.SCCMAgentConnection = oAgent;
                                        break;
                                    case "ServiceWindow":
                                        serviceWindowGrid1.SCCMAgentConnection = oAgent;
                                        break;
                                    case "Collectionvariables":
                                        CollectionVariablesGrid1.SCCMAgentConnection = oAgent;
                                        break;
                                    case "WMIBrowser":
                                        wmiBroser.SCCMAgentConnection = oAgent;
                                        break;
                                    case "Advertisements":
                                        advertisementGrid1.SCCMAgentConnection = oAgent;
                                        break;
                                    case "SettingsMgmt":
                                        SettingsMgmtGrid.SCCMAgentConnection = oAgent;
                                        break;
                                    case "SWDistSummary":
                                        SWDistSummaryGrid1.SCCMAgentConnection = oAgent;
                                        break;
                                    case "CCMEval":
                                        CCMEvalGrid.SCCMAgentConnection = oAgent;
                                        break;
                                    case "PwrSettings":
                                        PwrSettingsPane.SCCMAgentConnection = oAgent;
                                        break;
                                    case "LogMonitoring":
                                        LogViewPane.SCCMAgentConnection = oAgent;
                                        break;

                                }
                                break;
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        private void ButtonHWInvSplit_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.HardwareInventory(false);
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void ButtonHWInvFull_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.HardwareInventory(true);
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void ButtonSWInvSplit_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.SoftwareInventory(false);
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void ButtonSWInvFull_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.SoftwareInventory(true);
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void ButtonDDR_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.DataDiscovery(true);
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void ButtonMachinePolicy_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.RequestMachinePolicyAssignments();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void ButtonUserPolicy_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.RequestUserAssignments();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void ButtonEvaluateUpdates_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.SoftwareUpdatesAgentAssignmentEvaluationCycle();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void ButtonScanUpdates_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.ForceUpdateScan();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void ButtonDCMEval_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.DCMPolicy();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btResetPolicy_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.ResetPolicy(true);
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void rStatus_TextChanged(object sender, TextChangedEventArgs e)
        {
            rStatus.ScrollToEnd();
        }

        bool isResizing = false;
        Point startPt;
        private void PSGrip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(this.PSGrip);
            startPt = e.GetPosition(this.PSGrip);
            isResizing = true;
        }

        private void PSGrip_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isResizing)
            {
                Point newPt = e.GetPosition(this.PSGrip);
                StatusDock.Height -= newPt.Y - startPt.Y;
                isResizing = false;
                this.PSGrip.ReleaseMouseCapture();
            }
        }

        private void btOpenPSConsole_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                Process Explorer = new Process();
                Explorer.StartInfo.FileName = "powershell.exe";

                if (!string.IsNullOrEmpty(tb_Username.Text))
                {
                    //Run powershell as different user...
                    if (tb_Username.Text.Contains(@"\"))
                    {
                        Explorer.StartInfo.UserName = tb_Username.Text.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        Explorer.StartInfo.Domain = tb_Username.Text.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    }
                    else
                    {
                        Explorer.StartInfo.Domain = Environment.UserDomainName;
                        Explorer.StartInfo.UserName = tb_Username.Text;
                    }
                    Explorer.StartInfo.UseShellExecute = false;


                    //Decode PW
                    string sPW = common.Decrypt(pb_Password.Password, Application.ResourceAssembly.ManifestModule.Name);
                    Explorer.StartInfo.Password = ToSecure(sPW);
                }
                if ((bool)cb_ssl.IsChecked)
                    Explorer.StartInfo.Arguments = @"-NoExit -Command Enter-PSSession " + oAgent.TargetHostname + " -Port " + tb_wsmanport.Text + " -UseSSL";
                else
                    Explorer.StartInfo.Arguments = @"-NoExit -Command Enter-PSSession " + oAgent.TargetHostname + " -Port " + tb_wsmanport.Text;
                Explorer.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                Explorer.Start();
            }
            catch (Exception ex)
            {
                myTrace.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        public static System.Security.SecureString ToSecure(string current)
        {
            var secure = new System.Security.SecureString();
            foreach (var c in current.ToCharArray()) secure.AppendChar(c);
            return secure;
        }

        private void treeView1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem_Selected(sender, e);
        }

        private void tvMonitoring_Loaded(object sender, RoutedEventArgs e)
        {
            tviMonitorOverview.IsSelected = true;
        }

        private void treeView1_Loaded(object sender, RoutedEventArgs e)
        {
            tviOverview.IsSelected = true;
        }

        private void tvInventory_Loaded(object sender, RoutedEventArgs e)
        {
            tviInvOverview.IsSelected = true;
        }

        private void tvSWDist_Loaded(object sender, RoutedEventArgs e)
        {
            tviSWDistOverview.IsSelected = true;
        }

        private void pb_Password_KeyDown(object sender, KeyEventArgs e)
        {
            bPasswordChanged = true;
        }

        private void btClientMachineAuthentication_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.ClientMachineAuthentication(false);
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btClearingproxysettingscache_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.ClearingProxySettingsCache();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btLSRefreshLocationsTask_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.RefreshLocationServicesTask();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btLSTimeoutRefreshTask_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.TimeoutLocationServicesTask();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btNAPaction_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.NAPIntervalEnforcement();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btPeerDPStatusreporting_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.PeerDistributionPointStatusTask();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btPeerDPPendingpackagecheckschedule_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.PeerDPPackageCheck();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btRefreshDefaultMPTask_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.RefreshDefaultMPTask();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btRefreshingcertificatesinADonMP_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.CertificateMaintenanceCycle();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btStateSystempolicycachecleanout_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.StateMessageManagerTask();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btStatesystempolicybulksendhigh_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.StateSystemPolicyBulksendHigh();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btStatesystempolicybulksendlow_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.StateSystemPolicyBulksendLow();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btSendUnsentStateMessage_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.SendUnsentStatusMessages();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btExternaleventdetection_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.ExternalEventDetectionMessage();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btRequestMachineAssignments_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.RequestMachinePolicyAssignments();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btEvaluateMachinePolicies_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.EvaluateMachinePolicyAssignments();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btMachinePolicyAgentCleanup_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.MachinePolicyAgentCleanupCycle();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btPolicyAgentValidateMachinePolicy_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.ValidateMachineAssignments();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btRequestUserAssignment_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.RequestUserAssignments();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btEvaluateUserAssignment_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.EvaluateUserPolicies();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btUserPolicyAgentCleanup_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.UserPolicyAgentCleanupCycle();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btPolicyAgentValidateUserPolicy_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.UserPolicyAgentCleanupCycle();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void ButtonIDMIF_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.IDMIFCollection();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void ButtonFileCollection_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.FileCollection();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btSWMeteringUsageRport_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.SWMeteringUsageReport();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btMSISourceListUpdate_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.MSISourceListUpdate();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btDCMPolicyAction_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.DCMPolicy();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btATMStatusCheckPolicy_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.AMTProvisionCycle();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btAppManUserPolicyAction_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.AppManUserPolicyAction();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btAppManMachinePolicyAction_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.AppManPolicyAction();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btAppManGlobalEvaluation_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.AppManGlobalEvaluation();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btPowerManagerSummarizer_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.PowerMgmtStartSummarizationTask();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void tb_TargetComputer2_KeyUp(object sender, KeyEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (e.Key == Key.Enter)
            {
                tb_TargetComputer2.Text = tb_TargetComputer2.Text.Trim();
                tb_TargetComputer.Text = tb_TargetComputer2.Text;
                bt_Connect_Click(sender, null);
            }
            Mouse.OverrideCursor = Cursors.Arrow;

        }

        private void tb_TargetComputer2_Populating(object sender, PopulatingEventArgs e)
        {
            try
            {
                AutoCompleteBox oSender = sender as AutoCompleteBox;
                List<string> lcomputers = new List<string>();
                oSender.ItemsSource = Properties.Settings.Default.recentlyUsedComputers;
                oSender.PopulateComplete();
            }
            catch { }
        }

        private void tb_TargetComputer_KeyUp(object sender, KeyEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (e.Key == Key.Enter)
            {
                tb_TargetComputer.Text = tb_TargetComputer.Text.Trim();
                tb_TargetComputer2.Text = tb_TargetComputer.Text;
                bt_Connect_Click(sender, null);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_Ping_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Net.NetworkInformation.Ping oPing = new System.Net.NetworkInformation.Ping();
                System.Net.NetworkInformation.PingOptions oPingOptions = new System.Net.NetworkInformation.PingOptions();

                oPingOptions.DontFragment = true;

                // Create a buffer of 32 bytes of data to be transmitted. 
                string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 120;
                System.Net.NetworkInformation.PingReply reply = oPing.Send(tb_TargetComputer2.Text.Trim(), timeout, buffer, oPingOptions);
                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    rStatus.AppendText("System is online !" + reply.Address.ToString() + "\r");
                    rStatus.AppendText("Address: " + reply.Address.ToString() + "\r");
                    rStatus.AppendText("RoundTrip time: " + reply.RoundtripTime + "\r");

                    /*rStatus.AppendText("Time to live: " + reply.Options.Ttl + "\r");
                    rStatus.AppendText("Don't fragment: " + reply.Options.DontFragment + "\r");
                    rStatus.AppendText("Buffer size: " + reply.Buffer.Length + "\r");*/
                }
                else
                {
                    rStatus.AppendText("Unable to ping the target device... !" + "\r");
                    rStatus.AppendText(reply.Status.ToString() + "\r");
                }
            }
            catch
            {
                rStatus.AppendText("Unable to ping the target device... !" + "\r");
            }


        }

        private void bt_RegConsole_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                console.registerConsoleExtension();
                rStatus.AppendText("Console extension registered.");
            }
            catch { myTrace.WriteError("Unable to register console extension..."); }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_UnRegConsole_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                console.unregisterConsoleExtension();
                rStatus.AppendText("Console extension removed.");
            }
            catch { myTrace.WriteError("Unable to remove console extension..."); }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_About_Click(object sender, RoutedEventArgs e)
        {
            this.AboutPanel.IsSelected = true;
        }

        private void btResetPausedSWDist_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                if (!oAgent.Client.AgentActions.ResetPausedSWDist())
                    myTrace.WriteError("Unable to reset paused SWDist...");
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btResetProvisioningMode_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                if (!oAgent.Client.AgentActions.ResetProvisioningMode())
                    myTrace.WriteError("Unable to reset ProvisioningMode...");
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btResetSystemTaskExclude_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                if (!oAgent.Client.AgentActions.SystemTaskExclude())
                    myTrace.WriteError("Unable to reset SystemTaskExclude...");
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btDeleteIsCacheCopyNeededCallBack_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                if (!oAgent.Client.AgentActions.IsCacheCopyNeededCallBack())
                    myTrace.WriteError("Unable to delete IsCacheCopyNeededCallBack...");
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

    }

    public class MyTraceListener : TraceListener, INotifyPropertyChanged
    {
        RichTextBox oROutTB;
        private readonly StringBuilder builder;

        public MyTraceListener(ref RichTextBox TB)
        {
            oROutTB = TB;
            this.builder = new StringBuilder();
        }

        public string Trace
        {
            get { return this.builder.ToString(); }
        }

        public override void Write(string message)
        {
            //Paragraph p = oROutTB.Document.Blocks.FirstBlock as Paragraph;
            //p.LineHeight = 10; 

            this.builder.Append(message.Replace("PSCode Information: 0 :", ""));
            oROutTB.AppendText(message.Replace("PSCode Information: 0 :", ""));

            this.OnPropertyChanged(new PropertyChangedEventArgs("Trace"));
        }

        public override void WriteLine(string message)
        {
            this.builder.AppendLine(message.Replace("PSCode Information: 0 :", "") + "\r\n");

            TextRange tr = new TextRange(oROutTB.Document.ContentEnd, oROutTB.Document.ContentEnd);
            tr.Text = message.Replace("PSCode Information: 0 :", "") + "\r\n";
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
            tr.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);

            this.OnPropertyChanged(new PropertyChangedEventArgs("Trace"));
        }

        public void WriteError(string message)
        {
            //message = message + "\r\n";
            this.builder.AppendLine(message.Replace("PSCode Information: 0 :", "") + "\r\n");

            TextRange tr = new TextRange(oROutTB.Document.ContentEnd, oROutTB.Document.ContentEnd);
            tr.Text = message.Replace("PSCode Information: 0 :", "") + "\r\n";
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
            tr.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);

            this.OnPropertyChanged(new PropertyChangedEventArgs("Trace"));
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    public class Common
    {
        internal static SCCMAgent iAgent;

        public static SCCMAgent Agent
        {
            get
            {
                return iAgent;
            }
            set { iAgent = value; }
        }
    }

    public class console
    {
        /// <summary>
        /// Create MMC Extension for CM12
        /// </summary>
        public static void registerConsoleExtension()
        {
            //SCCM Console Installed ?
            string sArchitecture = System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE").ToLower();
            RegistryKey rAdminUI = null;
            switch (sArchitecture)
            {
                case "x86":
                    rAdminUI = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\ConfigMgr10\Setup");
                    break;
                case "amd64":
                    rAdminUI = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\ConfigMgr10\Setup");
                    break;
                case "ia64":
                    rAdminUI = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\ConfigMgr10\Setup");
                    break;
            }

            if (rAdminUI != null)
            {
                string sUIPath = rAdminUI.GetValue("UI Installation Directory", "").ToString();
                if (Directory.Exists(sUIPath))
                {
                    Directory.CreateDirectory(sUIPath + @"\XmlStorage\Extensions\Actions\3fd01cd1-9e01-461e-92cd-94866b8d1f39");
                    TextWriter tw1 = new StreamWriter(sUIPath + @"\XmlStorage\Extensions\Actions\3fd01cd1-9e01-461e-92cd-94866b8d1f39\sccmclictr.xml");
                    tw1.WriteLine(string.Format(Properties.Resources.ConsoleExtension, System.Reflection.Assembly.GetExecutingAssembly().Location));
                    tw1.Close();

                    Directory.CreateDirectory(sUIPath + @"\XmlStorage\Extensions\Actions\ed9dee86-eadd-4ac8-82a1-7234a4646e62");
                    tw1 = new StreamWriter(sUIPath + @"\XmlStorage\Extensions\Actions\ed9dee86-eadd-4ac8-82a1-7234a4646e62\sccmclictr.xml");
                    tw1.WriteLine(string.Format(Properties.Resources.ConsoleExtension, System.Reflection.Assembly.GetExecutingAssembly().Location));
                    tw1.Close();
                }
            }
            else
            {
                throw new Exception("no CM12 console installed.");

            }
        }

        /// <summary>
        /// Remove MMC Extension for CM12
        /// </summary>
        public static void unregisterConsoleExtension()
        {
            string sArchitecture = System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE").ToLower();
            RegistryKey rAdminUI = null;
            switch (sArchitecture)
            {
                case "x86":
                    rAdminUI = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\ConfigMgr10\Setup");
                    break;
                case "amd64":
                    rAdminUI = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\ConfigMgr10\Setup");
                    break;
                case "ia64":
                    rAdminUI = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\ConfigMgr10\Setup");
                    break;
            }

            //SCCM Console Installed ?
            if (rAdminUI != null)
            {
                string sUIPath = rAdminUI.GetValue("UI Installation Directory", "").ToString();

                if (File.Exists(sUIPath + @"\XmlStorage\Extensions\Actions\3fd01cd1-9e01-461e-92cd-94866b8d1f39\sccmclictr.xml"))
                {
                    try
                    {
                        File.Delete(sUIPath + @"\XmlStorage\Extensions\Actions\3fd01cd1-9e01-461e-92cd-94866b8d1f39\sccmclictr.xml");
                    }
                    catch { }
                }

                if (File.Exists(sUIPath + @"\XmlStorage\Extensions\Actions\ed9dee86-eadd-4ac8-82a1-7234a4646e62\sccmclictr.xml"))
                {
                    try
                    {
                        File.Delete(sUIPath + @"\XmlStorage\Extensions\Actions\ed9dee86-eadd-4ac8-82a1-7234a4646e62\sccmclictr.xml");
                    }
                    catch { }
                }

            }
            else
            {
                throw new Exception("no CM12 console installed.");
            }
        }
    }



}
