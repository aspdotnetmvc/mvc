using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace SMGP
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
    }
}
