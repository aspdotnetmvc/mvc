using DBTools;
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.DB
{
    public class DBHelper : DBTools.DBHelper
    {
        public override string ConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["mysql"].ConnectionString;
            }
        }

        public override DBTools.DataBaseType DBType
        {
            get { return DBTools.DataBaseType.MySql; }
        }
        private static DBHelper _Instance = new DBHelper();
        public static DBHelper Instance
        {
            get
            {
                return _Instance;
            }
        }

        /// <summary>
        /// 把程序中传递查询参数的对象转为DBTools 里面的查询对象
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public override DBTools.ParamList GetParamList(object query)
        {
            SMS.Model.QueryParams qp = (SMS.Model.QueryParams)query;
            DBTools.ParamList pl = new DBTools.ParamList();
            pl.ispage = qp.ispage;
            pl.page = qp.page;
            pl.rows = qp.rows;
            pl.addRange(qp.getALL());
            return pl;
        }

                /// <summary>
        /// 把ResultSet 转为Model 中的QueryResult ,使dbtools 和其他项目解耦
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rs"></param>
        /// <returns></returns>
        public QueryResult ToQueryResult(ResultSet rs)
        {
            QueryResult qr = new QueryResult();
            qr.Value = rs.Value;
            qr.Total = rs.Total;
            return qr;
        }
        /// <summary>
        /// 把ResultSet 转为Model 中的QueryResult ,使dbtools 和其他项目解耦
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rs"></param>
        /// <returns></returns>
        public QueryResult<T> ToQueryResult<T>(ResultSet<T> rs)
        {
            QueryResult<T> qr = new QueryResult<T>();
            qr.Value = rs.Value;
            qr.Total = rs.Total;
            return qr;
        }

        public static string NewID()
        {
            return System.Guid.NewGuid().ToString();
        }
        public static string EnsureID(BaseModel model){
            if (string.IsNullOrWhiteSpace(model.ID))
            {
                model.ID = NewID();
            }
            return model.ID;
        }
    }
}
