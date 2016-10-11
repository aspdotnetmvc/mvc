using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DAL
{
    public class Login
    {
        public static int Logon(string accountID, string pass)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_Account ");
            strSql.Append(" where UserCode=@UserCode and UserPassword=@UserPassword");
            MySqlParameter[] parameters = {
					new MySqlParameter("@UserCode", MySqlDbType.VarChar,16),
					new MySqlParameter("@UserPassword", MySqlDbType.VarChar,16)};
            parameters[0].Value = accountID;
            parameters[1].Value = pass;

            SMS.Model.SysAccount account = new SMS.Model.SysAccount();
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["UserStatus"].ToString() == "0")
                {
                    //帐号禁用
                    return 2;
                }
                DataTable dt = DBUtility.MySqlHelper.Query("select r.* from plat_AccountRole as ar left join plat_Role as r on ar.RoleID = r.RoleID and ar.UserCode='" + accountID + "'").Tables[0];
                account.UserCode = ds.Tables[0].Rows[0]["UserCode"].ToString();
                account.UserName = ds.Tables[0].Rows[0]["UserName"].ToString();
                List<string> roles = new List<string>();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        SMS.Model.Role role = new SMS.Model.Role();
                        role.RoleID = row["RoleID"].ToString();
                        role.RoleName = row["RoleName"].ToString();
                        if (!string.IsNullOrEmpty(role.RoleID))
                        {
                            roles.Add(role.RoleID);
                        }
                    }
                }
                account.Roles = roles;
                System.Web.HttpContext.Current.Session["Login"] = account;

                //strSql.Remove(0, strSql.Length);
                //strSql.Append("select groupid from mastergroup where masterid='" + model.MASTERID + "'");
                //object obj = OracleHelper.ExecuteScalar(strSql.ToString());
                //if (obj != null)
                //{
                //    System.Web.HttpContext.Current.Session["GroupId"] = obj;
                //}
                //else
                //{
                //    System.Web.HttpContext.Current.Session["GroupId"] = 1;
                //}

                //string ip = Helper.GetIpAddress();
                //if (HttpRuntime.Cache[model.MASTERID] == null)
                //{
                //    HttpRuntime.Cache.Insert(model.MASTERID, ip, null, DateTime.Now.AddHours(3.0), TimeSpan.Zero);
                //}
                //else
                //{
                //    if (ip != HttpRuntime.Cache[model.MASTERID].ToString())
                //    {
                //        HttpRuntime.Cache.Insert(model.MASTERID, ip, null, DateTime.Now.AddHours(3.0), TimeSpan.Zero);
                //    }
                //}
                return 1;
            }
            else
            {
                //用户名或密码不正确
                return 0;
            }
        }

        public static void IsLogin()
        {
            object obj = System.Web.HttpContext.Current.Session["Login"];
            if (obj == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("/login.aspx", true);
            }
        }
    }
}
