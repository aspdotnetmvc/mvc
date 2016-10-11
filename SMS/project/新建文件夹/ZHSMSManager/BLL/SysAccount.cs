using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public partial class SysAccount
    {
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public static bool Exists(string UserCode)
        {
            return DAL.SysAccount.Exists(UserCode);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static bool Add(SMS.Model.SysAccount account)
        {
            account.PassWord = DESEncrypt.Encrypt(account.PassWord);
            return DAL.SysAccount.Add(account);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public static bool Update(SMS.Model.SysAccount account)
        {
            return DAL.SysAccount.Update(account);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public static bool Del(string account)
        {
            if (DAL.SysAccount.Del(account))
            {
                return DAL.SysAccount.DelAccountRole(account);
            }
            return false;
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static SMS.Model.SysAccount GetAccount(string account)
        {
            return DAL.SysAccount.GetAccount(account);
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public static List<SMS.Model.SysAccount> GetAccounts()
        {
            return DAL.SysAccount.GetAccounts();
        }
        public static string GetEncrypt(string str)
        {
            return DESEncrypt.Encrypt(str);
        }
        public static bool ChanagePass(string account, string password)
        {
            return DAL.SysAccount.ChanagePass(account, DESEncrypt.Encrypt(password));
        }

        public static bool DelByUserCode(string UserCode)
        {
            return DAL.SysAccount.DelByUserCode(UserCode);
        }
        public static bool DelByRoleID(string RoleID)
        {
            return DAL.SysAccount.DelByRoleID(RoleID);
        }
        //public static bool UpdateSecretKey(string account, string secretKey, string password)
        //{
        //    return DAL.SysAccount.UpdateSecretKey(account, secretKey, password);
        //}
    }
}