using System;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using sccmclictr.automation;
using System.IO;
using System.Windows.Controls.Ribbon;

namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MainMenu_PSScripts : UserControl
    {
        internal SCCMAgent oAgent;

        public MainMenu_PSScripts()
        {
            InitializeComponent();
            LoadScripts();
        }

        private void LoadScripts()
        {
            try
            {
                string sCurrentDir = Properties.Settings.Default.ScriptPath;
                if (string.IsNullOrEmpty(sCurrentDir))
                    sCurrentDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                LoadPSFolders(new DirectoryInfo(System.IO.Path.Combine(sCurrentDir, "PSSCripts")), btRunPS);
            }
            catch {}
        }

        private void LoadPSFolders(DirectoryInfo Dir, RibbonMenuButton Menu)
        {
            foreach (string sFile in Directory.GetFiles(Dir.FullName, "*.ps1", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    RibbonButton PSItem = new RibbonButton();
                    PSItem.Label = System.IO.Path.GetFileNameWithoutExtension(new FileInfo(sFile).FullName);
                    PSItem.Tag = sFile;
                    PSItem.SmallImageSource = new BitmapImage(new Uri("/Plugin_PSScripts;component/Images/PS.ico", UriKind.Relative));
                    PSItem.ToolTip = sFile;
                    PSItem.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    PSItem.Click  += PSItem_Click;
                    Menu.Items.Add(PSItem);
                }
                catch { }
            }

            foreach (string sDir in Directory.GetDirectories(Dir.FullName, "*", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    RibbonMenuButton PSFolder = new RibbonMenuButton();
                    PSFolder.Label = new DirectoryInfo(sDir).Name;
                    PSFolder.SmallImageSource = new BitmapImage(new Uri("/Plugin_PSScripts;component/Images/Folder_16.png", UriKind.Relative));
                    PSFolder.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    PSFolder.ToolTip = sDir;
                    LoadPSFolders(new DirectoryInfo(sDir), PSFolder);

                    Menu.Items.Add(PSFolder);
                }
                catch { }
            }

        }

        void PSItem_Click(object sender, EventArgs e)
        {
            try
            {
                //Get PS from File
                string sFile = ((RibbonButton)sender).Tag.ToString();

                StreamReader streamReader = new StreamReader(sFile, Encoding.UTF8);
                string text = streamReader.ReadToEnd();
                streamReader.Close();


                Type t = System.Reflection.Assembly.GetEntryAssembly().GetType("ClientCenter.Common", false, true);

                //Get Agent
                System.Reflection.PropertyInfo pInfo = t.GetProperty("Agent");
                oAgent = (SCCMAgent)pInfo.GetValue(null, null);

                //Run PS and trace result...
                string sRes = oAgent.Client.GetStringFromPS(text);
                oAgent.PSCode.TraceInformation(sRes);

            }
            catch { }
        }
    }
}
