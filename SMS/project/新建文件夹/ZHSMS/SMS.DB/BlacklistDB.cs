using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SMS.DB
{
    public class BlacklistDB
    {
        public static bool Add(List<string> numbers)
        {
            string sql = "insert into Blacklist(Number,Remark,AddTime) values (@Number,null,Now())";
            var BlackList = from num in numbers select new { Number = num };

            DBHelper.Instance.Execute(sql, BlackList);

            return true;
        }

        public static bool Del(List<string> numbers)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from Blacklist ");
            strSql.Append(" where Number=@Number");
            var BlackList = from num in numbers select new { Number = num };

            DBHelper.Instance.Execute(strSql.ToString(), BlackList);
            return true;
        }

        public static List<string> GetNumbers()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select Number from Blacklist ");

            return DBHelper.Instance.Query<string>(strSql.ToString());
        }

    }
}
