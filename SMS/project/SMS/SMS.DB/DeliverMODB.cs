using SMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SMS.DB
{
    public class DeliverMODB
    {
        public static bool Add(MOSMS mo)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into DeliverMO(");
            strSql.Append("AccountID,SPNumber,UserNumber,ReceiveTime,SerialNumber,Gateway,MessageContent,Service,Status)");
            strSql.Append(" values (");
            strSql.Append("@AccountID,@SPNumber,@UserNumber,@ReceiveTime,@SerialNumber,@Gateway,@Message,@Service,0)");
            DBHelper.Instance.Execute(strSql.ToString(), mo);
            return true;
        }

        public static List<MOSMS> Gets(string spNumber, DateTime beginTime, DateTime endTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select AccountID,SPNumber,UserNumber,ReceiveTime,SerialNumber,Gateway,MessageContent as Message,Service from DeliverMO ");
            strSql.Append(" where SPNumber=@SPNumber and ReceiveTime>=@BeginTime and ReceiveTime<=@EndTime");
            return DBHelper.Instance.Query<MOSMS>(strSql.ToString(), new { SPNumber = spNumber, BeginTime = beginTime, EndTime = endTime });
        }

        /// <summary>
        /// 获取MO 缓存
        /// </summary>
        /// <returns></returns>
        public static List<MOSMS> GetMOCache()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select AccountID,SPNumber,UserNumber,ReceiveTime,SerialNumber,Gateway,MessageContent as Message,Service from DeliverMO ");
            strSql.Append(" where ReceiveTime>=@BeginTime and Status=0");
            return DBHelper.Instance.Query<MOSMS>(strSql.ToString(), new { BeginTime = DateTime.Now.AddDays(-2) });
        }
        /// <summary>
        /// 更新获取状态
        /// </summary>
        /// <param name="mo"></param>
        public static void UpdateMOStatus(MOSMS mo)
        {
            string sql = "update DeliverMO set Status=1 where SerialNumber= @SerialNumber";
            DBHelper.Instance.Execute(sql, mo);
        }
    }
}
