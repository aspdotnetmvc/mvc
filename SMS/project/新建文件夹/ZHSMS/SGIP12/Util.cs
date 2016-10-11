using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace SGIP
{
    public class Util
    {
        //private static object _SyncLockObject = new object(); 

        public static string Get_MMDDHHMMSS_String(DateTime dt)
        {
            string s = dt.Month.ToString().PadLeft(2, '0');
            s += dt.Day.ToString().PadLeft(2, '0');
            s += dt.Hour.ToString().PadLeft(2, '0');
            s += dt.Minute.ToString().PadLeft(2, '0');
            s += dt.Second.ToString().PadLeft(2, '0');
            return s;
        }

        public static string Get_YYYYMMDD_String(DateTime dt)
        {
            string s = dt.Year.ToString().PadLeft(4, '0');
            s += dt.Month.ToString().PadLeft(2, '0');
            s += dt.Day.ToString().PadLeft(2, '0');
            return s;
        }

        internal static void WriteToStream(byte[] bytes, NetworkStream Stream)
        {
            if (Stream.CanWrite)
            {
                //lock (_SyncLockObject) 
                //{
                Stream.Write(bytes, 0, bytes.Length);
                //}
            }
        }

        internal static byte[] ReadFromStream(int Length, NetworkStream Stream)
        {
            byte[] bytes = null;
            if (Stream.CanRead)
            {
                if (Stream.DataAvailable)
                {
                    bytes = new byte[Length];
                    int r = 0;
                    int l = 0;
                    //lock (_SyncLockObject) 
                    {
                        while (l < Length)
                        {
                            r = Stream.Read(bytes, l, Length - l);
                            l += r;
                        }
                    }
                }
            }
            return bytes;
        }


        public static uint bytes2Uint(byte[] bs, int index)
        {
            byte[] dst = new byte[4];
            Buffer.BlockCopy(bs, index, dst, 0, 4);
            byte num = dst[0];
            dst[0] = dst[3];
            dst[3] = num;
            num = dst[1];
            dst[1] = dst[2];
            dst[2] = num;
            return BitConverter.ToUInt32(dst, 0);
        }

        public static byte[] uint2Bytes(uint u)
        {
            byte[] bytes = BitConverter.GetBytes(u);
            byte num = bytes[0];
            bytes[0] = bytes[3];
            bytes[3] = num;
            num = bytes[1];
            bytes[1] = bytes[2];
            bytes[2] = num;
            return bytes;
        }
    }
}
