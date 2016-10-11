using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMSPlatform.DAL;

namespace SMSPlatform
{
    public class AccountServer
    {
        private volatile static AccountServer _accountServer;
        private static object lockHelper = new object();
        Dictionary<string, SMS.Model.AuditEnterprise> dic = new Dictionary<string, SMS.Model.AuditEnterprise>();

        private AccountServer()
        {
        }

        public static AccountServer Instance
        {
            get
            {
                if (_accountServer == null)
                {
                    lock (lockHelper)
                    {
                        if (_accountServer == null)
                        {
                            _accountServer = new AccountServer();
                            return _accountServer;
                        }
                    }
                }
                return _accountServer;
            }
        }
        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        internal SMS.Model.EnterpriseUser GetAccount(string account)
        {
            SMS.Model.EnterpriseUser a = CacheManager<SMS.Model.EnterpriseUser>.Instance.Get(account);
            if (a == null)
            {
                a = GetAccountFromDB(account);
                if (a != null)
                {
                    CacheManager<SMS.Model.EnterpriseUser>.Instance.Set(a.AccountCode, a);
                }
            }
            return a;
        }

        internal SMS.Model.EnterpriseUser GetAccountFromDB(string accountCode)
        {
            EnterpriseUser a = null;
            //数据库操作
            try
            {
                a = EnterpriseUserDB.GetEnterprise(accountCode);
                if (a != null)
                {
                    if (!string.IsNullOrEmpty(a.Signature) && a.Signature.StartsWith("【"))
                    {
                        a.Signature = a.Signature.Substring(1, a.Signature.Length - 2);
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return a;
        }
        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        internal List<SMS.Model.EnterpriseUser> GetAccounts()
        {
            List<SMS.Model.EnterpriseUser> list = CacheManager<SMS.Model.EnterpriseUser>.Instance.GetAll();//.Get(account);
            if (list == null)
            {
                //数据库操作
                try
                {
                    list = EnterpriseUserDB.GetEnterprises();
                }
                catch
                {
                    return new List<SMS.Model.EnterpriseUser>();
                }
            }
            return list;
        }
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        internal bool CreateAccount(SMS.Model.EnterpriseUser account)
        {
            //数据库操作
            bool ok = false;
            try
            {
                ok = EnterpriseUserDB.Add(account);
            }
            catch (Exception ex)
            {
                return false;
            }
            if (ok)
            {
                if (!string.IsNullOrEmpty(account.Signature))
                {
                    account.Signature = account.Signature.Substring(1, account.Signature.Length - 2);
                }
                CacheManager<SMS.Model.EnterpriseUser>.Instance.Set(account.AccountCode, account);
            }
            return ok;
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        internal bool DelAccount(string account)
        {
            try
            {
                bool ok = EnterpriseUserDB.Del(account);
                if (ok)
                {
                    CacheManager<SMS.Model.EnterpriseUser>.Instance.Del(account);
                }
                return ok;
            }
            catch (Exception ex)
            {
            }
            return false;
        }
        internal bool UpdateAccount(SMS.Model.EnterpriseUser user, string type)
        {
            try
            {
                SMS.Model.EnterpriseUser account = GetAccount(user.AccountCode);
                if (account == null) return false;
                bool ok = true;
                if (type == "info")
                {
                    ok = EnterpriseUserDB.UpdateAccontInfo(user);
                }
                if (type == "set")
                {
                    ok = EnterpriseUserDB.UpdateAccountSetting(user);
                }
                if (ok)
                {
                    account = user;
                    CacheManager<SMS.Model.EnterpriseUser>.Instance.Set(account.AccountCode, account);
                }
                return ok;
            }
            catch (Exception ex)
            {
            }
            return false;
        }
        /// <summary>
        /// 更新短信设置
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        internal bool UpdateAccountSMS(SMS.Model.EnterpriseUser user)
        {
            try
            {
                var rc = SMSProxy.GetSMSService().GetChannel(user.Channel);
                if (rc.Success)
                {
                    Channel c = rc.Value;
                    user.SMSType = c.SMSType;
                }
                else
                {
                    return false;
                }
                 
                bool ok = EnterpriseUserDB.UpdateAccountSMS(user);
                if (ok)
                {
                    SMS.Model.EnterpriseUser account = GetAccountFromDB(user.AccountCode);

                    CacheManager<SMS.Model.EnterpriseUser>.Instance.Set(account.AccountCode, account);
                }
                return ok;
            }
            catch (Exception ex)
            {
            }
            return false;
        }
        /// <summary>
        /// 企业修改密码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool ChangePass(string accountCode, string pass)
        {
            try
            {
                SMS.Model.EnterpriseUser account = GetAccount(accountCode);
                if (account == null) return false;
                string appPass = DESEncrypt.Encrypt(pass, string.IsNullOrEmpty(account.SecretKey) ? account.AccountID : account.SecretKey);
                bool ok = EnterpriseUserDB.ChangePass(accountCode, DESEncrypt.Encrypt(pass), appPass);
                if (ok)
                {
                    account.Password = DESEncrypt.Encrypt(pass);
                    account.AppPassword = appPass;
                    CacheManager<SMS.Model.EnterpriseUser>.Instance.Set(account.AccountCode, account);
                }
                return ok;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        #region 短信模板

        /// <summary>
        /// 添加短信模板
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        internal bool AddSMSTempletContent(SMSTemplet content)
        {
            bool ok = false;
            try
            {
                ok = SMSTempletDB.Add(content);
                if (ok)
                {
                    CacheManager<SMSTemplet>.Instance.Set(content.TempletID, content);
                }
            }
            catch
            {
            }
            return ok;
        }
        /// <summary>
        /// 审核短信模板
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        internal bool UpdateAuditSMSTemplet(SMSTemplet content)
        {
            bool ok = false;
            try
            {
                ok = SMSTempletDB.Update(content);
                if (ok)
                {
                    CacheManager<SMSTemplet>.Instance.Set(content.TempletID, content);
                }
                return ok;
            }
            catch (Exception ex)
            {
            }
            return false;

        }

        internal bool DelSMSTemplet(string templetID)
        {
            bool ok = false;
            try
            {
                ok = SMSTempletDB.Del(templetID);
                if (ok)
                {
                    CacheManager<SMSTemplet>.Instance.Del(templetID);
                }
                return ok;
            }
            catch (Exception ex)
            {
            }
            return false;

        }

        /// <summary>
        /// 修改模板内容
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        internal bool UpdateSMSTemplet(SMSTemplet content)
        {
            bool ok = false;
            try
            {
                ok = SMSTempletDB.UpdateContent(content);
                if (ok)
                {
                    CacheManager<SMSTemplet>.Instance.Set(content.TempletID, content);
                }
                return ok;
            }
            catch (Exception ex)
            {
            }
            return false;

        }

        /// <summary>
        /// 获取模板内容
        /// </summary>
        /// <param name="templetID"></param>
        /// <returns></returns>
        internal SMSTemplet GetSMSTemplet(string templetID)
        {
            SMSTemplet content = CacheManager<SMSTemplet>.Instance.Get(templetID);
            if (content == null)
            {
                //数据库操作
                try
                {
                    content = SMSTempletDB.GetSMSTempetContent(templetID);
                    if (content != null)
                    {
                        CacheManager<SMSTemplet>.Instance.Set(content.TempletID, content);
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return content;
        }

        /// <summary>
        /// 获取备案的短信模板
        /// </summary>
        /// <returns></returns>
        internal List<SMSTemplet> GetSuccessSMSTemplets(string account)
        {
            List<SMSTemplet> list = CacheManager<SMSTemplet>.Instance.GetAll().Where(c => c.AuditState == SMS.Model.TempletAuditType.Success && c.AccountCode == account).ToList();
            return list;
        }
        /// <summary>
        /// 获取所有短信模板
        /// </summary>
        /// <returns></returns>
        internal List<SMSTemplet> GetSMSTemplets()
        {
            List<SMSTemplet> list = CacheManager<SMSTemplet>.Instance.GetAll();
            if (list == null)
            {
                return new List<SMSTemplet>();
            }
            return list;
        }

        #endregion

        public void LoadAccountCache()
        {
            List<SMS.Model.EnterpriseUser> list = EnterpriseUserDB.GetEnterprises();
            foreach (var v in list)
            {
                if (!string.IsNullOrEmpty(v.Signature))
                {
                    v.Signature = v.Signature.Substring(1, v.Signature.Length - 2);
                }
                CacheManager<SMS.Model.EnterpriseUser>.Instance.Set(v.AccountCode, v);
            }
         

            List<SMSTemplet> sc = SMSTempletDB.GetSMSTempets();
            foreach (var v in sc)
            {
                CacheManager<SMSTemplet>.Instance.Set(v.TempletID, v);
            }
        }
    }
}