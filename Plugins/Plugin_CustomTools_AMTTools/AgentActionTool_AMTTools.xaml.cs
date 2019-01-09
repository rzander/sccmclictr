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
using System.IO;
using sccmclictr.automation;

using System.Net;
using System.Web;
using System.Management.Automation;
using System.Collections.ObjectModel;


namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
 
    public partial class CustomTools_AMTTools : UserControl
    {
        public CustomTools_AMTTools()
        {
            InitializeComponent();

            StartCheck();

        }

        private void rb_KVMView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                Type t = System.Reflection.Assembly.GetEntryAssembly().GetType("ClientCenter.Common", false, true);
                //Get the Hostname
                System.Reflection.PropertyInfo pInfo = t.GetProperty("Hostname");

                string sHost = (string)pInfo.GetValue(null, null);

                //Custom Code here...

                Process KVMView = new Process();
                KVMView.StartInfo.FileName = string.Format(AgentActionTools.Properties.Resources.KVMViewFile); //".\\KVMView.exe";
                KVMView.StartInfo.Arguments = string.Format(AgentActionTools.Properties.Resources.KVMViewArgs, sHost);
                KVMView.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                KVMView.Start();
                
                //...
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void rb_AlarmExt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Type t = System.Reflection.Assembly.GetEntryAssembly().GetType("ClientCenter.Common", false, true);
                //Get the Hostname
                System.Reflection.PropertyInfo pInfo = t.GetProperty("Hostname");

                string sHost = (string)pInfo.GetValue(null, null);

                //Custom Code here...

                Process AlarmExt = new Process();
                AlarmExt.StartInfo.FileName = string.Format(AgentActionTools.Properties.Resources.AlarmExtFile);
                AlarmExt.StartInfo.Arguments = string.Format(AgentActionTools.Properties.Resources.AlarmExtArgs, sHost);
                AlarmExt.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                AlarmExt.Start();

                //...
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void rb_MPSView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Type t = System.Reflection.Assembly.GetEntryAssembly().GetType("ClientCenter.Common", false, true);
                //Get the Hostname
                System.Reflection.PropertyInfo pInfo = t.GetProperty("Hostname");

                string sHost = (string)pInfo.GetValue(null, null);

                //Custom Code here...

                Process MPSView = new Process();
                MPSView.StartInfo.FileName = string.Format(AgentActionTools.Properties.Resources.MPSViewFile);
                MPSView.StartInfo.Arguments = string.Format(AgentActionTools.Properties.Resources.MPSViewArgs, sHost);
                MPSView.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                MPSView.Start();
                
                //...
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void rb_Download_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string svProDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\vPro";
                string sLocalPath = Environment.ExpandEnvironmentVariables("%TEMP%");
                string sZipFile = System.IO.Path.Combine(sLocalPath, "IntelvProSCCMAddOn-v2.zip");
                string svProTempFolder = System.IO.Path.Combine(sLocalPath, "vProTempFolder");
                string sMSI = System.IO.Path.Combine(svProTempFolder, "x64\\IntelvProSccmAddOnx64.msi");
                string sUnpacked = System.IO.Path.Combine(svProTempFolder, "Unpacked");

                if (!Directory.Exists(svProDir))
                {
                    Directory.CreateDirectory(svProDir);
                }

                if (!File.Exists(sZipFile))
                {
                    string swgetVpro = string.Format("wget http://downloadmirror.intel.com/21835/eng/IntelvProSCCMAddOn-v2.zip -OutFile \"{0}\"", sZipFile);

                    try
                    {
                        var oResult = _RunPS(swgetVpro);
                    }
                    catch { }
                }

                if (File.Exists(sZipFile))
                {
                    if (!Directory.Exists(svProTempFolder))
                    {
                        Directory.CreateDirectory(svProTempFolder);
                        Directory.CreateDirectory(sUnpacked);

                        string sUnZip = string.Format("(new-object -com shell.application).namespace(\"{0}\").CopyHere((new-object -com shell.application).namespace(\"{1}\").Items(),16)", svProTempFolder, sZipFile);

                        try
                        {
                            var oResult = _RunPS(sUnZip);
                        }
                        catch { }

                        string sMSIUnpack = string.Format("& msiexec.exe /a `\"{0}`\" /qb TARGETDIR=`\"{1}`\" |Write-Output", sMSI, sUnpacked);

                        try
                        {
                            var oResult = _RunPS(sMSIUnpack);
                        }
                        catch { }



                        string sCopyItems = string.Format("Copy-Item \"{0}\\*\" \"{1}\" -Exclude \"IntelvProSccmAddOnx64.msi\" -Force", sUnpacked, svProDir);

                        try
                        {
                            var oResult = _RunPS(sCopyItems);
                        }
                        catch { }
                    }

                    Directory.Delete(svProTempFolder, true);
                    File.Delete(sZipFile);
                }
                else
                {
                    MessageBox.Show("IntelvProSCCMAddOn-v2.zip could not be downloaded, try manually. Copy the required Files to: " + svProDir);
                    Process.Start("http://downloadmirror.intel.com/21835/eng/IntelvProSCCMAddOn-v2.zip");
                    Process.Start(svProDir);
                }

                StartCheck();
            }
            catch { }
        }

        private void StartCheck()
        {
            rb_Download.Visibility = System.Windows.Visibility.Collapsed;
            
            string sCurrentDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\vPro\\KVMView.exe";
            //string curFile = @"c:\temp\test.txt";
            if (File.Exists(sCurrentDir))
            {
                rb_AlarmExt.Visibility = System.Windows.Visibility.Visible; // .IsVisible = false;
                rb_KVMView.Visibility = System.Windows.Visibility.Visible;
                rb_MPSView.Visibility = System.Windows.Visibility.Visible;
                rb_Download.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                rb_AlarmExt.Visibility = System.Windows.Visibility.Collapsed; // .IsVisible = false;
                rb_KVMView.Visibility = System.Windows.Visibility.Collapsed;
                rb_MPSView.Visibility = System.Windows.Visibility.Collapsed;
                rb_Download.Visibility = System.Windows.Visibility.Visible;
            }

        }


        /// <summary>
        /// Run PowerShell
        /// </summary>
        /// <param name="PSScript">PowerShell Script</param>
        /// <returns></returns>
        public Collection<PSObject> _RunPS(string PSScript)
        {
            PowerShell PowerShellInstance = PowerShell.Create();

            PowerShellInstance.AddScript(PSScript);

            Collection<PSObject> PSOutput = PowerShellInstance.Invoke();

            return PSOutput;

        }

    }
}
