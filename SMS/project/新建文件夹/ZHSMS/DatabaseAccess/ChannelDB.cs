using MySql.Data.MySqlClient;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DatabaseAccess
{
    public class ChannelDB
    {

        /// <summary>
        /// 添加通道信息
        /// </summary>
        public static bool AddChannle(Channel channel)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into Channel(");
            strSql.Append("ChannelID,ChannelName,Remark,SMSType)");
            strSql.Append(" values (");
            strSql.Append("@ChannelID,@ChannelName,@Remark,@SMSType)");
            DBHelper.Instance.Execute(strSql.ToString(), channel);
            return true;
        }

        /// <summary>
        /// 添加通道绑定网关信息
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="gateway"></param>
        /// <returns></returns>
        public static bool AddChannelGatewayBind(string channel, List<string> gateways)
        {
            if (gateways == null) return false;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into ChannelGatewayBind(");
            strSql.Append("ChannelID,Gateway)");
            strSql.Append(" values (");
            strSql.Append("@ChannelID,@Gateway)");
            var bind = from g in gateways select new { ChannelID = channel, Gateway = g };
            DBHelper.Instance.Execute(strSql.ToString(), bind);
            return true;
        }
        /// <summary>
        /// 根据通道获取绑定的网关
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static List<string> GetGatewaysByChannel(string channel)
        {
            List<string> list = new List<string>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select Gateway from ChannelGatewayBind ");
            strSql.Append(" where ChannelID=@ChannelID");
            return DBHelper.Instance.Query<string>(strSql.ToString(), new { ChannelID = channel });

        }
        /// <summary>
        /// 根据通道删除绑定的网关
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static bool DelGatewayByChannel(string channel)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from ChannelGatewayBind ");
            strSql.Append(" where ChannelID=@ChannelID");
            DBHelper.Instance.Execute(strSql.ToString(), new { ChannelID = channel });
            return true;
        }
        /// <summary>
        /// 根据网关删除通道绑定的网关
        /// </summary>
        /// <param name="gateway"></param>
        /// <returns></returns>
        public static bool DelGatewayByGateway(string gateway)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from ChannelGatewayBind ");
            strSql.Append(" where Gateway=@Gateway");
            DBHelper.Instance.Execute(strSql.ToString(), new { Gateway = gateway });

            return true;
        }

        /// <summary>
        /// 获取通道信息
        /// </summary>
        /// <param name="channel">通道编号</param>
        /// <returns></returns>
        public static Channel GetChannel(string channel)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM Channel");
            strSql.Append(" where ChannelID=@ChannelID");
            return DBHelper.Instance.Query<Channel>(strSql.ToString(), new { ChannelID = channel }).FirstOrDefault();
        }
        /// <summary>
        /// 获取通道信息
        /// </summary>
        public static List<Channel> GetChannels()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM Channel");
            return DBHelper.Instance.Query<Channel>(strSql.ToString());
        }
        /// <summary>
        /// 更新通道
        /// </summary>
        public static bool UpdateChannel(Channel channel)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Channel set ");
            strSql.Append("ChannelName=@ChannelName,"); 
            strSql.Append("SMSType=@SMSType,");
            strSql.Append("Remark=@Remark");
            strSql.Append(" where ChannelID=@ChannelID");

            DBHelper.Instance.Execute(strSql.ToString(), channel);

            return true;
        }
        /// <summary>
        /// 删除通道
        /// </summary>
        /// <param name="channel">通道编号</param>
        /// <returns></returns>
        public static bool DelChannel(string channel)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from Channel ");
            strSql.Append(" where ChannelID=@ChannelID");
            DBHelper.Instance.Execute(strSql.ToString(), new { ChannelID = channel });
			
            return true;
        }


    }
}
