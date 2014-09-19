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
using System.Xml;

namespace ClientCenter
{
    /// <summary>
    /// Interaction logic for EventMonitoring.xaml
    /// </summary>
    public partial class EventMonitoring : UserControl
    {
        private SCCMAgent oAgent;
        public MyTraceListener Listener;

        public EventMonitoring()
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

                        /*iHistory = oAgent.Client.SoftwareDistribution.ExecutionHistory.OrderBy(t => t._RunStartTime).ToList();
                        dataGrid1.BeginInit();
                        dataGrid1.ItemsSource = iHistory;
                        dataGrid1.EndInit(); */
                    }
                    catch { }
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
        }

        private void bt_StartMonitoring_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                oAgent.Client.Monitoring.AsynchronousScript.Connect();
                oAgent.Client.Monitoring.AsynchronousScript.Command = Properties.Settings.Default.PSEventQuery;

                oAgent.Client.Monitoring.AsynchronousScript.TypedOutput += new EventHandler(AsynchronousScript_TypedOutput);
                oAgent.Client.Monitoring.AsynchronousScript.Run();
                bt_StartMonitoring.IsEnabled = false;
                bt_StopMonitoring.IsEnabled = true;
            }
            catch(Exception ex)
            {
                ex.Message.ToString();
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        void AsynchronousScript_TypedOutput(object sender, EventArgs e)
        {
            List<object> oResult = sender as List<object>;
            if (oResult != null)
            {
                foreach (var o in oResult)
                {
                    if (o.GetType() == typeof(System.Collections.Hashtable))
                    {
                        System.Collections.Hashtable HT = o as System.Collections.Hashtable;
                        string sClass = HT["__CLASS"].ToString();

                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            if (!string.IsNullOrEmpty(sClass))
                            {
                                //richTextBox1.AppendText(DateTime.Now.ToString() + " " + sClass + "\r");
                                //richTextBox1.ScrollToEnd();
                                TreeViewItem tvRoot = new TreeViewItem();
                                tvRoot.Name = sClass;
                                tvRoot.Tag = HT;
                                tvRoot.Header = DateTime.Now.ToString() + " " + sClass;
                                if (sClass == "CCM_PolicyAgent_PolicyDownloadStarted")
                                {
                                    tvRoot.Background = new SolidColorBrush(Colors.LightGoldenrodYellow);
                                }

                                foreach (System.Collections.DictionaryEntry oPair in HT)
                                {
                                    if (!oPair.Key.ToString().StartsWith("__"))
                                    {
                                        switch (oPair.Key.ToString())
                                        {
                                            case "DateTime":
                                                tvRoot.Items.Add(oPair.Key + " : " + System.Management.ManagementDateTimeConverter.ToDateTime(oPair.Value.ToString()).ToString());
                                                break;
                                            case "TIME_CREATED":
                                                tvRoot.Items.Add(oPair.Key + " : " + new DateTime(long.Parse(oPair.Value.ToString())).ToString());
                                                break;
                                            case "DownloadSource":
                                                TreeViewItem tvi = new TreeViewItem();
                                                tvi.Header = oPair.Key + " : " + oPair.Value.ToString();
                                                tvi.Items.Add(null);
                                                tvi.Tag = oPair.Value.ToString();
                                                tvi.Expanded += new RoutedEventHandler(tvi_Expanded);
                                                tvRoot.Items.Add(tvi);
                                                break;
                                            default:
                                                tvRoot.Items.Add(oPair.Key + " : " + oPair.Value.ToString());
                                                break;
                                        }
                                    }
                                }
                                tvRoot.IsExpanded = false;
                                tvRoot.IsSelected = true;
                                treeView1.Items.Add(tvRoot);
                                tvRoot.BringIntoView();
                            }
                        }));
                    }
                }
            }


        }

        void tvi_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = ((TreeViewItem)sender);
            try
            {
                TextBox tb = new TextBox();
                tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                tb.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                tb.Text = sccmclictr.automation.policy.localpolicy.FormatXML(sccmclictr.automation.policy.localpolicy.DownloadPolicyFromURL(tvi.Tag.ToString()));

                tvi.Items.Clear();
                
                /*
                WebBrowser oWeb = new WebBrowser();
                string sPol = sccmclictr.automation.policy.localpolicy.DownloadPolicyFromURL(tvi.Tag.ToString());
                tvi.Items.Clear();
                oWeb.NavigateToString(sPol);
                oWeb.MinHeight = 100;
                oWeb.MinWidth = 400;
                oWeb.Width = tvi.DesiredSize.Width - 30;
                oWeb.Height = sPol.Split('\n').Length * 14;
                oWeb.MaxHeight = treeView1.DesiredSize.Height - 50;*/
                tvi.Items.Add(tb);
                tvi.BringIntoView();
            }
            catch { }
            
        }


        public void bt_StopMonitoring_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                oAgent.Client.Monitoring.AsynchronousScript.TypedOutput -= AsynchronousScript_TypedOutput;
                oAgent.Client.Monitoring.AsynchronousScript.Close();
                bt_StartMonitoring.IsEnabled = true;
                bt_StopMonitoring.IsEnabled = false;
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        public void bt_ClearMonitoring_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                treeView1.Items.Clear();
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2 | (e.Key == Key.C & Keyboard.Modifiers == ModifierKeys.Control))
            {
                try
                {
                    Clipboard.SetData(DataFormats.Text, treeView1.SelectedItem.ToString());
                }
                catch { }
            }
        }
    }
}
