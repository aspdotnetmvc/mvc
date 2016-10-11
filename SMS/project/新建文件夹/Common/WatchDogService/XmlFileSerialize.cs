using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WatchDogService
{
    public class XmlFileSerialize
    {
        string _file = "";

        public XmlFileSerialize(string file)
        {
            _file = file;
        }

        public bool FileExist()
        {
            if (System.IO.File.Exists(_file))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Serialize<T>(T obj)
        {
            using (FileStream fs = new FileStream(_file, FileMode.Create))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T));
                formatter.Serialize(fs, obj);
            }
        }

        public T DeSerialize<T>()
        {
            T obj;
            using (FileStream fs = new FileStream(_file, FileMode.Open, FileAccess.Read))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T));
                obj = (T)formatter.Deserialize(fs);
            }
            return obj;
        }
    }
}
