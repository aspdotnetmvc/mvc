using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace MessageTools.Tests
{
    [TestClass()]
    public class MailHelperTests
    {
        [TestMethod()]
        public void SendMailTest()
        {
            try
            {
                new MailHelper().SendMail("测试", "测试发送");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false);
            }
           //System.Threading.Thread.Sleep(10000);
            Assert.IsTrue(true);
        }
    }
}
