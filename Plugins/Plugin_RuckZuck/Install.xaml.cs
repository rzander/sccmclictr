using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using sccmclictr.automation;

namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for Install.xaml
    /// </summary>
    public partial class Install : Window
    {
        public Install()
        {
            InitializeComponent();
        }
        public Install(SCCMAgent oAgt)
        {
            InitializeComponent();
            InstPanel.oAgent = oAgt;
        }

        public SCCMAgent oAgent;
    }
}
