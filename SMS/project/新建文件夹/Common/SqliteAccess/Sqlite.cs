using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System;
using System.Data.Common;
using System.Threading;

namespace SqliteAccess
{
    public class Sqlite
    {
        public Sqlite(string connectionString)
        {
            _connString = connectionString;
        }

        string _connString;

        /// <summary>
        /// 获得连接对象
        /// </summary>
        /// <returns></returns>
        private SQLiteConnection GetSQLiteConnection()
        {
            return new SQLiteConnection(_connString);
        }

        public DataSet ExecuteDataset(string cmdText, params SQLiteParameter[] p)
        {
            DataSet ds = new DataSet();
            using (SQLiteConnection connection = GetSQLiteConnection())
            {
                connection.Open();
                using(SQLiteCommand command = new SQLiteCommand())
                {
                    command.Parameters.Clear();
                    command.Connection = connection;
                    command.CommandText = cmdText;
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 30;
                    if (p != null)
                    {
                        command.Parameters.AddRange(p);
                    }
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(command))
                    {
                        da.Fill(ds);
                    }
                }
            }
            return ds;
        }

        public DataRow ExecuteDataRow(string cmdText, params SQLiteParameter[] p)
        {
            DataSet ds = ExecuteDataset(cmdText, p);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            return null;
        }

        /// <summary>
        /// 返回受影响的行数
        /// </summary>
        /// <param name="cmdText">a</param>
        /// <param name="commandParameters">传入的参数</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string cmdText, params SQLiteParameter[] p)
        {
            int r = 0;
            using (SQLiteConnection connection = GetSQLiteConnection())
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Parameters.Clear();
                    command.Connection = connection;
                    command.CommandText = cmdText;
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 30;
                    if (p != null)
                    {
                        command.Parameters.AddRange(p);
                    }
                    SQLiteTransaction trans = connection.BeginTransaction();
                    
                    try
                    {
                        r = command.ExecuteNonQuery();
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                    }
                }
            }
            return r;
        }
        /// <summary>
        /// 返回SqlDataReader对象
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters">传入的参数</param>
        /// <returns></returns>
        public bool Exists(string cmdText, params SQLiteParameter[] p)
        {
            SQLiteDataReader reader;
            using (SQLiteConnection connection = GetSQLiteConnection())
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Parameters.Clear();
                    command.Connection = connection;
                    command.CommandText = cmdText;
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 30;
                    if (p != null)
                    {
                        command.Parameters.AddRange(p);
                    }
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                    return reader.HasRows; ;
                }
            }
        }
        /// <summary>
        /// 返回结果集中的第一行第一列，忽略其他行或列
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters">传入的参数</param>
        /// <returns></returns>
        public object ExecuteScalar(string cmdText, params SQLiteParameter[] p)
        {
            using (SQLiteConnection connection = GetSQLiteConnection())
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Parameters.Clear();
                    command.Connection = connection;
                    command.CommandText = cmdText;
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 30;
                    if (p != null)
                    {
                        command.Parameters.AddRange(p);
                    }
                    return command.ExecuteScalar();
                }
            }
        }
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="recordCount">返回记录总数</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="cmdText">查询记录SQL</param>
        /// <param name="countText">查询记录总数SQL</param>
        /// <param name="p">查询记录参数</param>
        /// <returns></returns>
        public DataSet ExecutePager(ref int recordCount, int pageIndex, int pageSize, string cmdText, string countText, params SQLiteParameter[] p)
        {
            if (recordCount < 0)
                recordCount = int.Parse(ExecuteScalar(countText, p).ToString());
            DataSet ds = new DataSet();

            using (SQLiteConnection connection = GetSQLiteConnection())
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Parameters.Clear();
                    command.Connection = connection;
                    command.CommandText = cmdText;
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 30;
                    if (p != null)
                    {
                        command.Parameters.AddRange(p);
                    }
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(command))
                    {
                        da.Fill(ds, (pageIndex - 1) * pageSize, pageSize, "result");
                    }
                }
            }
            return ds;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <paramname="path"></param>
        /// <param name="newPassword"></param>
        /// <param name="oldPassword">没有密码时为空</param>
        public bool ChangePassword(string newPassword, string oldPassword = null)
        {
            try
            {
                using (SQLiteConnection connection = GetSQLiteConnection())
                {
                    connection.Open();
                    connection.ChangePassword(newPassword);
                    connection.Close();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 查询数据库中的所有数据类型信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetSchema()
        {
            using (SQLiteConnection connection = GetSQLiteConnection())
            {
                connection.Open();
                DataTable data = connection.GetSchema("TABLES");
                data = connection.GetSchema("TABLES");
                connection.Close();
                return data;
            }
        }
    }
}