using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTestDBAccess
{
   public class Util
    {
        public static Random Rand = new Random();
        public static string GenRandStr(int len)
        {
            string str = "ABCDEFGHIJKLMWOPQRSTUVWXYZabcdefghijklmwopqrstuvwxyz1234567890";
            string r = "";
            for (int i = 0; i < len; i++)
            {
                r += str[Rand.Next(62)];
            }
            return r;
        }
    }
}
