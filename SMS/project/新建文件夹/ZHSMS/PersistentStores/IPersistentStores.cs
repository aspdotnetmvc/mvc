using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentStores
{
    public interface IPersistentStores<T>
    {
        void Add(string key, T content);
        void Set(string key, T content);
        void Del(string key);
        void Clear();
        List<KeyValuePair<string, T>> Load();
        void Write(List<KeyValuePair<string, T>> data);
    }
}
