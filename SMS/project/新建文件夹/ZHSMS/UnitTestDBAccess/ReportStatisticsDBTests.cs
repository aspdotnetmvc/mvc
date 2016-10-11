using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMSModel;
namespace DatabaseAccess.Tests
{
    [TestClass()]
    public class ReportStatisticsDBTests
    {
        public SMS GetSMS()
        {
            SMS sms = new SMS()
            {
                Account = "account",
                Audit = AuditType.Auto,
                AuditTime = DateTime.Now.ToString(),
                AuditUser = "test",
                Channel = "default",
                Content = "ceshi neirong",
                Level = LevelType.Level0,
                IsSplit = true,
                SendTime = DateTime.Now,
                SerialNumber = System.Guid.NewGuid(),
                Signature = "【1111】",
                LinkID = System.Guid.NewGuid().ToString(),
                SPNumber = "1234",
                StatusReport = StatusReportType.Enabled,
                Extend = null,
                FailureCase = "",
                Filter = FilterType.Failure,
                Number = new List<string> { "123", "1235", "234" },
                NumberCount = 3,
                WapURL = "wapurl"
            };
            return sms;
        }
        [TestMethod()]
        public void AddSMSHistoryTest()
        {
            SMS sms = GetSMS();
            var b = ReportStatisticsDB.AddSMSHistory(sms);
            Assert.IsTrue(b);
        }

        [TestMethod()]
        public void GetSMSRecordByAccountTest()
        {
            var r = ReportStatisticsDB.GetSMSRecordByAccount("account", new DateTime(2016, 4, 28), DateTime.Now);
            Assert.IsTrue(r.Count > 0);
        }

        [TestMethod()]
        public void GetStatisticsByAccountTest()
        {
            var r = ReportStatisticsDB.GetStatisticsByAccount("account", new DateTime(2016, 4, 28), DateTime.Now);
            Assert.IsTrue(r.Count > 0);
        }

        [TestMethod()]
        public void GetStatisticsByDateTest()
        {
            var r = ReportStatisticsDB.GetStatisticsByDate(new DateTime(2016, 4, 28), DateTime.Now);
            Assert.IsTrue(r.Count > 0);
        }

        [TestMethod()]
        public void GetSMSStatisticsTest()
        {
            var r = ReportStatisticsDB.GetSMSStatistics(new DateTime(2016, 4, 28), DateTime.Now);
            Assert.IsTrue(r.Length > 0);
        }

        [TestMethod()]
        public void GetSMSStatisticsByAccountTest()
        {
            var r = ReportStatisticsDB.GetSMSStatisticsByAccount("account", new DateTime(2016, 4, 28), DateTime.Now);
            Assert.IsTrue(r.Length > 0);
        }

        [TestMethod()]
        public void GetReportStatisticsTest()
        {
            SMS sms = GetSMS();
            var b = ReportStatisticsDB.AddSMSHistory(sms);
            Assert.IsTrue(b);
            var r = ReportStatisticsDB.GetReportStatistics(sms.SerialNumber, sms.SendTime);
            Assert.IsNotNull(r);
        }

        [TestMethod()]
        public void UpdateTest()
        {
            SMS sms = GetSMS();
            var b = ReportStatisticsDB.AddSMSHistory(sms);
            Assert.IsTrue(b);
            var r = ReportStatisticsDB.GetReportStatistics(sms.SerialNumber, sms.SendTime);
            Assert.IsNotNull(r);
            ReportStatistics rs = new ReportStatistics()
            {
                SerialNumber = sms.SerialNumber,
                SendCount = 100,
                SendTime = sms.SendTime
            };
            b = ReportStatisticsDB.Update("account", rs);
            Assert.IsTrue(b);

        }

        [TestMethod()]
        public void GetTablesTest()
        {
            var l = ReportStatisticsDB.GetTables();
            Assert.IsTrue(l.Count > 0);
        }
    }
}
