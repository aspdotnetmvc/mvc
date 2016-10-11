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
    public class SqliteA
    {
        public SqliteA(string connectionString)
        {
            _sc = new SQLiteConnection(connectionString);
            _sc.Open();
        }

        private ReaderWriterLockSlim _rwlock = new ReaderWriterLockSlim();
        SQLiteConnection _sc;
        SQLiteTransaction _trans;

        /// <summary>
        /// 获得连接对象
        /// </summary>
        /// <returns></returns>
        private SQLiteConnection GetSQLiteConnection()
        {
            if (_sc.State == ConnectionState.Closed)
            {
                _sc.Open();
                while (true)
                {
                    if (_sc.State == ConnectionState.Open) break;
                }
            }
            return _sc;
        }

        public DataSet ExecuteDataset(string cmdText, params SQLiteParameter[] p)
        {
            _rwlock.EnterReadLock();
            DataSet ds = new DataSet();
            SQLiteConnection connection = GetSQLiteConnection();
            {
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
                        try
                        {
                            da.Fill(ds);
                        }
                        finally
                        {
                            _rwlock.ExitReadLock();
                        }
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

        public void BeginTransaction()
        {
            _trans = _sc.BeginTransaction();
        }

        public void Commit()
        {
            _trans.Commit();
        }

        public void Rollback()
        {
            _trans.Rollback();
        }
        /// <summary>
        /// 返回受影响的行数
        /// </summary>
        /// <param name="cmdText">a</param>
        /// <param name="commandParameters">传入的参数</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string cmdText, params SQLiteParameter[] p)
        {
            _rwlock.EnterWriteLock();
            int r = 0;
            SQLiteConnection connection = GetSQLiteConnection();
            {
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
                    
                    try
                    {
                        r = command.ExecuteNonQuery();
                        
                    }
                    catch
                    {
                        
                    }
                    finally
                    {
                        _rwlock.ExitWriteLock();
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
            _rwlock.EnterReadLock();
            SQLiteDataReader reader;
            SQLiteConnection connection = GetSQLiteConnection();
            {
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
                    try
                    {
                        reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                    }
                    finally
                    {
                        _rwlock.ExitReadLock();
                    }
                }
                return reader.HasRows;;
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
            _rwlock.EnterReadLock();
            SQLiteConnection connection = GetSQLiteConnection();
            {
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
                    try
                    {
                        return command.ExecuteScalar();
                    }
                    finally
                    {
                        _rwlock.ExitReadLock();
                    }
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
            _rwlock.EnterReadLock();
            if (recordCount < 0)
                recordCount = int.Parse(ExecuteScalar(countText, p).ToString());
            DataSet ds = new DataSet();

            SQLiteConnection connection = GetSQLiteConnection();
            {
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
                        try
                        {
                            da.Fill(ds, (pageIndex - 1) * pageSize, pageSize, "result");
                        }
                        finally
                        {
                            _rwlock.ExitReadLock();
                        }
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
            _rwlock.EnterWriteLock();
            try
            {
                SQLiteConnection connection = GetSQLiteConnection();
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
            finally
            {
                _rwlock.ExitWriteLock();
            }
            return true;
        }

        /// <summary>
        /// 查询数据库中的所有数据类型信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetSchema()
        {
            _rwlock.EnterReadLock();
            SQLiteConnection connection = GetSQLiteConnection();
            {
                DataTable data = connection.GetSchema("TABLES");
                try
                {
                    data = connection.GetSchema("TABLES");
                }
                finally
                {
                    _rwlock.ExitReadLock();
                }
                connection.Close();
                return data;
            }
        }
    }
}