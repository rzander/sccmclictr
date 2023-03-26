using System;
using System.Text;
using System.Windows.Media.Imaging;
using sccmclictr.automation;
using System.IO;
using System.Windows.Controls.Ribbon;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using Cursors = System.Windows.Input.Cursors;
using UserControl = System.Windows.Controls.UserControl;

namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MainMenu_PSScripts : UserControl
    {
        internal SCCMAgent oAgent;
        internal string scriptdir = "";
        delegate void AnonymousDelegate();

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
                {
                    sCurrentDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    sCurrentDir = System.IO.Path.Combine(sCurrentDir, "PSScripts");
                    if(!Directory.Exists(sCurrentDir))
                    {
                        Directory.CreateDirectory(sCurrentDir);
                    }
                }

                scriptdir = sCurrentDir;

                LoadPSFolders(new DirectoryInfo(sCurrentDir), btRunPS);
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
                    PSItem.Click += PSItem_Click;
                    
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
                    PSFolder.Tag = sDir;
                    LoadPSFolders(new DirectoryInfo(sDir), PSFolder);

                    Menu.Items.Add(PSFolder);
                }
                catch { }
            }

        }

        private void PSFolder_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                string sArg = "";
                if(sender.GetType() == typeof(RibbonMenuButton))
                    sArg = (sender as RibbonMenuButton).Tag as string;

                if (sender.GetType() == typeof(RibbonSplitButton))
                    sArg = (sender as RibbonSplitButton).Tag as string;

                if (string.IsNullOrEmpty(sArg))
                    sArg = scriptdir;

                System.Diagnostics.Process Explorer = new System.Diagnostics.Process();
                Explorer.StartInfo.FileName = "Explorer.exe";
                Explorer.StartInfo.Arguments = "\"" + sArg + "\"" as string;
                Explorer.Start();
            }
            catch { }
        }

        void PSItem_Click(object sender, EventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                _rgPS.IsDropDownOpen = false;
                Application.DoEvents();
                btRunPS.IsDropDownOpen = false;
                Application.DoEvents();

                //Get PS from File
                string sFile = ((RibbonButton)sender).Tag.ToString();

                StreamReader streamReader = new StreamReader(sFile, Encoding.UTF8);
                string text = streamReader.ReadToEnd();
                streamReader.Close();

                Type t = System.Reflection.Assembly.GetEntryAssembly().GetType("ClientCenter.Common", false, true);

                //Get Agent
                System.Reflection.PropertyInfo pInfo = t.GetProperty("Agent");
                oAgent = (SCCMAgent)pInfo.GetValue(null, null);

                Application.DoEvents();
                Thread.Sleep(200);
                Application.DoEvents();

                string sRet = oAgent.Client.GetStringFromPS(text);
                oAgent.PSCode.TraceInformation(sRet);
            }
            finally
            {
                btRunPS.IsDropDownOpen = false;
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        private void btRunPS_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PSFolder_MouseDown(sender, null);
        }

        private void btRunPS_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.F5)
            {
                btRunPS.Items.Clear();
                LoadScripts();
            }
        }
    }
}
