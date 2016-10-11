using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSPlatService
{
    public sealed class CacheManager<T> : IDisposable
    {
        private class Cache<CT>
        {
            public CT Content { get; set; }
        }

        private volatile static CacheManager<T> mng = null;
        private static object lockHelper = new object();

        int time = 60 * 30;
        Dictionary<string, Cache<T>> _cache = new Dictionary<string, Cache<T>>();

        private CacheManager()
        {
        }
        public T Get(string key)
        {
            Cache<T> c = null;
            _cache.TryGetValue(key, out c);
            if (c != null)
            {
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
            c.Content = content;
            _cache.Add(key, c);
            return true;
        }

        public bool Set(string key, T content)
        {
            Cache<T> c;
            _cache.TryGetValue(key, out c);
            if (c == null)
            {
                c = new Cache<T>();
                c.Content = content;
                _cache.Add(key, c);
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
            foreach (var c in _cache.Values)
            {
                cs.Add(c.Content);
            }
            return cs;
        }


        public List<T> Query(Func<string, bool> query)
        {

            return new List<T>();
        }

        public bool Del(string key)
        {
            if (_cache.ContainsKey(key))
            {
                _cache.Remove(key);
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
        }
    }
}