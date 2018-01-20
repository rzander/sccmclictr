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
using System.Collections;
using ClientCenter.Logs;

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
                        if (oAgent != value)
                        {
                            oAgent = value;
                            try
                            {
                                TabPanel.Items.Clear();
                                MI.Items.Clear();
                            }
                            catch { }
                        }
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

                            List<PSObject> lRes = oAgent.Client.GetObjectsFromPS(string.Format("Get-Content '{0}' -Tail {1}", sFile, iLines));
                            foreach (PSObject oLine in lRes)
                            {
                                string sOrg = oLine.ToString();
                                LG.LogLines.Add(LogEntry.ParseLogLine(sOrg));
                            }

                            ti.Header = sName;
                            ti.Name = sName.Split('.')[0];
                            ti.Content = LG;

                            TabPanel.Items.Add(ti);
                        }
                    }
                    catch { }
                }

                if (TabPanel.Items.Count > 0)
                {
                    try
                    {
                        ((TabItem)TabPanel.Items[0]).IsSelected = true;
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

        private void MI_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                foreach (CheckBox cb in MI.Items)
                {
                    try
                    {
                        if (cb.IsChecked == true)
                            cb.IsChecked = false;
                    }
                    catch { }
                }

                TabPanel.Items.Clear();
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

    }
}
