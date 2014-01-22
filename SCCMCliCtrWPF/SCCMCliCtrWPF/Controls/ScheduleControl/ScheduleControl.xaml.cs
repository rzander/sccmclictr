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
        private int daysVisible;

        public int DaysVisible
        {
            get 
            {
                if (daysVisible == 0)
                    return 7;
                else
                    return daysVisible; 
            }
            set 
            {
                daysVisible = value;
                UserControl_Loaded(this, null);
            }
        }
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
                color = Colors.Red;
            }
            public ScheduledTime(DateTime startDateTime, TimeSpan duration, Color brushColor, string title)
            {
                StartDateTime = startDateTime;
                Duration = duration;
                color = brushColor;
                Title = title;
            }
            public ScheduledTime(DateTime startDateTime, TimeSpan duration, Color brushColor, string title, Boolean ISLocal, string SErviceWindowID, uint? SWType)
            {
                StartDateTime = startDateTime;
                Duration = duration;
                color = brushColor;
                Title = title;
                isLocal = ISLocal;
                ServiceWindowID = SErviceWindowID;
                ServiceWindowType = SWType;
            }
            public DateTime StartDateTime;
            public TimeSpan Duration;
            public Color color;
            public Boolean isLocal;
            public string ServiceWindowID;
            public string Title;
            public uint? ServiceWindowType;
        }
        
        public event EventHandler DeleteClick;
        private delegate void NoArgDelegate();
        public void Refresh()
        {
            stackPanel2.Children.Clear();
            
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render,

                (NoArgDelegate)delegate
            {
            });

            UserControl_Loaded(this, null);
            
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
                DC.DeleteClick += DC_DeleteClick;
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

        void DC_DeleteClick(object sender, EventArgs e)
        {
            if (this.DeleteClick != null)
                this.DeleteClick(sender, e);
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
                    //Move it to next day
                    endTime = new TimeSpan(24, 0, 0);
                    AddSchedule(new ScheduledTime(new DateTime(oTime.StartDateTime.Year, oTime.StartDateTime.Month, oTime.StartDateTime.Day, 0, 0, 0).AddDays(1), (oTime.StartDateTime.TimeOfDay + oTime.Duration).Subtract(new TimeSpan(24, 0, 0)), oTime.color, oTime.Title, oTime.isLocal, oTime.ServiceWindowID, oTime.ServiceWindowType)); ;
                }

                if ((oTime.StartDateTime.Date - DateTime.Now.Date) <= new TimeSpan(DaysVisible, 0, 0, 0))
                {
                    DC.timeList.Add(new DayControl.timeRange(startTime, endTime, oTime.color, oTime.Title, oTime.isLocal, oTime.ServiceWindowID, oTime.ServiceWindowType));
                }
            }
        }
    }


}
