using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace LogService.Tests
{
    [TestClass()]
    public class LogDBTests
    {
        [TestMethod()]
        public void WriteTest()
        {
            LogInterface.LogMessage log = new LogInterface.LogMessage();
            log.Date = DateTime.Now;
            log.Event = "Test";
            log.Ip = "127.0.0.1";
            log.Level = "test";
            log.Message = "测试消息";
            log.Recorder = "testRecorder";
            log.Thread = "";

            var b = LogDB.Write(log);
            Assert.IsTrue(b);
           // LogDB.CreateSubmeterTable("submeter" + UnitTestDBAccess.Util.GenRandStr(5));
            b = LogDB.Write("test", "", "127.0.0.1", "test recorder", "error", "123412312423423", DateTime.Now);
            Assert.IsTrue(b);
            b = LogDB.Write("test", "", "127.0.0.1", "test recorder", "error", "123412312423423", DateTime.Now.ToString());
            Assert.IsTrue(b);

            var l = LogDB.GetLogs("test", "test recorder", "error", DateTime.Now.AddDays(-1).ToString(), DateTime.Now.AddDays(1).ToString());
            Assert.IsTrue(l.Count >= 2);
        }
    }
}
