using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace BXM.Utils
{
    /// <summary>  
    /// 简单的压缩  
    /// </summary>  
    public static class CompressHelper
    {
        /// <summary>  
        /// 压缩字符串  
        /// </summary>  
        /// <param name="str"></param>  
        /// <returns></returns>  
        public static byte[] CompressString(string str)
        {
            return CompressBytes(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>  
        /// 压缩二进制  
        /// </summary>  
        /// <param name="str"></param>  
        /// <returns></returns>  
        public static byte[] CompressBytes(byte[] str)
        {
            var ms = new MemoryStream(str) { Position = 0 };
            var outms = new MemoryStream();
            using (var deflateStream = new DeflateStream(outms, CompressionMode.Compress, true))
            {
                var buf = new byte[1024];
                int len;
                while ((len = ms.Read(buf, 0, buf.Length)) > 0)
                    deflateStream.Write(buf, 0, len);
            }
            return outms.ToArray();
        }
        /// <summary>  
        /// 解压字符串  
        /// </summary>  
        /// <param name="str"></param>  
        /// <returns></returns>  
        public static string DecompressString(byte[] str)
        {
            return Encoding.UTF8.GetString(DecompressBytes(str));
        }
        /// <summary>  
        /// 解压二进制  
        /// </summary>  
        /// <param name="str"></param>  
        /// <returns></returns>  
        public static byte[] DecompressBytes(byte[] str)
        {
            var ms = new MemoryStream(str) { Position = 0 };
            var outms = new MemoryStream();
            using (var deflateStream = new DeflateStream(ms, CompressionMode.Decompress, true))
            {
                var buf = new byte[1024];
                int len;
                while ((len = deflateStream.Read(buf, 0, buf.Length)) > 0)
                    outms.Write(buf, 0, len);
            }
            return outms.ToArray();
        }
    }

    /// <summary>
    /// 压缩数据
    /// </summary>
    public class GZip
    {
        /// <summary>
        /// 将字节数组进行压缩后返回压缩的字节数组
        /// </summary>
        /// <param name="data">需要压缩的数组</param>
        /// <returns>压缩后的数组</returns>
        public static byte[] Compress(byte[] data)
        {
            MemoryStream stream = new MemoryStream();
            GZipStream gZipStream = new GZipStream(stream, CompressionMode.Compress);
            gZipStream.Write(data, 0, data.Length);
            gZipStream.Close();
            return stream.ToArray();
        }

        /// <summary>
        /// 解压字符数组
        /// </summary>
        /// <param name="data">压缩的数组</param>
        /// <returns>解压后的数组</returns>
        public static byte[] Decompress(byte[] data)
        {
            MemoryStream stream = new MemoryStream();

            GZipStream gZipStream = new GZipStream(new MemoryStream(data), CompressionMode.Decompress);

            byte[] bytes = new byte[40960];
            int n;
            while ((n = gZipStream.Read(bytes, 0, bytes.Length)) != 0)
            {
                stream.Write(bytes, 0, n);
            }
            gZipStream.Close();
            return stream.ToArray();
        }
    }
}
