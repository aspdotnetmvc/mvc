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
    public class PrepaidRecordDBTests
    {
        [TestMethod()]
        public void Test()
        {
            var oaccount = "operaterAccount";
            var prepaidAccount = "prepaidAccount";
            var b = PrepaidRecordDB.Add(oaccount, prepaidAccount, 100);
            Assert.IsTrue(b);
            var list = PrepaidRecordDB.Get(DateTime.MinValue, DateTime.MaxValue);
            Assert.IsTrue(list.Count > 0);
        }

    }
}
