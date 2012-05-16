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

namespace ClientCenter
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public SCCMAgent oAgent;
        public MyTraceListener myTrace;

        public MainPage()
        {
            InitializeComponent();
            ThemeManager.SetActiveTheme(NavigationPaneTheme.WindowsLive);
            Style s = new Style();
            s.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Collapsed));
            tabNavigationPanels.ItemContainerStyle = s;
        }

        private void bt_Connect2_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            ConnectionDock.Visibility = System.Windows.Visibility.Collapsed;

            oAgent = new SCCMAgent(tb_TargetComputer2.Text, null, null);
            oAgent.connect();
            MyTraceListener myTrace = new MyTraceListener(ref tStatus);
            myTrace.TraceOutputOptions = TraceOptions.None;
            oAgent.PSCode.Listeners.Add(myTrace);

            agentSettingItem1.SCCMAgentConnection = oAgent;
            
            navigationPane1.IsEnabled = true;

            Mouse.OverrideCursor = Cursors.Arrow;
           
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
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

                            break;
                        }
                    }
                    catch { }
                }
            }
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
            oAgent.Client.AgentActions.RequestMachinePolicyAssignments(); 
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void ButtonEvaluateUpdates_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.SoftwareUpdatesAgentAssignmentEvaluationCycle();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void ButtonDCMEval_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            oAgent.Client.AgentActions.DCMPolicyEnforcement();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

    }

    public class MyTraceListener : TraceListener, INotifyPropertyChanged
    {
        TextBox oOutTB;
        private readonly StringBuilder builder;

        public MyTraceListener(ref TextBox TB)
        {
            oOutTB = TB;
            this.builder = new StringBuilder();
        }

        public string Trace
        {
            get { return this.builder.ToString(); }
        }

        public override void Write(string message)
        {
            this.builder.Append(message.Replace("PSCode Information: 0 :", ""));
            oOutTB.Text = oOutTB.Text + message.Replace("PSCode Information: 0 :", "");
            this.OnPropertyChanged(new PropertyChangedEventArgs("Trace"));
        }

        public override void WriteLine(string message)
        {
            this.builder.AppendLine(message.Replace("PSCode Information: 0 :", ""));
            oOutTB.Text = oOutTB.Text + message.Replace("PSCode Information: 0 :", "") + "\r\n";
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
