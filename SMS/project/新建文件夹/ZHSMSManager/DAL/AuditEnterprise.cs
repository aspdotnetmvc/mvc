using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DAL
{
    public class AuditEnterprise
    {
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static bool Add(SMS.Model.AuditEnterprise model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into plat_AuditEnterprise(");
            strSql.Append("EnterpriseCode,AuditResult,Auditor,AuditTime,EnterpriseResult,AuditRemark,CreateTime)");
            strSql.Append(" values (");
            strSql.Append("@EnterpriseCode,@AuditResult,@Auditor,@AuditTime,@EnterpriseResult,@AuditRemark,@CreateTime)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@EnterpriseCode", MySqlDbType.VarChar,64),
					new MySqlParameter("@AuditResult", MySqlDbType.Int16,5),
					new MySqlParameter("@Auditor", MySqlDbType.VarChar,64),
					new MySqlParameter("@AuditTime", MySqlDbType.DateTime),
					new MySqlParameter("@EnterpriseResult", MySqlDbType.Int16,5),
					new MySqlParameter("@AuditRemark", MySqlDbType.VarChar,128),
					new MySqlParameter("@CreateTime", MySqlDbType.DateTime)};
            parameters[0].Value = model.EnterpriseCode;
            parameters[1].Value = model.AuditResult == true ? 1 : 0;
            parameters[2].Value = model.Auditor;
            parameters[3].Value = model.AuditTime;
            parameters[4].Value = model.EnterpriseResult == true ? 1 : 0;
            parameters[5].Value = model.AuditRemark;
            parameters[6].Value = model.CreateTime;

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
        /// 更新一条数据
        /// </summary>
        public static bool Update(SMS.Model.AuditEnterprise model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update plat_AuditEnterprise set ");
            strSql.Append("AuditResult=@AuditResult,");
            strSql.Append("Auditor=@Auditor,");
            strSql.Append("AuditTime=@AuditTime,");
            strSql.Append("EnterpriseResult=@EnterpriseResult,");
            strSql.Append("AuditRemark=@AuditRemark");
            strSql.Append(" where EnterpriseCode=@EnterpriseCode");
            MySqlParameter[] parameters = {
					new MySqlParameter("@EnterpriseCode", MySqlDbType.VarChar,64),
					new MySqlParameter("@AuditResult", MySqlDbType.Int16,5),
					new MySqlParameter("@Auditor", MySqlDbType.VarChar,64),
					new MySqlParameter("@AuditTime", MySqlDbType.DateTime),
					new MySqlParameter("@EnterpriseResult", MySqlDbType.Int16,5),
					new MySqlParameter("@AuditRemark", MySqlDbType.VarChar,128)};
            parameters[0].Value = model.EnterpriseCode;
            parameters[1].Value = model.AuditResult;
            parameters[2].Value = model.Auditor;
            parameters[3].Value = model.AuditTime;
            parameters[4].Value = model.EnterpriseResult;
            parameters[5].Value = model.AuditRemark;

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
        /// 删除一条数据
        /// </summary>
        public static bool Delete(string code)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from plat_AuditEnterprise ");
            strSql.Append(" where EnterpriseCode=@EnterpriseCode");
            MySqlParameter[] parameters = {
					new MySqlParameter("@EnterpriseCode", MySqlDbType.VarChar,64)
			};
            parameters[0].Value = code;

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
        /// 获取未审核和审核不通过的企业
        /// </summary>
        public static List<SMS.Model.AuditEnterprise> GetAuditOrFailureEnterprise()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_AuditEnterprise where AuditResult='0' or EnterpriseResult='0'");
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString());
            List<SMS.Model.AuditEnterprise> list = new List<SMS.Model.AuditEnterprise>();
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
        static SMS.Model.AuditEnterprise DataRowToModel(DataRow row)
        {
            SMS.Model.AuditEnterprise model = new SMS.Model.AuditEnterprise();
            if (row != null)
            {
                if (row["EnterpriseCode"] != null)
                {
                    model.EnterpriseCode = row["EnterpriseCode"].ToString();
                }
                if (row["AuditResult"] != null && row["AuditResult"].ToString() != "")
                {
                    model.AuditResult = row["AuditResult"].ToString() == "1" ? true : false;
                }
                if (row["Auditor"] != null)
                {
                    model.Auditor = row["Auditor"].ToString();
                }
                if (row["AuditTime"] != null && row["AuditTime"].ToString() != "")
                {
                    model.AuditTime = DateTime.Parse(row["AuditTime"].ToString());
                }
                if (row["CreateTime"] != null && row["CreateTime"].ToString() != "")
                {
                    model.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
                }
                if (row["EnterpriseResult"] != null && row["EnterpriseResult"].ToString() != "")
                {
                    model.EnterpriseResult = row["EnterpriseResult"].ToString() == "1" ? true : false;
                }
                if (row["AuditRemark"] != null)
                {
                    model.AuditRemark = row["AuditRemark"].ToString();
                }
            }
            return model;
        }
    }
}
