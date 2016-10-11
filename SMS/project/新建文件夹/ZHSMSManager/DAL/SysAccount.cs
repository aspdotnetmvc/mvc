using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DAL
{
    public class SysAccount
    {
        public static bool Exists(string UserCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from plat_Account");
            strSql.Append(" where UserCode=@UserCode ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@UserCode", MySqlDbType.VarChar,16)			};
            parameters[0].Value = UserCode;

            return DBUtility.MySqlHelper.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static bool Add(SMS.Model.SysAccount account)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into plat_Account(");
            strSql.Append("UserCode,UserName,UserPassword,AddTime,UserStatus)");
            strSql.Append(" values (");
            strSql.Append("@UserCode,@UserName,@UserPassword,@AddTime,@UserStatus)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@UserCode", MySqlDbType.VarChar,16),
					new MySqlParameter("@UserName", MySqlDbType.VarChar,32),
					new MySqlParameter("@UserPassword", MySqlDbType.VarChar,64),
					new MySqlParameter("@AddTime", MySqlDbType.DateTime),
					new MySqlParameter("@UserStatus", MySqlDbType.Int16,5)};
            parameters[0].Value = account.UserCode;
            parameters[1].Value = account.UserName;
            parameters[2].Value = account.PassWord;
            parameters[3].Value = account.AddTime;
            parameters[4].Value = account.Status == true ? 1 : 0;

            int rows = DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            if (rows > 0)
            {
                foreach (var v in account.Roles)
                {
                    AddAccountRole(account.UserCode, v);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public static bool Update(SMS.Model.SysAccount account)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update plat_Account set ");
            strSql.Append("UserName=@UserName,");
            strSql.Append("UserPassword=@UserPassword,");
            strSql.Append("AddTime=@AddTime,");
            strSql.Append("UserStatus=@UserStatus");
            strSql.Append(" where UserCode=@UserCode ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@UserName", MySqlDbType.VarChar,32),
					new MySqlParameter("@UserPassword", MySqlDbType.VarChar,64),
					new MySqlParameter("@AddTime", MySqlDbType.DateTime),
					new MySqlParameter("@UserStatus", MySqlDbType.Int16,5),
					new MySqlParameter("@UserCode", MySqlDbType.VarChar,16)};
            parameters[0].Value = account.UserName;
            parameters[1].Value = account.PassWord;
            parameters[2].Value = account.AddTime;
            parameters[3].Value = account.Status == true ? 1 : 0;
            parameters[4].Value = account.UserCode;

            int rows = DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            if (rows > 0)
            {
                if (DelAccountRole(account.UserCode))
                {
                    foreach (var v in account.Roles)
                    {
                        AddAccountRole(account.UserCode, v);
                    }
                    return true;
                }
            }
            return false;
        }

        ///// <summary>
        ///// 修改密钥
        ///// </summary>
        ///// <param name="account"></param>
        ///// <param name="secretKey"></param>
        ///// <returns></returns>
        //public static bool UpdateSecretKey(string account, string secretKey, string password)
        //{
        //    MySqlTransaction myTran = DBUtility.MySqlHelper.CreateTransaction();
        //    try
        //    {
        //        if (UpdateSecretKey(myTran, account, secretKey))
        //        {
        //            if (UpdatePass(myTran, account, password))
        //            {
        //                myTran.Commit();
        //                return true;
        //            }
        //        }
        //        myTran.Rollback();
        //        return false;
        //    }
        //    catch
        //    {
        //        myTran.Rollback();
        //        return false;
        //    }
        //}

        //static bool UpdateSecretKey(MySqlTransaction myTrans, string account, string secretKey)
        //{
        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append("update plat_Account set ");
        //    strSql.Append("SecretKey=@SecretKey");
        //    strSql.Append(" where UserCode=@UserCode ");
        //    MySqlParameter[] parameters = {
        //            new MySqlParameter("@SecretKey", MySqlDbType.VarChar,16),
        //            new MySqlParameter("@UserCode", MySqlDbType.VarChar,16)};
        //    parameters[0].Value = secretKey;
        //    parameters[1].Value = account;

        //    int rows = DBUtility.MySqlHelper.ExecuteNonQuery(myTrans, strSql.ToString(), parameters);
        //    if (rows > 0)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //static bool UpdatePass(MySqlTransaction myTrans, string account, string password)
        //{
        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append("update plat_Account set ");
        //    strSql.Append("UserPassword=@UserPassword");
        //    strSql.Append(" where UserCode=@UserCode ");
        //    MySqlParameter[] parameters = {
        //            new MySqlParameter("@UserPassword", MySqlDbType.VarChar,20),
        //            new MySqlParameter("@UserCode", MySqlDbType.VarChar,16)};
        //    parameters[0].Value = password;
        //    parameters[1].Value = account;

        //    int rows = DBUtility.MySqlHelper.ExecuteNonQuery(myTrans, strSql.ToString(), parameters);
        //    if (rows > 0)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public static bool Del(string UserCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from plat_Account ");
            strSql.Append(" where UserCode=@UserCode ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@UserCode", MySqlDbType.VarChar,16)			};
            parameters[0].Value = UserCode;

            int rows = DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            return true;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static SMS.Model.SysAccount GetAccount(string account)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_Account ");
            strSql.Append(" where UserCode=@UserCode ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@UserCode", MySqlDbType.VarChar,16)			};
            parameters[0].Value = account;

            SMS.Model.SysAccount model = new SMS.Model.SysAccount();
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        public static bool ChanagePass(string account, string password)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update plat_Account set ");
            strSql.Append("UserPassword=@UserPassword");
            strSql.Append(" where UserCode=@UserCode ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@UserPassword", MySqlDbType.VarChar,64),
					new MySqlParameter("@UserCode", MySqlDbType.VarChar,16)};
            parameters[0].Value = password;
            parameters[1].Value = account;

            int rows = DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            return false;
        }

        public static List<SMS.Model.SysAccount> GetAccounts()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_Account ");
            List<SMS.Model.SysAccount> list = new List<SMS.Model.SysAccount>();
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(DataRowToModel(row));
                }
            }
            return list;
        }

        static bool AddAccountRole(string account, string role)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into plat_AccountRole(");
            strSql.Append("UserCode,RoleID,UpdateTime)");
            strSql.Append(" values (");
            strSql.Append("@UserCode,@RoleID,@UpdateTime)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@UserCode", MySqlDbType.VarChar,16),
					new MySqlParameter("@RoleID", MySqlDbType.VarChar,32),
					new MySqlParameter("@UpdateTime", MySqlDbType.DateTime)};
            parameters[0].Value = account;
            parameters[1].Value = role;
            parameters[2].Value = DateTime.Now;

            int rows = DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static List<string> GetAccountRoles(string account)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select RoleID from plat_AccountRole ");
            strSql.Append(" where UserCode=@UserCode ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@UserCode", MySqlDbType.VarChar,16)			};
            parameters[0].Value = account;

            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            List<string> list = new List<string>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(row["RoleID"].ToString());
                }
            }
            return list;
        }

        public static bool DelAccountRole(string account)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from plat_AccountRole ");
            strSql.Append(" where UserCode=@UserCode ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@UserCode", MySqlDbType.VarChar,16)			};
            parameters[0].Value = account;

            int rows = DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool DelByUserCode(string UserCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from plat_AccountRole ");
            strSql.Append(" where UserCode=@UserCode ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@UserCode", MySqlDbType.VarChar,64)			};
            parameters[0].Value = UserCode;
            DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            return true;
        }

        public static bool DelByRoleID(string RoleID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from plat_AccountRole ");
            strSql.Append(" where RoleID=@RoleID ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@RoleID", MySqlDbType.VarChar,64)			};
            parameters[0].Value = RoleID;
            DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            return true;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        static SMS.Model.SysAccount DataRowToModel(DataRow row)
        {
            SMS.Model.SysAccount model = new SMS.Model.SysAccount();
            if (row != null)
            {
                if (row["UserCode"] != null)
                {
                    model.UserCode = row["UserCode"].ToString();
                }
                if (row["UserName"] != null)
                {
                    model.UserName = row["UserName"].ToString();
                }
                if (row["UserPassword"] != null)
                {
                    model.PassWord = row["UserPassword"].ToString();
                }
                if (row["AddTime"] != null && row["AddTime"].ToString() != "")
                {
                    model.AddTime = DateTime.Parse(row["AddTime"].ToString());
                }
                if (row["UserStatus"] != null && row["UserStatus"].ToString() != "")
                {
                    model.Status = (ushort)row["UserStatus"] == 1 ? true : false;
                }
            }
            model.Roles = GetAccountRoles(model.UserCode);
            return model;
        }
    }
}
