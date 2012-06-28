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
    /// Interaction logic for AgentComponents.xaml
    /// </summary>
    public partial class AgentComponents : UserControl
    {
        private SCCMAgent oAgent;

        public AgentComponents()
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
                        dataGrid1.BeginInit();
                        dataGrid1.ItemsSource = oAgent.Client.Components.InstalledComponents.OrderBy(t => t.DisplayName).ToList();
                        dataGrid1.EndInit();
                    }
                    catch { }
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
        }
    }
}
