using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Configuration;

namespace ZKD
{
    public class NumberValidate
    {
        public static string MobilePattern = ConfigurationManager.AppSettings["MobilePattern"];
        /// <summary>
        /// 判断是否合法的
        /// </summary>
        /// <param name="MobileNo"></param>
        /// <returns></returns>
        public static bool IsValidMobileNo(string MobileNo)
        {
           
            return Regex.IsMatch(MobileNo, MobilePattern);
        }
    }
}