using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Text.RegularExpressions;

namespace DBTools
{
    public class MySqlAdapter : DBAdapter
    {
        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="connString"></param>
        /// <returns></returns>
        public override IDbConnection OpenDatabase()
        {
            dbconn = new MySqlConnection();

            dbconn.ConnectionString = this.ConnectionString;
            dbconn.Open();

            return dbconn;
        }

        public override string GetPageSql(string sql, string orderBy = null)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return sql + " limit @start,@rows";
            }
            else
            {
                return sql + " order by " + orderBy + " limit @start,@rows";
            }
        }

    }
}