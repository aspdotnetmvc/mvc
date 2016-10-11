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
    public class KeywordsGatewayBindDBTests
    {
        [TestMethod()]
        public void Test()
        {
            var keyGroups = WordfilteDB.GetKeyGroups();
            Assert.IsNotNull(keyGroups);
            Assert.IsTrue(keyGroups.Count > 0);

            var keyTypes = WordfilteDB.GetKeyTypes();
            Assert.IsNotNull(keyTypes);
            Assert.IsTrue(keyTypes.Count > 0);

            var keyGroup = keyGroups.First().Key;

            var b = WordfilteDB.ExistGroup(keyGroup);
            Assert.IsTrue(b);
            var keyType = keyTypes.First().Key;
            b = WordfilteDB.ExistType(keyType);
            Assert.IsTrue(b);

            var addKeyGroup = "AddKeyGroup" + UnitTestDBAccess.Util.GenRandStr(10);
            b = WordfilteDB.AddKeyGroup(addKeyGroup, "添加keyGroup 测试");
            Assert.IsTrue(b);
            b = WordfilteDB.DelKeyGroup(addKeyGroup);
            Assert.IsTrue(b);

            var addKeyType = "AddKeyType" + UnitTestDBAccess.Util.GenRandStr(10);
            b = WordfilteDB.AddKeyType(addKeyType, "添加keyType 测试");
            Assert.IsTrue(b);

            b = WordfilteDB.AddGroupTypeBind(addKeyGroup, new List<string>() { addKeyType, UnitTestDBAccess.Util.GenRandStr(15) });
            Assert.IsTrue(b);

            var kgt = WordfilteDB.GetKeyTypesByGroup(addKeyGroup);
            Assert.IsNotNull(kgt);
            Assert.IsTrue(kgt.Count > 0);
            var ktg = WordfilteDB.GetKeywordsGroupByType(addKeyType);
            Assert.IsNotNull(ktg);
            Assert.IsTrue(ktg.Count > 0);

            b = WordfilteDB.DelKeyTypesByGroup(addKeyGroup);
            Assert.IsTrue(b);

            Keywords keywords1 = new Keywords()
            {
                KeyGroup=addKeyGroup,
                KeywordsType=addKeyType,
                Enable=true,
                ReplaceKeywords="",
                Words = "words" + UnitTestDBAccess.Util.GenRandStr(15)
            };
            Keywords keywords2 = new Keywords()
            {
                KeyGroup = addKeyGroup,
                KeywordsType = addKeyType,
                Enable = true,
                ReplaceKeywords = "",
                Words = "words" + UnitTestDBAccess.Util.GenRandStr(15)
            };
            Keywords keywords3 = new Keywords()
            {
                KeyGroup = addKeyGroup,
                KeywordsType = addKeyType,
                Enable = true,
                ReplaceKeywords = "",
                Words = "words"+UnitTestDBAccess.Util.GenRandStr(15)
            };
            List<Keywords> list = new List<Keywords>();
            list.Add(keywords1);
            list.Add(keywords2);
            list.Add(keywords3);
            b = WordfilteDB.Add(addKeyGroup, list);
            Assert.IsTrue(b);
            var list2 = WordfilteDB.Gets(addKeyGroup);
            Assert.IsNotNull(list2);
            Assert.IsTrue(list2.Count > 2);
            var list3 = WordfilteDB.GetKeywordsByType(addKeyType);
            Assert.IsNotNull(list3);
            Assert.IsTrue(list3.Count > 2);

            b = WordfilteDB.KeywordsEnabled(addKeyGroup, keywords1.Words, false);
            Assert.IsTrue(b);
            keywords1.ReplaceKeywords = "replace";
            b = WordfilteDB.Update(keywords1);
            Assert.IsTrue(b);
            var i = WordfilteDB.GetCountKeywords(addKeyGroup);
            Assert.IsTrue(i > 0);
            var arr = WordfilteDB.Get(addKeyGroup);
            Assert.IsNotNull(arr);
            Assert.IsTrue(arr.Length > 0);

            var klist = WordfilteDB.GetKeywordsByKeyword("words");
            Assert.IsNotNull(klist);
            Assert.IsTrue(klist.Count > 0);

            i = WordfilteDB.GetAllKeywordCount();
            Assert.IsTrue(i > 0);

            var list4 = WordfilteDB.GetAllKeywords(1, 2);
            Assert.IsTrue(list4.Count==2);

            var gateway = GatewayConfigDB.GetConfigs().FirstOrDefault();

            Assert.IsNotNull(gateway);

            b = KeywordsGatewayBindDB.Add(addKeyGroup, gateway.Gateway);
            Assert.IsTrue(b);

            var gts = KeywordsGatewayBindDB.GetGateways(addKeyGroup);
            Assert.IsTrue(gts.Count > 0);

            var kg = KeywordsGatewayBindDB.GetkeyGroup(gateway.Gateway);
            Assert.IsNotNull(kg);


            gts = KeywordsGatewayBindDB.GetGateways();
            Assert.IsTrue(gts.Count > 0);

            b = KeywordsGatewayBindDB.Del(gateway.Gateway);
            Assert.IsTrue(b);


            List<string> keywords = (from k in list select k.Words).ToList();
            b = WordfilteDB.Del(addKeyGroup, keywords);
            Assert.IsTrue(b);

        }
    }
}
