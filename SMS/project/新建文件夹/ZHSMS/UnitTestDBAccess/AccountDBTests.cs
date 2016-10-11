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
    public class AccountDBTests
    {

        public Guid guid
        {
            get
            {
                return System.Guid.NewGuid();
            }
        }

        [TestMethod()]
        public void AccountTest()
        {
            Account account = new Account()
            {
                AccountID = guid.ToString(),
                SMSNumber = 10
            };
            var b = AccountDB.CreateAccount(account);
            Assert.IsTrue(b);
            b = AccountDB.AccountPrepaid(account.AccountID, 10);
            Assert.IsTrue(b);
            int c = AccountDB.GetSMSNumberByAccount(account.AccountID);
            Assert.AreEqual(20, c);
            var a = AccountDB.GetAccount(account.AccountID);
            Assert.IsNotNull(a);
            Assert.AreEqual(a.AccountID, account.AccountID);
            Assert.AreEqual(a.SMSNumber, 20);

            b = AccountDB.DeductAccountSMSCharge(account.AccountID, 10);
            Assert.IsTrue(b);
            c = AccountDB.GetSMSNumberByAccount(account.AccountID);
            Assert.AreEqual(10, c);
            b = AccountDB.ReAccountSMSCharge(account.AccountID, 10);
            Assert.IsTrue(b);

            c = AccountDB.GetSMSNumberByAccount(account.AccountID);
            Assert.AreEqual(20, c);

            var accs = AccountDB.GetAccounts();

            Assert.IsNotNull(accs);
            Assert.IsTrue(accs.Count > 0);
            b = AccountDB.DelAccount(account.AccountID);
            Assert.IsTrue(b);
        }
    }
}
