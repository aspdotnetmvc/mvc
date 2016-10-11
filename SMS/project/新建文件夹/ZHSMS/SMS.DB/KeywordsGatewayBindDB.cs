using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SMS.DB
{
    public class KeywordsGatewayBindDB
    {
        /// <summary>
        /// 添加关键词组与网关绑定信息
        /// </summary>
        /// <param name="keywordGroup"></param>
        /// <param name="gateway"></param>
        /// <returns></returns>
        public static bool Add(string keywordGroup, string gateway)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into KeywordsGatewayBind(");
            strSql.Append("KeyGroup,Gateway)");
            strSql.Append(" values (");
            strSql.Append("@KeyGroup,@Gateway)");

            DBHelper.Instance.Execute(strSql.ToString(), new { KeyGroup = keywordGroup, Gateway = gateway });
            return true;
        }
        /// <summary>
        /// 删除网关指定的关键词组绑定信息
        /// </summary>
        /// <param name="gateway"></param>
        /// <returns></returns>
        public static bool Del(string gateway)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("delete from KeywordsGatewayBind ");
                strSql.Append(" where Gateway=@Gateway");
                DBHelper.Instance.Execute(strSql.ToString(), new { Gateway = gateway });
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool DelKeyGroup(string keyGroup)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("delete from KeywordsGatewayBind ");
                strSql.Append(" where KeyGroup=@KeyGroup");
                DBHelper.Instance.Execute(strSql.ToString(), new { KeyGroup = keyGroup });
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 根据网关获取绑定的关键词组名
        /// </summary>
        /// <param name="gateway"></param>
        /// <returns></returns>
        public static string GetkeyGroup(string gateway)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select KeyGroup from KeywordsGatewayBind ");
            strSql.Append(" where Gateway=@Gateway");
            return DBHelper.Instance.ExecuteScalar<string>(strSql.ToString(), new { Gateway = gateway });
        }
        /// <summary>
        /// 根据关键词组获取绑定的网关
        /// </summary>
        /// <param name="keywordGroup"></param>
        /// <returns></returns>
        public static List<string> GetGateways(string keywordGroup)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select Gateway from KeywordsGatewayBind ");
            strSql.Append(" where KeyGroup=@KeyGroup");
            return DBHelper.Instance.Query<string>(strSql.ToString(), new { KeyGroup = keywordGroup });
        }
        /// <summary>
        /// 获取所有已绑定的网关
        /// </summary>
        /// <returns></returns>
        public static List<string> GetGateways()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select Gateway from KeywordsGatewayBind ");

            return DBHelper.Instance.Query<string>(strSql.ToString());

        }
    }
}
