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

//using System.IO;
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
            InitializeComponent();

            Application.Current.Exit += new ExitEventHandler(Current_Exit);
            ThemeManager.SetActiveTheme(NavigationPaneTheme.WindowsLive);
            Style s = new Style();
            s.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Collapsed));
            tabNavigationPanels.ItemContainerStyle = s;

            try
            {
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    //Pass Parameter like: http://sccmclictr.codeplex.com/releases/clickonce/SCCMCliCtrWPF.application?Computer2
                    Uri launchUri = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.ActivationUri;
                    tb_TargetComputer.Text = launchUri.Query.Replace("?", "");
                    tb_TargetComputer.Text = tb_TargetComputer.Text.Replace("&ProjectName=sccmclictr", "");

                    tb_TargetComputer2.Text = tb_TargetComputer.Text;
                }
                //sccmclictr.automation.common.Decrypt(Properties.Settings.Default.Password, Application.ResourceAssembly.ManifestModule.Name);
                pb_Password.Password = Properties.Settings.Default.Password;
            }
            catch { }

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
                oAgent.diconnect();
            }
            catch { }
        }

        private void bt_Connect_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (bPasswordChanged)
            {
                Properties.Settings.Default.Password = common.Encrypt(pb_Password.Password, Application.ResourceAssembly.ManifestModule.Name);
                Properties.Settings.Default.Save();
                pb_Password.Password = Properties.Settings.Default.Password;
                bPasswordChanged = false;
            }

            string sTarget = tb_TargetComputer.Text;
            try
            {
                
                if(sender == bt_Connect2)
                    sTarget = tb_TargetComputer2.Text;

                rStatus.Document = new FlowDocument();
                myTrace = new MyTraceListener(ref rStatus);
                myTrace.TraceOutputOptions = TraceOptions.None;

                if (string.IsNullOrEmpty(tb_Username.Text))
                {
                    oAgent = new SCCMAgent(sTarget, null, null);
                }
                else
                {
                    oAgent = new SCCMAgent(sTarget, tb_Username.Text, common.Decrypt(pb_Password.Password, Application.ResourceAssembly.ManifestModule.Name));
                }
                oAgent.connect();
                oAgent.PSCode.Listeners.Add(myTrace);


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
                

                navigationPane1.IsEnabled = true;
                ribAgenTActions.IsEnabled = true;

                ConnectionDock.Visibility = System.Windows.Visibility.Collapsed;
                ribbon1.IsEnabled = true;
                agentSettingItem1.IsEnabled = true;

                this.WindowTitle = sTarget;
                
            }
            catch(Exception ex)
            {
                ribbon1.IsEnabled = false;
                navigationPane1.IsEnabled = false;
                agentSettingItem1.IsEnabled = false;
                myTrace.WriteError("Unable to connect: " + sTarget);
                myTrace.WriteError("Error: "+ ex.Message);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                Explorer.StartInfo.Arguments = @"-NoExit -Command Enter-PSSession " + oAgent.TargetHostname;
                Explorer.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                Explorer.Start();
            }
            catch (Exception ex)
            {
                myTrace.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
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
            message = message + "\r\n";
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



}
