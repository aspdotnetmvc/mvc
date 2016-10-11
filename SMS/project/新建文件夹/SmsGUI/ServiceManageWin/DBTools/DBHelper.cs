using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using Dapper;

namespace DBTools
{
    /// <summary>
    /// 执行sql ，抛出异常
    /// </summary>
    public class DBHelper
    {
        #region Virtual
        public virtual string ConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["MySql"];
            }
        }
        public virtual DataBaseType DBType
        {
            get
            {
                return DataBaseType.MySql;
            }
        }
        /// <summary>
        /// 生成Paramlist ,用于给DBTools 和前台项目解耦
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual ParamList GetParamList(Object query){
            return new ParamList();
        }

        #endregion

        #region 获取DBAdapter
        public DBAdapter GetDBAdapter()
        {
            return DBTools.DBFactory.GetAdapter(DBType, ConnectionString);
        }

        #endregion

        #region 查询
        public List<dynamic> Query(string sql, object query = null, IDbTransaction tran = null)
        {
            DBAdapter adapter = null; ;

            try
            {
                if (tran == null)
                {
                    adapter = GetDBAdapter();
                }
                var conn = tran != null ? tran.Connection : adapter.OpenDatabase();
                var m = conn.Query(sql, query).ToList();
                return m;
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                if (adapter != null)
                {
                    adapter.CloseDatabase();
                }
            }
        }

        public List<T> Query<T>(string sql, object query = null, IDbTransaction tran = null)
        {
            DBAdapter adapter = null; ;

            try
            {
                if (tran == null)
                {
                    adapter = GetDBAdapter();
                }
                var conn = tran != null ? tran.Connection : adapter.OpenDatabase();
                var m = conn.Query<T>(sql, query).ToList<T>();
                return m;
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                if (adapter != null)
                {
                    adapter.CloseDatabase();
                }
            }
        }

        public T ExecuteScalar<T>(string sql, object query = null, IDbTransaction tran = null)
        {
            DBAdapter adapter = null; ;

            try
            {
                if (tran == null)
                {
                    adapter = GetDBAdapter();
                }
                var conn = tran != null ? tran.Connection : adapter.OpenDatabase();
                var m = conn.ExecuteScalar<T>(sql, query, tran);
                return m;
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                if (adapter != null)
                {
                    adapter.CloseDatabase();
                }
            }
        }

        #endregion

        #region Result

        /// <summary>
        /// 返回一个ResultSet  查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pl"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public ResultSet<T> GetResultSet<T>(string sql, string OrderBy, ParamList pl, IDbTransaction tran = null)
        {
            DBAdapter adapter = GetDBAdapter();

            try
            {
                var conn = adapter.OpenDatabase();
                if (pl.ispage)
                {
                    string pagesql = adapter.GetPageSql(sql, OrderBy);
                    string countsql = adapter.GetCountSql(sql);

                    var m = conn.Query<T>(pagesql, pl.Parameters, tran).ToList<T>();
                    var c = conn.ExecuteScalar<int>(countsql, pl.Parameters, tran);
                    return new ResultSet<T>() { Value = m, Total = c };
                }
                else
                {
                    var m = conn.Query<T>(sql + " order by " + OrderBy, pl.Parameters, tran).ToList<T>();
                    return new ResultSet<T>() { Value = m, Total = m.Count };
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                adapter.CloseDatabase();
            }
        }

        /// <summary>
        /// 返回一个ResultSet  查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="OrderBy"></param>
        /// <param name="pl"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public ResultSet GetResultSet(string sql, string OrderBy, ParamList pl, IDbTransaction tran = null)
        {
            DBAdapter adapter = GetDBAdapter();

            try
            {
                var conn = adapter.OpenDatabase();
                if (pl.ispage)
                {
                    string pagesql = adapter.GetPageSql(sql, OrderBy);
                    string countsql = adapter.GetCountSql(sql);

                    var m = conn.Query(pagesql, pl.Parameters, tran);
                    var c = conn.ExecuteScalar<int>(countsql, pl.Parameters, tran);
                    return ResultSet.GetResultSet(m, c);
                }
                else
                {
                    var m = conn.Query(sql + " order by " + OrderBy, pl.Parameters, tran);
                    return ResultSet.GetResultSet(m);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                adapter.CloseDatabase();
            }
        }
        #endregion

        #region 执行sql
        /// <summary>
        /// 执行sql，返回受影响记录数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="model"></param>
        /// <param name="tran"></param>
        public int Execute(string sql, object model = null, IDbTransaction tran = null)
        {
            DBAdapter adapter = null; ;

            try
            {
                if (tran == null)
                {
                    adapter = GetDBAdapter();
                }
                var conn = tran != null ? tran.Connection : adapter.OpenDatabase();
                return conn.Execute(sql, model, tran);

            }
            catch (Exception ex) { throw ex; }
            finally
            {
                if (adapter != null)
                {
                    adapter.CloseDatabase();
                }
            }
        }
        /// <summary>
        /// 执行sql，返回受影响条记录数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pl"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public int Execute(string sql, ParamList pl, IDbTransaction tran = null)
        {
            DBAdapter adapter = null; ;

            try
            {
                if (tran == null)
                {
                    adapter = GetDBAdapter();
                }
                var conn = tran != null ? tran.Connection : adapter.OpenDatabase();
                return conn.Execute(sql, pl.Parameters, tran);

            }
            catch { throw; }
            finally
            {
                if (adapter != null)
                {
                    adapter.CloseDatabase();
                }
            }
        }




        /// <summary>
        /// 同一个sql语句，批量操作多个对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="models"></param>
        /// <param name="tran"></param>
        public int Execute(string sql, IEnumerable<object> models, IDbTransaction tran = null)
        {
            DBAdapter adapter = null; ;

            try
            {
                if (tran == null)
                {
                    adapter = GetDBAdapter();
                }
                var conn = tran != null ? tran.Connection : adapter.OpenDatabase();
                return conn.Execute(sql, models, tran);
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                if (adapter != null)
                {
                    adapter.CloseDatabase();
                }
            }
        }

        /// <summary>
        /// 使用事务批量执行多个语句
        /// </summary>
        /// <param name="sqllist"></param>
        /// <param name="pl"></param>
        /// <returns></returns>
        public void Execute(List<string> sqllist, object model = null, IDbTransaction tran = null)
        {
            DBAdapter adapter = null; ;

            try
            {
                if (tran == null)
                {
                    adapter = GetDBAdapter();
                    tran = adapter.BeginTransaction();
                }
                var conn = tran.Connection;

                foreach (var sql in sqllist)
                {
                    conn.Execute(sql, model, tran);
                }
                if (adapter != null)
                {
                    adapter.Commit();
                }
            }
            catch
            {
                if (adapter != null)
                {
                    adapter.Rollback();
                }
                throw;
            }
            finally
            {
                if (adapter != null)
                {
                    adapter.CloseDatabase();
                }
            }

        }
        /// <summary>
        /// 使用事务批量执行sql语句
        /// </summary>
        /// <param name="sqllist"></param>
        /// <param name="pl"></param>
        /// <param name="tran"></param>
        public void Execute(List<string> sqllist, ParamList pl, IDbTransaction tran = null)
        {
            DBAdapter adapter = null; ;

            try
            {
                if (tran == null)
                {
                    adapter = GetDBAdapter();
                    tran = adapter.BeginTransaction();
                }
                var conn = tran.Connection;

                foreach (var sql in sqllist)
                {
                    conn.Execute(sql, pl.Parameters, tran);
                }
                if (adapter != null)
                {
                    adapter.Commit();
                }
            }
            catch
            {
                if (adapter != null)
                {
                    adapter.Rollback();
                }
                throw;
            }
            finally
            {
                if (adapter != null)
                {
                    adapter.CloseDatabase();
                }
            }

        }
        #endregion

    }
}
