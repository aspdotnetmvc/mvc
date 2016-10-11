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
    public class SMSDBTests
    {
        [TestMethod()]
        public void SMSDBTest()
        {
            var accountId = "account";
            SMS sms = new SMS()
            {
                Account = accountId,
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

            var b = SMSDB.AddSms(accountId, sms, 0);
            Assert.IsTrue(b);
         
            var l = SMSDB.GetSMSByAudit(DateTime.Now.AddDays(-5), DateTime.Now.AddDays(5));
            Assert.IsNotNull(l);
            Assert.IsTrue(l.Count > 0);
            l = SMSDB.GetSMSByAudit(accountId, DateTime.Now.AddDays(-5), DateTime.Now.AddDays(5));
            Assert.IsNotNull(l);
            Assert.IsTrue(l.Count > 0);
            b = SMSDB.AuditSMS(sms.SerialNumber, true);
            Assert.IsTrue(b);
            var c = SMSDB.CountSMS(accountId);
            Assert.IsTrue(c > 0);

            var gsms = SMSDB.GetSMS(sms.SerialNumber);
            Assert.IsNotNull(gsms);
            Assert.IsTrue(sms.SerialNumber == gsms.SerialNumber);

            b = SMSDB.SetSMSLevel(sms.SerialNumber, (LevelType)5);
            Assert.IsTrue(b);
            gsms = SMSDB.GetSMS(sms.SerialNumber);
            Assert.IsTrue(gsms.Level == (LevelType)5);

            b = SMSDB.SetSMSLevel(sms.SerialNumber, (LevelType)12);
            Assert.IsTrue(b);
            gsms = SMSDB.GetSMS(sms.SerialNumber);



            SMS sms2 = new SMS()
            {
                Account = accountId,
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
            SMS sms3 = new SMS()
            {
                Account = accountId,
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
                Number = new List<string> { "123" },

                NumberCount = 1,
                WapURL = "wapurl"
            };
            b = SMSDB.AddSms(accountId, sms2, 1);
            Assert.IsTrue(b);
            b = SMSDB.AddSms(accountId, sms3, 1);
            Assert.IsTrue(b);

          

            b = SMSDB.DelSMSs(new List<Guid>() { sms2.SerialNumber, sms3.SerialNumber });
            Assert.IsTrue(b);
            b = SMSDB.AddSms(accountId, sms, 1);
            Assert.IsTrue(b);
            b = SMSDB.AddSms(accountId, sms2, 1);
            Assert.IsTrue(b);
            b = SMSDB.AddSms(accountId, sms3, 1);
            Assert.IsTrue(b);
            l = SMSDB.GetSendSMSByOne(1000, "default");
            Assert.IsTrue(l.Count >= 1);
            l = SMSDB.GetSendSMSByLessHundred(1000, "default");
            Assert.IsTrue(l.Count >= 1);

            b = SMSDB.DelSMSs(new List<Guid>() { sms.SerialNumber, sms2.SerialNumber, sms3.SerialNumber });
            Assert.IsTrue(b);

            sms.SMSTimer = DateTime.Now;
            sms2.SMSTimer = DateTime.Now;
            sms3.SMSTimer = DateTime.Now;
            b = SMSDB.AddSms(accountId, sms, 1);
            Assert.IsTrue(b);
            b = SMSDB.AddSms(accountId, sms2, 1);
            Assert.IsTrue(b);
            b = SMSDB.AddSms(accountId, sms3, 1);
            Assert.IsTrue(b);

            l = SMSDB.GetSendSMSBySMSTimer(DateTime.Now.AddMinutes(1), "default");

            Assert.IsTrue(l.Count >= 3);
            b = SMSDB.DelSMSs(new List<Guid>() { sms.SerialNumber, sms2.SerialNumber, sms3.SerialNumber });
            Assert.IsTrue(b);
        }
    }
}
