using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL
{
    public class AccountEnterprise
    {
        public static bool Exists(string EnterpriseCode)
        {
            return DAL.AccountEnterprise.Exists(EnterpriseCode);
        }
        public static DataTable GetAccountEnterprise()
        {
            return DAL.AccountEnterprise.GetAccountEnterprise();
        }
        public static DataTable GetAccountEnterpriseByUserCode(string UserCode)
        {
            return DAL.AccountEnterprise.GetAccountEnterpriseByUserCode(UserCode);
        }
        public static DataTable GetAccountEnterprise(string UserCode)
        {
            return DAL.AccountEnterprise.GetAccountEnterprise(UserCode);
        }
        public static List<string> GetEnterpriseByUserCode(string UserCode)
        {
            return DAL.AccountEnterprise.GetEnterpriseByUserCode(UserCode);
        }
        public static List<SMS.Model.EnterpriseUser> GetEnterpriseUserByUserCode(string UserCode)
        {
            return DAL.AccountEnterprise.GetEnterpriseUserByUserCode(UserCode);
        }
        public static bool Add(SMS.Model.AccountEnterprise model)
        {
            return DAL.AccountEnterprise.Add(model);
        }
        public static bool Del(string EnterpriseCode)
        {
            return DAL.AccountEnterprise.Del(EnterpriseCode);
        }
        public static bool DelByUserCodeAndEnterpriseCode(string UserCode, string EnterpriseCode)
        {
            return DAL.AccountEnterprise.DelByUserCodeAndEnterpriseCode(UserCode, EnterpriseCode);
        }

        public static bool DelByUserCode(string UserCode)
        {
            return DAL.AccountEnterprise.DelByUserCode(UserCode);
        }
         public static DataTable GetAccountEnterpriseByAccoutID(string accountID)
        {
            return DAL.AccountEnterprise.GetAccountEnterpriseByAccoutID(accountID);
        }
    }

    public class EnterpriseManage
    {
        public static bool Add(SMS.Model.EnterpriseManage model)
        {
            return DAL.EnterpriseManage.Add(model);
        }

        public static bool Update(SMS.Model.EnterpriseManage model)
        {
            return DAL.EnterpriseManage.Update(model);
        }

        public static bool Delete(Int32 rsid)
        {
            return DAL.EnterpriseManage.Delete(rsid);
        }

        public static List<SMS.Model.EnterpriseManage> GetEnManageByEnCode(string enCode)
        {
            return DAL.EnterpriseManage.GetEnManageByEnCode(enCode);
        }

        public static List<SMS.Model.EnterpriseManage> GetEnManageInfo()
        {
            return DAL.EnterpriseManage.GetEnManageInfo();
        }

        public static List<SMS.Model.EnterpriseManage> GetEnterpriseByCM(string channelManager)
        {
            return DAL.EnterpriseManage.GetEnterpriseByCM(channelManager);
        }

        public static List<SMS.Model.EnterpriseManage> GetEnterpriseByCS(string csManager)
        {
            return DAL.EnterpriseManage.GetEnterpriseByCS(csManager);
        }
    }
}
