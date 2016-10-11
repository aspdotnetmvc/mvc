using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;
using System.Threading;
using BXM.DatabaseHelper;

namespace MySQLAccess
{
    public class mySqlHelper
    {
        private volatile static mySqlHelper _instance = null;
        private static readonly object lockHelper = new object();
        public static mySqlHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockHelper)
                    {
                        if (_instance == null)
                            _instance = new mySqlHelper();
                    }
                }
                return _instance;
            }
        }

        public MySqlTransaction CreateTransaction()
        {
            return (MySqlTransaction)MySQLHelper.Instance.CreateTransaction();
        }

        public void FreeTransaction(MySqlTransaction transaction)
        {
            MySQLHelper.Instance.FreeTransaction(transaction);
        }

        public int ExecuteNonQuery(MySqlTransaction transaction, string commandText, params MySqlParameter[] commandParameters)
        {
            return MySQLHelper.Instance.ExecuteNonQuery(transaction, commandText, commandParameters);
        }

        public int ExecuteNonQuery(MySqlTransaction transaction, string commandText)
        {
            return ExecuteNonQuery(transaction, commandText, null);
        }

        public int ExecuteNonQuery(string commandText, params MySqlParameter[] commandParameters)
        {
            return MySQLHelper.Instance.ExecuteNonQuery(commandText, commandParameters);
        }

        public int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(commandText, null);
        }

        public object[] ExecuteReaderRow(string commandText, params MySqlParameter[] commandParameters)
        {
            return MySQLHelper.Instance.ExecuteReaderRow(commandText, commandParameters);
        }

        public bool Exists(string commandText, MySqlParameter[] commandParameters)
        {
            return MySQLHelper.Instance.Exists(commandText, commandParameters);
        }

        public DataSet ExecuteDataset(string commandText, params MySqlParameter[] commandParameters)
        {
            return MySQLHelper.Instance.ExecuteDataset(commandText, commandParameters);
        }

        public DataSet ExecuteDataset(string commandText)
        {
            return ExecuteDataset(commandText, null);
        }
    }
}