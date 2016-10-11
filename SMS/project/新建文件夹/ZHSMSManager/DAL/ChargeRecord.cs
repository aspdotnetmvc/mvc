using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DAL
{
    public class ChargeRecord
    {
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static bool Add(SMS.Model.ChargeRecord model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into plat_ChargeRecord(");
            strSql.Append("OperatorAccount,PrepaidAccount,ThenRate,PrepaidType,Money,SMSCount,RemainSMSCount,PrepaidTime,ChargeFlag,Remark)");
            strSql.Append(" values (");
            strSql.Append("@OperatorAccount,@PrepaidAccount,@ThenRate,@PrepaidType,@Money,@SMSCount,@RemainSMSCount,@PrepaidTime,@ChargeFlag,@Remark)");
            MySqlParameter[] parameters = {
					new MySqlParameter("@OperatorAccount", MySqlDbType.VarChar,32),
					new MySqlParameter("@PrepaidAccount", MySqlDbType.VarChar,64),
					new MySqlParameter("@ThenRate", MySqlDbType.Decimal,5),
					new MySqlParameter("@PrepaidType", MySqlDbType.Int32,5),
					new MySqlParameter("@Money", MySqlDbType.Decimal,10),
					new MySqlParameter("@SMSCount", MySqlDbType.Int32,10),
					new MySqlParameter("@RemainSMSCount", MySqlDbType.Int32,10),
					new MySqlParameter("@PrepaidTime", MySqlDbType.DateTime),
					new MySqlParameter("@ChargeFlag", MySqlDbType.Int32,5),
					new MySqlParameter("@Remark", MySqlDbType.VarChar,1024)};
            parameters[0].Value = model.OperatorAccount;
            parameters[1].Value = model.PrepaidAccount;
            parameters[2].Value = model.ThenRate;
            parameters[3].Value = model.PrepaidType;
            parameters[4].Value = model.Money;
            parameters[5].Value = model.SMSCount;
            parameters[6].Value = model.RemainSMSCount;
            parameters[7].Value = model.PrepaidTime;
            parameters[8].Value = model.ChargeFlag;
            parameters[9].Value = model.Remark;
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
        public static List<SMS.Model.ChargeRecord> GetRecordsByUsercode(string userCode, DateTime startTime, DateTime endTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from plat_ChargeRecord ");
            strSql.Append(" where PrepaidAccount=@PrepaidAccount and PrepaidTime>=@StartTime and PrepaidTime<=@EndTime");
            MySqlParameter[] parameters = {
					new MySqlParameter("@PrepaidAccount", MySqlDbType.VarChar,64),
                    new MySqlParameter("@StartTime", MySqlDbType.DateTime),
                    new MySqlParameter("@EndTime", MySqlDbType.DateTime)
			};
            parameters[0].Value = userCode;
            parameters[1].Value = startTime;
            parameters[2].Value = endTime;

            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            List<SMS.Model.ChargeRecord> list = new List<SMS.Model.ChargeRecord>();
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
        public static DataTable GetChargeStaticsByUserCode(string userCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select sum(Money) Money,sum(SMSCount) SMSCount from plat_ChargeRecord ");
            strSql.Append(" where PrepaidAccount=@PrepaidAccount and ChargeFlag=0");
            MySqlParameter[] parameters = {
					new MySqlParameter("@PrepaidAccount", MySqlDbType.VarChar,64)
			};
            parameters[0].Value = userCode;
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            return ds.Tables[0];
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static List<SMS.Model.ChargeStatics> GetChargeStatics()
        {
            List<SMS.Model.ChargeStatics> list = new List<SMS.Model.ChargeStatics>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select PrepaidAccount, sum(Money) Money,sum(SMSCount) SMSCount from plat_ChargeRecord ");
            strSql.Append(" where ChargeFlag=0 group by PrepaidAccount");

            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString());
            if (ds.Tables.Count == 0) return list;
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    SMS.Model.ChargeStatics mc = new SMS.Model.ChargeStatics();
                    if (row["PrepaidAccount"] != null)
                    {
                        mc.Enterprese = row["PrepaidAccount"].ToString();
                    }
                    if (row["Money"] != null && row["Money"].ToString() != "")
                    {
                        mc.TotalMoney = decimal.Parse(row["Money"].ToString());
                    }
                    if (row["SMSCount"] != null && row["SMSCount"].ToString() != "")
                    {
                        mc.SMSCount = long.Parse(row["SMSCount"].ToString());
                    }
                    list.Add(mc);
                }
            }
            return list;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static List<SMS.Model.ChargeStatics> GetChargeStatics(DateTime StartDate, DateTime EndDate)
        {
            List<SMS.Model.ChargeStatics> list = new List<SMS.Model.ChargeStatics>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select PrepaidAccount, sum(Money) Money,sum(SMSCount) SMSCount from plat_ChargeRecord ");
            strSql.Append(" where ChargeFlag=0 and PrepaidTime between @StartDate and @EndDate group by PrepaidAccount");

            MySqlParameter[] parameters = {
					new MySqlParameter("@StartDate", MySqlDbType.DateTime){Value=StartDate},
					new MySqlParameter("@EndDate", MySqlDbType.DateTime){Value=EndDate}
			};
            DataSet ds = DBUtility.MySqlHelper.Query(strSql.ToString(), parameters);
            if (ds.Tables.Count == 0) return list;
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    SMS.Model.ChargeStatics mc = new SMS.Model.ChargeStatics();
                    if (row["PrepaidAccount"] != null)
                    {
                        mc.Enterprese = row["PrepaidAccount"].ToString();
                    }
                    if (row["Money"] != null && row["Money"].ToString() != "")
                    {
                        mc.TotalMoney = decimal.Parse(row["Money"].ToString());
                    }
                    if (row["SMSCount"] != null && row["SMSCount"].ToString() != "")
                    {
                        mc.SMSCount = long.Parse(row["SMSCount"].ToString());
                    }
                    list.Add(mc);
                }
            }
            return list;
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        static SMS.Model.ChargeRecord DataRowToModel(DataRow row)
        {
            SMS.Model.ChargeRecord model = new SMS.Model.ChargeRecord();
            if (row != null)
            {
                if (row["OperatorAccount"] != null)
                {
                    model.OperatorAccount = row["OperatorAccount"].ToString();
                }
                if (row["PrepaidAccount"] != null)
                {
                    model.PrepaidAccount = row["PrepaidAccount"].ToString();
                }
                if (row["ThenRate"] != null && row["ThenRate"].ToString() != "")
                {
                    model.ThenRate = decimal.Parse(row["ThenRate"].ToString());
                }
                if (row["PrepaidType"] != null && row["PrepaidType"].ToString() != "")
                {
                    model.PrepaidType = (ushort)row["PrepaidType"];
                }
                if (row["Money"] != null && row["Money"].ToString() != "")
                {
                    model.Money = decimal.Parse(row["Money"].ToString());
                }
                if (row["SMSCount"] != null && row["SMSCount"].ToString() != "")
                {
                    model.SMSCount = int.Parse(row["SMSCount"].ToString());
                }
                if (row["RemainSMSCount"] != null && row["RemainSMSCount"].ToString() != "")
                {
                    model.RemainSMSCount = int.Parse(row["RemainSMSCount"].ToString());
                }
                if (row["PrepaidTime"] != null && row["PrepaidTime"].ToString() != "")
                {
                    model.PrepaidTime = DateTime.Parse(row["PrepaidTime"].ToString());
                }
                if (row["ChargeFlag"] != null && row["ChargeFlag"].ToString() != "")
                {
                    model.ChargeFlag = (ushort)row["ChargeFlag"];
                }
                if (row["Remark"] != null)
                {
                    model.Remark = row["Remark"].ToString();
                }
            }
            return model;
        }
    }
}
