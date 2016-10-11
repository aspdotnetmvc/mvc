using BXM.Utils;
using DatabaseAccess;
using LogClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace PersistentStores
{
    public class MysqlPersistentStores<T> : IPersistentStores<T>
    {
        string _space;
        public MysqlPersistentStores(string space)
        {
            _space = space;
        }
        public void Add(string key, T content)
        {
            try
            {
                string s = "";
                if (content is string)
                {
                    s = content.ToString();
                }
                else
                {
                    s = JsonSerialize.Instance.Serialize<T>(content);
                }
                CacheDataDB.Add(_space, key.ToString(), s);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("RedisPersistentStores", "PersistentStores.Add", ex.ToString());
            }
        }

        public void Set(string key, T content)
        {
            try
            {
                string s = "";
                if (content is string)
                {
                    s = content.ToString();
                }
                else
                {
                    s = JsonSerialize.Instance.Serialize<T>(content);
                }

                CacheDataDB.Update(_space, key.ToString(), s);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("RedisPersistentStores", "PersistentStores.Set", ex.ToString());
            }
        }

        public void Del(string key)
        {
            try
            {
                DatabaseAccess.CacheDataDB.Del(_space, key.ToString());
            }
            catch (Exception ex)
            {
                LogHelper.LogError("RedisPersistentStores", "PersistentStores.Del", ex.ToString());
            }
        }

        public List<KeyValuePair<string, T>> Load()
        {
            try
            {
                List<KeyValuePair<string, T>> list = new List<KeyValuePair<string, T>>();
                var dt = CacheDataDB.Get(_space);
                foreach (var row in dt)
                {
                    T t = JsonSerialize.Instance.Deserialize<T>(row.NContent);
                    KeyValuePair<string, T> o = new KeyValuePair<string, T>(row.NKey, t);
                    list.Add(o);
                }
                return list;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("RedisPersistentStores", "PersistentStores.Load", ex.ToString());
            }
            return new List<KeyValuePair<string, T>>();
        }


        public void Clear()
        {
            try
            {
                CacheDataDB.Clear(_space);
            }
            catch (Exception ex)
            {

                LogHelper.LogError("RedisPersistentStores", "PersistentStores.Clear", ex.ToString());
            }
        }

        public void Write(List<KeyValuePair<string, T>> data)
        {
            throw new NotImplementedException();
        }
    }
}
