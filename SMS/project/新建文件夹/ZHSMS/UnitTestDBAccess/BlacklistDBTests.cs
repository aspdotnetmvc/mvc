using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace DatabaseAccess.Tests
{
    [TestClass()]
    public class BlacklistDBTests
    {
        [TestMethod()]
        public void Test()
        {
            List<string> black = new List<string>() { "123456", "234567", "345678" };
            var b = BlacklistDB.Add(black);
            Assert.IsTrue(b);
            var list = BlacklistDB.GetNumbers();
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Contains("123456"));
            Assert.IsTrue(list.Contains("234567"));
            Assert.IsTrue(list.Contains("345678"));
            b = BlacklistDB.Del(black);
            list = BlacklistDB.GetNumbers();
            Assert.IsNotNull(list);
            Assert.IsTrue(!list.Contains("123456"));
            Assert.IsTrue(!list.Contains("234567"));
            Assert.IsTrue(!list.Contains("345678"));

        }
    }
}
