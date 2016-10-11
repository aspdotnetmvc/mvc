using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Text.RegularExpressions;


namespace DBTools
{
    public abstract class DBAdapter
    {
        #region  事务和连接
        public string ConnectionString
        {
            get;
            set;
        }
        protected IDbConnection dbconn;
        /// <summary>
        /// 获取打开的连接
        /// </summary>
        public IDbConnection Connection
        {
            get { return dbconn; }
        }
        private IDbTransaction dbtran;
        /// <summary>
        /// 获取打开的事务
        /// </summary>
        protected IDbTransaction Transaction
        {
            get { return dbtran; }
            set { dbtran = value; }
        }

        #region  打开连接 抽象方法
        /// <summary>
        /// 打开连接
        /// </summary>
        /// <returns></returns>
        public abstract IDbConnection OpenDatabase();
        #endregion
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        /// <returns></returns>
        public void CloseDatabase()
        {
            try
            {
                if (dbconn != null)
                {
                    if (dbconn.State != ConnectionState.Closed)
                    {
                        dbconn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// 检查数据库是否已连接，若未连接则连接。
        /// </summary>
        private void confirmConnection()
        {
            if (dbconn == null || dbconn.State != ConnectionState.Open)
            {
                if (string.IsNullOrWhiteSpace(ConnectionString))
                {
                    throw new Exception("数据库连接未设置！");
                }
                else
                {
                    OpenDatabase();
                }
            }
        }
        /// <summary>
        /// 打开事务
        /// </summary>
        /// <returns></returns>
        public IDbTransaction BeginTransaction()
        {
            confirmConnection();
            dbtran = dbconn.BeginTransaction();
            return dbtran;
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit()
        {
            dbtran.Commit();
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void Rollback()
        {
            try
            {
                dbtran.Rollback();
            }
            catch { }
           
        }
        #endregion


        #region sql 处理
        public abstract string GetPageSql(string sql, string orderBy);
        public string GetCountSql(string sql)
        {
            return "select count(1) from (" + sql + ") tab_base";
        }
        #endregion
    }
}