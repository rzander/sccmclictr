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
using System.Globalization;

namespace ClientCenter.Controls
{
    /// <summary>
    /// Interaction logic for ServiceWindowGrid.xaml
    /// </summary>
    public partial class ServiceWindowGrid : UserControl
    {
        private SCCMAgent oAgent;
        public MyTraceListener Listener;
        public event EventHandler RequestRefresh;

        public ServiceWindowGrid()
        {
            InitializeComponent();
            scheduleControl1.DeleteClick += scheduleControl1_DeleteClick;
        }

        void scheduleControl1_DeleteClick(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(CloseButton))
            {
                string SWindowID = ((CloseButton)sender).ID;
                if (oAgent.isConnected)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    try
                    {
                        oAgent.Client.RequestedConfig.DeleteServiceWindow(SWindowID);
                        if (this.RequestRefresh != null)
                            this.RequestRefresh(sender, e);

                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
                
            }

            try
            {
                if (oAgent.isConnected)
                {
                    scheduleControl1.ScheduledTimes.Clear();
                    SCCMAgentConnection = oAgent;
                    scheduleControl1.Refresh();
                }
            }
            catch(Exception ex)
            {
                ex.Message.ToString();
            }
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

                            try
                            {
                                foreach (sccmclictr.automation.policy.requestedConfig.CCM_ServiceWindow oSRW in oAgent.Client.RequestedConfig.ServiceWindow)
                                {
                                    GetSchedules(oSRW.DecodedSchedule, oSRW.ServiceWindowID, oSRW.PolicySource, oSRW.ServiceWindowType);
                                }
                            }
                            catch { }
                        }

                    }
                    catch(Exception ex)
                    {
                        ex.Message.ToString();
                    }
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
        }

        public void GetSchedules(object Schedule)
        {
            GetSchedules(Schedule, "", "", 1);
        }
        public void GetSchedules(object Schedule, string ServiceWindowID)
        {
            GetSchedules(Schedule, ServiceWindowID, "", 1);
        }
        public void GetSchedules(object Schedule, string ServiceWindowID, string PolicySource, uint? SWType)
        {
            Color cForeColor = Colors.Blue;
            switch (SWType)
            {
                case 1:
                    cForeColor = Colors.Blue;
                    break;
                case 2:
                    cForeColor = Colors.Brown;
                    break;
                case 3:
                    cForeColor = Colors.Violet;
                    break;
                case 4:
                    cForeColor = Colors.Red;
                    break;
                case 5:
                    cForeColor = Colors.Orange;
                    break;
                case 6:
                    cForeColor = Colors.Green;
                    break;

                default:
                    cForeColor = Colors.Blue;
                    break;
            }
            Boolean isLocal = false;
            if(string.Compare(PolicySource, "LOCAL", true) == 0)
                isLocal = true;

            var oWin = Schedule;
            try
            {
                switch (oWin.GetType().Name)
                {
                    case ("List`1"):
                        foreach (var subsched in oWin as List<object>)
                        {
                            GetSchedules(subsched, ServiceWindowID, PolicySource, SWType);
                        }
                        break;
                    case ("SMS_ST_NonRecurring"):
                        ScheduleDecoding.SMS_ST_NonRecurring oSchedNonRec = ((ScheduleDecoding.SMS_ST_NonRecurring)oWin);

                        string sDayNonRec = new DateTime(2012, 7, oSchedNonRec.StartTime.Day).DayOfWeek.ToString();

                        DateTime dNextRunNonRec = oSchedNonRec.NextStartTime;
                        if (oSchedNonRec.StartTime + new TimeSpan(oSchedNonRec.DayDuration, oSchedNonRec.HourDuration, 0, 0) >= DateTime.Now.Date)
                        {

                            ScheduleControl.ScheduledTime oControl = new ScheduleControl.ScheduledTime(dNextRunNonRec, new TimeSpan(oSchedNonRec.DayDuration, oSchedNonRec.HourDuration, oSchedNonRec.MinuteDuration, 0), cForeColor, "Non Recuring", isLocal, ServiceWindowID, SWType);
                            scheduleControl1.ScheduledTimes.Add(oControl);
                        }
                        break;
                    case ("SMS_ST_RecurInterval"):
                        ScheduleDecoding.SMS_ST_RecurInterval oSchedInt = ((ScheduleDecoding.SMS_ST_RecurInterval)oWin);
                        DateTime dNextStartTimeInt = oSchedInt.NextStartTime;
                        DateTime dNextRunInt = dNextStartTimeInt;

                        string sRecurTextInt = string.Format("Occours Every ({0})Day(s)", oSchedInt.DaySpan);
                        if (oSchedInt.DaySpan == 0)
                            sRecurTextInt = string.Format("Occours Every ({0})Hour(s)", oSchedInt.HourSpan);

                        //Check if there is a schedule today... (past)
                        if (oSchedInt.PreviousStartTime.AddDays(oSchedInt.DayDuration).AddHours(oSchedInt.HourDuration).Date == DateTime.Now.Date)
                            dNextRunInt = oSchedInt.PreviousStartTime;

                        while (dNextRunInt.Date < DateTime.Now.Date + new TimeSpan(scheduleControl1.DaysVisible, 0, 0, 0))
                        {
                            scheduleControl1.ScheduledTimes.Add(new ScheduleControl.ScheduledTime(dNextRunInt, new TimeSpan(oSchedInt.DayDuration, oSchedInt.HourDuration, oSchedInt.MinuteDuration, 0), cForeColor, sRecurTextInt + " " + ServiceWindowID, isLocal, ServiceWindowID, SWType));
                            dNextRunInt = dNextRunInt + new TimeSpan(oSchedInt.DaySpan, oSchedInt.HourSpan, oSchedInt.MinuteSpan, 0);
                        }
                        break;
                    case ("SMS_ST_RecurWeekly"):
                        ScheduleDecoding.SMS_ST_RecurWeekly oSched = ((ScheduleDecoding.SMS_ST_RecurWeekly)oWin);

                        string sDay = new DateTime(2012, 7, oSched.Day).DayOfWeek.ToString();
                        DateTime dNextStartTime = oSched.NextStartTime;
                        string sRecurText = string.Format("Occours Every ({0})weeks on {1} {2}", oSched.ForNumberOfWeeks, sDay, ServiceWindowID);
                        DateTime dNextRun = dNextStartTime;

                        if (oSched.PreviousStartTime.AddDays(oSched.DayDuration).AddHours(oSched.HourDuration).AddSeconds(-1).Date == DateTime.Now.Date)
                            dNextRun = oSched.PreviousStartTime;

                        while (dNextRun.Date <= DateTime.Now.Date + new TimeSpan(scheduleControl1.DaysVisible, 0, 0, 0))
                        {
                            scheduleControl1.ScheduledTimes.Add(new ScheduleControl.ScheduledTime(dNextRun, new TimeSpan(oSched.DayDuration, oSched.HourDuration, oSched.MinuteDuration, 0), cForeColor, sRecurText, isLocal, ServiceWindowID, SWType));
                            dNextRun = dNextRun + new TimeSpan(oSched.ForNumberOfWeeks * 7, 0, 0, 0);
                        }
                        break;
                    case ("SMS_ST_RecurMonthlyByWeekday"):
                        ScheduleDecoding.SMS_ST_RecurMonthlyByWeekday oSchedRMBW = ((ScheduleDecoding.SMS_ST_RecurMonthlyByWeekday)oWin);

                        string sDayRMBW = new DateTime(2012, 7, oSchedRMBW.Day).DayOfWeek.ToString();
                        string sRecure = "";
                        switch (oSchedRMBW.WeekOrder)
                        {
                            case 1:
                                sRecure = "1st";
                                break;
                            case 2:
                                sRecure = "2nd";
                                break;
                            case 3:
                                sRecure = "3rd";
                                break;
                            case 4:
                                sRecure = "4th";
                                break;
                            case 5:
                                sRecure = "5th";
                                break;
                        }

                        string sRecurTextRMBW = string.Format("Occours Every ({0}) Month on the {1} {2}", oSchedRMBW.ForNumberOfMonths, sRecure, sDayRMBW);
                        DateTime dNextRunRMBW = oSchedRMBW.NextStartTime;

                        while (dNextRunRMBW.Date < DateTime.Now.Date + new TimeSpan(scheduleControl1.DaysVisible, 0, 0, 0))
                        {
                            scheduleControl1.ScheduledTimes.Add(new ScheduleControl.ScheduledTime(dNextRunRMBW, new TimeSpan(oSchedRMBW.DayDuration, oSchedRMBW.HourDuration, oSchedRMBW.MinuteDuration, 0), cForeColor, sRecurTextRMBW, isLocal, ServiceWindowID, SWType));
                            dNextRunRMBW = dNextRunRMBW.AddMonths(oSchedRMBW.ForNumberOfMonths);
                        }
                        break;
                    case ("SMS_ST_RecurMonthlyByDate"):
                        break;
                }
            }
            catch { }
        }

        private void bt_Reload_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                scheduleControl1_DeleteClick(sender, null);
            }
            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_NewServiceWindow_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                ServiceWindowNew oSWForm = new ServiceWindowNew();
                oSWForm.datePicker1.SelectedDate = DateTime.Now;
                oSWForm.timeBox.Text = DateTime.Now.ToString("t", new CultureInfo("de-CH"));
                oSWForm.ShowDialog();
                if (oSWForm.DialogResult.HasValue && oSWForm.DialogResult.Value)
                {
                    ScheduleDecoding.SMS_ST_NonRecurring oSched = new ScheduleDecoding.SMS_ST_NonRecurring();
                    if (oSWForm.datePicker1.SelectedDate != null)
                    {
                        oSched.StartTime = (DateTime)oSWForm.datePicker1.SelectedDate;
                        TimeSpan oTime = TimeSpan.Parse(oSWForm.timeBox.Text, new CultureInfo("de-CH"));
                        oSched.StartTime = oSched.StartTime.Add(oTime);
                        oSched.IsGMT = false;

                        //Define duration
                        TimeSpan tsDuration = DateTime.ParseExact(oSWForm.tbDuration.Text, "t", new CultureInfo("de-CH")).TimeOfDay;
                        oSched.MinuteDuration = tsDuration.Minutes;
                        oSched.HourDuration = tsDuration.Hours;
                        oSched.DayDuration = 0;

                        uint iType = uint.Parse(((ComboBoxItem)oSWForm.swTypeComboBox.SelectedItem).Tag.ToString());
                        oAgent.Client.RequestedConfig.CreateServiceWindow(oSched.ScheduleID, iType);

                        //Refresh control
                        scheduleControl1_DeleteClick(sender, null);
                    }
                }
            }

            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void bt_CleanServiceWindow_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                foreach (sccmclictr.automation.policy.requestedConfig.CCM_ServiceWindow oSW in oAgent.Client.RequestedConfig.ServiceWindow)
                {
                    try
                    {
                        if (string.Compare(oSW.PolicySource, "local", true) == 0)
                        {
                            if (oSW.ServiceWindowType != 6)
                            {
                                oAgent.Client.RequestedConfig.DeleteServiceWindow(oSW.ServiceWindowID);
                            }
                        }
                    }
                    catch { }
                }

                //Refresh control
                scheduleControl1_DeleteClick(sender, null);
            }

            catch { }
            Mouse.OverrideCursor = Cursors.Arrow;
        }
    }
}
