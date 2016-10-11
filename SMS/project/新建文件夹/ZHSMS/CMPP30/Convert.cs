using System;
using System.Text;

namespace CMPP
{
    /// <summary>
    /// 编码帮助类。
    /// </summary>
    internal static class Convert
    {

        #region 公有方法
        /// <summary>
        /// 字节流解码。
        /// </summary>
        public static string ToString(byte[] buffer, int startIndex, int length, CEncoding encoding)
        {
            switch (encoding)
            {
                case CEncoding.GBK:
                    return System.Text.UnicodeEncoding.GetEncoding("gb2312").GetString(buffer, startIndex, length);
                case CEncoding.ASCII:
                case CEncoding.BINARY:
                    return System.Text.Encoding.ASCII.GetString(buffer, startIndex, length);
                case CEncoding.UCS2:
                    return System.Text.Encoding.BigEndianUnicode.GetString(buffer, startIndex, length);
                default:
                    return "";
            }
        }
        /// <summary>
        /// 字节流编码。
        /// </summary>
        public static byte[] ToBytes(string value, byte coding)
        {
            if (value != null)
            {
                switch (coding)
                {
                    case CMPP30.CODING_GBK:
                        return UnicodeEncoding.GetEncoding("gb2312").GetBytes(value);
                    case CMPP30.CODING_ASCII:
                    case CMPP30.CODING_BINARY:
                        return Encoding.ASCII.GetBytes(value);
                    case CMPP30.CODING_UCS2:
                        return Encoding.BigEndianUnicode.GetBytes(value);
                }
            }
            return null;
        }
        /// <summary>
        /// 计算字符串长度（该长度为转换为字节流后的长度）。
        /// </summary>
        public static byte Length(string value, byte coding)
        {
            byte[] buffer = ToBytes(value, coding);
            return buffer == null ? (byte)0 : (byte)buffer.Length;
        }
        /// <summary>
        /// 字节流编码。
        /// </summary>
        public static byte[] ToBytes(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return bytes;
        }
        /// <summary>
        /// 字节流编码。
        /// </summary>
        public static byte[] ToBytes(ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return bytes;
        }
        /// <summary>
        /// 字节流解码。
        /// </summary>
        public static uint ToUInt32(byte[] bytes, int index)
        {
            Array.Reverse(bytes, index, 4);
            return BitConverter.ToUInt32(bytes, index);
        }
        /// <summary>
        /// 字节流解码。
        /// </summary>
        public static ulong ToUInt64(byte[] bytes, int index)
        {
            Array.Reverse(bytes, index, 8);
            return BitConverter.ToUInt64(bytes, index);
        }
        #endregion

    }
}
