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
using System.Threading;

namespace ClientCenter.Controls
{
    /// <summary>
    /// Interaction logic for ScheduleControl.xaml
    /// </summary>
    public partial class DayControl : UserControl
    {
        int iMaxCount = 0;
        bool bReordered = false;

        public DayControl()
        {
            InitializeComponent();
        }

        public double BlockHeight = 0;

        public List<timeRange> timeList = new List<timeRange>();

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!bReordered)
            {
                //Order Dates and rearrange to prevent conflicts... it's 00:23.. never ask how it works :-)
                foreach (timeRange tr in timeList)
                {
                    if (tr.endTime < tr.startTime)
                    {
                        TimeSpan oCache = tr.endTime;
                        tr.endTime = tr.startTime;
                        tr.startTime = oCache;
                    }

                    if (tr.endTime.TotalHours > 24)
                        tr.endTime = new TimeSpan(24, 0, 0);
                    if (tr.startTime.TotalHours > 24)
                        tr.endTime = new TimeSpan(24, 0, 0);

                    List<timeRange> lconflict = timeList.Where(t => (t.startTime <= tr.endTime) & (t.endTime >= tr.startTime)).ToList();
                    if (lconflict.Count > 1)
                    {
                        foreach (timeRange tconf in lconflict.Where(t => t != tr))
                        {
                            if (tconf.offset == tr.offset)
                            {
                                tconf.offset++;
                            }

                            if (tconf.offset > iMaxCount)
                                iMaxCount = tconf.offset;
                        }


                    }
                    else
                    {
                        tr.offset = 0;
                    }

                    tr.offset.ToString();
                }

                bReordered = true;
            }

            if (e.NewSize.Width == 0)
                return;

            stackPanel1.Children.Clear();
            timeList = timeList.OrderBy(t => t.startTime).ToList();
            int iMax = 24;
            for (int i = 1; i <= iMax; i++)
            {
                Rectangle oRect = new Rectangle();
                oRect.Width = e.NewSize.Width;
                oRect.Height = Math.Round(e.NewSize.Height / iMax);
                BlockHeight = oRect.Height;
                oRect.Stroke = Brushes.Black;
                oRect.StrokeThickness = 0.1;
                oRect.Name = "Hour" + i.ToString();
                oRect.Margin = new Thickness(0, -1, 0, 0);
                if (i < 7)
                {
                    oRect.Fill = new SolidColorBrush(Colors.LightBlue);
                }
                if (i > 18)
                {
                    oRect.Fill = new SolidColorBrush(Colors.LightBlue);
                }
                stackPanel1.Children.Add(oRect);
            }

            if (ControlCanvas.Children.Count > 1)
            {
                ControlCanvas.Children.RemoveRange(1, ControlCanvas.Children.Count - 1);
            }



            foreach (timeRange tr in timeList)
            {
                TimeBox TB = new TimeBox();
                TB.StartTime = tr.startTime;
                TB.EndTime = tr.endTime;
                
                LinearGradientBrush lBack = new LinearGradientBrush();
                lBack.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x6F, 0x00, 0x00), 1));
                lBack.GradientStops.Add(new GradientStop(tr.color, 0.314));
                lBack.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xFC, 0x64, 0x64), 1));
                lBack.Opacity = .7;
                TB.Brush = lBack;
                TB.ToolTip = tr.Text;

                TB.BlockHeight = BlockHeight;
                TB.Position = tr.offset;
                TB.Width = (e.NewSize.Width / (iMaxCount + 1)) - 10;
                TB.xMargin = ((e.NewSize.Width / (iMaxCount + 1)) * TB.Position) + 5;
                
                ControlCanvas.Children.Add(TB);
            }


            

        }

        public class TimeBox : GroupBox
        {
            public TimeBox()
            {
                lBack = new LinearGradientBrush();
                lBack.EndPoint = new Point(0.5, 1);
                lBack.StartPoint = new Point(0.5, 0);
                lBack.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x6F, 0x00, 0x00), 1));
                lBack.GradientStops.Add(new GradientStop(Colors.Red, 0.314));
                lBack.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xFC, 0x64, 0x64), 1));
                lBack.Opacity = .7;
                this.Header = null;
                this.BorderThickness = new Thickness(0);
                this.BorderBrush = new SolidColorBrush(Colors.Black);
                this.Background = lBack;

                this.Loaded += new RoutedEventHandler(TimeBox_Loaded);
                this.MouseEnter += TimeBox_MouseEnter;
                this.MouseLeave += TimeBox_MouseLeave;


            }

            void TimeBox_MouseLeave(object sender, MouseEventArgs e)
            {
                this.Brush.Opacity = .7;
            }

            void TimeBox_MouseEnter(object sender, MouseEventArgs e)
            {
                this.Brush.Opacity = 1;
            }

            void TimeBox_Loaded(object sender, RoutedEventArgs e)
            {
                this.Height = (BlockHeight - 1) * (EndTime.TotalHours - StartTime.TotalHours);
                this.Margin = new Thickness(xMargin, ((BlockHeight - 1) * StartTime.TotalHours) - 1, 0, 0);
            }

            public TimeSpan StartTime;
            public TimeSpan EndTime;
            public double BlockHeight;
            public int Position;
            public double xMargin;
            private LinearGradientBrush lBack;
            public LinearGradientBrush Brush
            {
                get { return lBack; }
                set
                {
                    lBack = value;
                    this.Background = value;
                }
            }
        }

        public class timeRange
        {
            public timeRange(TimeSpan StartTime, TimeSpan EndTime, string Title)
            {
                startTime = StartTime;
                endTime = EndTime;
                offset = 0;
                color = Colors.Red;
            }
            public timeRange(TimeSpan StartTime, TimeSpan EndTime, Color brushcolor, string Title)
            {
                startTime = StartTime;
                endTime = EndTime;
                offset = 0;
                color = brushcolor;
                Text = Title;
            }
            public TimeSpan startTime;
            public TimeSpan endTime;
            internal int offset;
            public Color color;
            public String Text;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

    }
}
