using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace DatabaseAccess.Tests
{
    [TestClass()]
    public class CacheDataDBTests
    {
        [TestMethod()]
        public void Test()
        {
            var b = CacheDataDB.Add("space", "key", "content");
            Assert.IsTrue(b);

            var dl = CacheDataDB.Get("space");
            Assert.IsNotNull(dl);

            var d = dl.FirstOrDefault();


            b = CacheDataDB.Update("space", "key", "content2");
            dl = CacheDataDB.Get("space");
            Assert.IsNotNull(dl);

            var d1 = dl.FirstOrDefault();
            b = (d1.NContent == "content2");
            Assert.IsTrue(b);

        }
    }
}
