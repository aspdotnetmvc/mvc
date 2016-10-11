using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Util
{
    public class MyCache
    {
        #region 单例
        private MyCache()
        {
            cache = new Dictionary<string, object>();
        }
        private static object locker = new object();
        private static MyCache _instance;
        public static MyCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new MyCache();
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion

        private Dictionary<string, Object> cache;
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Object Get(string key)
        {
            if (cache.ContainsKey(key))
            {
                return cache[key];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void Set(string key, Object val)
        {
            if (cache.ContainsKey(key))
            {
                cache[key] = val;
            }
            else
            {
                cache.Add(key, val);
            }
        }
        /// <summary>
        /// 判断是否包含某个key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return cache.ContainsKey(key);
        }
    }
}
