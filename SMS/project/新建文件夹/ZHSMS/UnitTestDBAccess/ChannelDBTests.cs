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
    public class ChannelDBTests
    {
        [TestMethod()]
        public void Test()
        {
            string gatewayName = System.Guid.NewGuid().ToString();

            var b = GatewayConfigDB.Exist(gatewayName);

            Assert.IsFalse(b);
            GatewayConfiguration gateway = new GatewayConfiguration()
            {
                Gateway = "testGateway11",
                HandlingAbility = 100,
                Operators = new List<string>() { "telecom", "mobile" }

            };
            b = GatewayConfigDB.Add(gateway);
            Assert.IsTrue(b);
            gateway.HandlingAbility = 1000;
            b = GatewayConfigDB.Update(gateway);
            Assert.IsTrue(b);
            b = GatewayConfigDB.Exist(gateway.Gateway);
            Assert.IsTrue(b);
            var g = GatewayConfigDB.GetConfig(gateway.Gateway);
            Assert.IsNotNull(g);
            Assert.AreEqual(g.HandlingAbility, 1000);

            Channel channel = new Channel()
            {
                ChannelID = "channel",
                ChannelName = "单元测试",
                Remark = "单元测试"
            };
            b = ChannelDB.AddChannle(channel);
            Assert.IsTrue(b);
            b = ChannelDB.AddChannelGatewayBind(channel.ChannelID, new List<string>() { gateway.Gateway });
            Assert.IsTrue(b);
            b = ChannelDB.DelGatewayByChannel(channel.ChannelID);
            Assert.IsTrue(b);
            b = ChannelDB.AddChannelGatewayBind(channel.ChannelID, new List<string>() { gateway.Gateway });
            Assert.IsTrue(b);
            b = ChannelDB.DelGatewayByGateway(gateway.Gateway);
            Assert.IsTrue(b);

            channel.Remark = "测试更新";
            b = ChannelDB.UpdateChannel(channel);
            Assert.IsTrue(b);
            var c = ChannelDB.GetChannel(channel.ChannelID);
            Assert.IsNotNull(c);
            Assert.AreEqual(c.ChannelID, channel.ChannelID);
            b = ChannelDB.AddChannelGatewayBind(channel.ChannelID, new List<string>() { gateway.Gateway });
            Assert.IsTrue(b);
            var gs = ChannelDB.GetGatewaysByChannel(channel.ChannelID);
            Assert.IsTrue(gs.Count > 0);
            var cs = ChannelDB.GetChannels();
            Assert.IsTrue(cs.Count > 0);

            b = ChannelDB.DelGatewayByChannel(channel.ChannelID);
            Assert.IsTrue(b);
            b = ChannelDB.DelChannel(channel.ChannelID);
            Assert.IsTrue(b);

            b = GatewayConfigDB.Del(gateway.Gateway);

            Assert.IsTrue(b);
        }
    }
}
