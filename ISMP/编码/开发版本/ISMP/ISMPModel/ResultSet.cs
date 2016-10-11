using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ISMPModel
{
    /// <summary>
    /// 查询结果集
    /// </summary>
    /// 
    [Serializable]
    public class ResultSet<T>
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public int Total { get; set; }

        public List<T> Value { get; set; }

        /// <summary>
        /// 转换为Json String
        /// 序列化为{total:0,rows:[{},{}]}的格式
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 单独把Value 序列化
        /// </summary>
        /// <returns></returns>
        public string ListToJson()
        {
            IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            return JsonConvert.SerializeObject(Value, Newtonsoft.Json.Formatting.Indented, timeFormat);
        }

    }
    /// <summary>
    /// ResultSet<T> 的特例，T 的类型是IDictionary<string, object>，用于弱类型集合查询。
    /// </summary>
    [Serializable]
    public class ResultSet : ResultSet<IDictionary<string, object>>
    {
        /// <summary>
        /// 获取一个ResultSet
        /// </summary>
        /// <param name="list"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static ResultSet GetResultSet(IEnumerable<dynamic> list, int? total = null)
        {
            var query = from m in list select CopyNew((IDictionary<string, object>)m);
            ResultSet rs = new ResultSet();
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
        /// 获取某行，某列的值
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="colName"></param>
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
    }
}
