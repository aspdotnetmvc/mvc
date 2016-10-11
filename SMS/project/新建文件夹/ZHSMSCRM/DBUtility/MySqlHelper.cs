using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;
using System.Threading;

namespace DBUtility
{
    public class MySqlHelper
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString;

        public static bool Exists(string sql, params MySqlParameter[] parameters)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, sql, parameters);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return false;
                        }
                        else
                        {
                            if (Convert.ToInt32(obj) > 0)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    catch (MySqlException e)
                    {
                        //throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
            return false;
        }

        public static MySqlTransaction CreateTransaction()
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            return conn.BeginTransaction();
        }

        public static int ExecuteNonQuery(MySqlTransaction transaction, string sql, params MySqlParameter[] parameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            PrepareCommand(cmd, transaction.Connection, transaction, sql, parameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        public static int ExecuteNonQuery(string sql, params MySqlParameter[] parameters)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, sql, parameters);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (MySqlException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteInsertImage(string sql, byte[] fs)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    MySqlParameter myParameter = new MySqlParameter("@fs", SqlDbType.Image);
                    myParameter.Value = fs;
                    cmd.Parameters.Add(myParameter);
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (MySqlException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        public static object[] ExecuteReader(string sql)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    try
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        MySqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        object[] rows = null;
                        if (myReader.Read())
                        {
                            rows = new object[myReader.FieldCount];
                            myReader.GetValues(rows);
                        }
                        myReader.Close();
                        return rows;
                    }
                    catch (MySqlException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        public static object[] ExecuteReader(string sql, params MySqlParameter[] parameters)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, sql, parameters);
                        MySqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        object[] rows = null;
                        if (myReader.Read())
                        {
                            rows = new object[myReader.FieldCount];
                            myReader.GetValues(rows);
                        }
                        myReader.Close();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (MySqlException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }
        public static DataSet Query(string sql)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    using (MySqlDataAdapter command = new MySqlDataAdapter(sql, connection))
                    {
                        command.Fill(ds, "ds");
                    }
                }
                catch (MySqlException ex)
                {
                    //throw new Exception(ex.Message);
                }
                return ds;
            }
        }
        public static DataSet Query(string sql, params MySqlParameter[] parameters)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {

                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        PrepareCommand(cmd, connection, null, sql, parameters);
                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                        {
                            da.Fill(ds, "ds");
                            cmd.Parameters.Clear();
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    //throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }

                return ds;
            }
        }

        static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string sql, MySqlParameter[] parameters)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            if (parameters != null)
            {
                foreach (MySqlParameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        //List<MySqlConnection> reads;
        //List<MySqlConnection> writes;
        //int activateRead = 0;
        //int activateWrite = 0;

        //private volatile static MySqlHelper _instance;
        //private static object lockHelper = new object();

        //private MySqlHelper()
        //{
        //    reads = new List<MySqlConnection>();
        //    writes = new List<MySqlConnection>();

        //    foreach (ConnectionStringSettings c in ConfigurationManager.ConnectionStrings)
        //    {
        //        if (c.Name.Substring(0, 5) == "write")
        //        {
        //            writes.Add(new MySqlConnection(c.ConnectionString));
        //            try
        //            {
        //                writes[writes.Count - 1].Open();
        //            }
        //            catch
        //            {
        //            }
        //        }
        //        if (c.Name.Substring(0, 4) == "read")
        //        {
        //            reads.Add(new MySqlConnection(c.ConnectionString));
        //            try
        //            {
        //                reads[reads.Count - 1].Open();
        //            }
        //            catch
        //            {
        //            }
        //        }
        //    }
        //}

        //internal static MySqlHelper Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //        {
        //            lock (lockHelper)
        //            {
        //                if (_instance == null)
        //                {
        //                    _instance = new MySqlHelper();
        //                    return _instance;
        //                }
        //            }
        //        }
        //        return _instance;
        //    }
        //}

        //internal MySqlConnection GetWriteConnection()
        //{
        //    if (writes[activateWrite].Ping()) return writes[activateWrite];

        //    for (int r = 0; r < 3; r++)
        //    {
        //        for (int i = 0; i < writes.Count; i++)
        //        {
        //            if (writes[i].Ping())
        //            {
        //                activateWrite = i;
        //                return writes[i];
        //            }
        //            else
        //                if (writes[i].State == ConnectionState.Closed)
        //                {
        //                    try
        //                    {
        //                        writes[i].Open();
        //                    }
        //                    catch
        //                    {
        //                    }
        //                }
        //        }
        //        Thread.Sleep(1000);
        //    }
        //    throw new Exception("Database connection error.");
        //}

        //internal MySqlConnection GetReadConnection()
        //{
        //    if (reads[activateRead].Ping()) return reads[activateRead];
        //    for (int r = 0; r < 3; r++)
        //    {
        //        for (int i = 0; i < reads.Count; i++)
        //        {
        //            if (reads[i].Ping())
        //            {
        //                activateRead = i;
        //                return reads[i];
        //            }
        //            else
        //                if (reads[i].State == ConnectionState.Closed)
        //                {
        //                    try
        //                    {
        //                        reads[i].Open();
        //                    }
        //                    catch
        //                    {
        //                    }
        //                }
        //        }
        //        Thread.Sleep(1000);
        //    }

        //    throw new Exception("Database connection error.");
        //}

        //public static MySqlTransaction CreateTransaction()
        //{
        //    return MySqlHelper.Instance.GetWriteConnection().BeginTransaction();
        //}

        //public static int ExecuteNonQuery(MySqlTransaction transaction, string sql, params MySqlParameter[] parameters)
        //{
        //    MySqlCommand cmd = new MySqlCommand();
        //    cmd.CommandText = sql;
        //    cmd.CommandType = CommandType.Text;

        //    if (transaction != null)
        //    {
        //        cmd.Connection = transaction.Connection;
        //        cmd.Transaction = transaction;
        //    }
        //    else
        //    {
        //        cmd.Connection = MySqlHelper.Instance.GetWriteConnection();
        //    }

        //    if (parameters != null)
        //    {
        //        foreach (MySqlParameter parms in parameters)
        //        {
        //            cmd.Parameters.Add(parms);
        //        }
        //    }
        //    int r = cmd.ExecuteNonQuery();
        //    cmd.Parameters.Clear();
        //    return r;
        //}

        //public static int ExecuteNonQuery(string sql, params MySqlParameter[] parameters)
        //{
        //    MySqlCommand cmd = new MySqlCommand();
        //    cmd.Connection = MySqlHelper.Instance.GetWriteConnection();
        //    cmd.CommandText = sql;
        //    cmd.CommandType = CommandType.Text;

        //    if (parameters != null)
        //    {
        //        foreach (MySqlParameter parms in parameters)
        //        {
        //            cmd.Parameters.Add(parms);
        //        }
        //    }
        //    int r = cmd.ExecuteNonQuery();
        //    cmd.Parameters.Clear();
        //    return r;

        //}

        //public static object[] ExecuteReaderRow(string sql, params MySqlParameter[] parameters)
        //{
        //    MySqlCommand cmd = new MySqlCommand();
        //    cmd.CommandText = sql;
        //    cmd.CommandType = CommandType.Text;
        //    cmd.Connection = MySqlHelper.Instance.GetReadConnection();

        //    if (parameters != null)
        //    {
        //        foreach (MySqlParameter parms in parameters)
        //        {
        //            cmd.Parameters.Add(parms);
        //        }
        //    }

        //    MySqlDataReader dr = null;
        //    object[] rows = null;
        //    dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
        //    if (dr.Read())
        //    {
        //        rows = new object[dr.FieldCount];
        //        dr.GetValues(rows);
        //        dr.Close();
        //    }
        //    cmd.Parameters.Clear();
        //    return rows;
        //}

        //public static bool Exists(string sql, MySqlParameter[] parameters)
        //{
        //    MySqlCommand cmd = new MySqlCommand();
        //    cmd.Connection = MySqlHelper.Instance.GetReadConnection();
        //    cmd.CommandText = sql;
        //    cmd.CommandType = CommandType.Text;

        //    if (parameters != null)
        //    {
        //        foreach (MySqlParameter parms in parameters)
        //        {
        //            cmd.Parameters.Add(parms);
        //        }
        //    }
        //    if(cmd.ExecuteScalar() != null) 
        //    {
        //        cmd.Parameters.Clear();
        //        return true;
        //    }
        //    else
        //    {
        //        cmd.Parameters.Clear();
        //        return false;
        //    }
        //}

        //public static DataSet Query(string sql)
        //{
        //    DataSet ds = new DataSet();
        //    MySqlCommand cmd = new MySqlCommand();
        //    cmd.Connection = MySqlHelper.Instance.GetReadConnection();
        //    cmd.CommandText = sql;
        //    cmd.CommandType = CommandType.Text;

        //    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
        //    {
        //        da.Fill(ds, "ds");
        //        cmd.Parameters.Clear();
        //        return ds;
        //    }
        //}

        //public static DataSet Query(string sql, params MySqlParameter[] parameters)
        //{
        //    DataSet ds = new DataSet();
        //    MySqlCommand cmd = new MySqlCommand();
        //    cmd.Connection = MySqlHelper.Instance.GetReadConnection();
        //    cmd.CommandText = sql;
        //    cmd.CommandType = CommandType.Text;

        //    if (parameters != null)
        //    {
        //        foreach (MySqlParameter parms in parameters)
        //        {
        //            cmd.Parameters.Add(parms);
        //        }
        //    }

        //    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
        //    {
        //        da.Fill(ds, "ds");
        //        cmd.Parameters.Clear();
        //        return ds;
        //    }
        //}
    }
}