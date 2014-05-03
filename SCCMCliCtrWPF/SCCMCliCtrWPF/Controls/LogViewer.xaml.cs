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
using System.Reflection;
using sccmclictr.automation;
using sccmclictr.automation.functions;
using System.Management.Automation;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace ClientCenter.Controls
{
    /// <summary>
    /// Interaction logic for LogViewer.xaml
    /// </summary>
    public partial class LogViewer : UserControl
    {
        private SCCMAgent oAgent;
        public MyTraceListener Listener;

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
                    }
                    catch { }
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
        }

        public LogViewer()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                int iLines = int.Parse(tbLines.Text);
                TabPanel.Items.Clear();

                foreach (object ob in MI.Items)
                {
                    try
                    {
                        if (ob.GetType() != typeof(CheckBox))
                            continue;

                        CheckBox cb = ob as CheckBox;

                        if (cb.IsChecked == true)
                        {
                            //DG.DataContext = oData;
                            string sFile = cb.Tag as string;
                            string sName = cb.Content as string;
                            TabItem ti = new TabItem();
                            LogGrid LG = new LogGrid();


                            List<PSObject> lRes = oAgent.Client.GetObjectsFromPS(string.Format("Get-Content {0} -Tail {1}", sFile, iLines));
                            foreach (PSObject oLine in lRes)
                            {

                                string sOrg = oLine.ToString();

                                //Check if Line has at least 5 Tabs (e.g. WindowsUpdate.log)
                                if (sOrg.Count(f => f == '\t') > 4)
                                {
                                    try
                                    {
                                        string sText = sOrg.Split('\t')[5];
                                        string sComp = sOrg.Split('\t')[4];
                                        string sDate = sOrg.Split('\t')[0];
                                        string sTime = sOrg.Split('\t')[1];

                                        DateTime logdate = DateTime.ParseExact(sDate + " " + sTime, "yyyy-MM-dd HH:mm:ss:fff", CultureInfo.InvariantCulture);

                                        LG.LogLines.Add(new LogGrid.LogEntry() { LogText = sText, Component = sComp, Date = logdate });
                                        continue;
                                    }
                                    catch { }
                                }

                                //Check for SCCM Log format
                                if (sOrg.StartsWith("<![LOG["))
                                {
                                    try
                                    {
                                        string sText = sOrg.Substring(7, sOrg.IndexOf("]LOG]!>") - 7);
                                        string sTemp = sOrg.Substring(sOrg.IndexOf("LOG]!>") + 7);

                                        List<string> parts = Regex.Matches(sTemp, @"[\""].+?[\""]|[^ ]+").Cast<Match>().Select(m => m.Value).ToList();

                                        string sComp = parts.First(p => p.StartsWith("component")).Split('=')[1].Replace("\"", "");
                                        string sDate = parts.First(p => p.StartsWith("date")).Split('=')[1].Replace("\"", ""); ;
                                        string sTime = parts.First(p => p.StartsWith("time")).Split('=')[1].Replace("\"", "").Split('-')[0];
                                        DateTime logdate = DateTime.ParseExact(sDate + " " + sTime, "MM-dd-yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                                        LG.LogLines.Add(new LogGrid.LogEntry() { LogText = sText, Component = sComp, Date = logdate });
                                        continue;
                                    }
                                    catch { }
                                }

                                if (!string.IsNullOrEmpty(sOrg))
                                {
                                    LG.LogLines.Add(new LogGrid.LogEntry() { LogText = sOrg, Component = "", Date = DateTime.Now });
                                }



                            }


                            ti.Header = sName;
                            ti.Name = sName.Split('.')[0];
                            ti.Content = LG;

                            TabPanel.Items.Add(ti);
                        }
                    }
                    catch { }
                }
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (oAgent != null)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    if (MI.Items.Count <= 3)
                    {
                        string sLogPath = oAgent.Client.AgentProperties.LocalSCCMAgentLogPath;
                        foreach (string sFile in oAgent.Client.AgentProperties.LocalSCCMAgentLogFiles)
                        {
                            CheckBox CB = new CheckBox();
                            CB.Tag = sLogPath + "\\" + sFile;
                            CB.Content = sFile;
                            MI.Items.Add(CB);
                        }
                    }
                }
                catch { }
                Mouse.OverrideCursor = Cursors.Arrow;
            }

        }

    }
}
