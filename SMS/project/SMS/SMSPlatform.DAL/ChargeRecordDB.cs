using MySql.Data.MySqlClient;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SMSPlatform.DAL
{
    public class ChargeRecordDB
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
            DBHelper.Instance.Execute(strSql.ToString(), model);
            return true;
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
            return DBHelper.Instance.Query<ChargeRecord>(strSql.ToString(), new { PrepaidAccount = userCode, StartTime = startTime, EndTime = endTime });
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static List<SMS.Model.ChargeStatics> GetChargeStatics()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select PrepaidAccount as Enterprese, sum(Money) TotalMoney,sum(SMSCount) SMSCount from plat_ChargeRecord ");
            strSql.Append(" where ChargeFlag=0 group by PrepaidAccount");
            return DBHelper.Instance.Query<ChargeStatics>(strSql.ToString());
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static List<SMS.Model.ChargeStatics> GetChargeStatics(DateTime StartDate, DateTime EndDate)
        {
            List<SMS.Model.ChargeStatics> list = new List<SMS.Model.ChargeStatics>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select PrepaidAccount Enterprese, sum(Money) TotalMoney,sum(SMSCount) SMSCount from plat_ChargeRecord ");
            strSql.Append(" where ChargeFlag=0 and PrepaidTime between @StartDate and @EndDate group by PrepaidAccount");
            return DBHelper.Instance.Query<ChargeStatics>(strSql.ToString(), new { StartDate = StartDate, EndDate = EndDate });
        }
    }
}
