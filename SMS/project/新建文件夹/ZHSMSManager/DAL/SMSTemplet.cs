using MySql.Data.MySqlClient;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DAL
{
    public class SMSTempletDB
    {
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static bool Add(SMSTemplet model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into plat_SMSTempletAudit(");
            strSql.Append("AccountCode,TempletContent,SubmitTime,AuditTime,UserCode,AuditState,AuditLevel,Remark,TempletID,AccountName,Signature)");
            strSql.Append(" values (");
            strSql.Append("@AccountCode,@TempletContent,@SubmitTime,@AuditTime,@UserCode,@AuditState,@AuditLevel,@Remark,@TempletID,@AccountName,@Signature)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AccountCode", MySqlDbType.VarChar,64),
					new MySqlParameter("@TempletContent", MySqlDbType.VarChar,1024),
					new MySqlParameter("@SubmitTime", MySqlDbType.DateTime),
					new MySqlParameter("@AuditTime", MySqlDbType.VarChar,32),
					new MySqlParameter("@UserCode", MySqlDbType.VarChar,16),
					new MySqlParameter("@AuditState", MySqlDbType.Int16,4),
					new MySqlParameter("@AuditLevel", MySqlDbType.Int16,4),
					new MySqlParameter("@Remark", MySqlDbType.VarChar,512),
					new MySqlParameter("@TempletID", MySqlDbType.VarChar,64),
					new MySqlParameter("@AccountName", MySqlDbType.VarChar,128),
                    new MySqlParameter("@Signature", MySqlDbType.VarChar,128)};
            parameters[0].Value = model.AccountCode;
            parameters[1].Value = model.TempletContent;
            parameters[2].Value = model.SubmitTime;
            parameters[3].Value = model.AuditTime;
            parameters[4].Value = model.UserCode;
            parameters[5].Value = (int)model.AuditState;
            parameters[6].Value = (int)model.AuditLevel;
            parameters[7].Value = model.Remark;
            parameters[8].Value = model.TempletID;
            parameters[9].Value = model.AccountName;
            parameters[10].Value = model.Signature;

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

        public static bool Update(SMSTemplet model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update plat_SMSTempletAudit set ");
            strSql.Append("AuditTime=@AuditTime,");
            strSql.Append("UserCode=@UserCode,");
            strSql.Append("AuditState=@AuditState,");
            strSql.Append("Remark=@Remark");
            strSql.Append(" where TempletID=@TempletID");
            MySqlParameter[] parameters = {
					new MySqlParameter("@AuditTime", MySqlDbType.VarChar,32),
					new MySqlParameter("@UserCode", MySqlDbType.VarChar,16),
					new MySqlParameter("@AuditState", MySqlDbType.Int16,4),
					new MySqlParameter("@Remark", MySqlDbType.VarChar,512),
					new MySqlParameter("@TempletID", MySqlDbType.VarChar,64)};
            parameters[0].Value = model.AuditTime;
            parameters[1].Value = model.UserCode;
            parameters[2].Value = model.AuditState;
            parameters[3].Value = model.Remark;
            parameters[4].Value = model.TempletID;

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

        public static bool UpdateContent(SMSTemplet model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update plat_SMSTempletAudit set ");
            strSql.Append("AuditState=@AuditState,");
            strSql.Append("TempletContent=@TempletContent,");
            strSql.Append("SubmitTime=@SubmitTime,");
            strSql.Append("Remark=@Remark");
            strSql.Append(" where TempletID=@TempletID");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@AuditState", MySqlDbType.Int16,4),
					new MySqlParameter("@TempletContent", MySqlDbType.VarChar,1024),
					new MySqlParameter("@SubmitTime", MySqlDbType.VarChar,16),
                    new MySqlParameter("@Remark", MySqlDbType.VarChar,512),
					new MySqlParameter("@TempletID", MySqlDbType.VarChar,64)};
            parameters[0].Value = (int)model.AuditState;
            parameters[1].Value = model.TempletContent;
            parameters[2].Value = model.SubmitTime;
            parameters[3].Value = model.TempletID;
            parameters[4].Value = model.TempletID;

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

        public static bool Del(string templetID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from plat_SMSTempletAudit ");
            strSql.Append(" where TempletID=@TempletID ");
            MySqlParameter[] parameters = {
					new MySqlParameter("@TempletID", MySqlDbType.VarChar,64)			};
            parameters[0].Value = templetID;

            int rows = DBUtility.MySqlHelper.ExecuteNonQuery(strSql.ToString(), parameters);
            return true;
        }

        public static SMSTemplet GetSMSTempetContent(string templetID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_SMSTempletAudit ");
            strSql.Append(" where TempletID=@TempletID");
            MySqlParameter[] parameters = {
					new MySqlParameter("@TempletID", MySqlDbType.VarChar,64)
			};
            parameters[0].Value = templetID;

            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(),parameters);
            if (ds.Tables.Count == 0) return null;
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            return null;
        }

        /// <summary>
        /// 获取短信模板
        /// </summary>
        public static List<SMSTemplet> GetSMSTempets()
        {
            List<SMSTemplet> list = new List<SMSTemplet>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_SMSTempletAudit");
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString());
            if (ds.Tables.Count == 0) return list;
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(DataRowToModel(row));
                }
            }
            return list;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static SMSTemplet DataRowToModel(DataRow row)
        {
            SMSTemplet model = new SMSTemplet();
            if (row != null)
            {
                if (row["TempletID"] != null)
                {
                    model.TempletID = row["TempletID"].ToString();
                }
                if (row["AccountName"] != null)
                {
                    model.AccountName = row["AccountName"].ToString();
                }
                if (row["AccountCode"] != null)
                {
                    model.AccountCode = row["AccountCode"].ToString();
                }
                if (row["TempletContent"] != null)
                {
                    model.TempletContent = row["TempletContent"].ToString();
                }
                if (row["SubmitTime"] != null && row["SubmitTime"].ToString() != "")
                {
                    model.SubmitTime = DateTime.Parse(row["SubmitTime"].ToString());
                }
                if (row["AuditTime"] != null)
                {
                    model.AuditTime = row["AuditTime"].ToString();
                }
                if (row["UserCode"] != null)
                {
                    model.UserCode = row["UserCode"].ToString();
                }
                if (row["AuditState"] != null && row["AuditState"].ToString() != "")
                {
                    model.AuditState = (SMS.Model.TempletAuditType)int.Parse(row["AuditState"].ToString());
                }
                if (row["AuditLevel"] != null && row["AuditLevel"].ToString() != "")
                {
                    model.AuditLevel = (SMS.Model.TempletAuditLevelType)int.Parse(row["AuditLevel"].ToString());
                }
                if (row["Remark"] != null)
                {
                    model.Remark = row["Remark"].ToString();
                }
                if (row["Signature"] != null)
                {
                    model.Signature = row["Signature"].ToString();
                }
            }
            return model;
        }
    }
}
