using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMSModel;
using System.Security.Cryptography;
namespace DatabaseAccess.Tests
{
    [TestClass()]
    public class AuditFailureDBTests
    {
        public Guid guid
        {
            get
            {
                return System.Guid.NewGuid();
            }
        }

        [TestMethod()]
        public void Test()
        {
            var account = AccountDB.GetAccounts().First();
            Assert.IsNotNull(account);
            SMS sms = new SMS()
            {
                Account = account.AccountID,
                Audit = AuditType.Auto,
                AuditTime = DateTime.Now.ToString(),
                AuditUser = "test",
                Channel = "default",
                Content = "ceshi neirong",
                Level = LevelType.Level0,
                IsSplit = true,
                SendTime = DateTime.Now,
                SerialNumber = guid,
                Signature = "【1111】",
                LinkID = guid.ToString(),
                SPNumber = "1234",
                StatusReport = StatusReportType.Enabled,
                Extend = null,
                FailureCase = "",
                Filter = FilterType.Failure,
                Number = new List<string> { "123", "1235", "234" },
                NumberCount = 3,
                WapURL = "wapurl"
            };
            var b = AuditFailureDB.Add(sms, "失败原因");
            Assert.IsTrue(b);
            var fl = AuditFailureDB.GetFailureSMSByAccount(account.AccountID);
            Assert.IsNotNull(fl);
            Assert.IsTrue(fl.Count > 0);
            var sms_1 = fl.First(s => s.SerialNumber == sms.SerialNumber);
            Assert.AreEqual(sms.WapURL, sms_1.WapURL);
            Assert.AreEqual(sms.SMSTimer.ToString(), sms_1.SMSTimer.ToString());
            var fl2 = AuditFailureDB.GetFailureSMSByAccount(DateTime.MinValue, DateTime.Now);
            Assert.IsNotNull(fl2);
            Assert.IsTrue(fl2.Count > 0);
        
            b = AuditFailureDB.Del(sms.SerialNumber);
            Assert.IsTrue(b);
            fl = AuditFailureDB.GetFailureSMSByAccount(account.AccountID);
            Assert.IsNull(fl.FirstOrDefault(s => s.SerialNumber == sms.SerialNumber));
        }
        [TestMethod]
        public void TestMD5()
        {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            string expect = "cbff36039c3d0212b3e34c23dcde1456";
            string res = BitConverter.ToString(hashmd5.ComputeHash(Encoding.UTF8.GetBytes("123.com"))).Replace("-", "").ToLower();  

            Assert.AreEqual(expect, res);
        }
    }
}
