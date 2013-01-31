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
using sccmclictr.automation.functions;
using sccmclictr.automation.schedule;

namespace ClientCenter.Controls
{
    /// <summary>
    /// Interaction logic for ServiceWindowGrid.xaml
    /// </summary>
    public partial class ServiceWindowGrid : UserControl
    {
        private SCCMAgent oAgent;
        public MyTraceListener Listener;
        public ServiceWindowGrid()
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
                        scheduleControl1.DaysVisible = 7;
                        if (oAgent.isConnected)
                        {
                            scheduleControl1.ScheduledTimes.Clear();
                            foreach (sccmclictr.automation.policy.actualConfig.CCM_ServiceWindow oSRW in oAgent.Client.ActualConfig.ServiceWindow)
                            {
                                GetSchedules(oSRW.DecodedSchedule);
                            }
                        }

                    }
                    catch { }
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
        }

        public void GetSchedules(object Schedule)
        {
            var oWin = Schedule;
            switch (oWin.GetType().Name)
            {
                case ("List`1"):
                    foreach (var subsched in oWin as List<object>)
                    {
                        GetSchedules(subsched);
                    }
                    break;
                case ("SMS_ST_NonRecurring"):
                    break;
                case ("SMS_ST_RecurInterval"):
                    break;
                case ("SMS_ST_RecurWeekly"):
                    ScheduleDecoding.SMS_ST_RecurWeekly oSched = ((ScheduleDecoding.SMS_ST_RecurWeekly)oWin);

                    string sDay = new DateTime(2009, 2, oSched.Day).DayOfWeek.ToString();
                    DateTime dNextStartTime = oSched.NextStartTime;
                    string sRecurText = string.Format("Occours Every ({0})weeks on {1}", oSched.ForNumberOfWeeks, sDay);
                    DateTime dNextRun = dNextStartTime;

                    //Check if there is a schedule today... (past)
                    if (oSched.PreviousStartTime.Date == DateTime.Now.Date)
                        dNextRun = oSched.PreviousStartTime;

                    while (dNextRun.Date < DateTime.Now.Date + new TimeSpan(scheduleControl1.DaysVisible, 0, 0, 0))
                    {
                        scheduleControl1.ScheduledTimes.Add(new ScheduleControl.ScheduledTime(dNextRun, new TimeSpan(oSched.DayDuration, oSched.HourDuration, oSched.MinuteDuration, 0)));
                        //add_Appointment(dNextRun, dNextRun + new TimeSpan(oSchedule.DayDuration, oSchedule.HourDuration, oSchedule.MinuteDuration, 0), oSchedule.IsGMT);
                        dNextRun = dNextRun + new TimeSpan(oSched.ForNumberOfWeeks * 7, 0, 0, 0);
                    }
                    break;
                case ("SMS_ST_RecurMonthlyByWeekday"):
                    break;
                case ("SMS_ST_RecurMonthlyByDate"):
                    break;
            }
        }

        private void bt_Reload_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                scheduleControl1.InitializeComponent();
                
                if (oAgent.isConnected)
                {
                    scheduleControl1.ScheduledTimes.Clear();

                    foreach (sccmclictr.automation.policy.actualConfig.CCM_ServiceWindow oSRW in oAgent.Client.ActualConfig.ServiceWindow)
                    {
                        GetSchedules(oSRW.DecodedSchedule);
                    }

                    scheduleControl1.DaysVisible = 7;
                }

            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }
    }
}
