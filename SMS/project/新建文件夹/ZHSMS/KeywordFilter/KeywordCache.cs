
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KeywordFilter
{
    public sealed class KeywordCache
    {
        private volatile static KeywordCache cache = null;
        private static object lockHelper = new object();

        Dictionary<string, string[]> _cache = new Dictionary<string, string[]>();

        int maxCache = 100000; 

        private KeywordCache()
        {
            
        }

        public void Get(string key, out string[] content)
        {
            _cache.TryGetValue(key, out content);
        }

        public void Add(string key, string[] content)
        {
            if (_cache.ContainsKey(key))
            {
                return;
            }
            lock (lockHelper)
            {
                if (_cache.Keys.Count >= maxCache)
                {
                    string[] ks = _cache.Keys.ToArray();
                    int dn = maxCache / 100;
                    for (int i = 0; i < dn; i++)
                    {
                        _cache.Remove(ks[i]);
                    }
                }
            }
            _cache.Add(key, content);
        }

        public static KeywordCache Instance
        {
            get
            {
                if (cache == null)
                {
                    lock (lockHelper)
                    {
                        if (cache == null)
                        {
                            cache = new KeywordCache();
                            return cache;
                        }
                    }
                }
                return cache;
            }
        }
    }
}
