using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ISMP_SMS
{


    public class NumberHelper
    {
        private static string[] Lucky = {
        "998","996","918","986","988","968","958",
        "886","866","818","816",
        "778","776","718",
        "688","699","698","618",
        "521","558","556","520","518","528","568","598","588","586",
        "338","336","366","388",
        "228","226","218","298",
        "198","118","168","188","166","198","158"
    };

        public static string GetNumberType(ulong number)
        {
            string num = number.ToString();
            Regex reg;
            reg = new Regex("(?:0(?=1)|1(?=2)|2(?=3)|3(?=4)|4(?=5)|5(?=6)|6(?=7)|7(?=8)|8(?=9)){5}\\d");
            if (reg.IsMatch(num))
            {
                return "ABCDEF";
            }

            reg = new Regex("^(?:0(?=1)|1(?=2)|2(?=3)|3(?=4)|4(?=5)|5(?=6)|6(?=7)|7(?=8)|8(?=9)){2}\\d[\\d][\\d](?:0(?=1)|1(?=2)|2(?=3)|3(?=4)|4(?=5)|5(?=6)|6(?=7)|7(?=8)|8(?=9)){2}\\d");
            string result;
            if (reg.IsMatch(num))
            {
                return "ABC**ABC";
            }

            reg = new Regex("[\\d](?:0(?=1)|1(?=2)|2(?=3)|3(?=4)|4(?=5)|5(?=6)|6(?=7)|7(?=8)|8(?=9)){2}\\d[\\d](?:0(?=1)|1(?=2)|2(?=3)|3(?=4)|4(?=5)|5(?=6)|6(?=7)|7(?=8)|8(?=9)){2}\\d$");
            if (reg.IsMatch(num))
            {
                return "*ABC*ABC";
            }

            reg = new Regex("^(?:0(?=1)|1(?=2)|2(?=3)|3(?=4)|4(?=5)|5(?=6)|6(?=7)|7(?=8)|8(?=9)){2}\\d{4}");
            if (reg.IsMatch(num))
            {
                return "ABCABC*";
            }

            reg = new Regex("(?:0(?=1)|1(?=2)|2(?=3)|3(?=4)|4(?=5)|5(?=6)|6(?=7)|7(?=8)|8(?=9)){2}\\d{4}");
            if (reg.IsMatch(num))
            {
                return "ABCABC";
            }



            reg = new Regex("(?:0(?=1)|1(?=2)|2(?=3)|3(?=4)|4(?=5)|5(?=6)|6(?=7)|7(?=8)|8(?=9)){3}\\d{5}");
            if (reg.IsMatch(num))
            {
                return "ABCDABCD";
            }

            reg = new Regex("(\\d)((?!\\1)\\d)\\1\\2\\1\\2");
            if (reg.IsMatch(num))
            {
                return "ABABAB";
            }

            reg = new Regex("^(\\d)((?!\\1)\\d)\\1\\2\\1\\2");
            if (reg.IsMatch(num))
            {
                return "ABABAB*";
            }

            reg = new Regex("^(\\d)((?!\\1)\\d)[\\d][\\d](\\d)((?!\\1)\\d)[\\d][\\d]");
            if (reg.IsMatch(num))
            {
                return "AB**AB**";
            }

            reg = new Regex("[\\d][\\d](\\d)((?!\\1)\\d)[\\d][\\d](\\d)((?!\\1)\\d)$");
            if (reg.IsMatch(num))
            {
                return "**AB**AB";
            }

            reg = new Regex("(\\d)\\1((?!\\1)\\d)\\2((?!\\1)\\d)\\3");
            if (reg.IsMatch(num))
            {
                return "AABBCC";
            }

            reg = new Regex("^(\\d)\\1((?!\\1)\\d)\\2((?!\\1)\\d)\\3");
            if (reg.IsMatch(num))
            {
                return "AABBCC*";
            }

            reg = new Regex("([\\d])\\1{4,}");
            if (reg.IsMatch(num))
            {
                return "AAAAA";
            }

            //AABBB
            reg = new Regex("(\\d)\\1((?!\\1)\\d)\\2\\2");
            if (reg.IsMatch(num))
            {
                return "AABBB";
            }

            reg = new Regex("(?:0(?=1)|1(?=2)|2(?=3)|3(?=4)|4(?=5)|5(?=6)|6(?=7)|7(?=8)|8(?=9)){2}\\d{3}");
            if (reg.IsMatch(num))
            {
                return "ABCAB";
            }

            //AABCC
            reg = new Regex("(\\d)\\1((?!\\1)\\d)((?!\\1)\\d)\\3");
            if (reg.IsMatch(num))
            {
                return "AABCC";
            }


            reg = new Regex("(\\d)((?!\\1)\\d)\\2[\\d]");
            if (reg.IsMatch(num))
            {
                return "ABBA";
            }

            reg = new Regex("(?:0(?=1)|1(?=2)|2(?=3)|3(?=4)|4(?=5)|5(?=6)|6(?=7)|7(?=8)|8(?=9)){3}\\d");
            if (reg.IsMatch(num))
            {
                return "ABCD";
            }

            reg = new Regex("(\\d)((?!\\1)\\d)\\1\\2");
            if (reg.IsMatch(num))
            {
                return "ABAB";
            }

            //AABB
            reg = new Regex("(\\d)\\1((?!\\1)\\d)\\2");
            if (reg.IsMatch(num))
            {
                return "AABB";
            }

            reg = new Regex("(.)\\1{3}");
            if (reg.IsMatch(num))
            {
                return "AAAA";
            }

            reg = new Regex("(\\d)\\1\\1((?!\\1)\\d)");
            if (reg.IsMatch(num))
            {
                return "AAAB";
            }

            reg = new Regex("(?:0(?=1)|1(?=2)|2(?=3)|3(?=4)|4(?=5)|5(?=6)|6(?=7)|7(?=8)|8(?=9)){2}\\d");
            if (reg.IsMatch(num))
            {
                return "ABC";
            }

            reg = new Regex("(.)\\1{2}");
            if (reg.IsMatch(num))
            {
                return "AAA";
            }

            //吉祥号
            string t = num.Substring(num.Length - 3, 3);
            foreach (string lck in Lucky)
            {
                if (t == lck)
                {
                    return "Lucky";
                }
            }

            reg = new Regex("(.)\\1{1}");
            if (reg.IsMatch(num))
            {
                return "AA";
            }


            return "";
        }


        public static ulong GetOrdinaryNumber(int maxNumber)
        {
            Random rd = new Random();
            while (true)
            {
                ulong n = (ulong)rd.Next(maxNumber);
                if (GetNumberType(n) == "") return n;
            }
        }
    }
}