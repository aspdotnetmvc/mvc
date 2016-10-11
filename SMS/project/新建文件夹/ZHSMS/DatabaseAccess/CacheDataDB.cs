using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DatabaseAccess
{
    public class CacheDataDB
    {
        public static bool Add(string space, string key, string content)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into CacheData(");
            strSql.Append("NSpace,NKey,NContent)");
            strSql.Append(" values (");
            strSql.Append("@Space,@Key,@Content)");
            DBHelper.Instance.Execute(strSql.ToString(), new { Space = space, Key = key, Content = content });

            return true;

        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public static bool Update(string space, string key, string content)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update CacheData set ");
            strSql.Append("NContent=@Content");
            strSql.Append(" where NSpace=@Space and NKey=@Key");
            DBHelper.Instance.Execute(strSql.ToString(), new { Space = space, Key = key, Content = content });

            return true;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public static bool Del(string space, string key)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from CacheData ");
            strSql.Append(" where NSpace=@Space and NKey=@Key");
            DBHelper.Instance.Execute(strSql.ToString(), new { Space = space, Key = key });

            return true;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public static bool Clear(string space)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from CacheData ");
            strSql.Append(" where NSpace=@Space");
            DBHelper.Instance.Execute(strSql.ToString(), new { Space = space });

            return true;
        }

        public static IEnumerable<dynamic> Get(string space)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from CacheData ");
            strSql.Append(" where NSpace=@Space");
            var list = DBHelper.Instance.Query(strSql.ToString(), new { Space = space });
            return list;
        }
    }
}
