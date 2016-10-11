using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject2
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string Content = "【123】1234";
            string Signature = "";
            string newContent = "";
            var b = SMS.Util.SMSHelper.GetSignature(Content, ref Signature,ref newContent);
            Assert.IsTrue(b);
            Assert.AreEqual("【123】", Signature);
            Assert.AreEqual("1234", newContent);

            Content = "aaaa【】";
            b = SMS.Util.SMSHelper.GetSignature(Content, ref Signature, ref newContent);
            Assert.IsFalse(b);

            Content = "【】aaaa";
            b = SMS.Util.SMSHelper.GetSignature(Content, ref Signature, ref newContent);
            Assert.IsFalse(b);

            Content = "aaaa【abc】";
            b = SMS.Util.SMSHelper.GetSignature(Content, ref Signature, ref newContent);
            Assert.IsTrue(b);
            Assert.AreEqual("【abc】", Signature);
            Assert.AreEqual("aaaa", newContent);

            Content = "【aaa】【ssss】aabc";
            b = SMS.Util.SMSHelper.GetSignature(Content, ref Signature, ref newContent);
            Assert.IsTrue(b);
            Assert.AreEqual("【aaa】", Signature);
            Assert.AreEqual("【ssss】aabc", newContent);

            Content = "aaa【ssss】a【abc】";
            b = SMS.Util.SMSHelper.GetSignature(Content, ref Signature, ref newContent);
            Assert.IsTrue(b);
            Assert.AreEqual("【abc】", Signature);
            Assert.AreEqual("aaa【ssss】a", newContent);

            Content = "aaa【ssss】a ab";
            b = SMS.Util.SMSHelper.GetSignature(Content, ref Signature, ref newContent);
            Assert.IsFalse(b);
        }
    }
}
