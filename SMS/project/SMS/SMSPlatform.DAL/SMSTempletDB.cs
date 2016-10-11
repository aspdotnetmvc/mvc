using MySql.Data.MySqlClient;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SMSPlatform.DAL
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

            DBHelper.Instance.Execute(strSql.ToString(), model);
            return true;

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
            DBHelper.Instance.Execute(strSql.ToString(), model);
            return true;
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
            DBHelper.Instance.Execute(strSql.ToString(), model);
            return true;
        }

        public static bool Del(string templetID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from plat_SMSTempletAudit ");
            strSql.Append(" where TempletID=@TempletID ");
            DBHelper.Instance.Execute(strSql.ToString(), new { TempletID = templetID });
            return true;
        }

        public static SMSTemplet GetSMSTempetContent(string templetID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_SMSTempletAudit ");
            strSql.Append(" where TempletID=@TempletID");
           return  DBHelper.Instance.Query<SMSTemplet>(strSql.ToString(), new { TempletID = templetID }).FirstOrDefault();
 
        }

        /// <summary>
        /// 获取短信模板
        /// </summary>
        public static List<SMSTemplet> GetSMSTempets()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_SMSTempletAudit");
            return DBHelper.Instance.Query<SMSTemplet>(strSql.ToString());
        }
    }
}
