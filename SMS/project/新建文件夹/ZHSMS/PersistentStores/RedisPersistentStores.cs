using LogClient;
using RedisAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentStores
{
    
    public class RedisPersistentStores<T>:IPersistentStores<T>
    {
        
        string _space;

        public RedisPersistentStores(string space)
        {
            _space = space;
        }

        public void Add(string key, T content)
        {
            try
            {
                RedisHelper.Hash_Set<T>(_space, key, content);
            }
            catch(Exception ex)
            {
                LogHelper.LogError("RedisPersistentStores", "RedisPersistentStores.Add", ex.ToString());
            }
        }

        public void Set(string key, T content)
        {
            try
            {
                RedisHelper.Hash_Set<T>(_space, key, content);
            }
            catch(Exception ex)
            {
                LogHelper.LogError("RedisPersistentStores", "RedisPersistentStores.Set", ex.ToString());
            }
        }

        public void Del(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key)) return;
                RedisHelper.Hash_Remove(_space, key);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("RedisPersistentStores", "RedisPersistentStores.Del", ex.ToString());
            }
        }

        public void Clear()
        {
            try
            {
                RedisHelper.Hash_Remove(_space);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("RedisPersistentStores", "RedisPersistentStores.Clear", ex.ToString());
            }
        }

        public List<KeyValuePair<string, T>> Load()
        {
            try
            {
                return RedisHelper.Hash_GetAll<T>(_space);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("RedisPersistentStores", "RedisPersistentStores.Load", ex.ToString());
            }

            return new List<KeyValuePair<string,T>>();
        }

        public void Write(List<KeyValuePair<string, T>> data)
        {
            throw new NotImplementedException();
        }
    }
}
