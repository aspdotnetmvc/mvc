using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BXM.Utils
{
    public class XmlSerialize
    {
        #region 文件
        public static void Serialize<T>(T obj,string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Create))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T));
                formatter.Serialize(fs, obj);
            }
        }

        public static T DeSerialize<T>(string file)
        {
            T obj;
            try
            {
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
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
        #endregion

        #region 字符串
    
        #endregion

    }
}
