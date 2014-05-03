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

namespace ClientCenter.Controls
{
    /// <summary>
    /// Interaction logic for LogGrid.xaml
    /// </summary>
    public partial class LogGrid : UserControl
    {
        public LogGrid()
        {
            InitializeComponent();
            LogLines = new List<LogEntry>();
        }

        public class LogEntry
        {
            public string LogText { get; set; }
            public string Component { get; set; }
            public DateTime Date { get; set; }
        }

        public List<LogEntry> LogLines
        {
            get { return LogGridList.ItemsSource as List<LogEntry>; }
            set { LogGridList.ItemsSource  = value; }
        }
    }
}
