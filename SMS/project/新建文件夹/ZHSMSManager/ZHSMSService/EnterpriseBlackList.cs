using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SMSPlatService
{
    /// <summary>
    /// 企业黑名单 缓存
    /// </summary>
    public class EnterpriseBlackList
    {
        private EnterpriseBlackList()
        {
            loadData();
        }

        private static readonly EnterpriseBlackList _instance = new EnterpriseBlackList();
        public static EnterpriseBlackList Instance
        {
            get { return _instance; }
        }
        public Dictionary<string, List<string>> BlackList = new Dictionary<string, List<string>>();

        public void loadData()
        {
            DataTable dt = DAL.EnterpriseBlackList.getAllEnterpriseBlackList();
            if (dt != null)
            {
                foreach (DataRow r in dt.Rows)
                {
                    AddBlackNumber(getValue(r, "AccountCode"), getValue(r, "Number"));
                }
            }
        }

        public void AddBlackNumber(string EnterpriseCode, string number)
        {
            List<string> list = GetEnterpriseBlackNumbers(EnterpriseCode);
            if (list == null)
            {
                list = new List<string>();
                BlackList.Add(EnterpriseCode, list);
            }

            list.Add(number);
        }
        public void AddBlackNumbers(string EnterpriseCode, List<string> Numbers)
        {
            List<string> list = GetEnterpriseBlackNumbers(EnterpriseCode);
            if (list == null)
            {
                list = new List<string>();
                BlackList.Add(EnterpriseCode, list);
            }

            list.AddRange(Numbers);
        }
        public void DeleteBlackNumbers(string EnterpriseCode, List<string> Numbers)
        {
            List<string> list = GetEnterpriseBlackNumbers(EnterpriseCode);
            if (list != null)
            {
                list = list.Where(n => !Numbers.Any(d => d == n)).ToList();
                BlackList[EnterpriseCode] = list;
            }
        }

        /// <summary>
        /// Numbers 是逗号隔开的号码。
        /// 返回在黑名单中的号码列表
        /// 没有则返回空字符串
        /// </summary>
        /// <param name="EnterpriseCode"></param>
        /// <param name="Numbers"></param>
        /// <returns></returns>
        public string CheckNumber(string EnterpriseCode, string Numbers)
        {
            var ns = Numbers.Split(',').ToList();
            return CheckNumber(EnterpriseCode, ns);
        }
        public string CheckNumber(string EnterpriseCode, List<string> Numbers)
        {
            if (BlackList.ContainsKey(EnterpriseCode))
            {
                var list = BlackList[EnterpriseCode];
                if (list == null)
                {
                    return "";
                }
                var r = Numbers.Where(n => list.Any(b => b == n));
                if (r.Any())
                {
                    return string.Join(",", r);
                }
            }
            return "";

        }
        public List<string> GetEnterpriseBlackNumbers(string EnterpriseCode)
        {
            if (BlackList.ContainsKey(EnterpriseCode))
            {
                return BlackList[EnterpriseCode];
            }
            else
            {
                return null;
            }
        }

        private string getValue(DataRow dr, string col)
        {
            var o = dr[col];
            if (o == null) return null;
            return o.ToString();
        }
    }
}
