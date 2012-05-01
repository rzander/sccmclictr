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
using sccmclictr.automation;

namespace ClientCenter
{
    /// <summary>
    /// Interaction logic for AgentSettingItem.xaml
    /// </summary>
    public partial class AgentSettingItem : UserControl
    {
        private SCCMAgent oAgent;
        public AgentSettingItem()
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
                    oAgent = value;
                    AgentSettings.IsEnabled = true;

                    Mouse.OverrideCursor = Cursors.Wait;

                    imgSiteCode_MouseLeftButtonDown(this, null);
                    imgAgentVersion_MouseLeftButtonDown(this, null);
                    imgMP_MouseLeftButtonDown(this, null);

                    Mouse.OverrideCursor = Cursors.Arrow;

                }
            }
        }

        private void imgSiteCode_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            tbSiteCode.Text = oAgent.Client.AgentProperties.AssignedSite;
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void imgAgentVersion_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            tbAgentVersion.Text = oAgent.Client.AgentProperties.ClientVersion;
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void imgMP_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            tbMP.Text = oAgent.Client.AgentProperties.ManagementPoint;
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void imgINetMP_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            tbInetMP.Text = oAgent.Client.AgentProperties.ManagementPointInternet;
            Mouse.OverrideCursor = Cursors.Arrow;
        }

    }
}
