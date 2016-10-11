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
    public class DeliverMODBTests
    {
        [TestMethod()]
        public void Test()
        {
            string spnumber = "12345";
            var gateway = GatewayConfigDB.GetConfigs().FirstOrDefault();
            Assert.IsNotNull(gateway);
            MOSMS mo = new MOSMS(gateway.Gateway, System.Guid.NewGuid().ToString(),
              new DateTime(2016, 4, 1), "测试短信上行", "123214", spnumber, "test");

            var b = DeliverMODB.Add(mo);
            Assert.IsTrue(b);
            var l = DeliverMODB.Gets(spnumber, new DateTime(2016, 3, 1), new DateTime(2016, 4, 30));
            Assert.IsTrue(l.Count > 0);

            b = DeliverMODB.Del(spnumber);
            Assert.IsTrue(b);

            l = DeliverMODB.Gets(spnumber, new DateTime(2016, 3, 1), new DateTime(2016, 4, 30));
            Assert.IsTrue(l.Count == 0);
        }
    }
}
