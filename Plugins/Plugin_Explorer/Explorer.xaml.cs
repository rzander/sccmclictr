using System;
using System.Windows;
using System.Diagnostics;

using sccmclictr.automation;
using System.Windows.Controls.Ribbon;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Collections.Specialized;
using System.Xml;
using System.Reflection;

namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AgentActionTool_Explorer : System.Windows.Controls.UserControl
    {
        public SCCMAgent oAgent;
        public AgentActionTool_Explorer()
        {
            InitializeComponent();
            btExplore.IsEnabled = SCCMCliCtr.Customization.CheckLicense();
            if (!btExplore.IsEnabled)
            {
                gExplore.ToolTip = "Please make a donation to get access to this feature !";
            }

            foreach(string sPath in xmlFolders)
            {
                try
                {
                    RibbonButton bR = new RibbonButton();
                    bR.Label = sPath;
                    bR.Tag = sPath;
                    bR.SmallImageSource = new BitmapImage(new Uri(@"/Plugin_Explorer;component/Images/shell32.dll_I010b_0409.ico", UriKind.Relative));
                    bR.ToolTip = sPath;
                    bR.Click += btC_Click;
                    btExplore.Items.Add(bR);
                }
                catch { }
            }
            
        }

        private void btC_Click(object sender, RoutedEventArgs e)
        {
            try{

                if (((FrameworkElement)sender).Tag != null)
                {
                    Type t = System.Reflection.Assembly.GetEntryAssembly().GetType("ClientCenter.Common", false, true);
                    System.Reflection.PropertyInfo pInfo = t.GetProperty("Agent");
                    oAgent = (SCCMAgent)pInfo.GetValue(null, null);
                    string sHost = oAgent.TargetHostname;

                    string sTag = ((FrameworkElement)sender).Tag.ToString();
                    string sShare = "";
                    switch(sTag)
                    {
                        case "C":
                            sShare = "C$";
                            break;
                        case "Admin":
                            sShare = "Admin$";
                            break;
                        case "WBEM":
                            sShare = @"Admin$\System32\wbem";
                            break;
                        case "ccmsetup":
                            sShare = @"Admin$\ccmsetup\logs";
                            break;
                        case "CCMLOGS":
                            if (oAgent.isConnected)
                                sShare = oAgent.Client.AgentProperties.LocalSCCMAgentLogPath.Replace(':', '$');
                            else
                                sShare = @"Admin$\ccm\logs";
                            break;
                        default:
                            sShare = sTag;
                            break;

                    }


                    //Connect IPC$ if not already connected (not needed with integrated authentication)
                    //if (!oAgent.ConnectIPC())
                    //    oAgent.ConnectIPC = true;

                    Process Explorer = new Process();
                    Explorer.StartInfo.FileName = "Explorer.exe";
                    Explorer.StartInfo.Arguments = @"\\" + oAgent.TargetHostname + @"\" + sShare;
                    Explorer.Start();
                }
            }
            catch{}
        }

        public static string ConfigPath
        {
            get
            {
                //Get XML Settings
                return Assembly.GetExecutingAssembly().Location + ".config";
            }
        }

        internal static StringCollection xmlFolders
        {
            get
            {
                try
                {
                    StringCollection sResults = new StringCollection();
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(ConfigPath);
                    var xNodes = xDoc.SelectNodes("//configuration/applicationSettings/AgentActionTools.Properties.Settings/setting[@name='Folders']/value/ArrayOfString/string");
                    foreach (XmlNode xNode in xNodes)
                    {
                        sResults.Add(xNode.InnerText.ToString());
                    }

                    return sResults;
                }
                catch { }

                return Properties.Settings.Default.Folders;
            }
        }

    }
}
