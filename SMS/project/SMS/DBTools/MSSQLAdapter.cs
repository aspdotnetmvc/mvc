using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections;
using System.Text.RegularExpressions;

namespace DBTools
{
    public class MSSQLAdapter : DBAdapter
    {
        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="connString"></param>
        /// <returns></returns>
        public override IDbConnection OpenDatabase()
        {
            dbconn = new SqlConnection();

            dbconn.ConnectionString = this.ConnectionString;
            dbconn.Open();

            return dbconn;
        }

        public override string GetPageSql(string sql, string orderBy = null)
        {
            throw new NotImplementedException();
        }
        public override IDataAdapter GetDataAdapter(IDbCommand cmd)
        {
            return new SqlDataAdapter((SqlCommand)cmd);
        }


    }
}