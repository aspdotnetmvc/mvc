using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBTools
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
