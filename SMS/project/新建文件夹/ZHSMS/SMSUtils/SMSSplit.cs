using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSUtils
{
    public class SMSSplit
    {
        public static int GetSplitNumber(string smsContent, string smsSignature, out int pkSize)
        {

            if ((smsContent + smsSignature).Length <= 70)
            {
                pkSize = 70;
                return 1;
            }
            pkSize = 67;
            int pkNum = getCeiling((smsContent + smsSignature).Length, pkSize);
            return pkNum;
            //if (Encoding.GetEncoding("gb2312").GetByteCount(smsContent + smsSignature) <= 140)
            //{
            //    pkSize = 140;
            //    return 1;
            //}
            //pkSize = 134;
            //int msgLen = GetLen(smsContent + smsSignature);
            //int pkNum = msgLen / pkSize;
            //if ((msgLen % pkSize) > 0) pkNum++;
            //return pkNum;
        }
        public static int getCeiling(int l, int p)
        {
            return Convert.ToInt32(Math.Ceiling(l * 1.0 / p));
        }

        public static int GetLen(string str)
        {
            if (string.IsNullOrEmpty(str)) return 0;
            return str.Length;
        }

        public static string GetSubString(string str, int len)
        {
            int n = GetLen(str);

            if (n > len)
            {
                return str.Substring(0, len);
            }
            else
            {
                return str;
            }
        }
    }
}
