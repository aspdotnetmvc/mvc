using MySql.Data.MySqlClient;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SMSPlatform.DAL
{
    public class EnterpriseUserDB
    {
        public static bool Exists(string accountCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from plat_EnterpriseUser");
            strSql.Append(" where AccountCode=@AccountCode");
            var c = DBHelper.Instance.ExecuteScalar<int>(strSql.ToString(), new { AccountCode = accountCode });
            return c > 0;
        }
        public static List<SMS.Model.EnterpriseUser> GetEnterprises()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select en.AccountCode Code,en.*,et.* from plat_EnterpriseInformation as en left join plat_EnterpriseUser as et on en.AccountCode = et.AccountCode");
            var dt = DBHelper.Instance.GetDataTable(strSql.ToString());
            List<SMS.Model.EnterpriseUser> list = new List<SMS.Model.EnterpriseUser>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(DataRowToModel(row));
                }
            }
            return list;
        }

        public static List<SMS.Model.EnterpriseUser> GetEnterprisesByAccountEnterprise(string UserCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select en.AccountCode Code,en.*,et.* from plat_EnterpriseInformation as en left join plat_EnterpriseUser as et on en.AccountCode = et.AccountCode  left join AccountEnterprise as ee on  en.AccountCode=ee.EnterpriseCode where ee.UserCode=" + UserCode);
            var dt = DBHelper.Instance.GetDataTable(strSql.ToString());
            List<SMS.Model.EnterpriseUser> list = new List<SMS.Model.EnterpriseUser>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(DataRowToModel(row));
                }
            }
            return list;
        }

        public static SMS.Model.EnterpriseUser GetEnterprise(string user)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select en.AccountCode Code,en.*,et.* from plat_EnterpriseInformation as en inner join plat_EnterpriseUser as et on en.AccountCode = et.AccountCode and en.AccountCode=@AccountCode");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AccountCode", MySqlDbType.VarChar,64)
			};
            parameters[0].Value = user;
            var dt = DBHelper.Instance.GetDataTable(strSql.ToString(), parameters);
            if (dt.Rows.Count > 0)
            {
                return DataRowToModel(dt.Rows[0]);
            }
            return null;
        }

        public static bool Add(SMS.Model.EnterpriseUser user)
        {
            var myTran = DBHelper.Instance.GetDBAdapter().BeginTransaction();
            if (AddInfo(myTran, user))
            {
                if (AddUserSetting(myTran, user))
                {
                    if (AddSMSSetting(myTran, user))
                    {
                        myTran.Commit();
                        return true;
                    }
                }
            }
            myTran.Rollback();
            return false;

        }
        /// <summary>
        /// 修改密钥
        /// </summary>
        /// <param name="account"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public static bool UpdateSecretKey(string account, string secretKey, string password)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update plat_EnterpriseUser set ");
            strSql.Append("SecretKey=@SecretKey,");
            strSql.Append("AppPassword=@AppPassword");
            strSql.Append(" where AccountCode=@AccountCode ");
            DBHelper.Instance.Execute(strSql.ToString(), new { SecretKey = secretKey, AccountCode = account, AppPassword = password });
            return true;
        }

        static bool AddInfo(IDbTransaction myTran, SMS.Model.EnterpriseUser model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into plat_EnterpriseInformation(");
            strSql.Append("AccountCode,Name,Contact,Telephone,Province,City,Address)");
            strSql.Append(" values (");
            strSql.Append("@AccountCode,@Name,@Contact,@Phone,@Province,@City,@Address)");
            DBHelper.Instance.Execute(strSql.ToString(), model, myTran);
          return true;
            
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        static bool AddUserSetting(IDbTransaction myTran, SMS.Model.EnterpriseUser model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into plat_EnterpriseUser(");
            strSql.Append("AccountID,AccountPassword,Priority,Audit,SPNumber,IsEnable,RegisterDate,AccountCode,IsAgent,ParentAccountCode,SecretKey,AppPassword,IsOpen)");
            strSql.Append(" values (");
            strSql.Append("@AccountID,@Password,@Priority,@Audit,@SPNumber,@Enabled,@RegisterDate,@AccountCode,@IsAgent,@ParentAccountCode,@SecretKey,@AppPassword,@IsOpen)");

            DBHelper.Instance.Execute(strSql.ToString(), model, myTran);
            return true;
        }

        static bool AddSMSSetting(IDbTransaction myTran, SMS.Model.EnterpriseUser user)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into plat_SMSSetting(");
            strSql.Append("AccountCode,StatusReport,FilterType,Channel,Signature,SMSType)");
            strSql.Append(" values (");
            strSql.Append("@AccountCode,@StatusReport,@FilterType,@Channel,@Signature,@SMSType)");
            DBHelper.Instance.Execute(strSql.ToString(), user, myTran);
            return true;
        }

        static DataTable GetSMSSetting(string enterprise)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_SMSSetting ");
            strSql.Append(" where AccountCode=@AccountCode");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@AccountCode", MySqlDbType.VarChar,64)
            };
            parameters[0].Value = enterprise;

            return DBHelper.Instance.GetDataTable(strSql.ToString(), parameters);
        }

        public static bool Del(string user)
        {
            List<string> sqllist = new List<string>();
            sqllist.Add("delete from plat_EnterpriseUser where AccountCode=@AccountCode ");
            sqllist.Add("delete from plat_EnterpriseInformation where AccountCode=@AccountCode ");
            sqllist.Add("delete from plat_SMSSetting where AccountCode=@AccountCode ");
            DBHelper.Instance.Execute(sqllist, new { AccountCode=user });

            return true;
        }
        public static bool UpdateAccountSetting(SMS.Model.EnterpriseUser user)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update plat_EnterpriseUser set ");
            strSql.Append("Priority=@Priority,");
            strSql.Append("Audit=@Audit,");
            strSql.Append("SPNumber=@SPNumber,");
            strSql.Append("IsEnable=@Enabled,");
            strSql.Append("IsAgent=@IsAgent,");
            strSql.Append("IsOpen=@IsOpen,");
            strSql.Append("ParentAccountCode=@ParentAccountCode");
            strSql.Append(" where AccountCode=@AccountCode ");
 
            DBHelper.Instance.Execute(strSql.ToString(), user);
            return true;
        }
        public static bool UpdateAccontInfo(SMS.Model.EnterpriseUser user)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update plat_EnterpriseInformation set ");
            strSql.Append("Name=@Name,");
            strSql.Append("Contact=@Contact,");
            strSql.Append("Telephone=@Phone,");
            strSql.Append("Province=@Province,");
            strSql.Append("City=@City,");
            strSql.Append("Address=@Address");
            strSql.Append(" where AccountCode=@AccountCode ");
            DBHelper.Instance.Execute(strSql.ToString(), user);
            return true;
        }
        public static bool UpdateAccountSMS(SMS.Model.EnterpriseUser user)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update plat_SMSSetting set ");
            strSql.Append("StatusReport=@StatusReport,");
            strSql.Append("FilterType=@FilterType,");
            strSql.Append("SMSType=@SMSType,");
            strSql.Append("Signature=@Signature,");
            strSql.Append("Channel=@Channel");
            strSql.Append(" where AccountCode=@AccountCode");
      

            if (!string.IsNullOrEmpty(user.Signature))
            {
                if (!user.Signature.StartsWith("【"))
                {
                    user.Signature = "【" + user.Signature + "】";
                }
            }

            DBHelper.Instance.Execute(strSql.ToString(), user);
            return true;
        }

        public static bool ChangePass(string account, string pass, string appPass)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update plat_EnterpriseUser set ");
            strSql.Append("AccountPassword=@AccountPassword,");
            strSql.Append("AppPassword=@AppPassword");
            strSql.Append(" where AccountCode=@AccountCode ");
            DBHelper.Instance.Execute(strSql.ToString(), new { AccountCode = account, AccountPassword = pass, AppPassword=appPass });
            return true;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        static SMS.Model.EnterpriseUser DataRowToModel(DataRow row)
        {
            SMS.Model.EnterpriseUser user = new SMS.Model.EnterpriseUser();
            if (row != null)
            {
                if (row["Name"] != null)
                {
                    user.Name = row["Name"].ToString();
                }
                if (row["Contact"] != null)
                {
                    user.Contact = row["Contact"].ToString();
                }
                if (row["Telephone"] != null)
                {
                    user.Phone = row["Telephone"].ToString();
                }
                if (row["Province"] != null)
                {
                    user.Province = row["Province"].ToString();
                }
                if (row["City"] != null)
                {
                    user.City = row["City"].ToString();
                }
                if (row["Address"] != null)
                {
                    user.Address = row["Address"].ToString();
                }
                if (row["AccountID"] != null)
                {
                    user.AccountID = row["AccountID"].ToString();
                }
                if (row["Code"] != null)
                {
                    user.AccountCode = row["Code"].ToString();
                }
                if (row["AccountPassword"] != null)
                {
                    user.Password = row["AccountPassword"].ToString();
                }
                if (row["Priority"] != null && row["Priority"].ToString() != "")
                {
                    user.Priority = int.Parse(row["Priority"].ToString());
                }
                if (row["Audit"] != null && row["Audit"].ToString() != "")
                {
                    user.Audit = (SMS.Model.AccountAuditType)((ushort)row["Audit"]);
                }
                if (row["SPNumber"] != null)
                {
                    user.SPNumber = row["SPNumber"].ToString();
                }
                if (row["IsEnable"] != null && row["IsEnable"].ToString() != "")
                {
                    user.Enabled = (ushort)row["IsEnable"] == 1 ? true : false;
                }
                if (row["RegisterDate"] != null && row["RegisterDate"].ToString() != "")
                {
                    user.RegisterDate = DateTime.Parse(row["RegisterDate"].ToString());
                }
                if (row["IsAgent"] != null && row["IsAgent"].ToString() != "")
                {
                    user.IsAgent = (ushort)row["IsAgent"] == 1 ? true : false;
                }
                if (row["ParentAccountCode"] != null)
                {
                    user.ParentAccountCode = row["ParentAccountCode"].ToString();
                }
                if (row["SecretKey"] != null)
                {
                    user.SecretKey = row["SecretKey"].ToString();
                }
                if (row["IsOpen"] != null && row["IsOpen"].ToString() != "")
                {
                    user.IsOpen = (ushort)row["IsOpen"] == 1 ? true : false;
                }
                if (row["AppPassword"] != null)
                {
                    user.AppPassword = row["AppPassword"].ToString();
                }
                DataTable dt = GetSMSSetting(user.AccountCode);
                if (dt != null)
                {
                    DataRow dr = dt.Rows[0];
                    if (dr["StatusReport"] != null && dr["StatusReport"].ToString() != "")
                    {
                        user.StatusReport = (StatusReportType)(ushort)dr["StatusReport"];
                    }

                    if (dr["FilterType"] != null && dr["FilterType"].ToString() != "")
                    {
                        user.FilterType = (ushort)dr["FilterType"];
                    }
                    if (dr["Channel"] != null)
                    {
                        user.Channel = dr["Channel"].ToString();
                    }
                    if (dr["Signature"] != null && dr["Signature"].ToString() != "")
                    {
                        user.Signature = dr["Signature"].ToString();
                    }
                    if (dr["SMSType"] != null && dr["SMSType"].ToString() != "")
                    {
                        user.SMSType = (SMSType)(int)dr["SMSType"];
                    }
                }
            }
            return user;
        }
    }
}