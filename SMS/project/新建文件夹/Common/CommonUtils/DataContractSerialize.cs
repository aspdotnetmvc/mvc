using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BXM.Utils
{
    public class DataContractSerialize
    {
        #region Serialize

        /// <summary>
        /// DataContract序列化
        /// </summary>
        /// <param name="FormatObject"></param>
        /// <returns></returns>
        public static String Serialize(object FormatObject)
        {
            Type T = FormatObject.GetType();
            //DataContract方式序列化
            MemoryStream ms = new MemoryStream();
            DataContractSerializer ser = new DataContractSerializer(T);
            ser.WriteObject(ms, FormatObject);
            byte[] array = ms.ToArray();
            ms.Close();

            string DataContractString = Encoding.UTF8.GetString(array, 0, array.Length);
            return DataContractString;
        }

        #endregion

        #region Deserialize


        /// <summary>
        /// DataContract反序列化
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string DataContractString)
        {

            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(DataContractString));
            DataContractSerializer ser = new DataContractSerializer(typeof(T));
            T O = (T)ser.ReadObject(ms);
            ms.Close();
            return O;

        }
        #endregion
    }
}
