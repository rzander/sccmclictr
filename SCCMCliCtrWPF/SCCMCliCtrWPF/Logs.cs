using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace ClientCenter.Logs
{
    public class LogEntry
    {
        public string LogText { get; set; }
        public string Component { get; set; }
        public DateTime Date { get; set; }

        public static LogEntry ParseLogLine(string sOrg)
        {
            DateTime LastDT = new DateTime();

            //Check if Line has at least 5 Tabs (e.g. WindowsUpdate.log)
            if (sOrg.Count(f => f == '\t') > 4)
            {
                try
                {
                    string sText = sOrg.Split('\t')[5];
                    string sComp = sOrg.Split('\t')[4];
                    string sDate = sOrg.Split('\t')[0];
                    string sTime = sOrg.Split('\t')[1];

                    DateTime logdate = DateTime.ParseExact(sDate + " " + sTime, "yyyy-MM-dd HH:mm:ss:fff", CultureInfo.InvariantCulture);
                    LastDT = logdate;
                    return new LogEntry() { LogText = sText, Component = sComp, Date = logdate };
                }
                catch { }
            }

            //Check for SCCM Log format
            if (sOrg.StartsWith("<![LOG["))
            {
                try
                {
                    string sText = sOrg.Substring(7, sOrg.IndexOf("]LOG]!>") - 7);
                    string sTemp = sOrg.Substring(sOrg.IndexOf("LOG]!>") + 7);

                    List<string> parts = Regex.Matches(sTemp, @"[\""].+?[\""]|[^ ]+").Cast<Match>().Select(m => m.Value).ToList();

                    string sComp = parts.First(p => p.StartsWith("component")).Split('=')[1].Replace("\"", "");
                    string sDate = parts.First(p => p.StartsWith("date")).Split('=')[1].Replace("\"", ""); ;
                    string sTime = parts.First(p => p.StartsWith("time")).Split('=')[1].Replace("\"", "").Split('-')[0];

                    //Drop the UTC offset CM provides in minutes from the end of the time 
                    DateTime logdate = DateTime.ParseExact(sDate + " " + sTime.Substring(0, 12), "MM-dd-yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);

                    LastDT = logdate;
                    return new LogEntry() { LogText = sText, Component = sComp, Date = logdate };
                }
                catch { }
            }

            if (!string.IsNullOrEmpty(sOrg))
            {
                return new LogEntry() { LogText = sOrg, Component = "", Date = LastDT };
            }
            else
            {
                return new LogEntry();
            }
        }
    }
}
