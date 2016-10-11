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
    public class LevelModifyRecordDBTests
    {
        [TestMethod()]
        public void Test()
        {
            LevelModifyRecord record = new LevelModifyRecord()
            {
                AccountID= "account",
                Content="content",
                ModifyContent="modify",
                ModifyTime=DateTime.Now,
                SendTime=DateTime.Now,
                SerialNumber=System.Guid.NewGuid()
            };
        }
    }
}
