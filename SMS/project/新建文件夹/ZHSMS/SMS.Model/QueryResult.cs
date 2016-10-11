using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{/// <summary>
    /// 查询结果集
    /// </summary>
    /// 
    [Serializable]
    public class QueryResult<T>
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public int Total { get; set; }

        public List<T> Value { get; set; }

        public override string ToString()
        {
            IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            if (Value == null || Value.Count == 0)
            {
                return "{\"total\":0,\"rows\":[]}";
            }
            return JsonConvert.SerializeObject(new { total = this.Total, rows = Value }, Newtonsoft.Json.Formatting.Indented, timeFormat);
        }
    }
    /// <summary>
    /// ResultSet<T> 的特例，T 的类型是IDictionary<string, object>，用于弱类型集合查询。
    /// </summary>
    [Serializable]
    public class QueryResult : QueryResult<IDictionary<string, object>>
    {
        /// <summary>
        /// 获取一个ResultSet
        /// </summary>
        /// <param name="list"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static QueryResult GetResultSet(IEnumerable<dynamic> list, int? total = null)
        {
            var query = from m in list select CopyNew((IDictionary<string, object>)m);
            QueryResult rs = new QueryResult();
            rs.Value = query.ToList();
            if (total == null)
            {
                rs.Total = rs.Value.Count;
            }
            else
            {
                rs.Total = (int)total;
            }
            return rs;
        }
        private static IDictionary<string, object> CopyNew(IDictionary<string, object> row)
        {
            IDictionary<string, object> dic = new Dictionary<string, object>();
            foreach (var key in row.Keys)
            {
                dic.Add(key, row[key]);
            }
            return dic;
        }


        /// <summary>
        /// 获取value 某行某列的值
        /// </summary>
        /// <param name="index"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public string get(int rowIndex, string colName)
        {
            if (Value.Count > rowIndex)
            {
                var v = Value[rowIndex][colName];
                if (v == null) return string.Empty;
                else return v.ToString();
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// 获取某行 某列的值 object
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public object getObject(int rowIndex, string colName)
        {
            if (Value.Count > rowIndex)
            {
                return Value[rowIndex][colName];
            }
            else
            {
                return null;
            }
        }
    }
}
