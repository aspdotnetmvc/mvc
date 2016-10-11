using MySql.Data.MySqlClient;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DAL
{
    /// <summary>
    /// 企业黑名单
    /// </summary>
    public class EnterpriseBlackList
    {
        public static bool AddEnterpriseBlackList(string EnterpriseCode, List<string> Numbers)
        {
            MySqlTransaction myTran = DBUtility.MySqlHelper.CreateTransaction();
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
        static bool Add(MySqlTransaction myTrans, string EnterpriseCode, string number)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into plat_EnterpriseBlackList(");
            strSql.Append("AccountCode,Number,Remark,AddTime)");
            strSql.Append(" values (");
            strSql.Append("@AccountCode,@Number,@Remark,@AddTime)");
            MySqlParameter[] parameters = { 
                    new MySqlParameter("@AccountCode", MySqlDbType.VarChar,64),
					new MySqlParameter("@Number", MySqlDbType.VarChar,24),
					new MySqlParameter("@Remark", MySqlDbType.VarChar,128),
					new MySqlParameter("@AddTime", MySqlDbType.DateTime)};
            parameters[0].Value = EnterpriseCode;
            parameters[1].Value = number;
            parameters[2].Value = System.DBNull.Value;
            parameters[3].Value = DateTime.Now;
            int rows = 0;
            if (myTrans == null)
            {
                rows = DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            }
            else
            {
                rows = DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            }
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool DelEnterpriseBlackList(string EnterpriseCode, List<string> Numbers)
        {
            MySqlTransaction myTran = DBUtility.MySqlHelper.CreateTransaction();
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
        public static bool Del(MySqlTransaction myTrans, string EnterpriseCode, string number)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from plat_EnterpriseBlackList where AccountCode=@AccountCode and Number=@Number");
            MySqlParameter[] parameters = { 
                    new MySqlParameter("@AccountCode", MySqlDbType.VarChar,64),
					new MySqlParameter("@Number", MySqlDbType.VarChar,24)};
            parameters[0].Value = EnterpriseCode;
            parameters[1].Value = number;
            int rows = 0;
            if (myTrans == null)
            {
                rows = DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            }
            else
            {
                rows = DBUtility.MySqlHelper.ExecuteNonQuery(myTrans,strSql.ToString(), parameters);
            }
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
            MySqlParameter[] parameters = { 
                    new MySqlParameter("@AccountCode", MySqlDbType.VarChar,64)};
            parameters[0].Value = EnterpriseCode;
            var ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            List<string> list = new List<string>();
            if (ds.Tables.Count == 0) return list;
            list = (from r in ds.Tables[0].AsEnumerable() select getString(r, "Number")).ToList();
           
            return list;
        }

       /// <summary>
       /// 查询全部数据，用于加载缓存
       /// </summary>
       /// <returns></returns>
        public static DataTable getAllEnterpriseBlackList()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select AccountCode, Number from plat_EnterpriseBlackList ");
 
            var ds = DBUtility.MySqlHelper.Query(strSql.ToString());
            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else return null;
        }
        public static string getString(DataRow dr, string col)
        {
            var d = dr[col];
            if (d == null) return "";
            return d.ToString();
        }
    }
}
