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
using System.Management.Automation;

namespace ClientCenter.Controls
{
    /// <summary>
    /// Interaction logic for WMIBrowser.xaml
    /// </summary>
    public partial class WMIBrowser : UserControl
    {
        private SCCMAgent oAgent;
        public MyTraceListener Listener;
        List<string> lClasses = new List<string>();
        public List<string> lAdhocQueries = new List<string>();

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
                        miAdhocInv.Items.Clear();
                        foreach(string sInv in lAdhocQueries)
                        {
                            MenuItem mi = new MenuItem();
                            mi.Header = sInv.Split('|')[0];
                            mi.Tag = sInv;
                            miAdhocInv.Items.Add(mi);
                        }
                    }
                    catch { }
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
        }

        public WMIBrowser()
        {
            InitializeComponent();
        }

        private void cb_Namespace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                if (oAgent.isConnected)
                {
                    lClasses.Clear();

                    List<PSObject> oClasses = oAgent.Client.GetObjectsFromPS("Get-WmiObject -Namespace '" + ((ListBoxItem)cb_Namespace.SelectedValue).Content.ToString() + "' -List | where { !$_.Name.StartsWith(\"__\") }", false);

                    foreach (PSObject oClass in oClasses)
                    {
                        lClasses.Add(oClass.Properties["Name"].Value.ToString());
                    }

                    cb_Classes.ItemsSource = lClasses.OrderBy(t => t);
                }
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void cb_Classes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                if (oAgent.isConnected)
                {
                    TreeViewItem tvMain = null;

                    foreach (TreeViewItem tvi in tvObjects.Items)
                    {
                        if (tvi.Header == cb_Classes.SelectedValue.ToString())
                        {
                            tvMain = tvi;
                            continue;
                        }
                    }

                    if (tvMain == null)
                    {
                        tvMain = new TreeViewItem();
                        tvMain.Header = cb_Classes.SelectedValue.ToString();
                        //tvMain.Name = cb_Classes.SelectedValue.ToString();
                        tvObjects.Items.Add(tvMain);
                    }
                    else
                    {
                        tvMain.Items.Clear();
                    }

                    List<PSObject> oObj = oAgent.Client.GetObjects(cb_Namespace.Text, "SELECT * FROM " + cb_Classes.SelectedValue.ToString(), false);


                    foreach (PSObject PSObj in oObj)
                    {
                        try
                        {
                            System.Collections.ObjectModel.ObservableCollection<wmiProp> oColl = new System.Collections.ObjectModel.ObservableCollection<wmiProp>();
                            PSObj.Properties.Where(w => w.GetType() == typeof(PSProperty) & !w.Name.StartsWith("_")).ToList().ForEach(x => oColl.Add(new wmiProp(x.Name, x.Value)));

                            //Dictionary<string, string> obj = PSObj.Properties.Where(w => w.GetType() == typeof(PSProperty) & !w.Name.StartsWith("_")).ToList

                            /*Dictionary<string, string> obj = PSObj.Properties.Where(w => w.GetType() == typeof(PSProperty) & !w.Name.StartsWith("_")).ToDictionary(t => t.Name, x => x.Value.ToString());
                            foreach(KeyValuePair<string, string> sVal in obj)
                            {
                                oColl.Add(new wmiProp(sVal.Key, sVal.Value));
                            }*/

                            DataGrid dg = new DataGrid();
                            dg.CanUserSortColumns = true;
                            dg.CanUserResizeColumns = true;
                            dg.AutoGenerateColumns = true;
                            dg.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                            dg.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                            dg.SelectionUnit = DataGridSelectionUnit.CellOrRowHeader;

                            dg.ItemsSource = oColl;

                            TreeViewItem tvi = new TreeViewItem();
                            tvi.Header = PSObj.Properties["__RELPATH"].Value;
                            tvi.Items.Add(dg);

                            tvMain.Items.Add(tvi);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }

                }
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        public class wmiProp
        {
            private string _Value;
            public wmiProp(string sKey, string sValue)
            {
                Property = sKey;
                if (!string.IsNullOrEmpty(sValue))
                    Value = sValue;
                else
                    Value = "";
            }

            public wmiProp(string sKey, object oValue)
            {
                Property = sKey;
                if (oValue != null)
                    Value = oValue.ToString();
                else
                    Value = "";
            }
            public string Property { get; set; }
            public string Value 
            { 
                get
                {
                    return _Value;
                } 
                set
                {
                    _Value = value;

                    //Check if it's in a DateTime Format
                    if (value.Length == 25)
                    {
                        if (value[14] == '.')
                        {
                            _Value = System.Management.ManagementDateTimeConverter.ToDateTime(value).ToString();
                        }
                    }
                }
            }
        }

        private void bt_ClearWMIBrowser_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                tvObjects.Items.Clear();
                //cb_Namespace.SelectedIndex = -1;
                cb_Classes.SelectedIndex = -1;
            }
            catch { }

            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void miAdhocInv_Click(object sender, RoutedEventArgs e)
        {
            string sTag = ((MenuItem)e.Source).Tag.ToString();
            string sName = sTag.Split('|')[0];
            string sNamespace = sTag.Split('|')[1];
            string sQuery = sTag.Split('|')[2];

            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                if (oAgent.isConnected)
                {
                    TreeViewItem tvMain = null;

                    foreach (TreeViewItem tvi in tvObjects.Items)
                    {
                        if (tvi.Header == sName)
                        {
                            tvMain = tvi;
                            continue;
                        }
                    }

                    if (tvMain == null)
                    {
                        tvMain = new TreeViewItem();
                        tvMain.Header = sName;
                        tvObjects.Items.Add(tvMain);
                    }
                    else
                    {
                        tvMain.Items.Clear();
                    }

                    //List<PSObject> oObj = oAgent.Client.GetObjects(sNamespace, sQuery, false);
                    List<PSObject> oObj = oAgent.Client.GetCimObjects(sNamespace, sQuery, true); //use CIMObject to get a valid date format

                    foreach (PSObject PSObj in oObj)
                    {
                        System.Collections.ObjectModel.ObservableCollection<wmiProp> oColl = new System.Collections.ObjectModel.ObservableCollection<wmiProp>();
                        //PSObj.Properties.Where(w => w.GetType() == typeof(PSProperty) & !w.Name.StartsWith("_")).ToList().ForEach(x => oColl.Add(new wmiProp(x.Name, x.Value)));
                        PSObj.Properties.Where(w => w.Value != null & !w.Name.StartsWith("Cim")).ToList().ForEach(x => oColl.Add(new wmiProp(x.Name, x.Value)));

                        DataGrid dg = new DataGrid();
                        dg.CanUserSortColumns = true;
                        dg.CanUserResizeColumns = true;
                        dg.SelectionUnit = DataGridSelectionUnit.CellOrRowHeader;
                        dg.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                        dg.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                        dg.ItemsSource = oColl;

                        //TreeViewItem tvi = new TreeViewItem();
                        //tvi.Header = PSObj.Properties["__RELPATH"].Value;
                        //tvi.Items.Add(dg);
                        tvMain.Items.Add(dg);
                        tvMain.ExpandSubtree();
                    }
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
