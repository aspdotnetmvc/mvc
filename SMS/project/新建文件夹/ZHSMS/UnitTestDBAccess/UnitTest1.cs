using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestDBAccess
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            int p = 67;

          

            int r = getCeiling(71, p);
            Assert.AreEqual(r, 2);
              r = getCeiling(134, p);
            Assert.AreEqual(r, 2);
            r = getCeiling(135, p);
            Assert.AreEqual(r,3);
            r = getCeiling(132, p);
            Assert.AreEqual(r, 2);
        }

        public int getCeiling(int l, int p)
        {
            return Convert.ToInt32(Math.Ceiling(l * 1.0 / p));
        }
    }
}
