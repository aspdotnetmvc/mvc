using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace BXM.Utils
{
    public class TypeCheck
    {

        #region 对象处理
        /// <summary>
        /// 判断对象是否为Int32类型的数字
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(object expression)
        {
            if (expression != null)
                return IsNumeric(expression.ToString());

            return false;

        }

        /// <summary>
        /// 判断对象是否为Int32类型的数字
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(string expression)
        {
            if (expression != null)
            {
                string str = expression;
                if (str.Length > 0 && str.Length <= 11 && Regex.IsMatch(str, @"^[-]?[0-9]*[.]?[0-9]*$"))
                {
                    if ((str.Length < 10) || (str.Length == 10 && str[0] == '1') || (str.Length == 11 && str[0] == '-' && str[1] == '1'))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否为Double类型
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsDouble(object expression)
        {
            if (expression != null)
                return Regex.IsMatch(expression.ToString(), @"^([0-9])[0-9]*(\.\w*)?$");

            return false;
        }

        /// <summary>
        /// 是否为时间类型
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static bool IsDateTime(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                DateTime dateTime;
                if (DateTime.TryParse(str, out dateTime))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 验证时间
        /// </summary>
        /// <param name="strDate"></param>
        /// <returns></returns>
        public static bool IsDate(string strDate)
        {
            if (string.IsNullOrEmpty(strDate))
                return false;
            string s_reg = @"^(?ni:(?=\\d)((?'year'((1[6-9])|([2-9]\\d))\\d\\d)(?'sep'[/.-])(?'month'0?[1-9]|1[012])\\2
(?'day'((?<!(\\2((0?[2469])|11)\\2))31)|(?<!\\2(0?2)\\2)(29|30)|((?<=((1[6-9]|[2-9]\\d)(0[48]|
[2468][048]|[13579][26])|(16|[2468][048]|[3579][26])00)\\2\\3\\2)29)|((0?[1-9])|(1\\d)|(2[0-
8])))(?:(?=\\x20\\d)\\x20|$))?((?<time>((0?[1-9]|1[012])(:[0-5]?\\d){0,2}(\\x20[AP]M))|([01]?
\\d|2[0-3])(:[0-5]?\\d){1,2}))?)$";
            Regex reg = new Regex(s_reg);
            if (reg.IsMatch(strDate.ToLower()))
                return true;
            else
                return false;
        }

        #endregion
    }
}
