using System;
using System.Windows;
using System.Windows.Controls;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CustomTools_EnablePSRemoting : UserControl
    {
        public CustomTools_EnablePSRemoting()
        {
            InitializeComponent();
        }

        private async void btEnableWinRM_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => {
                try
                {
                    Type t = System.Reflection.Assembly.GetEntryAssembly().GetType("ClientCenter.Common", false, true);
                    //Get the Hostname
                    System.Reflection.PropertyInfo pInfo = t.GetProperty("Hostname");
                    string sHost = (string)pInfo.GetValue(null, null);

                    //Run PS to enable WinRM
                    string sPSCode = "Invoke-WmiMethod -ComputerName " + sHost + " -Namespace root\\cimv2 -Class Win32_Process -Name Create -ArgumentList '\"C:\\Windows\\system32\\WindowsPowerShell\\v1.0\\powershell.exe\" \"Enable-PSRemoting -Force\"'";
                    PowerShell PowerShellInstance = PowerShell.Create();
                    PowerShellInstance.AddScript(sPSCode);

                    Collection<PSObject> PSOutput = PowerShellInstance.Invoke();

                    MessageBox.Show("WinRM enabled, please try to connect...", sHost);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

        }
    }
}
