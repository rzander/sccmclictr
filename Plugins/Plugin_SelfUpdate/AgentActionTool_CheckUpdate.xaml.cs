using System;
using System.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using RZUpdate;

namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CustomTools_SelfUpdate : System.Windows.Controls.UserControl
    {
        //public SCCMAgent oAgent;
        public CustomTools_SelfUpdate()
        {
            InitializeComponent();

            try
            {
                //Disbale SSL/TLS Errors
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                //Disable CRL Check
                System.Net.ServicePointManager.CheckCertificateRevocationList = false;

                //No check on first start...
                if (Properties.Settings.Default.LastUpdateCheck == new DateTime(2016,1,1))
                {
                    Properties.Settings.Default.LastUpdateCheck = DateTime.Now;
                    Properties.Settings.Default.Save(); //Fixed 25.5.2016 
                    return;
                }


                if ((DateTime.Now - Properties.Settings.Default.LastUpdateCheck) >= new TimeSpan(2, 0, 0, 0) & Properties.Settings.Default.AutoUpdateEnabled)
                {
                    //btCheckUpdate.IsEnabled = SCCMCliCtr.Customization.CheckLicense();
                    string sVersion = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location).FileVersion; ;

                    RZUpdater oRZUpdate = new RZUpdater();
                    var oUpdate = oRZUpdate.CheckForUpdateAsync("Client Center for Configuration Manager", sVersion, "Zander Tools" ).Result;

                    try
                    {
                        Properties.Settings.Default.LastUpdateCheck = DateTime.Now;
                        Properties.Settings.Default.Save();

                        if (IsUserAnAdmin())
                        {

                            //Delete an old RZUpdate.exe
                            if (File.Exists(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "RZGet.exe")))
                            {
                                try
                                {
                                    File.Delete(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "RZGet.exe"));
                                }
                                catch { }
                            }

                            if (oUpdate != null)
                            {
                                //Console.WriteLine("New Version: " + oUpdate.SW.ProductVersion);
                                ExtractSaveResource("AgentActionTools.RZGet.exe", Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "RZGet.exe"));

                                if (System.Windows.MessageBox.Show("Do you want to install Version: " + oUpdate.SW.ProductVersion, "Update available", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    try
                                    {
                                        Process.Start(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "RZGet.exe"), "install \"SCCMCliCtr\"");
                                        Process.GetCurrentProcess().Kill();
                                    }
                                    catch
                                    {
                                        System.Windows.MessageBox.Show("updated failed. Please run ClientCenter with Admin rights to install updates... ", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (oUpdate != null)
                            {
                                System.Windows.MessageBox.Show("An newer Version is available: " + oUpdate.SW.ProductVersion + ". You have to start ClientCenter as Admin to install the update", "Update available", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        public static void ExtractSaveResource(String filename, String location)
        {
            try
            {
                //  Assembly assembly = Assembly.GetExecutingAssembly();
                Assembly a = Assembly.GetExecutingAssembly();
                // Stream stream = assembly.GetManifestResourceStream("Installer.Properties.mydll.dll"); // or whatever 
                string my_namespace = a.GetName().Name.ToString();
                Stream resFilestream = a.GetManifestResourceStream(filename);
                if (resFilestream != null)
                {
                    BinaryReader br = new BinaryReader(resFilestream);
                    FileStream fs = new FileStream(location, FileMode.Create); // say 
                    BinaryWriter bw = new BinaryWriter(fs);
                    byte[] ba = new byte[resFilestream.Length];
                    resFilestream.Read(ba, 0, ba.Length);
                    bw.Write(ba);
                    br.Close();
                    bw.Close();
                    resFilestream.Close();
                }
                // this.Close(); 
            }
            catch { }
        }

        private void btCheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //string sVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
                string sVersion = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location).FileVersion; ;

                RZUpdater oRZUpdate = new RZUpdater();
                var oUpdate = oRZUpdate.CheckForUpdateAsync("Client Center for Configuration Manager", sVersion, "Zander Tools").Result;

                try
                {
                    Properties.Settings.Default.LastUpdateCheck = DateTime.Now;
                    Properties.Settings.Default.Save();

                    if (IsUserAnAdmin())
                    {

                        if (oUpdate != null)
                        {
                            //Console.WriteLine("New Version: " + oUpdate.SW.ProductVersion);
                            ExtractSaveResource("AgentActionTools.RZUpdate.exe", Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "RZGet.exe"));

                            if (System.Windows.MessageBox.Show("Do you want to install Version: " + oUpdate.SW.ProductVersion, "Update available", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                /*new Thread(() =>
                                {
                                    Thread.CurrentThread.IsBackground = true;
                                    if (oUpdate.Download())
                                    {
                                        if (oUpdate.Install(true))
                                        {
                                        }
                                        else
                                        {
                                            System.Windows.MessageBox.Show("Installation failed...");
                                        }
                                    }
                                }).Start();*/

                                Process.Start(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "RZGet.exe"), "SCCMCliCtr");
                                Process.GetCurrentProcess().Kill();
                            }

                        }
                        else
                        {
                            System.Windows.MessageBox.Show("No update available...", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        if (oUpdate != null)
                        {
                            System.Windows.MessageBox.Show("An newer Version is available: " + oUpdate.SW.ProductVersion + ". You have to start ClientCenter as Admin to install the update", "Update available", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch { }

                /*
                //Start only if updater.exe is not yet running...
                if (Process.GetProcessesByName("updater.exe").Count() == 0)
                {
                    Process.Start("updater.exe", "/checknow");
                }*/
            }
            catch { }
        }

        [DllImport("shell32.dll")]
        public static extern bool IsUserAnAdmin();

    }
}
