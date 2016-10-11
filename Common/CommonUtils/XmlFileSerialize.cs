using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BXM.Utils
{
    public class XmlFileSerialize
    {
        string _file = "";

        public XmlFileSerialize(string file)
        {
            _file = file;
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
            try
            {
                using (FileStream fs = new FileStream(_file, FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer formatter = new XmlSerializer(typeof(T));
                    obj = (T)formatter.Deserialize(fs);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return obj;
        }
    }
}
