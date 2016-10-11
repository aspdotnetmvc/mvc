using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    /// <summary>
    /// 企业黑名单
    /// </summary>
    public class EnterpriseBlackList
    {
        public static bool AddEnterpsieBlackList(string EnterpriseCode, List<string> Numbers)
        {
            return DAL.EnterpriseBlackList.AddEnterpriseBlackList(EnterpriseCode, Numbers);
        }
        public static bool DelEnterpriseBlackList(string EnterpriseCode, List<string> Numbers)
        {
            return DAL.EnterpriseBlackList.DelEnterpriseBlackList(EnterpriseCode, Numbers);
        }
        public List<string> getEnterpriseBlackList(string EnterpriseCode)
        {
            return DAL.EnterpriseBlackList.getEnterpriseBlackList(EnterpriseCode);
        }
    }
}
