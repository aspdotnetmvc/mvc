using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Util
{
    public class SMSHelper
    {
        /// <summary>
        /// 判断短信内容是否包含签名，并返回签名和内容。
        /// </summary>
        /// <param name="Content"></param>
        /// <param name="Signature"></param>
        /// <returns></returns>
        public static bool GetSignature(string Content, ref string Signature,ref string newContent)
        {
            //判断短信本身是否含有signature.
            string regex = @"^【.+?】|【.+?】$";

            bool hasSignature = System.Text.RegularExpressions.Regex.IsMatch(Content, regex);
            if (!hasSignature) return false;
            if (Content.StartsWith("【"))
            {
                Signature = System.Text.RegularExpressions.Regex.Match(Content, regex, System.Text.RegularExpressions.RegexOptions.Multiline).Value;
            }
            else
            {
                Signature = System.Text.RegularExpressions.Regex.Match(Content, regex, System.Text.RegularExpressions.RegexOptions.Multiline | System.Text.RegularExpressions.RegexOptions.RightToLeft).Value;
            }

            newContent = Content.Replace(Signature, "");
            return true;
        }
        /// <summary>
        /// 根据号码数获取短信优先级
        /// 单条短信返回6
        /// 1-100内返回3
        /// 100以上返回1
        /// </summary>
        /// <param name="Count"></param>
        /// <returns></returns>
        public static int GetPriorityByNumberCount(int count)
        {
            if (count == 1)
            {
                return 6;
            }
            if (count >= 100)
            {
                return 1;
            }
            return 3;
        }
    }
}
