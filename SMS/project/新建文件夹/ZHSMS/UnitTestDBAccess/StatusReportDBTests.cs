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
    public class StatusReportDBTests
    {
        [TestMethod()]
        public void AddStatusReportTest()
        {
            var accountId = "account";
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

            StatusReport report = new StatusReport();
            report.StatusCode = 3100;
            report.Describe = "发送成功";
            report.Gateway = "CMPP";
            report.Message = sms;
            report.Serial = sms.SerialNumber.ToString();
            report.SplitNumber = 1;
            report.SplitTotal = 1;
            report.Succeed = true;

            var b = StatusReportDB.AddStatusReport(accountId, report);
            Assert.IsTrue(b);
            report.Succeed = false;
            b = StatusReportDB.Update(report);
            Assert.IsTrue(b);

            b = StatusReportDB.CloneTable("statusReport_" + UnitTestDBAccess.Util.GenRandStr(5));
            Assert.IsTrue(b);
            var tables = StatusReportDB.GetTables();
            Assert.IsTrue(tables.Count > 0);

            var sr = StatusReportDB.GetStatusReport(sms.SerialNumber, DateTime.Now);

            Assert.IsTrue(sr.Count > 0);

        }
    }
}
