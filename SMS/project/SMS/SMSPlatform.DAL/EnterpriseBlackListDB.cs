using SMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SMSPlatform.DAL
{
    /// <summary>
    /// 企业黑名单
    /// </summary>
    public class EnterpriseBlackListDB
    {
        public static bool AddEnterpriseBlackList(string EnterpriseCode, List<string> Numbers)
        {
            var myTran = DBHelper.Instance.GetDBAdapter().BeginTransaction();
            try
            {
                foreach (var n in Numbers)
                {
                    Add(myTran, EnterpriseCode, n);
                }
                myTran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                myTran.Rollback();
                return false;
            }

        }
        static bool Add(IDbTransaction myTrans, string EnterpriseCode, string number)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into plat_EnterpriseBlackList(");
            strSql.Append("AccountCode,Number,AddTime)");
            strSql.Append(" values (");
            strSql.Append("@AccountCode,@Number,Now())");
            DBHelper.Instance.Execute(strSql.ToString(), new { AccountCode = EnterpriseCode, Number = number }, myTrans);
            return true;
        }

        public static bool DelEnterpriseBlackList(string EnterpriseCode, List<string> Numbers)
        {
            var myTran = DBHelper.Instance.GetDBAdapter().BeginTransaction();
            try
            {
                foreach (var n in Numbers)
                {
                    Del(myTran, EnterpriseCode, n);
                }
                myTran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                myTran.Rollback();
                return false;
            }
        }
        public static bool Del(IDbTransaction myTrans, string EnterpriseCode, string number)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from plat_EnterpriseBlackList where AccountCode=@AccountCode and Number=@Number");
            DBHelper.Instance.Execute(strSql.ToString(), new { AccountCode = EnterpriseCode, Number = number }, myTrans);
            return true;
        }

        /// <summary>
        /// 查询企业所有黑名单
        /// </summary>
        /// <param name="EnterpriseCode"></param>
        /// <returns></returns>
        public static List<string> getEnterpriseBlackList(string EnterpriseCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select Number from plat_EnterpriseBlackList where AccountCode=@AccountCode ");

            return DBHelper.Instance.Query<string>(strSql.ToString(), new { AccountCode = EnterpriseCode });
        }

        /// <summary>
        /// 查询全部数据，用于加载缓存
        /// </summary>
        /// <returns></returns>
        public static QueryResult getAllEnterpriseBlackList()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select AccountCode, Number from plat_EnterpriseBlackList ");
            var r=  DBHelper.Instance.GetResultSet(strSql.ToString(), "AccountCode", new DBTools.ParamList() { ispage=false});
            return DBHelper.Instance.ToQueryResult(r);
        }

    }
}
