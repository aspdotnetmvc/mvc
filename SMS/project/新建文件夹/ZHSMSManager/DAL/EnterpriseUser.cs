using MySql.Data.MySqlClient;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DAL
{
    public class EnterpriseUser
    {
        public static bool Exists(string accountCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from plat_EnterpriseUser");
            strSql.Append(" where AccountCode=@AccountCode");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AccountCode", MySqlDbType.VarChar,64)
			};
            parameters[0].Value = accountCode;
            return DBUtility.MySqlHelper.Exists(strSql.ToString(), parameters);
        }
        public static List<SMS.Model.EnterpriseUser> GetEnterprises()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select en.AccountCode Code,en.*,et.* from plat_EnterpriseInformation as en left join plat_EnterpriseUser as et on en.AccountCode = et.AccountCode");
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString());
            List<SMS.Model.EnterpriseUser> list = new List<SMS.Model.EnterpriseUser>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
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
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString());
            List<SMS.Model.EnterpriseUser> list = new List<SMS.Model.EnterpriseUser>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
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
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            return null;
        }

        public static bool Add(SMS.Model.EnterpriseUser user)
        {
            MySqlTransaction myTran = DBUtility.MySqlHelper.CreateTransaction();
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
            MySqlParameter[] parameters = {
					new MySqlParameter("@SecretKey", MySqlDbType.VarChar,16),
					new MySqlParameter("@AccountCode", MySqlDbType.VarChar,16),
					new MySqlParameter("@AppPassword", MySqlDbType.VarChar,64)};
            parameters[0].Value = secretKey;
            parameters[1].Value = account;
            parameters[2].Value = password;

            int rows = DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            return false;
        }

        static bool AddInfo(MySqlTransaction myTran, SMS.Model.EnterpriseUser model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into plat_EnterpriseInformation(");
            strSql.Append("AccountCode,Name,Contact,Telephone,Province,City,Address)");
            strSql.Append(" values (");
            strSql.Append("@AccountCode,@Name,@Contact,@Telephone,@Province,@City,@Address)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AccountCode", MySqlDbType.VarChar,64),
					new MySqlParameter("@Name", MySqlDbType.VarChar,128),
					new MySqlParameter("@Contact", MySqlDbType.VarChar,32),
					new MySqlParameter("@Telephone", MySqlDbType.VarChar,32),
					new MySqlParameter("@Province", MySqlDbType.VarChar,16),
					new MySqlParameter("@City", MySqlDbType.VarChar,16),
					new MySqlParameter("@Address", MySqlDbType.VarChar,256)};
            parameters[0].Value = model.AccountCode;
            parameters[1].Value = model.Name;
            parameters[2].Value = model.Contact;
            parameters[3].Value = model.Phone;
            parameters[4].Value = model.Province;
            parameters[5].Value = model.City;
            parameters[6].Value = model.Address;
            int rows = 0;
            if (myTran == null)
            {
                rows = DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            }
            else
            {
                rows = DBUtility.MySqlHelper.ExecuteNonQuery(myTran, strSql.ToString(), parameters);
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

        /// <summary>
        /// 增加一条数据
        /// </summary>
        static bool AddUserSetting(MySqlTransaction myTran, SMS.Model.EnterpriseUser model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into plat_EnterpriseUser(");
            strSql.Append("AccountID,AccountPassword,Priority,Audit,SPNumber,IsEnable,RegisterDate,AccountCode,IsAgent,ParentAccountCode,SecretKey,AppPassword,IsOpen)");
            strSql.Append(" values (");
            strSql.Append("@AccountID,@AccountPassword,@Priority,@Audit,@SPNumber,@IsEnable,@RegisterDate,@AccountCode,@IsAgent,@ParentAccountCode,@SecretKey,@AppPassword,@IsOpen)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AccountID", MySqlDbType.VarChar,64),
					new MySqlParameter("@AccountPassword", MySqlDbType.VarChar,64),
					new MySqlParameter("@Priority", MySqlDbType.Int32,5),
					new MySqlParameter("@Audit", MySqlDbType.Int32,5),
					new MySqlParameter("@SPNumber", MySqlDbType.VarChar,48),
					new MySqlParameter("@IsEnable", MySqlDbType.Int32,5),
					new MySqlParameter("@RegisterDate", MySqlDbType.DateTime),
					new MySqlParameter("@AccountCode", MySqlDbType.VarChar,64),
					new MySqlParameter("@IsAgent", MySqlDbType.Int32,5),
					new MySqlParameter("@ParentAccountCode", MySqlDbType.VarChar,64),
					new MySqlParameter("@SecretKey", MySqlDbType.VarChar,16),
					new MySqlParameter("@AppPassword", MySqlDbType.VarChar,64),
					new MySqlParameter("@IsOpen", MySqlDbType.Int32,5)};
            parameters[0].Value = model.AccountID;
            parameters[1].Value = model.Password;
            parameters[2].Value = (ushort)model.Priority;
            parameters[3].Value = (ushort)model.Audit;
            parameters[4].Value = model.SPNumber;
            parameters[5].Value = model.Enabled == true ? 1 : 0;
            parameters[6].Value = model.RegisterDate;
            parameters[7].Value = model.AccountCode;
            parameters[8].Value = model.IsAgent == true ? 1 : 0;
            parameters[9].Value = model.ParentAccountCode;
            parameters[10].Value = model.SecretKey;
            parameters[11].Value = model.AppPassword;
            parameters[12].Value = model.IsOpen == true ? 1 : 0;

            int rows = 0;
            if (myTran == null)
            {
                rows = DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            }
            else
            {
                rows = DBUtility.MySqlHelper.ExecuteNonQuery(myTran, strSql.ToString(), parameters);
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

        static bool AddSMSSetting(MySqlTransaction myTran, SMS.Model.EnterpriseUser user)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into plat_SMSSetting(");
            strSql.Append("AccountCode,StatusReport,SMSLevel,FilterType,Channel,Signature,SMSType)");
            strSql.Append(" values (");
            strSql.Append("@AccountCode,@StatusReport,@SMSLevel,@FilterType,@Channel,@Signature,@SMSType)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AccountCode", MySqlDbType.VarChar,64),
					new MySqlParameter("@StatusReport", MySqlDbType.Int32,5),
					new MySqlParameter("@SMSLevel", MySqlDbType.Int32,5),
					new MySqlParameter("@FilterType", MySqlDbType.Int32,5),
					new MySqlParameter("@Channel", MySqlDbType.VarChar,64),
					new MySqlParameter("@Signature", MySqlDbType.VarChar,128),
                     new MySqlParameter("@SMSType", MySqlDbType.Int32,5)};
            parameters[0].Value = user.AccountCode;
            parameters[1].Value = user.StatusReport;
            parameters[2].Value = user.SMSLevel;
            parameters[3].Value = user.FilterType;
            parameters[4].Value = user.Channel;
            parameters[5].Value = user.Signature;
            parameters[6].Value = user.SMSType;
            int rows = 0;
            if (myTran == null)
            {
                rows = DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            }
            else
            {
                rows = DBUtility.MySqlHelper.ExecuteNonQuery(myTran, strSql.ToString(), parameters);
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

        static DataTable GetSMSSetting(string enterprise)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_SMSSetting ");
            strSql.Append(" where AccountCode=@AccountCode");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@AccountCode", MySqlDbType.VarChar,64)
            };
            parameters[0].Value = enterprise;

            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }

        public static bool Del(string user)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from plat_EnterpriseUser ");
            strSql.Append(" where AccountCode=@AccountCode ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AccountCode", MySqlDbType.VarChar,64)			};
            parameters[0].Value = user;
            DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);

            strSql.Clear();
            strSql.Append("delete from plat_EnterpriseInformation ");
            strSql.Append(" where AccountCode=@AccountCode ");
            MySqlParameter[] parameters1 = {
					new MySqlParameter("@AccountCode", MySqlDbType.VarChar,64)};
            parameters1[0].Value = user;
            DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters1);

            strSql.Clear();
            strSql.Append("delete from plat_SMSSetting ");
            strSql.Append(" where AccountCode=@AccountCode ");
            MySqlParameter[] parameters2 = {
					new MySqlParameter("@AccountCode", MySqlDbType.VarChar,64)};
            parameters2[0].Value = user;
            DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters2);

            return true;
        }
        public static bool UpdateAccountSetting(SMS.Model.EnterpriseUser user)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update plat_EnterpriseUser set ");
            strSql.Append("Priority=@Priority,");
            strSql.Append("Audit=@Audit,");
            strSql.Append("SPNumber=@SPNumber,");
            strSql.Append("IsEnable=@IsEnable,");
            strSql.Append("IsAgent=@IsAgent,");
            strSql.Append("IsOpen=@IsOpen,");
            strSql.Append("ParentAccountCode=@ParentAccountCode");
            strSql.Append(" where AccountCode=@AccountCode ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AccountCode", MySqlDbType.VarChar,64),
					new MySqlParameter("@Priority", MySqlDbType.Int32,5),
					new MySqlParameter("@Audit", MySqlDbType.Int32,5),
					new MySqlParameter("@SPNumber", MySqlDbType.VarChar,48),
					new MySqlParameter("@IsEnable", MySqlDbType.Int32,5),
                    new MySqlParameter("@IsOpen", MySqlDbType.Int32,5),
					new MySqlParameter("@IsAgent", MySqlDbType.Int32,5),
					new MySqlParameter("@ParentAccountCode", MySqlDbType.VarChar,64)};
            parameters[0].Value = user.AccountCode;
            parameters[1].Value = (ushort)user.Priority;
            parameters[2].Value = (ushort)user.Audit;
            parameters[3].Value = user.SPNumber;
            parameters[4].Value = user.Enabled == true ? 1 : 0;
            parameters[5].Value = user.IsOpen == true ? 1 : 0;
            parameters[6].Value = user.IsAgent == true ? 1 : 0;
            parameters[7].Value = user.ParentAccountCode;

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
        public static bool UpdateAccontInfo(SMS.Model.EnterpriseUser user)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update plat_EnterpriseInformation set ");
            strSql.Append("Name=@Name,");
            strSql.Append("Contact=@Contact,");
            strSql.Append("Telephone=@Telephone,");
            strSql.Append("Province=@Province,");
            strSql.Append("City=@City,");
            strSql.Append("Address=@Address");
            strSql.Append(" where AccountCode=@AccountCode ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@Name", MySqlDbType.VarChar,128),
					new MySqlParameter("@Contact", MySqlDbType.VarChar,32),
					new MySqlParameter("@Telephone", MySqlDbType.VarChar,32),
					new MySqlParameter("@Province", MySqlDbType.VarChar,16),
					new MySqlParameter("@City", MySqlDbType.VarChar,16),
					new MySqlParameter("@Address", MySqlDbType.VarChar,256),
					new MySqlParameter("@AccountCode", MySqlDbType.VarChar,64)};
            parameters[0].Value = user.Name;
            parameters[1].Value = user.Contact;
            parameters[2].Value = user.Phone;
            parameters[3].Value = user.Province;
            parameters[4].Value = user.City;
            parameters[5].Value = user.Address;
            parameters[6].Value = user.AccountCode;

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
        public static bool UpdateAccountSMS(SMS.Model.EnterpriseUser user)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update plat_SMSSetting set ");
            strSql.Append("StatusReport=@StatusReport,");
            strSql.Append("SMSLevel=@SMSLevel,");
            strSql.Append("FilterType=@FilterType,");
            strSql.Append("SMSType=@SMSType,");
            strSql.Append("Signature=@Signature,");
            strSql.Append("Channel=@Channel");
            strSql.Append(" where AccountCode=@AccountCode");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AccountCode", MySqlDbType.VarChar,64),
					new MySqlParameter("@StatusReport", MySqlDbType.Int32,5),
					new MySqlParameter("@SMSLevel", MySqlDbType.Int32,5),
					new MySqlParameter("@FilterType", MySqlDbType.Int32,5),
                    		new MySqlParameter("@SMSType", MySqlDbType.Int32,5),
					new MySqlParameter("@Channel", MySqlDbType.VarChar,64),
                    new MySqlParameter("@Signature", MySqlDbType.VarChar,128)};
            parameters[0].Value = user.AccountCode;
            parameters[1].Value = user.StatusReport;
            parameters[2].Value = user.SMSLevel;
            parameters[3].Value = user.FilterType;
            parameters[4].Value = user.SMSType;
            parameters[5].Value = user.Channel;

            if (!string.IsNullOrEmpty(user.Signature))
            {
                if (!user.Signature.StartsWith("【"))
                {
                    user.Signature = "【" + user.Signature + "】";
                }
            }


            parameters[6].Value = user.Signature;

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

        public static bool ChangePass(string account, string pass, string appPass)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update plat_EnterpriseUser set ");
            strSql.Append("AccountPassword=@AccountPassword,");
            strSql.Append("AppPassword=@AppPassword");
            strSql.Append(" where AccountCode=@AccountCode ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AccountCode", MySqlDbType.VarChar,64),
					new MySqlParameter("@AccountPassword", MySqlDbType.VarChar,64),
					new MySqlParameter("@AppPassword", MySqlDbType.VarChar,64)};
            parameters[0].Value = account;
            parameters[1].Value = pass;
            parameters[2].Value = appPass;

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
                    user.Priority = (SMS.Model.AccountPriorityType)((ushort)row["Priority"]);
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
                    if (dr["SMSLevel"] != null && dr["SMSLevel"].ToString() != "")
                    {
                        user.SMSLevel = (ushort)dr["SMSLevel"];
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