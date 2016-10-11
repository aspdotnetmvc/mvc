using MySql.Data.MySqlClient;
using SMSModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DatabaseAccess
{
    public class NumsectDB
    {
        /// <summary>
        /// 获取运营商的号段
        /// </summary>
        /// <returns></returns>
        public static List<NumOperators> GetOperators()
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from Numsect ");

            List<NumOperators> list = new List<NumOperators>();
            DataSet ds = MySQLAccess.mySqlHelper.Instance.ExecuteDataset(strSql.ToString());
            if (ds.Tables.Count == 0) return list; 
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(DataRowToNumOperators(row));
                }
            }
            return list;
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        static NumOperators DataRowToNumOperators(DataRow row)
        {
            NumOperators numbers = new NumOperators();
            if (row != null)
            {
                if (row["ID"] != null && row["ID"].ToString() != "")
                {
                    numbers.ID = int.Parse(row["ID"].ToString());
                }
                if (row["Operators"] != null)
                {
                    numbers.Operators = row["Operators"].ToString();
                }
                if (row["NumberSec"] != null)
                {
                    numbers.NumberSect = row["NumberSec"].ToString();
                }
            }
            return numbers;
        }
    }
}
