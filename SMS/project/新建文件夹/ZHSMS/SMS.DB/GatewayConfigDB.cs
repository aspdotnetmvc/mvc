using SMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SMS.DB
{
    public class GatewayConfigDB
    {
        /// <summary>
        /// 添加网关配置信息
        /// </summary>
        /// <param name="gatewayConfig"></param>
        /// <returns></returns>
        public static bool Exist(string gateway)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from GatewayConfig ");
            strSql.Append(" where Gateway=@Gateway");
            return DBHelper.Instance.ExecuteScalar<int>(strSql.ToString(), new { Gateway = gateway }) > 0;
        }

        /// <summary>
        /// 添加网关配置信息
        /// </summary>
        /// <param name="gatewayConfig"></param>
        /// <returns></returns>
        public static bool Add(GatewayConfiguration gatewayConfig)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into GatewayConfig(");
            strSql.Append("Gateway,Operators,HandlingAbility,MaxPackageSize)");
            strSql.Append(" values (");
            strSql.Append("@Gateway,@Operators,@HandlingAbility,@MaxPackageSize)");
            DBHelper.Instance.Execute(strSql.ToString(),
                new
                {
                    Gateway = gatewayConfig.Gateway,
                    Operators = string.Join(";", gatewayConfig.Operators),
                    HandlingAbility = gatewayConfig.HandlingAbility,
                    MaxPackageSize = gatewayConfig.MaxPackageSize
                });
            return true;
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public static bool Update(GatewayConfiguration gatewayConfig)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update GatewayConfig set ");
            strSql.Append("Operators=@Operators,");
            strSql.Append("MaxPackageSize=@MaxPackageSize,");
            strSql.Append("HandlingAbility=@HandlingAbility");
            strSql.Append(" where Gateway=@Gateway");
            DBHelper.Instance.Execute(strSql.ToString(),
                    new
                    {
                        Gateway = gatewayConfig.Gateway,
                        Operators = string.Join(";", gatewayConfig.Operators),
                        HandlingAbility = gatewayConfig.HandlingAbility,
                        MaxPackageSize= gatewayConfig.MaxPackageSize
                    });
            return true;
        }
        /// <summary>
        /// 删除网关
        /// </summary>
        /// <param name="gateway"></param>
        /// <returns></returns>
        public static bool Del(string gateway)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from GatewayConfig ");
            strSql.Append(" where Gateway=@Gateway");
            DBHelper.Instance.Execute(strSql.ToString(),
                 new
                 {
                     Gateway = gateway
                 });
            return true;
        }

        /// <summary>
        /// 获取指定网关配置
        /// </summary>
        /// <param name="gateway"></param>
        /// <returns></returns>
        public static GatewayConfiguration GetConfig(string gateway)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from GatewayConfig ");
            strSql.Append(" where Gateway=@Gateway");
            var r = DBHelper.Instance.Query(strSql.ToString(), new { Gateway = gateway }).FirstOrDefault();

            return ToGatewayConfiguration((object)r);
        }
        /// <summary>
        /// 获取所有网关配置
        /// </summary>
        /// <returns></returns>
        public static List<GatewayConfiguration> GetConfigs()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from GatewayConfig ");
            var r = DBHelper.Instance.Query(strSql.ToString());
            return (from gc in r select ToGatewayConfiguration((object)gc)).ToList();
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        static GatewayConfiguration ToGatewayConfiguration(dynamic row)
        {
            if (row == null) return null;

            return new GatewayConfiguration()
            {
                Gateway = ((string)row.Gateway),
                Operators = ((string)(row.Operators)).Split(';').ToList(),
                HandlingAbility = (int)(row.HandlingAbility),
                MaxPackageSize = (int)(row.MaxPackageSize)
            };
        }
    }
}
