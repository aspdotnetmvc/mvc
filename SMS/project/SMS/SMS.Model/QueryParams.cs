using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{/// <summary>
    /// 查询时的参数
    /// </summary>
    [Serializable]
    public class QueryParams
    {
        #region private
        private Dictionary<string, object> vals = new Dictionary<string, object>();
        private int _page = 1;
        private int _rows = 50;
        private bool _ispage = true;
        #endregion

        /// <summary>
        /// 要取得页码 从1开始
        /// </summary>
        public int page
        {
            get
            {
                return _page;
            }
            set
            {
                _page = value;
            }
        }
        /// <summary>
        /// 每页记录数,默认10000 ,即取全部数据
        /// </summary>
        public int rows
        {
            get
            {
                return _rows;
            }
            set
            {
                _rows = value;
            }
        }

        /// <summary>
        /// 获取参数值 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object get(string key)
        {
            if (vals.ContainsKey(key))
            {
                return vals[key];
            }
            else
            {
                return string.Empty;
            }
        }

        public Dictionary<string, object> getALL()
        {
            return this.vals;
        }
        /// <summary>
        /// 添加或设置一个参数,重复设置以最后一个为准
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public QueryParams add(string key, object val)
        {
            if (key == "page")
            {
                this.page = int.Parse(val.ToString());
                return this;
            }
            else if (key == "rows")
            {
                this.rows = int.Parse(val.ToString());
                return this;
            }

            if (vals.ContainsKey(key))
            {
                vals.Remove(key);
            }
            vals.Add(key, val);
            return this;
        }


        /// <summary>
        /// 是否分页查询
        /// </summary>
        public bool ispage
        {
            get
            {
                return _ispage;
            }
            set
            {
                _ispage = value;
            }
        }
    }
}
