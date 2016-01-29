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

namespace ClientCenter
{
    /// <summary>
    /// Interaction logic for InstallReapir.xaml
    /// </summary>
    public partial class InstallRepair : UserControl
    {
        private SCCMAgent oAgent;
        public MyTraceListener Listener;

        public InstallRepair()
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

                    }
                    catch { }
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
        }

        private void bt_CleanupMessageQueue_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                oAgent.Client.AgentActions.CleanupMessageQueue();
            }
            catch(Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_WMICheck_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
               Listener.WriteLine(oAgent.Client.Health.WMIVerifyRepository());
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_WMISalvage_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                Listener.WriteLine(oAgent.Client.Health.WMISalvageRepository());
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_WMIReset_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                Listener.WriteLine(oAgent.Client.Health.WMIResetRepository());
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_RegDLL_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                foreach(string sFile in Properties.Settings.Default.RegisterDLLs)
                {
                    oAgent.Client.Health.RegsiertDLL(sFile);
                }
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_DCOMGetDefaultLaunchPermission_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                Listener.WriteLine("DefaultLaunchPermission: " + oAgent.Client.Health.GetDCOMPerm(@"SOFTWARE\Microsoft\Ole", "DefaultLaunchPermission"));
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_DCOMSetDefaultLaunchPermission_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                string sDefaultACL = Properties.Settings.Default.DefaultLaunchPermission;
                oAgent.Client.Health.SetDCOMPerm(@"SOFTWARE\Microsoft\Ole", "DefaultLaunchPermission", sDefaultACL);
                Listener.WriteLine("DefaultLaunchPermission set to:" + sDefaultACL);
                
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_DCOMGetMachineAccessRestriction_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                Listener.WriteLine("MachineAccessRestriction: " + oAgent.Client.Health.GetDCOMPerm(@"SOFTWARE\Microsoft\Ole", "MachineAccessRestriction"));
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_DCOMSetMachineAccessRestriction_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                string sDefaultACL = Properties.Settings.Default.MachineAccessRestriction;
                oAgent.Client.Health.SetDCOMPerm(@"SOFTWARE\Microsoft\Ole", "MachineAccessRestriction", sDefaultACL);
                Listener.WriteLine("MachineAccessRestriction set to:" + sDefaultACL);

            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_DCOMGetMachineLaunchRestriction_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                Listener.WriteLine("MachineLaunchRestriction: " + oAgent.Client.Health.GetDCOMPerm(@"SOFTWARE\Microsoft\Ole", "MachineLaunchRestriction"));
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_DCOMSetMachineLaunchRestriction_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                string sDefaultACL = Properties.Settings.Default.MachineLaunchRestriction;
                oAgent.Client.Health.SetDCOMPerm(@"SOFTWARE\Microsoft\Ole", "MachineLaunchRestriction", sDefaultACL);
                Listener.WriteLine("MachineLaunchRestriction set to:" + sDefaultACL);
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_RemoveAgent_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                //Uninstall SCCM Agent...
                oAgent.Client.AgentActions.UninstallAgent();
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_RepairAgent_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                //Repair SCCM Agent...
                oAgent.Client.AgentActions.RepairAgent();
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_DelCCM_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                //Repair SCCM Agent...
                oAgent.Client.Health.DeleteCCMNamespace();
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_installAgent_Click(object sender, RoutedEventArgs e)
        {
            InstallAgent oA = new InstallAgent();
            try
            {
                if (string.IsNullOrEmpty(oA.tb_MPName.Text))
                {
                    oA.tb_MPName.Text = oAgent.Client.AgentProperties.ManagementPoint;
                }
                if (string.IsNullOrEmpty(oA.tb_SiteCode.Text))
                {
                    oA.tb_SiteCode.Text = oAgent.Client.AgentProperties.AssignedSite;
                }

                oA.RefreshMPandSiteCode();
            }
            catch { }

            oA.ShowDialog();
            if (oA.DialogResult == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;

                Listener.WriteLine("Installing ConfigMgr Agent on target device...");
                oAgent.PSCode.Listeners.Clear();
                Listener.WriteLine(oAgent.Client.GetStringFromPS(oA.tbInstallPS.Text));
                oAgent.PSCode.Listeners.Add(Listener);

                Mouse.OverrideCursor = Cursors.Arrow;

                Properties.Settings.Default.AgentInstallMP = oA.tb_MPName.Text;
                Properties.Settings.Default.AgentInstallSiteCode = oA.tb_SiteCode.Text;

                Properties.Settings.Default.Save();
            }
        }

        private void bt_DeleteAgentCertificates_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                //Delete Certificates in the cert store
                oAgent.Client.Health.DeleteSMSCertificates();
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btLogoff_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                if (MessageBox.Show("Warning: All users will logoff immediately without a Warning !", "Logoff all Users", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    oAgent.Client.GetStringFromPS("(quser) | ? { !$_.contains('USERNAME') } | % { logoff $_.substring(43,2).Trim() }");
                }
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btRestart_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                if (MessageBox.Show("Warning: All users will logoff immediately without a Warning and computer gets restarted !", "Restart Computer", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    oAgent.Client.GetStringFromPS("Restart-Computer -Force");
                }
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btShutdown_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                if (MessageBox.Show("Warning: All users will logoff immediately without a Warning and the computer shuts down !", "Logoff all Users", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    oAgent.Client.GetStringFromPS("Stop-Computer -Force");
                }
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

    }
}
