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
    public class AuditRecordDBTests
    {
        [TestMethod]
        public void Test()
        {
            var account = AccountDB.GetAccounts().First();
            Assert.IsNotNull(account);
            AuditRecord record = new AuditRecord()
            {
                AccountID = account.AccountID,
                AuditTime = DateTime.Now,
                Content = "123456",
                SerialNumber = System.Guid.NewGuid(),
                SendTime = DateTime.Now,
                Result = false
            };
            AuditRecord record2 = new AuditRecord()
            {
                AccountID = account.AccountID,
                AuditTime = DateTime.Now,
                Content = "123456",
                SerialNumber = System.Guid.NewGuid(),
                SendTime = DateTime.Now,
                Result = true
            };

            var b = AuditRecordDB.Add(record);
            Assert.IsTrue(b);
            b = AuditRecordDB.Add(record2);
            Assert.IsTrue(b);
            var r = AuditRecordDB.GetAudit(record2.SerialNumber);
            Assert.IsNotNull(record2);
            Assert.AreEqual(record2.Result, r.Result);
            Assert.AreEqual(record2.SendTime.ToString(), r.SendTime.ToString());
            var list = AuditRecordDB.GetAudit(DateTime.MinValue, DateTime.MaxValue);
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count > 1);
        }
      
    }
}
