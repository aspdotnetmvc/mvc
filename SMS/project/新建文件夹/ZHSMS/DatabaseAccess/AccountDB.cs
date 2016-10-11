using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using SMS.Model;

namespace DatabaseAccess
{
    public class AccountDB
    {
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool CreateAccount(Account account)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into Account(");
            strSql.Append("AccountID,SMSNumber)");
            strSql.Append(" values (");
            strSql.Append("@AccountID,@SMSNumber)");

            DBHelper.Instance.Execute(strSql.ToString(), account);
            return true;
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        public static bool DelAccount(string accountID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from Account ");
            strSql.Append(" where AccountID=@AccountID ");
          
            DBHelper.Instance.Execute(strSql.ToString(), new { AccountID = accountID });
            return true;

        }
        /// <summary>
        /// 账号充值
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static bool AccountPrepaid(string accountID, int quantity)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Account set ");
            strSql.Append("SMSNumber=SMSNumber+@SMSNumber ");
            strSql.Append(" where AccountID=@AccountID ");
            DBHelper.Instance.Execute(strSql.ToString(), new { AccountID = accountID, SMSNumber = quantity });
            return true;
        }
        /// <summary>
        /// 获取用户短信数
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        public static int GetSMSNumberByAccount(string accountID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select SMSNumber from Account ");
            strSql.Append(" where AccountID=@AccountID ");
            return DBHelper.Instance.ExecuteScalar<int>(strSql.ToString(), new { AccountID = accountID });
        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        public static Account GetAccount(string accountID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from Account ");
            strSql.Append(" where AccountID=@AccountID ");
            return DBHelper.Instance.Query<Account>(strSql.ToString(), new { AccountID = accountID }).FirstOrDefault();

        }
        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        public static List<Account> GetAccounts()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from Account ");

            return DBHelper.Instance.Query<Account>(strSql.ToString());


        }
        /// <summary>
        /// 短信扣费
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static bool DeductAccountSMSCharge(string accountID, int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Account set ");
            strSql.Append("SMSNumber=SMSNumber-@Count ");
            strSql.Append(" where AccountID=@AccountID ");
            DBHelper.Instance.Execute(strSql.ToString(), new { AccountID = accountID, Count = count });
            return true;
        }

        /// <summary>
        /// 短信返费
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static bool ReAccountSMSCharge(string accountID, int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Account set ");
            strSql.Append(" SMSNumber=SMSNumber+@Count ");
            strSql.Append(" where AccountID=@AccountID ");
            DBHelper.Instance.Execute(strSql.ToString(), new { AccountID = accountID, Count = count });
            return true;
        }
    }
}