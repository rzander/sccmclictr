﻿using System;
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
            }
            catch { }
        }

        private void bt_Connect_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            string sTarget = tb_TargetComputer.Text;
            try
            {
                
                if(sender == bt_Connect2)
                    sTarget = tb_TargetComputer2.Text;

                rStatus.Document = new FlowDocument();
                myTrace = new MyTraceListener(ref rStatus);
                myTrace.TraceOutputOptions = TraceOptions.None;

                oAgent = new SCCMAgent(sTarget, null, null);
                oAgent.connect();
                oAgent.PSCode.Listeners.Add(myTrace);

                agentSettingItem1.SCCMAgentConnection = oAgent;
                agentSettingItem1.Listener = myTrace;

                navigationPane1.IsEnabled = true;

                ConnectionDock.Visibility = System.Windows.Visibility.Collapsed;
                ribbon1.IsEnabled = true;
                agentSettingItem1.IsEnabled = true;
            }
            catch(Exception ex)
            {
                ribbon1.IsEnabled = false;
                navigationPane1.IsEnabled = false;
                agentSettingItem1.IsEnabled = false;
                myTrace.WriteError("Unable to connect: " + sTarget);
                myTrace.WriteError(ex.Message);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

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
                            switch(iTab.Tag.ToString())
                            {
                                case "Components":
                                    agentComponents1.SCCMAgentConnection = oAgent;
                                    break;
                            }
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
            this.builder.AppendLine(message.Replace("PSCode Information: 0 :", ""));

            TextRange tr = new TextRange(oROutTB.Document.ContentEnd, oROutTB.Document.ContentEnd);
            tr.Text = message.Replace("PSCode Information: 0 :", "") + "\r\n";
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
            tr.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);

            this.OnPropertyChanged(new PropertyChangedEventArgs("Trace"));
        }

        public void WriteError(string message)
        {
            this.builder.AppendLine(message.Replace("PSCode Information: 0 :", ""));

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