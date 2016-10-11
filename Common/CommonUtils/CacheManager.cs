using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BXM.Utils
{
    public delegate void ExpireCacheHandler<T>(List<T> caches);

    public sealed class CacheManager<T> : IDisposable
    {
        private class Cache<CT>
        {
            public CT Content { get; set; }
            public long Timestamp { get; set; }
        }

        private volatile static CacheManager<T> mng = null;
        private static object lockHelper = new object();

        int _expireTime = 60 * 30;
        bool _expireRemoveCache = true;
        public event ExpireCacheHandler<T> ExpireCache;
        /// <summary>
        /// 检测毫秒
        /// </summary>
        public double CheckMillisecond
        {
            get { return _timer.Interval; }
            set { _timer.Interval = value; }
        }
        /// <summary>
        /// 到期毫秒
        /// </summary>
        public int ExpireMillisecond
        {
            get { return _expireTime; }
            set { _expireTime = value; }
        }
        /// <summary>
        /// 过期清除Caceh
        /// </summary>
        public bool ExpireRemoveCache
        {
            get
            {
                return _expireRemoveCache;
            }

            set
            {
                _expireRemoveCache = value;
            }
        }

        Dictionary<string, Cache<T>> _cache = new Dictionary<string, Cache<T>>();
        System.Timers.Timer _timer = new System.Timers.Timer(1000 * 60);


        private CacheManager()
        {
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            long nowTicks = DateTime.Now.Ticks;
            List<T> expireCache = new List<T>();
            List<string> ks = new List<string>();
            lock (lockHelper)
            {
                foreach (string k in _cache.Keys)
                {
                    //10000000 -> 计算秒
                    //计算时间间隔是否大于某毫秒
                    if ((nowTicks - _cache[k].Timestamp) / 10000 > _expireTime)
                    {
                        expireCache.Add(_cache[k].Content);
                        ks.Add(k);
                    }
                }
            }
            if (_expireRemoveCache && expireCache.Count > 0)
            {
                foreach (string k in ks)
                {
                    _cache.Remove(k);
                }
            }
            if (ExpireCache!=null && expireCache.Count >0)
            {
                ExpireCache(expireCache);
            }
        }

        /// <summary>
        /// 让所有缓存过期并触发过期缓存事件
        /// </summary>
        public void TriggerExpireCache()
        {
            List<T> expireCache = new List<T>();
            foreach (string k in _cache.Keys)
            {
                expireCache.Add(_cache[k].Content);
            }
            _cache.Clear();
            if (ExpireCache != null && expireCache.Count > 0)
            {
                ExpireCache(expireCache);
            }
        }

        public int Count
        {
            get
            {
                return _cache.Count;
            }
        }
        public T Get(string key)
        {
            Cache<T> c = null;
            _cache.TryGetValue(key, out c);
            if (c != null)
            {
                c.Timestamp = DateTime.Now.Ticks;
                return c.Content;
            }
            return default(T);
        }

        public bool Add(string key, T content)
        {
            if (_cache.ContainsKey(key))
            {
                return false;
            }
            Cache<T> c = new Cache<T>();
            c.Timestamp = DateTime.Now.Ticks;
            c.Content = content;
            lock (lockHelper)
            {
                _cache.Add(key, c);
            }
            return true;
        }

        public bool ContainsKey(string key)
        {
            lock(lockHelper)
            {
                return _cache.ContainsKey(key);
            }
        }

        public bool Set(string key, T content)
        {
            Cache<T> c;
            _cache.TryGetValue(key, out c);
            if (c == null)
            {
                c = new Cache<T>();
                c.Timestamp = DateTime.Now.Ticks;
                c.Content = content;
                lock (lockHelper)
                {
                    _cache.Add(key, c);
                }
            }
            else
            {
                c.Content = content;
            }

            return true;
        }

        public List<T> GetAll()
        {
            List<T> cs = new List<T>();
            lock (lockHelper)
            {
                foreach (var c in _cache.Values)
                {
                    cs.Add(c.Content);
                }
            }
            return cs;
        }

        public List<T> Query(Func<T, bool> query)
        {
            List<T> cs = new List<T>();
            var t = _cache.Values.Cast<T>().Where<T>(query);
            cs.AddRange(t);
            return cs;
        }

        public bool Del(string key)
        {
            if (_cache.ContainsKey(key))
            {
                lock (lockHelper)
                {
                    _cache.Remove(key);
                }
            }
            return true;
        }

        public bool Del(List<string> keys)
        {
            lock (lockHelper)
            {
                foreach (string key in keys)
                {
                    _cache.Remove(key);
                }
            }
            return true;
        }

        public static CacheManager<T> Instance
        {
            get
            {
                if (mng == null)
                {
                    lock (lockHelper)
                    {
                        if (mng == null)
                        {
                            mng = new CacheManager<T>();
                            return mng;
                        }
                    }
                }
                return mng;
            }
        }

        public void Dispose()
        {
            _timer.Close();
            _cache.Clear();
            _cache = null;
        }
    }
}
