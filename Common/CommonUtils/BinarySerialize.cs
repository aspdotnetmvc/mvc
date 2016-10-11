using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace BXM.Utils
{
    public class BinarySerialize
    {
        /// <summary>
        /// 序列化反序列化对象
        /// </summary>
        public static byte[] Serialize(object obj)
        {
            BinaryFormatter binaryF = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(1024 * 10);
            binaryF.Serialize(ms, obj);
            ms.Seek(0, SeekOrigin.Begin);
            byte[] buffer = new byte[(int)ms.Length + 2];

            ms.Read(buffer, 0, buffer.Length);
            ms.Close();
            return buffer;
        }

        public static object Deserialize(byte[] buffer)
        {
            BinaryFormatter binaryF = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(buffer, 0, buffer.Length, false);
            object obj = binaryF.Deserialize(ms);
            ms.Close();
            return obj;
        }
    }

    /// <summary>  
    /// BinarySerialize class,provider basic Serialize and Deserialize method.  
    /// </summary>  
    public class BinarySerialize<T>
    {
        private string _strFilePath = string.Empty;

        public void Serialize(T obj, string strFilePath)
        {
            _strFilePath = strFilePath;
            using (FileStream fs = new FileStream(_strFilePath, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, obj);
            }
        }

        /// <summary>  
        /// Deserialize an instance of T.  
        /// </summary>  
        /// <typeparam name="T">Any type.</typeparam>  
        /// <returns>The result of deserialized.</returns>  
        public T DeSerialize(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            if (!fi.Exists)
                throw new ArgumentException("File specified is not exist!");
            T t;
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    t = (T)formatter.Deserialize(fs);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return t;
        }
    }
}
