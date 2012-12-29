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

using System.ComponentModel;

namespace ClientCenter.Controls
{

    /// <summary>
    /// Interaction logic for TimeScheduler.xaml
    /// </summary>
    public partial class ScheduleControl : UserControl
    {
        public int DaysVisible = 5;

        public DateTime Startdate = DateTime.Now;
        public ScheduleControl()
        {
            InitializeComponent();



        }

        public List<ScheduledTime> ScheduledTimes = new List<ScheduledTime>();

        public class ScheduledTime
        {
            public ScheduledTime(DateTime startDateTime, TimeSpan duration)
            {
                StartDateTime = startDateTime;
                Duration = duration;
            }
            public DateTime StartDateTime;
            public TimeSpan Duration;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            foreach (DayControl DC in stackPanel2.Children)
            {
                DC.Width = DaysDock1.ActualWidth / DaysVisible;
            }

            foreach (Label LDay in HeaderStack1.Children)
            {
                LDay.Width = DaysDock1.ActualWidth / DaysVisible;
            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            stackPanel2.Children.Clear();
            for (int iDay = 1; iDay <= DaysVisible; iDay++)
            {
                DayControl DC = new DayControl();
                DC.Width = DaysDock1.ActualWidth / DaysVisible;
                DC.Name = "Day" + iDay.ToString();
                DC.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                List<DayControl.timeRange> tList = new List<DayControl.timeRange>();

                stackPanel2.Children.Add(DC);

                Label lDay = new Label();
                lDay.Content = Startdate.AddDays(iDay - 1).ToString("dddd", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat);

                lDay.Width = DaysDock1.ActualWidth / DaysVisible;
                lDay.Margin = new Thickness(0, -2, 0, 0);
                HeaderStack1.Children.Add(lDay);
            }
            List<ScheduledTime> NextTimes = new List<ScheduledTime>();
            foreach (ScheduledTime oTime in ScheduledTimes)
            {
                AddSchedule(oTime);
            }
        }

        public void AddSchedule(ScheduledTime oTime)
        {
            //int iDay = (DateTime.Now.Date - oTime.StartDateTime).Days;
            int iDay = (oTime.StartDateTime.Date - DateTime.Now.Date).Days;
            if (iDay < DaysVisible & iDay >= 0)
            {

                DayControl DC = (DayControl)stackPanel2.Children[iDay];
                TimeSpan startTime = oTime.StartDateTime.TimeOfDay;
                TimeSpan endTime = oTime.StartDateTime.TimeOfDay + oTime.Duration;

                if (endTime.TotalHours > 24)
                {
                    endTime = new TimeSpan(24, 0, 0);
                    AddSchedule(new ScheduledTime(new DateTime(oTime.StartDateTime.Year, oTime.StartDateTime.Month, oTime.StartDateTime.Day, 0, 0, 0).AddDays(1), (oTime.StartDateTime.TimeOfDay + oTime.Duration).Subtract(new TimeSpan(24, 0, 0)))); ;
                }

                if ((oTime.StartDateTime.Date - DateTime.Now.Date) <= new TimeSpan(DaysVisible, 0, 0, 0))
                {
                    DC.timeList.Add(new DayControl.timeRange(startTime, endTime));
                }


            }
        }
    }


}
