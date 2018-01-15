using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClientCenter.Logs;
using System;

namespace ClientCenter.Tests
{
    [TestClass()]
    public class LogsTests
    {
        [TestMethod()]
        public void TestCMLogParsing()
        {
            string cmLogLine = "<![LOG[BEGIN ExecuteSystemTasks('PowerChanged')]LOG]!><time=\"07:57:18.385+00\" date=\"01-02-2018\" component=\"CcmExec\" context=\"\" type=\"1\" thread=\"14600\" file=\"systemtask.cpp:581\">";
            LogEntry cmParsingResult = LogEntry.ParseLogLine(cmLogLine);
            Assert.AreEqual("BEGIN ExecuteSystemTasks('PowerChanged')", cmParsingResult.LogText);
            Assert.AreEqual("CcmExec", cmParsingResult.Component);
            Assert.AreEqual(new DateTime(2018, 01, 02, 07, 57, 18, 385), cmParsingResult.Date);
        }

        [TestMethod()]
        public void TestWULogParsing()
        {
            Assert.Inconclusive();
        }
    }
}
