using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Dapper;


namespace ISMPModel
{
    /// <summary>
    /// 查询时的参数
    /// </summary>
    [Serializable]
    public class ParamList
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
        /// 分页开始记录
        /// </summary>
        private int start
        {
            get
            {
                return (page - 1) * rows;
            }
        }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string orderby
        {
            get;
            set;
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

        /// <summary>
        /// 获取参数值并转化为String
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string str(string key)
        {
            if (this.isnotnull(key))
            {
                return this.get(key).ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 添加或设置一个参数,重复设置以最后一个为准
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public ParamList add(string key, object val)
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
            else if (key == "orderby")
            {
                this.orderby = val.ToString();
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
        /// 把一个类型的值加入到参数表中
        /// </summary>
        /// <param name="model"></param>
        public ParamList add(dynamic model)
        {
            Type t = model.GetType();
            foreach (var field in t.GetProperties())
            {
                var ft = field.PropertyType;
                if (ft.IsPrimitive || ft == typeof(DateTime) || ft == typeof(String) || ft == typeof(Decimal) || ft.IsEnum || Tools.IsNullableType(ft))
                {
                    this.add(field.Name, field.GetValue(model, null));
                }
            }
            return this;
        }
        /// <summary>
        /// 判断不空，拼接查询条件时使用
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool isnotnull(string key)
        {
            object obj = this.get(key);
            if (obj == null)
            {
                return false;
            }
            return !string.IsNullOrWhiteSpace(obj.ToString());
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

        /// <summary>
        /// 查询参数
        /// </summary>
        public DynamicParameters Parameters
        {
            get
            {
                DynamicParameters parameters = new DynamicParameters();
                if (ispage)
                {
                    parameters.Add("start", this.start);
                    parameters.Add("rows", this.rows);
                }

                foreach (var key in vals.Keys)
                {
                    parameters.Add(key, this.get(key));
                }
                return parameters;
            }
        }
    }
}
