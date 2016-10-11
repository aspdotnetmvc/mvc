using MySql.Data.MySqlClient;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DatabaseAccess
{
    public class LevelModifyRecordDB
    {
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static bool Add(LevelModifyRecord record)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into LevelModifyRecord(");
            strSql.Append("AccountID,ModifyTime,ModifyContent,SerialNumber,SendTime,SMSContent)");
            strSql.Append(" values (");
            strSql.Append("@AccountID,@ModifyTime,@ModifyContent,@SerialNumber,@SendTime,@Content)");
            DBHelper.Instance.Execute(strSql.ToString(), record);
            return true;
        }

        public static List<LevelModifyRecord> Get(Guid serialNumber)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select AccountID,ModifyTime,ModifyContent,SerialNumber,SendTime,SMSContent as Content from LevelModifyRecord ");
            strSql.Append(" where SerialNumber=@SerialNumber");
            return DBHelper.Instance.Query<LevelModifyRecord>(strSql.ToString(), new { SerialNumber = serialNumber });
        }

        public static List<LevelModifyRecord> Get(DateTime beginTime, DateTime endTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from LevelModifyRecord ");
            strSql.Append(" where ModifyTime>=@BeginTime and ModifyTime<=@EndTime");
            return DBHelper.Instance.Query<LevelModifyRecord>(strSql.ToString(), new { BeginTime = beginTime, EndTime = endTime });

        }
    }
}
