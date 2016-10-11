using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SMSPlatService
{
    public partial class SMSPlatformService : MarshalByRefObject, SMSPlatInterface.ISMService
    {
        public long Heartbeat()
        {
            return DateTime.Now.Ticks;
        }

        #region 外部使用接口
        /// <summary>
        /// 短信发送
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="smsContent"></param>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<SMS.DTO.SMSDTO> SendSMS(string account, string password, string smsContent, List<string> numbers,string Source)
        {
            try
            {
                if (string.IsNullOrEmpty(account))
                {
                    return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null, "帐号不能为空");
                }
                if (numbers.Count == 0)
                {
                    return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null, "短信接收号码不能为空");
                }
                if (numbers.Count > 1000)
                {
                    return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null, "接收短信的号码过多，应少于1000个号码");
                }

                if (string.IsNullOrEmpty(smsContent)) return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null, "短信内容不能为空");

                #region  by lmw 2016-03-10 企业黑名单过滤

                string numberinblacklist = "";
                try
                {
                    numberinblacklist = EnterpriseBlackList.Instance.CheckNumber(account, numbers);

                    if (!string.IsNullOrWhiteSpace(numberinblacklist))
                    {
                        var blackNumbers = EnterpriseBlackList.Instance.GetEnterpriseBlackNumbers(account);
                        numbers = (from n in numbers where !blackNumbers.Any(b => b == n) select n).ToList();
                    }
                    if (numbers.Count == 0)
                    {
                        return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null, "号码全部在黑名单中：" + numberinblacklist);
                    }
                }
                catch (Exception ex)
                {

                }

                #endregion



                try
                {
                    SMS.Model.EnterpriseUser user = AccountServer.Instance.GetAccount(account);
                    if (user == null)
                    {
                        return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null, "用户不存在");
                    }
                    if (string.IsNullOrEmpty(user.SPNumber)) return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null, "企业SPNumber不能为空");

                    if (!user.Enabled)
                    {
                        return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null, "此帐户已被禁止发送短信");
                    }
                    if (!user.IsOpen) return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null, "此用户未开放对外接口功能");
                    if (DESEncrypt.Encrypt(password, string.IsNullOrEmpty(user.SecretKey) ? user.AccountID : user.SecretKey) != user.AppPassword)
                    {
                        return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null, "密码不正确");
                    }

                    return SendSMSDTO(smsContent, numbers, numberinblacklist, user, user.SPNumber,Source);
                }
                catch
                {
                    return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null, "短信出现异常");
                }

            }
            catch
            {
                return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null, "发送短信出现错误");
            }
        }

        private SMS.Model.RPCResult<SMS.DTO.SMSDTO> SendSMSDTO(string smsContent, List<string> numbers, string numberinblacklist, SMS.Model.EnterpriseUser user, string spnumber,string Source)
        {

            //拆分短信内容和签名
            int smsSize = 70;

            string smsSignature = user.Signature;
            if (!string.IsNullOrWhiteSpace(smsSignature))
            {
                smsSignature = "【" + user.Signature.TrimStart('【').TrimEnd('】') + "】";
            }
            int SmsSplitCount = SMS.Util.SMSSplit.GetSplitNumber(smsContent, smsSignature, out smsSize);




        
            string _content = smsContent;
            string _signature = smsSignature;
            //如果企业没有预设签名
            if (string.IsNullOrWhiteSpace(smsSignature))
            {
                if (!SMS.Util.SMSHelper.GetSignature(smsContent, ref _signature, ref _content))
                {
                    return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null, "短信没有签名");
                }
            }
          

            SMS.DTO.SMSDTO dto = new SMS.DTO.SMSDTO();
            dto.SMSNumbers = new List<SMS.Model.SMSNumber>();

            SMS.Model.SMSMessage sms = new SMS.Model.SMSMessage();
            dto.Message = sms;
            sms.Source = Source;
            sms.ID = System.Guid.NewGuid().ToString();
            sms.AccountID = user.AccountID;
            sms.Content = _content;
            sms.Signature = _signature;
            sms.SMSType = 0; //商业短信1，行业短信0
            sms.SplitNumber = SmsSplitCount;
            sms.SPNumber = spnumber;
            int numbercount = 0;


            AddSMSNumber(dto, numbers, OperatorType.notdefined);
            numbercount = numbers.Count;

 


            sms.NumberCount = numbercount;
            int Priority = SMS.Util.SMSHelper.GetPriorityByNumberCount(numbercount);
            if (sms.NumberCount == 0)
            {
                return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null, "待发送的短信数量为0");
            }

            sms.FeeTotalCount = sms.NumberCount * SmsSplitCount;
            sms.SendTime = DateTime.Now;

            sms.Channel = user.Channel;
            sms.SMSLevel = user.SMSLevel + Priority;
            sms.SPNumber = user.SPNumber;
            sms.StatusReportType = (int)user.StatusReport;
            if (user.Audit == SMS.Model.AccountAuditType.Auto)
            {
                sms.AuditType = SMS.Model.AuditType.Auto;
                sms.Status = "待发送";
            }
            else
            {
                //判断是否匹配模板
                bool match = SMSTempletMatching(user.AccountCode, smsContent);
                if (match)
                {
                    sms.AuditType = SMS.Model.AuditType.Template;
                    sms.Status = "待发送";
                }
                else
                {
                    sms.AuditType = SMS.Model.AuditType.Manual;
                    sms.Status = "待审核";
                }
            }

            var sr = SMSProxy.GetPretreatmentService().SendSMS(dto);

            if (sr.Success && !string.IsNullOrWhiteSpace(numberinblacklist))
            {
                return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(sr.Success, dto, "发送成功,以下在黑名单号码未发送：" + numberinblacklist);
            }
            else
            {
                return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(sr.Success, dto, sr.Message);

            }
        }


        /// <summary>
        /// 生成SMSNumber
        /// </summary>
        /// <param name="sms"></param>
        /// <param name="numbers"></param>
        /// <param name="operatorType"></param>
        private void AddSMSNumber(SMS.DTO.SMSDTO sms, List<string> numbers, SMS.Model.OperatorType operatorType)
        {
            if (numbers.Count > 0)
            {
                SMS.Model.SMSNumber sn = new SMS.Model.SMSNumber();
                sn.SMSID = sms.Message.ID;
                sn.ID = System.Guid.NewGuid().ToString();
                sn.Operator = operatorType;
                sn.Numbers = string.Join(",", numbers);
                sn.NumberCount = numbers.Count;
                sms.SMSNumbers.Add(sn);
            }
        }
        /// <summary>
        /// 短信发送
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="smsContent"></param>
        /// <param name="numbers"></param>
        /// <param name="spNumber"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<SMS.DTO.SMSDTO> SendSMS(string account, string password, string smsContent, List<string> numbers, string spNumber,string Source)
        {
            try
            {
                if (string.IsNullOrEmpty(account))
                {
                    return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null , "帐号不能为空");
                }
                if (string.IsNullOrEmpty(spNumber))
                {
                    return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null, "短信spNumber不能为空");
                }
                if (numbers.Count == 0)
                {
                    return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null , "短信接收号码不能为空");
                }

                if (string.IsNullOrEmpty(smsContent)) return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null , "短信内容不能为空");
                if (numbers.Count > 1000)
                {
                    return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null , "接收短信的号码过多，应少于1000个号码");
                }

                #region  by lmw 2016-03-10 企业黑名单过滤

                string numberinblacklist = "";
                try
                {
                    numberinblacklist = EnterpriseBlackList.Instance.CheckNumber(account, numbers);

                    if (!string.IsNullOrWhiteSpace(numberinblacklist))
                    {
                        // return new SMS.Model.RPCResult<PlainSMS>(false, sms, "号码中含有在黑名单号码：" + numberinblacklist);

                        var blackNumbers = EnterpriseBlackList.Instance.GetEnterpriseBlackNumbers(account);
                        numbers = (from n in numbers where !blackNumbers.Any(b => b == n) select n).ToList();
                    }
                    if (numbers.Count == 0)
                    {
                        return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null, "号码全部在黑名单中：" + numberinblacklist);
                    }
                }
                catch (Exception ex)
                {

                }

                #endregion

                try
                {
                    SMS.Model.EnterpriseUser user = AccountServer.Instance.GetAccount(account);
                    if (user == null)
                    {
                        return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null , "用户不存在");
                    }
                    if (!user.Enabled)
                    {
                        return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null, "此帐户已被禁止发送短信");
                    }
                    if (!user.IsOpen) return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null , "此用户未开放对外接口功能");
                    if (DESEncrypt.Encrypt(password, string.IsNullOrEmpty(user.SecretKey) ? user.AccountID : user.SecretKey) != user.AppPassword)
                        return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null, "密码不正确");

                    return SendSMSDTO(smsContent, numbers, numberinblacklist, user, spNumber,Source);
                }
                catch
                {
                    return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null , "短信出现异常");
                }
            }
            catch
            {
                return new SMS.Model.RPCResult<SMS.DTO.SMSDTO>(false, null, "发送短信出现错误");
            }
        }
        /// <summary>
        /// 获取用户上行短信
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMS.Model.MOSMS>> GetSMS(string account, string pass)
        {
            try
            {
                if (string.IsNullOrEmpty(account))
                {
                    return new SMS.Model.RPCResult<List<SMS.Model.MOSMS>>(false, null, "用户不能为空");
                }
                SMS.Model.EnterpriseUser user = AccountServer.Instance.GetAccount(account);
                if (user == null)
                {
                    return new SMS.Model.RPCResult<List<SMS.Model.MOSMS>>(false, null, "无此用户");
                }
                if (!user.IsOpen) return new SMS.Model.RPCResult<List<SMS.Model.MOSMS>>(false, null, "此用户未开放对外接口功能");
                if (DESEncrypt.Encrypt(pass, string.IsNullOrEmpty(user.SecretKey) ? user.AccountID : user.SecretKey) != user.AppPassword) return new SMS.Model.RPCResult<List<SMS.Model.MOSMS>>(false, null, "密码不正确");

                if (string.IsNullOrEmpty(user.SPNumber)) return new SMS.Model.RPCResult<List<SMS.Model.MOSMS>>(false, null, "企业SPNumber是空的");
                SMS.Model.RPCResult<List<SMS.Model.MOSMS>> r = SMSProxy.GetPretreatmentService().GetMOSmsBySPNumber(user.AccountID, user.SPNumber);
                return r;
            }
            catch
            {
                return new SMS.Model.RPCResult<List<SMS.Model.MOSMS>>(false, null, "获取用户上行短信出现错误");
            }
        }
        /// <summary>
        /// 获取用户一业务短信状态报告明细
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="msgID">业务号</param>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMS.Model.StatusReport>> GetReport(string account, string password, string msgID)
        {
            try
            {
                if (string.IsNullOrEmpty(account))
                {
                    return new SMS.Model.RPCResult<List<SMS.Model.StatusReport>>(false, null, "用户不能为空");
                }
                SMS.Model.EnterpriseUser user = AccountServer.Instance.GetAccount(account);
                if (user == null)
                {
                    return new SMS.Model.RPCResult<List<SMS.Model.StatusReport>>(false, null, "无此用户");
                }
                if (!user.IsOpen) return new SMS.Model.RPCResult<List<SMS.Model.StatusReport>>(false, null, "此用户未开放对外接口功能");
                if (DESEncrypt.Encrypt(password, string.IsNullOrEmpty(user.SecretKey) ? user.AccountID : user.SecretKey) != user.AppPassword) return new SMS.Model.RPCResult<List<SMS.Model.StatusReport>>(false, null, "密码不正确");

                SMS.Model.RPCResult<List<SMS.Model.StatusReport>> r = SMSProxy.GetPretreatmentService().GetCustomStatusReportBySerialNumber(msgID);
                return r;
            }
            catch
            {
                return new SMS.Model.RPCResult<List<SMS.Model.StatusReport>>(false, null, "获取用户短信状态报告出现错误");
            }
        }
        /// <summary>
        /// 获取用户短信状态报告明细
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMS.Model.StatusReport>> GetReport(string account, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(account))
                {
                    return new SMS.Model.RPCResult<List<SMS.Model.StatusReport>>(false, null, "用户不能为空");
                }
                SMS.Model.EnterpriseUser user = AccountServer.Instance.GetAccount(account);
                if (user == null)
                {
                    return new SMS.Model.RPCResult<List<SMS.Model.StatusReport>>(false, null, "无此用户");
                }
                if (!user.IsOpen) return new SMS.Model.RPCResult<List<SMS.Model.StatusReport>>(false, null, "此用户未开放对外接口功能");
                if (DESEncrypt.Encrypt(password, string.IsNullOrEmpty(user.SecretKey) ? user.AccountID : user.SecretKey) != user.AppPassword) return new SMS.Model.RPCResult<List<SMS.Model.StatusReport>>(false, null, "密码不正确");

                SMS.Model.RPCResult<List<SMS.Model.StatusReport>> r = SMSProxy.GetPretreatmentService().GetCustomStatusReportByAccount(user.AccountID);
                return r;
            }
            catch
            {
                return new SMS.Model.RPCResult<List<SMS.Model.StatusReport>>(false, null, "获取用户短信状态报告出现错误");
            }
        }
       
        #endregion

        #region 直客端接口
  
        /// <summary>
        /// 更新企业信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult UpdateEnterprise(SMS.Model.EnterpriseUser user)
        {
            try
            {
                SMS.Model.EnterpriseUser u = AccountServer.Instance.GetAccount(user.AccountCode);
                if (u == null) return new SMS.Model.RPCResult(false, "不存在此企业用户");
                bool ok = AccountServer.Instance.UpdateAccount(user, "info");
                if (ok)
                {
                    ok = AccountServer.Instance.UpdateAccount(user, "set");
                    if (ok)
                    {
                        return new SMS.Model.RPCResult(true, "");
                    }
                }
                return new SMS.Model.RPCResult(false, "更新企业设置失败");
            }
            catch
            {
                return new SMS.Model.RPCResult(false, "修改企业设置失败");
            }
        }
        /// <summary>
        /// 更新企业短信设置
        /// </summary>
        /// <param name="eusms"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult UpdateEnterpriseSMS(EnterpriseUser eusms)
        {
            try
            {
                bool ok = AccountServer.Instance.UpdateAccountSMS(eusms);
                if (ok)
                {
                    return new SMS.Model.RPCResult(true, "");
                }
                else
                {
                    return new SMS.Model.RPCResult(false, "修改短信设置失败");
                }
            }
            catch
            {
                return new SMS.Model.RPCResult(false, "修改短信设置失败");
            }
        }
        /// <summary>
        /// 修改企业密码
        /// </summary>
        /// <param name="accountCode"></param>
        /// <param name="oldPass"></param>
        /// <param name="newPass"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult ChangeEnterprisePass(string accountCode, string oldPass, string newPass)
        {
            try
            {
                SMS.Model.EnterpriseUser user = AccountServer.Instance.GetAccount(accountCode);
                if (user == null)
                {
                    return new SMS.Model.RPCResult(false, "企业不存在");
                }
                if (DESEncrypt.Encrypt(oldPass) != user.Password) return new SMS.Model.RPCResult(false, "原始密码不正确");
                bool ok = AccountServer.Instance.ChangePass(accountCode, newPass);
                if (ok)
                {
                    return new SMS.Model.RPCResult(true, "");
                }
                return new SMS.Model.RPCResult(false, "修改密码失败");
            }
            catch
            {
                return new SMS.Model.RPCResult(false, "修改密码失败");
            }
        }
       
       
      
        /// <summary>
        /// 根据帐号获取历史统计报告（数据库不包含缓存）
        /// </summary>
        /// <param name="account"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMS.Model.ReportStatistics>> GetStatisticsReportByAccount(string account, DateTime beginTime, DateTime endTime)
        {
            if (string.IsNullOrEmpty(account)) return new SMS.Model.RPCResult<List<SMS.Model.ReportStatistics>>(false, null, "企业帐号不能为空");
            SMS.Model.EnterpriseUser user = AccountServer.Instance.GetAccount(account);
            if (user == null) return new SMS.Model.RPCResult<List<SMS.Model.ReportStatistics>>(false, null, "不存在此企业用户");
            if (DateTime.Compare(beginTime, endTime) > 0)
            {
                DateTime dt = beginTime;
                beginTime = endTime;
                endTime = dt;
            }
            if (DateTime.Compare(endTime, DateTime.Now) > 0)
            {
                endTime = DateTime.Now;
            }
            if (DateTime.Compare(endTime.AddMonths(-1), beginTime) > 0)
            {
                return new SMS.Model.RPCResult<List<SMS.Model.ReportStatistics>>(false, null, "只允许查看1个月内的数据");
            }
            return SMSProxy.GetPretreatmentService().GetStatisticsReportByAccount(user.AccountID, beginTime, endTime);
        }
      
        
        #endregion

        #region 管理平台接口
        /// <summary>
        /// 添加企业
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult AddEnterprise(SMS.Model.EnterpriseUser user)
        {
            try
            {
                if (user.AccountCode == "-1") return new SMS.Model.RPCResult(false, "已存在此企业帐号");
                SMS.Model.EnterpriseUser a = AccountServer.Instance.GetAccount(user.AccountCode);
                if (a != null) return new SMS.Model.RPCResult(false, "已存在此企业帐号");
                if (string.IsNullOrEmpty(user.Signature)) return new SMS.Model.RPCResult(false, "企业签名不能为空");
                SMS.Model.Account account = new SMS.Model.Account();

                account.AccountID = "";
                account.SMSNumber = 0;
                if (!string.IsNullOrWhiteSpace(user.AccountID))
                {
                    account.AccountID = user.AccountID;
                }
                SMS.Model.RPCResult<Guid> r = SMSProxy.GetPretreatmentService().CreateAccount(account);
                if (!r.Success) return new SMS.Model.RPCResult(false, r.Message);
                user.AccountID = r.Value.ToString();
                user.Signature = "【" + user.Signature + "】";
                user.Channel = user.Channel == "-1-" ? "" : user.Channel;
                user.SecretKey = "";
                user.AppPassword = DESEncrypt.Encrypt(user.Password, user.AccountID);
                user.Password = DESEncrypt.Encrypt(user.Password);
                bool ok = AccountServer.Instance.CreateAccount(user);
                if (ok) return new SMS.Model.RPCResult(true, "创建成功");
                return new SMS.Model.RPCResult(false, "创建失败");
            }
            catch
            {
                return new SMS.Model.RPCResult(false, "创建失败");
            }
        }
        /// <summary>
        /// 删除企业
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult DelEnterprise(string account)
        {
            try
            {
                SMS.Model.EnterpriseUser user = AccountServer.Instance.GetAccount(account);
                if (user == null) return new SMS.Model.RPCResult(false, "不存在此企业用户");
                SMS.Model.RPCResult<SMS.Model.Account> rc = SMSProxy.GetPretreatmentService().GetAccount(user.AccountID);
                if (rc.Success)
                {
                    if (rc.Value.SMSNumber > 0)
                    {
                        return new SMS.Model.RPCResult(false, "此企业还剩余" + rc.Value.SMSNumber + "条短信，不能删除此企业");
                    }
                }
                else
                {
                    return new SMS.Model.RPCResult(false, rc.Message);
                }
                SMS.Model.RPCResult r = SMSProxy.GetPretreatmentService().DelAccount(user.AccountID);
                if (!r.Success) return new SMS.Model.RPCResult(false, r.Message);
                if (user.IsAgent)
                {
                    List<SMS.Model.EnterpriseUser> list = AccountServer.Instance.GetAccounts().Where(c => c.ParentAccountCode == user.AccountCode).ToList();
                    foreach (var v in list)
                    {
                        v.ParentAccountCode = "-1";
                        AccountServer.Instance.UpdateAccount(v, "set");
                    }
                }
                bool ok = AccountServer.Instance.DelAccount(user.AccountCode);
                if (ok)
                {
                    AccountServer.Instance.DelAuditEnterprise(user.AccountCode);
                    return new SMS.Model.RPCResult(true, "删除成功");
                }
                return new SMS.Model.RPCResult(false, "删除失败");
            }
            catch
            {
                return new SMS.Model.RPCResult(false, "删除企业失败");
            }
        }
        /// <summary>
        /// 获取代理商企业
        /// </summary>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMS.Model.EnterpriseUser>> GetAgentEnterprises()
        {
            try
            {
                List<SMS.Model.EnterpriseUser> list = AccountServer.Instance.GetAccounts().Where(c => c.IsAgent == true).ToList();
                return new SMS.Model.RPCResult<List<SMS.Model.EnterpriseUser>>(true, list, "");
            }
            catch
            {
                return new SMS.Model.RPCResult<List<SMS.Model.EnterpriseUser>>(false, null, "获取代理商企业失败");
            }
        }
        /// <summary>
        /// 获取需要审核的企业
        /// </summary>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMS.Model.EnterpriseUser>> GetAuditEnterprises()
        {
            try
            {
                List<SMS.Model.EnterpriseUser> list = AccountServer.Instance.GetAuditEnterprise();
                return new SMS.Model.RPCResult<List<SMS.Model.EnterpriseUser>>(true, list, "");
            }
            catch
            {
                return new SMS.Model.RPCResult<List<SMS.Model.EnterpriseUser>>(false, null, "获取审核企业失败");
            }
        }
        /// <summary>
        /// 企业审核
        /// </summary>
        /// <param name="enterprise"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult UpdateAuditEnterprise(SMS.Model.AuditEnterprise enterprise)
        {
            try
            {
                enterprise.AuditTime = DateTime.Now;
                bool ok = AccountServer.Instance.UpdateAuditEnterprise(enterprise);
                if (ok) return new SMS.Model.RPCResult(true, "");
                return new SMS.Model.RPCResult(false, "审核失败");
            }
            catch
            {
                return new SMS.Model.RPCResult(false, "企业审核失败");
            }
        }
        /// <summary>
        /// 获取所有终端用户
        /// </summary>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMS.Model.EnterpriseUser>> GetLowerEnterprise()
        {
            try
            {
                List<SMS.Model.EnterpriseUser> list = AccountServer.Instance.GetAccounts().Where(c => c.IsAgent == false).ToList();
                List<SMS.Model.EnterpriseUser> li = AccountServer.Instance.GetUnableManageEnterprise();
                foreach (var v in li)
                {
                    if (list.Contains(v))
                    {
                        list.Remove(v);
                    }
                }
                return new SMS.Model.RPCResult<List<SMS.Model.EnterpriseUser>>(true, list, "");
            }
            catch
            {
                return new SMS.Model.RPCResult<List<SMS.Model.EnterpriseUser>>(false, null, "获取失败");
            }
        }
        /// <summary>
        /// 更新企业设置
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult UpdateAccountSetting(SMS.Model.EnterpriseUser user)
        {
            SMS.Model.EnterpriseUser old = AccountServer.Instance.GetAccount(user.AccountCode);
            if (old.IsAgent != user.IsAgent)
            {
                if (!user.IsAgent)
                {
                    List<SMS.Model.EnterpriseUser> list = AccountServer.Instance.GetAccounts().Where(c => c.ParentAccountCode == user.AccountCode).ToList();
                    foreach (var v in list)
                    {
                        v.ParentAccountCode = "-1";
                        AccountServer.Instance.UpdateAccount(v, "set");
                    }
                }
            }
            bool ok = AccountServer.Instance.UpdateAccount(user, "set");
            if (ok) return new SMS.Model.RPCResult(true, "");
            return new SMS.Model.RPCResult(false, "操作失败");
        }
        /// <summary>
        /// 更新企业资料设置
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult UpdateAccontInfo(SMS.Model.EnterpriseUser user)
        {
            try
            {
                SMS.Model.EnterpriseUser old = AccountServer.Instance.GetAccount(user.AccountCode);
                if (old.IsAgent != user.IsAgent)
                {
                    if (!user.IsAgent)
                    {
                        List<SMS.Model.EnterpriseUser> list = AccountServer.Instance.GetAccounts().Where(c => c.ParentAccountCode == user.AccountCode).ToList();
                        foreach (var v in list)
                        {
                            v.ParentAccountCode = "-1";
                            AccountServer.Instance.UpdateAccount(v, "info");
                        }
                    }
                }
                bool ok = AccountServer.Instance.UpdateAccount(user, "info");
                if (ok) return new SMS.Model.RPCResult(true, "");
                return new SMS.Model.RPCResult(false, "操作失败");
            }
            catch
            {
                return new SMS.Model.RPCResult(false, "操作失败");
            }
        }
        /// <summary>
        /// 统计短信总营收(代理商下面的终端用户不在统计之列)
        /// </summary>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMS.Model.ChargeStatics>> GetChargeStatics(string account)
        {
            try
            {
                List<SMS.Model.EnterpriseUser> list = AccountServer.Instance.GetAccounts();
                if (!string.IsNullOrEmpty(account))
                {
                    list = list.FindAll(c => c.Name.Contains(account) || c.AccountCode.Contains(account));
                }
                List<SMS.Model.ChargeStatics> cs = DAL.ChargeRecord.GetChargeStatics();
                List<SMS.Model.ChargeStatics> mo = new List<SMS.Model.ChargeStatics>();
                foreach (var v in list)
                {
                    SMS.Model.EnterpriseUser user = AccountServer.Instance.GetAccount(v.AccountCode);
                    if (user == null)
                    {
                        continue;
                    }
                    if (user.ParentAccountCode != "-1")
                    {
                        continue;
                    }
                    SMS.Model.ChargeStatics mc = cs.Find(c => c.Enterprese == v.AccountCode);// new Model.ChargeStatics();
                    if (mc == null)
                    {
                        mc = new SMS.Model.ChargeStatics();
                        mc.Enterprese = v.AccountCode;
                        mc.TotalMoney = 0;
                        mc.SMSCount = 0;
                    }
                    SMS.Model.RPCResult<SMS.Model.Account> r = SMSProxy.GetPretreatmentService().GetAccount(user.AccountID);
                    if (r.Success)
                    {
                        mc.RemainSMSNumber = r.Value.SMSNumber;
                    }
                    else
                    {
                        mc.RemainSMSNumber = 0;
                    }
                    mo.Add(mc);

                }
                return new SMS.Model.RPCResult<List<SMS.Model.ChargeStatics>>(true, mo, "");
            }
            catch
            {
                return new SMS.Model.RPCResult<List<SMS.Model.ChargeStatics>>(false, null, "统计短信总营收失败");
            }
        }
        /// <summary>
        /// 企业帐号充值
        /// </summary>
        /// <param name="chargeRecord"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult AccountPrepaid(SMS.Model.ChargeRecord chargeRecord)
        {
            try
            {
                SMS.Model.EnterpriseUser user = AccountServer.Instance.GetAccount(chargeRecord.PrepaidAccount);
                SMS.Model.RPCResult<SMS.Model.Account> rBalance = SMSProxy.GetPretreatmentService().GetAccount(user.AccountID);
                if (rBalance.Success)
                {
                    chargeRecord.RemainSMSCount = rBalance.Value.SMSNumber;
                }
                if (user == null) return new SMS.Model.RPCResult(false, "帐号不存在");
                chargeRecord.ChargeFlag = 0;
                chargeRecord.Remark = "管理平台充值：" + chargeRecord.Remark;
                SMS.Model.RPCResult r = SMSProxy.GetPretreatmentService().AccountPrepaid(user.AccountID, (uint)chargeRecord.SMSCount, chargeRecord.OperatorAccount);
                if (r.Success)
                {
                    DAL.ChargeRecord.Add(chargeRecord);
                    return new SMS.Model.RPCResult(true, "");
                }
                return new SMS.Model.RPCResult(false, r.Message);
            }
            catch
            {
                return new SMS.Model.RPCResult(false, "充值异常");
            }
        }
        /// <summary>
        /// 根据企业获取短信统计报告（1年之内数据统计）
        /// </summary>
        /// <param name="account"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMS.Model.ReportStatistics>> GetStatisticsReportAllByAccount(string account, DateTime beginTime, DateTime endTime)
        {
            if (DateTime.Compare(beginTime, endTime) > 0)
            {
                DateTime dt = beginTime;
                beginTime = endTime;
                endTime = dt;
            }
            if (DateTime.Compare(endTime, DateTime.Now) > 0)
            {
                endTime = DateTime.Now;
            }
            if (DateTime.Compare(endTime.AddMonths(-1), beginTime) > 0)
            {
                return new SMS.Model.RPCResult<List<SMS.Model.ReportStatistics>>(false, null, "只允许查看1个月内的数据");
            }
            if (string.IsNullOrEmpty(account)) return new SMS.Model.RPCResult<List<SMS.Model.ReportStatistics>>(false, null, "帐号不能为空");
            SMS.Model.EnterpriseUser user = AccountServer.Instance.GetAccount(account);
            if (user == null) return new SMS.Model.RPCResult<List<SMS.Model.ReportStatistics>>(false, null, "帐号不存在");
            return SMSProxy.GetPretreatmentService().GetStatisticsReportAllByAccount(user.AccountID, beginTime, endTime);
        }
       
       
        /// <summary>
        /// 根据企业获取上行短信
        /// </summary>
        /// <param name="account"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMS.Model.MOSMS>> GetMOSMS(string account, DateTime beginTime, DateTime endTime)
        {
            if (DateTime.Compare(beginTime, endTime) > 0)
            {
                DateTime dt = beginTime;
                beginTime = endTime;
                endTime = dt;
            }
            if (DateTime.Compare(endTime, DateTime.Now) > 0)
            {
                endTime = DateTime.Now;
            }
            if (DateTime.Compare(endTime.AddMonths(-1), beginTime) > 0)
            {
                return new SMS.Model.RPCResult<List<SMS.Model.MOSMS>>(false, null, "只允许查看1个月内的数据");
            }
            if (string.IsNullOrEmpty(account)) return new SMS.Model.RPCResult<List<SMS.Model.MOSMS>>(false, null, "帐号不能为空");
            SMS.Model.EnterpriseUser user = AccountServer.Instance.GetAccount(account);
            if (user == null) return new SMS.Model.RPCResult<List<SMS.Model.MOSMS>>(false, null, "用户不存在");
            if (string.IsNullOrEmpty(user.SPNumber)) return new SMS.Model.RPCResult<List<SMS.Model.MOSMS>>(false, null, "企业SPNumber是空的");
            return SMSProxy.GetPretreatmentService().GetMOSMS(user.SPNumber, beginTime, endTime);
        }

        #region 短信平台接口
        /// <summary>
        /// 获取网关配置信息
        /// </summary>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMS.Model.GatewayConfiguration>> GetGatewayConfigs()
        {
            return SMSProxy.GetPretreatmentService().GetGatewayConfigs();
        }
        /// <summary>
        /// 添加黑名单
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult AddBlacklist(List<string> numbers)
        {
            return SMSProxy.GetPretreatmentService().AddBlacklist(numbers);
        }
        /// <summary>
        /// 删除黑名单
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult DelBlacklist(List<string> numbers)
        {
            return SMSProxy.GetPretreatmentService().DelBlacklist(numbers);
        }
        /// <summary>
        /// 获取黑名单
        /// </summary>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<string>> GetBlacklist()
        {
            return SMSProxy.GetPretreatmentService().GetBlacklist();
        }
        /// <summary>
        /// 添加敏感词组
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult AddKeywordsGroup(string keyGroup, string remark)
        {
            if (keyGroup == "-1") return new SMS.Model.RPCResult(false, "已存在此敏感词组");
            return SMSProxy.GetPretreatmentService().AddKeywordsGroup(keyGroup, remark);
        }

        /// <summary>
        /// 删除敏感词组
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult DelKeywordsGroup(string keyGroup)
        {
            var r = SMSProxy.GetPretreatmentService().DelKeywordGroup(keyGroup);
            return r;
        }
        /// <summary>
        /// 添加敏感词类别
        /// </summary>
        /// <param name="type"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult AddKeywordsType(string type, string remark)
        {
            if (type == "-1") return new SMS.Model.RPCResult(false, "已存在此敏感词类别");
            return SMSProxy.GetPretreatmentService().AddKeywordsType(type, remark);
        }

        /// <summary>
        /// 获取敏感词类别
        /// </summary>
        /// <returns></returns>
        public SMS.Model.RPCResult<Dictionary<string, string>> GetKeywordsTypes()
        {
            return SMSProxy.GetPretreatmentService().GetKeywordsTypes();
        }

        /// <summary>
        /// 添加关键词
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult AddKeywords(string keyGroup, List<SMS.Model.Keywords> keywords)
        {
            return SMSProxy.GetPretreatmentService().AddKeywords(keyGroup, keywords);
        }
        /// <summary>
        /// 删除关键词
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult DelKeywords(string keyGroup, List<string> keywords)
        {
            return SMSProxy.GetPretreatmentService().DelKeywords(keyGroup, keywords);
        }
        /// <summary>
        /// 获取关键词组内的关键词
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMS.Model.Keywords>> GetKeywords(string keyGroup)
        {
            return SMSProxy.GetPretreatmentService().GetKeywords(keyGroup);
        }
        /// <summary>
        /// 根据类型获取关键词
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMS.Model.Keywords>> GetKeywordsByType(string type)
        {
            return SMSProxy.GetPretreatmentService().GetKeywordsByType(type);
        }
        /// <summary>
        /// 敏感词状态启用与否
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <param name="keywords"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult KeywordsEnabled(string keyGroup, string keywords, bool enabled)
        {
            return SMSProxy.GetPretreatmentService().KeywordsEnabled(keyGroup, keywords, enabled);
        }

        /// <summary>
        /// 获取关键词组
        /// </summary>
        /// <returns></returns>
        public SMS.Model.RPCResult<Dictionary<string, string>> GetKeyGroups()
        {
            return SMSProxy.GetPretreatmentService().GetKeyGroups();
        }
        /// <summary>
        /// 添加关键词组与网关绑定信息
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <param name="gateway"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult AddkeyGroupGatewayBind(string keyGroup, string gateway)
        {
            return SMSProxy.GetPretreatmentService().AddkeyGroupGatewayBind(keyGroup, gateway);
        }
        /// <summary>
        /// 获取网关绑定的关键词组
        /// </summary>
        /// <param name="gateway"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<string> GetKeyGroupGatewayBinds(string gateway)
        {
            return SMSProxy.GetPretreatmentService().GetKeyGroupGatewayBinds(gateway);
        }
        /// <summary>
        /// 添加网关配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult AddGatewayConfig(SMS.Model.GatewayConfiguration config)
        {
            return SMSProxy.GetPretreatmentService().AddGatewayConfig(config);
        }
        /// <summary>
        /// 更新网关配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult UpdateGatewayConfig(SMS.Model.GatewayConfiguration config)
        {
            return SMSProxy.GetPretreatmentService().UpdateGatewayConfig(config);
        }
        /// <summary>
        /// 获取网关配置
        /// </summary>
        /// <param name="gateway"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<SMS.Model.GatewayConfiguration> GetGatewayConfig(string gateway)
        {
            return SMSProxy.GetPretreatmentService().GetGatewayConfig(gateway);
        }
        /// <summary>
        /// 删除网关配置
        /// </summary>
        /// <param name="gateway"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult DelGatewayConfig(string gateway)
        {
            return SMSProxy.GetPretreatmentService().DelGatewayConfig(gateway);
        }

        ///// <summary>
        ///// 获取预处理短信总数
        ///// </summary>
        ///// <param name="accountID"></param>
        ///// <returns></returns>
        //public SMS.Model.RPCResult<int> GetSMSCountByAccount(string account)
        //{
        //    if (string.IsNullOrEmpty(account)) return new SMS.Model.RPCResult<int>(false, 0, "帐号是空的");
        //    Model.EnterpriseUser user = AccountServer.Instance.GetAccount(account);
        //    if (user == null) return new SMS.Model.RPCResult<int>(false, 0, "用户不存在");
        //    return ZHSMSProxy.GetPretreatmentService().GetSMSCountByAccount(user.AccountID);
        //}
        ///// <summary>
        ///// 设置未发送短信级别
        ///// </summary>
        ///// <param name="serialNumber"></param>
        ///// <param name="level"></param>
        ///// <param name="operatorAccount"></param>
        ///// <returns></returns>
        //public SMS.Model.RPCResult SetSMSLevel(Guid serialNumber, SMS.Model.LevelType level, string operatorAccount)
        //{
        //    return ZHSMSProxy.GetPretreatmentService().SetSMSLevel(serialNumber, level, operatorAccount);
        //}
        ///// <summary>
        ///// 审核短信
        ///// </summary>
        ///// <param name="serialNumber"></param>
        ///// <param name="audit"></param>
        ///// <param name="operatorAccount"></param>
        ///// <returns></returns>
        //public SMS.Model.RPCResult AuditSMS(Guid serialNumber, bool audit, string operatorAccount, string resultCase)
        //{
        //    return ZHSMSProxy.GetPretreatmentService().AuditSMS(serialNumber, audit, operatorAccount, resultCase);
        //}
        ///// <summary>
        ///// 删除待发送的短信
        ///// </summary>
        ///// <param name="serialNumbers"></param>
        ///// <returns></returns>
        //public SMS.Model.RPCResult DelSMS(List<Guid> serialNumbers)
        //{
        //    return ZHSMSProxy.GetPretreatmentService().DelSMS(serialNumbers);
        //}
        ///// <summary>
        ///// 根据企业获取待审核的短信
        ///// </summary>
        ///// <param name="accountID"></param>
        ///// <param name="beginTime"></param>
        ///// <param name="endTime"></param>
        ///// <returns></returns>
        //public SMS.Model.RPCResult<List<PlainSMS>> GetSMSAuditByAccount(string account, DateTime beginTime, DateTime endTime)
        //{
        //    if (string.IsNullOrEmpty(account)) return new SMS.Model.RPCResult<List<PlainSMS>>(false, null, "帐号是空的");
        //    Model.EnterpriseUser user = AccountServer.Instance.GetAccount(account);
        //    if (user == null) return new SMS.Model.RPCResult<List<PlainSMS>>(false, null, "用户不存在");
        //    if (DateTime.Compare(beginTime, endTime) > 0)
        //    {
        //        DateTime dt = beginTime;
        //        beginTime = endTime;
        //        endTime = dt;
        //    }
        //    if (DateTime.Compare(endTime, DateTime.Now) > 0)
        //    {
        //        endTime = DateTime.Now;
        //    }
        //    if (DateTime.Compare(endTime.AddMonths(-1), beginTime) > 0)
        //    {
        //        return new SMS.Model.RPCResult<List<PlainSMS>>(false, null, "时间范围不允许超过1个月");
        //    }
        //    return ZHSMSProxy.GetPretreatmentService().GetSMSAuditByAccount(user.AccountID, beginTime, endTime);
        //}
        ///// <summary>
        ///// 根据时间获取全部审核的短信
        ///// </summary>
        ///// <param name="beginTime"></param>
        ///// <param name="endTime"></param>
        ///// <returns></returns>
        //public SMS.Model.RPCResult<List<PlainSMS>> GetSMSByAudit(DateTime beginTime, DateTime endTime)
        //{
        //    if (DateTime.Compare(beginTime, endTime) > 0)
        //    {
        //        DateTime dt = beginTime;
        //        beginTime = endTime;
        //        endTime = dt;
        //    }
        //    if (DateTime.Compare(endTime, DateTime.Now) > 0)
        //    {
        //        endTime = DateTime.Now;
        //    }
        //    if (DateTime.Compare(endTime.AddMonths(-1), beginTime) > 0)
        //    {
        //        return new SMS.Model.RPCResult<List<PlainSMS>>(false, null, "时间范围不允许超过1个月");
        //    }
        //    return ZHSMSProxy.GetPretreatmentService().GetSMSByAudit(beginTime, endTime);
        //}
        ///// <summary>
        ///// 获取全部审核失败短信
        ///// </summary>
        ///// <param name="beginTime"></param>
        ///// <param name="endTime"></param>
        ///// <returns></returns>
        //public SMS.Model.RPCResult<List<SMS.Model.FailureSMS>> GetSMSByAuditFailure(DateTime beginTime, DateTime endTime)
        //{
        //    if (DateTime.Compare(beginTime, endTime) > 0)
        //    {
        //        DateTime dt = beginTime;
        //        beginTime = endTime;
        //        endTime = dt;
        //    }
        //    if (DateTime.Compare(endTime, DateTime.Now) > 0)
        //    {
        //        endTime = DateTime.Now;
        //    }
        //    if (DateTime.Compare(endTime.AddMonths(-1), beginTime) > 0)
        //    {
        //        return new SMS.Model.RPCResult<List<SMS.Model.FailureSMS>>(false, null, "只允许查看1个月内的数据");
        //    }

        //    return ZHSMSProxy.GetPretreatmentService().GetSMSByAuditFailure(beginTime, endTime);
        //}
        ///// <summary>
        ///// 获取短信审核记录
        ///// </summary>
        ///// <param name="beginTime"></param>
        ///// <param name="endTime"></param>
        ///// <returns></returns>
        //public SMS.Model.RPCResult<List<SMS.Model.AuditRecord>> GetAuditRecords(DateTime beginTime, DateTime endTime)
        //{
        //    try
        //    {
        //        if (DateTime.Compare(beginTime, endTime) > 0)
        //        {
        //            DateTime dt = beginTime;
        //            beginTime = endTime;
        //            endTime = dt;
        //        }
        //        if (DateTime.Compare(endTime, DateTime.Now) > 0)
        //        {
        //            endTime = DateTime.Now;
        //        }
        //        if (DateTime.Compare(endTime.AddMonths(-1), beginTime) > 0)
        //        {
        //            return new SMS.Model.RPCResult<List<SMS.Model.AuditRecord>>(false, null, "只允许查看1个月内的数据");
        //        }

        //        return ZHSMSProxy.GetPretreatmentService().GetAuditRecords(beginTime, endTime);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageTools.MessageHelper.Instance.WirteError("GetAuditRecords 发生了异常：", ex);
        //        return new SMS.Model.RPCResult<List<SMS.Model.AuditRecord>>(false, null, "查询时发生了异常");
        //    }
        //}
        ///// <summary>
        ///// 获取短信级别调整记录
        ///// </summary>
        ///// <param name="beginTime"></param>
        ///// <param name="endTime"></param>
        ///// <returns></returns>
        //public SMS.Model.RPCResult<List<SMS.Model.LevelModifyRecord>> GetLevelModifyRecords(DateTime beginTime, DateTime endTime)
        //{
        //    if (DateTime.Compare(beginTime, endTime) > 0)
        //    {
        //        DateTime dt = beginTime;
        //        beginTime = endTime;
        //        endTime = dt;
        //    }
        //    if (DateTime.Compare(endTime, DateTime.Now) > 0)
        //    {
        //        endTime = DateTime.Now;
        //    }
        //    if (DateTime.Compare(endTime.AddMonths(-1), beginTime) > 0)
        //    {
        //        return new SMS.Model.RPCResult<List<SMS.Model.LevelModifyRecord>>(false, null, "只允许查看1个月内的数据");
        //    }
        //    return ZHSMSProxy.GetPretreatmentService().GetLevelModifyRecords(beginTime, endTime);
        //}
        #region 通道配置
        /// <summary>
        /// 添加通道
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult AddChannel(SMS.Model.Channel channel)
        {
            return SMSProxy.GetPretreatmentService().AddChannel(channel);
        }
        /// <summary>
        /// 更新通道
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult UpdateChannel(SMS.Model.Channel channel)
        {
            return SMSProxy.GetPretreatmentService().UpdateChannel(channel);
        }
        /// <summary>
        /// 获取所有通道
        /// </summary>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMS.Model.Channel>> GetChannels()
        {
            return SMSProxy.GetPretreatmentService().GetChannels();
        }
        /// <summary>
        /// 获取通道
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<SMS.Model.Channel> GetSMSChannel(string channel)
        {
            return SMSProxy.GetPretreatmentService().GetChannel(channel);
        }
        /// <summary>
        /// 删除通道
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult DelChannel(string channel)
        {
            return SMSProxy.GetPretreatmentService().DelChannel(channel);
        }
        /// <summary>
        /// 添加通道绑定网关信息
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="gateways"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult AddChannelGatewayBind(string channel, List<string> gateways)
        {
            return SMSProxy.GetPretreatmentService().AddChannelGatewayBind(channel, gateways);
        }
        /// <summary>
        /// 获取通道绑定的网关
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<string>> GetGatewaysByChannel(string channel)
        {
            return SMSProxy.GetPretreatmentService().GetGatewaysByChannel(channel);
        }
        #endregion
        #endregion

        #endregion

        #region 公共接口
        /// <summary>
        /// 获取用户余额
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pass">明文密码</param>
        /// <returns></returns>
        public SMS.Model.RPCResult<SMS.Model.UserBalance> GetBalanceByPlainPass(string account, string pass)
        {
            try
            {
                if (string.IsNullOrEmpty(account))
                {
                    return new SMS.Model.RPCResult<SMS.Model.UserBalance>(false, null, "用户不能为空");
                }
                SMS.Model.EnterpriseUser user = AccountServer.Instance.GetAccount(account);
                if (user == null)
                {
                    return new SMS.Model.RPCResult<SMS.Model.UserBalance>(false, null, "无此用户");
                }
                if (DESEncrypt.Encrypt(pass) != user.Password)
                {
                    return new SMS.Model.RPCResult<SMS.Model.UserBalance>(false, null, "密码错误");
                }
                SMS.Model.RPCResult<SMS.Model.Account> r = SMSProxy.GetPretreatmentService().GetAccount(user.AccountID);
                if (r.Success)
                {
                    SMS.Model.UserBalance o = new SMS.Model.UserBalance();
                    o.SmsBalance = r.Value.SMSNumber;
                    o.MmsBalance = 0;
                    return new SMS.Model.RPCResult<SMS.Model.UserBalance>(true, o, "");
                }
                return new SMS.Model.RPCResult<SMS.Model.UserBalance>(false, null, "");
            }
            catch
            {
                return new SMS.Model.RPCResult<SMS.Model.UserBalance>(false, null, "获取用户余额出现错误");
            }
        }

        ///// <summary>
        ///// 获取代理商终端企业
        ///// </summary>
        ///// <param name="account"></param>
        ///// <returns></returns>
        //public SMS.Model.RPCResult<List<Model.EnterpriseUser>> GetLowerEnterprises(string account)
        //{
        //    try
        //    {
        //        Model.EnterpriseUser user = AccountServer.Instance.GetAccount(account);
        //        if (user.IsAgent)
        //        {
        //            List<Model.EnterpriseUser> list = AccountServer.Instance.GetAccounts().Where(c => c.ParentAccountCode == account).ToList();
        //            if (list == null) return new SMS.Model.RPCResult<List<Model.EnterpriseUser>>(false, null, "无终端用户");
        //            List<Model.EnterpriseUser> li = AccountServer.Instance.GetUnableManageEnterprise();
        //            foreach (var v in li)
        //            {
        //                if (list.Contains(v))
        //                {
        //                    list.Remove(v);
        //                }
        //            }
        //            return new SMS.Model.RPCResult<List<Model.EnterpriseUser>>(true, list, "");
        //        }
        //        else
        //        {
        //            return new SMS.Model.RPCResult<List<Model.EnterpriseUser>>(false, null, "不是代理商企业");
        //        }
        //    }
        //    catch
        //    {
        //        return new SMS.Model.RPCResult<List<Model.EnterpriseUser>>(false, null, "获取代理商终端客户失败");
        //    }
        //}
        ///// <summary>
        ///// 获取未审核和审核不通过的企业
        ///// </summary>
        ///// <param name="account"></param>
        ///// <returns></returns>
        //public SMS.Model.RPCResult<List<Model.AuditEnterprise>> GetFailueOrUnAuditEnterprises(string account)
        //{
        //    try
        //    {
        //        List<Model.AuditEnterprise> ls = new List<Model.AuditEnterprise>();
        //        Model.EnterpriseUser user = AccountServer.Instance.GetAccount(account);
        //        if (user.IsAgent)
        //        {
        //            List<Model.EnterpriseUser> list = AccountServer.Instance.GetAccounts().Where(c => c.ParentAccountCode == account).ToList();
        //            if (list == null) return new SMS.Model.RPCResult<List<Model.AuditEnterprise>>(false, null, "无未审核和审核不通过的终端企业");
        //            List<Model.AuditEnterprise> li = AccountServer.Instance.GetUnableEnterprises();
        //            foreach (var v in li)
        //            {
        //                Model.EnterpriseUser a = list.Find(c => c.AccountCode == v.EnterpriseCode);
        //                if (a != null)
        //                {
        //                    v.EnterpriseName = a.Name;
        //                    ls.Add(v);
        //                }
        //            }
        //            return new SMS.Model.RPCResult<List<Model.AuditEnterprise>>(true, ls, "");
        //        }
        //        else
        //        {
        //            return new SMS.Model.RPCResult<List<Model.AuditEnterprise>>(false, null, "不是代理商企业");
        //        }
        //    }
        //    catch
        //    {
        //        return new SMS.Model.RPCResult<List<Model.AuditEnterprise>>(false, null, "获取失败");
        //    }
        //}

        /// <summary>
        /// 重置企业密码
        /// </summary>
        /// <param name="accountCode"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult ResetEnterprisePass(string accountCode, string password)
        {
            try
            {
                bool ok = AccountServer.Instance.ChangePass(accountCode, password);
                if (ok)
                {
                    return new SMS.Model.RPCResult(true, "");
                }
                return new SMS.Model.RPCResult(false, "重置密码失败");
            }
            catch
            {
                return new SMS.Model.RPCResult(false, "重置密码失败");
            }
        }

        /// <summary>
        /// 修改企业帐号密钥
        /// </summary>
        /// <param name="account">帐号</param>
        /// <param name="secretKey">密钥</param>
        /// <returns></returns>
        public SMS.Model.RPCResult UpdateAccountSecretKey(string accountCode, string secretKey)
        {
            try
            {
                if (string.IsNullOrEmpty(secretKey)) return new SMS.Model.RPCResult(false, "密钥是空的");
                string CleartextPass = "";
                SMS.Model.EnterpriseUser user = AccountServer.Instance.GetAccount(accountCode);
                if (user == null) return new SMS.Model.RPCResult(false, "不存在用户");
                if (secretKey == user.SecretKey) return new SMS.Model.RPCResult(false, "原始密钥和新密钥相同");
                CleartextPass = DESEncrypt.Decrypt(user.Password);
                bool ok = AccountServer.Instance.UpdateAccountSecretKey(accountCode, DESEncrypt.Encrypt(CleartextPass, secretKey), secretKey);
                if (ok)
                {
                    return new SMS.Model.RPCResult(true, "");
                }
                return new SMS.Model.RPCResult(false, "密钥修改失败");
            }
            catch
            {
                return new SMS.Model.RPCResult(false, "密钥修改失败");
            }
        }

        /// <summary>
        /// 获取企业信息
        /// </summary>
        /// <param name="accountCode"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<SMS.Model.EnterpriseUser> GetEnterprise(string accountCode)
        {
            try
            {
                if (string.IsNullOrEmpty(accountCode))
                {
                    return new SMS.Model.RPCResult<SMS.Model.EnterpriseUser>(false, null, "用户不能为空");
                }
                SMS.Model.EnterpriseUser user = AccountServer.Instance.GetAccount(accountCode);
                if (user == null)
                {
                    return new SMS.Model.RPCResult<SMS.Model.EnterpriseUser>(false, null, "无此用户");
                }
                return new SMS.Model.RPCResult<SMS.Model.EnterpriseUser>(true, user, "");
            }
            catch
            {
                return new SMS.Model.RPCResult<SMS.Model.EnterpriseUser>(false, null, "获取用户失败");
            }
        }
        /// <summary>
        /// 获取用户余额
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<SMS.Model.UserBalance> GetBalance(string account, string pass)
        {
            try
            {
                if (string.IsNullOrEmpty(account))
                {
                    return new SMS.Model.RPCResult<SMS.Model.UserBalance>(false, null, "用户不能为空");
                }
                SMS.Model.EnterpriseUser user = AccountServer.Instance.GetAccount(account);
                if (user == null)
                {
                    return new SMS.Model.RPCResult<SMS.Model.UserBalance>(false, null, "无此用户");
                }
                if (pass != user.Password)
                {
                    return new SMS.Model.RPCResult<SMS.Model.UserBalance>(false, null, "密码错误");
                }
                SMS.Model.RPCResult<SMS.Model.Account> r = SMSProxy.GetPretreatmentService().GetAccount(user.AccountID);
                if (r.Success)
                {
                    SMS.Model.UserBalance o = new SMS.Model.UserBalance();
                    o.SmsBalance = r.Value.SMSNumber;
                    o.MmsBalance = 0;
                    return new SMS.Model.RPCResult<SMS.Model.UserBalance>(true, o, "");
                }
                return new SMS.Model.RPCResult<SMS.Model.UserBalance>(false, null, "");
            }
            catch
            {
                return new SMS.Model.RPCResult<SMS.Model.UserBalance>(false, null, "获取用户余额出现错误");
            }
        }
        /// <summary>
        /// 获取企业充值记录
        /// </summary>
        /// <param name="accountCode"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMS.Model.ChargeRecord>> GetEnterpriseChargeRecord(string accountCode, DateTime beginTime, DateTime endTime)
        {
            try
            {
                if (string.IsNullOrEmpty(accountCode)) return new SMS.Model.RPCResult<List<SMS.Model.ChargeRecord>>(false, null, "帐号不能为空");
                if (DateTime.Compare(beginTime, endTime) > 0)
                {
                    DateTime dt = beginTime;
                    beginTime = endTime;
                    endTime = dt;
                }
                if (DateTime.Compare(endTime, DateTime.Now) > 0)
                {
                    endTime = DateTime.Now;
                }
                List<SMS.Model.ChargeRecord> list = DAL.ChargeRecord.GetRecordsByUsercode(accountCode, beginTime, endTime);
                return new SMS.Model.RPCResult<List<SMS.Model.ChargeRecord>>(true, list, "");
            }
            catch
            {
                return new SMS.Model.RPCResult<List<SMS.Model.ChargeRecord>>(false, null, "获取企业充值记录失败");
            }
        }
        ///// <summary>
        ///// 确认删除审核失败的短信
        ///// </summary>
        ///// <param name="serialNumber"></param>
        ///// <returns></returns>
        //public SMS.Model.RPCResult AffirmAuditFailureSMS(Guid serialNumber)
        //{
        //    return ZHSMSProxy.GetPretreatmentService().AffirmAuditFailureSMS(serialNumber);
        //}

        /// <summary>
        /// 根据词获取敏感词（模糊查询）
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMS.Model.Keywords>> GetKeywordsByKeyword(string keyword)
        {
            return SMSProxy.GetPretreatmentService().GetKeywordsByKeyword(keyword);
        }

        #endregion

        #region 短信模板

        /// <summary>
        /// 添加短信模板
        /// </summary>
        /// <param name="enterpiseCode"></param>
        /// <param name="templetContent"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<Guid> AddSMSTemplet(string enterpiseCode, string templetContent)
        {
            try
            {
                if (string.IsNullOrEmpty(enterpiseCode))
                {
                    return new SMS.Model.RPCResult<Guid>(false, new Guid(), "企业代码不能为空");
                }

                if (string.IsNullOrEmpty(templetContent))
                {
                    return new SMS.Model.RPCResult<Guid>(false, new Guid(), "短信模板内容不能为空");
                }
                if (string.IsNullOrEmpty(templetContent.Replace("*", "").Trim()))
                {
                    return new SMS.Model.RPCResult<Guid>(false, new Guid(), "短信模板无内容");
                }

                SMS.Model.EnterpriseUser user = AccountServer.Instance.GetAccount(enterpiseCode);
                if (user == null)
                {
                    return new SMS.Model.RPCResult<Guid>(false, new Guid(), "用户不存在");
                }
                if (!user.Enabled)
                {
                    return new SMS.Model.RPCResult<Guid>(false, new Guid(), "此帐户已被禁止");
                }
                SMSTemplet content = new SMSTemplet();
                content.TempletID = System.Guid.NewGuid().ToString("N");
                content.AccountCode = enterpiseCode;
                content.AccountName = user.Name;
                content.SubmitTime = DateTime.Now;
                content.TempletContent = templetContent;
                content.UserCode = "";
                content.Remark = "";
                content.AuditTime = "";
                content.AuditState = SMS.Model.TempletAuditType.NoAudit;
                content.AuditLevel = SMS.Model.TempletAuditLevelType.General;
                if (string.IsNullOrWhiteSpace(user.Signature))
                {
                    content.Signature = "";
                }
                else
                {
                    content.Signature = "【" + user.Signature + "】";
                }
                bool ok = AccountServer.Instance.AddSMSTempletContent(content);
                if (ok)
                {
                    return new SMS.Model.RPCResult<Guid>(true, new Guid(content.TempletID), "");
                }
                else
                {
                    return new SMS.Model.RPCResult<Guid>(false, new Guid(), "短信模板提交失败");
                }

            }
            catch
            {
                return new SMS.Model.RPCResult<Guid>(false, new Guid(), "发送短信出现错误");
            }
        }

        /// <summary>
        /// 审核短信模板
        /// </summary>
        /// <param name="templetID"></param>
        /// <param name="user"></param>
        /// <param name="auditStatus"></param>
        /// <param name="cause"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult AuditSMSTemplet(string templetID, string user, bool auditStatus, string cause)
        {
            try
            {
                if (string.IsNullOrEmpty(user))
                {
                    return new SMS.Model.RPCResult<Guid>(false, new Guid(), "需要审核人员信息");
                }
                if (!auditStatus)
                {
                    if (string.IsNullOrEmpty(cause))
                    {
                        return new SMS.Model.RPCResult(false, "请填写失败原因");
                    }
                }
                SMSTemplet content = AccountServer.Instance.GetSMSTemplet(templetID);
                if (content == null)
                {
                    return new SMS.Model.RPCResult(false, "系统不存在此短信模板");
                }
                content.UserCode = user;
                content.AuditTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                content.Remark = cause;
                content.AuditState = auditStatus == true ? SMS.Model.TempletAuditType.Success : SMS.Model.TempletAuditType.Failure;
                bool ok = AccountServer.Instance.UpdateAuditSMSTemplet(content);
                if (ok)
                {
                    return new SMS.Model.RPCResult(true, "");
                }
                return new SMS.Model.RPCResult(false, "审核失败");
            }
            catch
            {
                return new SMS.Model.RPCResult(false, "审核短信模板出现异常");
            }
        }

        /// <summary>
        /// 获取待审核的短信模板
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMSTemplet>> GetAuditSMSTemplet(DateTime beginTime, DateTime endTime)
        {
            if (DateTime.Compare(beginTime, endTime) > 0)
            {
                DateTime dt = beginTime;
                beginTime = endTime;
                endTime = dt;
            }
            if (DateTime.Compare(endTime, DateTime.Now) > 0)
            {
                endTime = DateTime.Now;
            }
            List<SMSTemplet> list = AccountServer.Instance.GetSMSTemplets().Where(c => c.AuditState == SMS.Model.TempletAuditType.NoAudit && c.SubmitTime >= beginTime && c.SubmitTime <= endTime).OrderBy(a => a.SubmitTime).ToList();
            return new SMS.Model.RPCResult<List<SMSTemplet>>(true, list, "");
        }

        /// <summary>
        /// 获取所有的短信模板
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMSTemplet>> GetAllSMSTemplet(DateTime beginTime, DateTime endTime)
        {
            if (DateTime.Compare(beginTime, endTime) > 0)
            {
                DateTime dt = beginTime;
                beginTime = endTime;
                endTime = dt;
            }
            if (DateTime.Compare(endTime, DateTime.Now) > 0)
            {
                endTime = DateTime.Now;
            }
            List<SMSTemplet> list = AccountServer.Instance.GetSMSTemplets().Where(c => c.SubmitTime >= beginTime && c.SubmitTime <= endTime).OrderByDescending(a => a.SubmitTime).ToList();
            return new SMS.Model.RPCResult<List<SMSTemplet>>(true, list, "");
        }

        /// <summary>
        /// 审核成功短信模板列表
        /// </summary>
        /// <param name="enterpise"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMSTemplet>> GetSuccessSMSTemplet(string enterpise, DateTime beginTime, DateTime endTime)
        {
            if (DateTime.Compare(beginTime, endTime) > 0)
            {
                DateTime dt = beginTime;
                beginTime = endTime;
                endTime = dt;
            }
            if (DateTime.Compare(endTime, DateTime.Now) > 0)
            {
                endTime = DateTime.Now;
            }
            List<SMSTemplet> list = new List<SMSTemplet>();
            if (string.IsNullOrEmpty(enterpise))
            {
                list = AccountServer.Instance.GetSMSTemplets().Where(c => c.AuditState == SMS.Model.TempletAuditType.Success && c.SubmitTime >= beginTime && c.SubmitTime <= endTime).ToList();
            }
            else
            {
                list = AccountServer.Instance.GetSMSTemplets().Where(c => c.AuditState == SMS.Model.TempletAuditType.Success && c.SubmitTime >= beginTime && c.SubmitTime <= endTime && (c.AccountCode.Contains(enterpise) || c.AccountName.Contains(enterpise))).ToList();
            }
            return new SMS.Model.RPCResult<List<SMSTemplet>>(true, list, ""); ;
        }
        /// <summary>
        /// 审核失败模板列表
        /// </summary>
        /// <param name="enterpise"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMSTemplet>> GetFailureSMSTemplet(string enterpise, DateTime beginTime, DateTime endTime)
        {
            if (DateTime.Compare(beginTime, endTime) > 0)
            {
                DateTime dt = beginTime;
                beginTime = endTime;
                endTime = dt;
            }
            if (DateTime.Compare(endTime, DateTime.Now) > 0)
            {
                endTime = DateTime.Now;
            }
            List<SMSTemplet> list = new List<SMSTemplet>();
            if (string.IsNullOrEmpty(enterpise))
            {
                list = AccountServer.Instance.GetSMSTemplets().Where(c => c.AuditState == SMS.Model.TempletAuditType.Failure && c.SubmitTime >= beginTime && c.SubmitTime <= endTime).ToList();
            }
            else
            {
                list = AccountServer.Instance.GetSMSTemplets().Where(c => c.AuditState == SMS.Model.TempletAuditType.Failure && c.SubmitTime >= beginTime && c.SubmitTime <= endTime && (c.AccountCode.Contains(enterpise) || c.AccountName.Contains(enterpise))).ToList();
            }
            return new SMS.Model.RPCResult<List<SMSTemplet>>(true, list, ""); ;
        }
        /// <summary>
        /// 直客端查看短信模板列表
        /// </summary>
        /// <param name="enterpiseCode"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMSTemplet>> GetZKDSMSTempletStauts(string enterpiseCode)
        {
            List<SMSTemplet> list = new List<SMSTemplet>();
            if (string.IsNullOrEmpty(enterpiseCode))
            {
                return new SMS.Model.RPCResult<List<SMSTemplet>>(false, new List<SMSTemplet>(), "企业代码不能为空");
            }

            list = AccountServer.Instance.GetSMSTemplets().Where(c => c.AccountCode == enterpiseCode).ToList();

            return new SMS.Model.RPCResult<List<SMSTemplet>>(true, list, ""); ;
        }

        public SMS.Model.RPCResult ZKDDelSMSTemplet(string templetID)
        {
            try
            {
                bool ok = AccountServer.Instance.DelSMSTemplet(templetID);
                if (ok) return new SMS.Model.RPCResult(true, "");
            }
            catch
            {
            }
            return new SMS.Model.RPCResult(false, "删除失败");
        }

        public SMS.Model.RPCResult ZKDUpdateSMSTemplet(string templetID, string content)
        {
            try
            {
                if (string.IsNullOrEmpty(content))
                {
                    return new SMS.Model.RPCResult(false, "短信模板内容不能为空");
                }
                SMSTemplet con = AccountServer.Instance.GetSMSTemplet(templetID);
                if (con == null)
                {
                    return new SMS.Model.RPCResult(false, "系统不存在此短信模板");
                }
                if (con.AuditState != SMS.Model.TempletAuditType.Success)
                {
                    con.AuditState = SMS.Model.TempletAuditType.NoAudit;
                    con.SubmitTime = DateTime.Now;
                    con.TempletContent = content;
                    con.Remark = "";
                    bool ok = AccountServer.Instance.UpdateSMSTemplet(con);
                    if (ok)
                    {
                        return new SMS.Model.RPCResult(true, "");
                    }
                    return new SMS.Model.RPCResult(false, "修改失败");
                }
                else
                {
                    return new SMS.Model.RPCResult(false, "短信模板内容已被审核，不能进行修改");
                }

            }
            catch
            {
                return new SMS.Model.RPCResult(false, "短信模板修改出现异常");
            }
        }

        public SMS.Model.RPCResult<SMSTemplet> GetSMSTemplet(string templetID)
        {
            try
            {
                SMSTemplet content = AccountServer.Instance.GetSMSTemplet(templetID);
                if (content != null)
                {
                    return new SMS.Model.RPCResult<SMSTemplet>(true, content, "");
                }
                return new SMS.Model.RPCResult<SMSTemplet>(false, null, "不存在此短信模板");
            }
            catch
            {
            }
            return new SMS.Model.RPCResult<SMSTemplet>(false, null, "获取失败");
        }



        /// <summary>
        /// 短信模板匹配
        /// </summary>
        /// <param name="account"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        bool SMSTempletMatching(string account, string content)
        {
            List<SMSTemplet> list = AccountServer.Instance.GetSuccessSMSTemplets(account);
            List<string> ts = new List<string>();
            foreach (SMSTemplet t in list)
            {
                ts.Add(t.TempletContent);
            }
            return TempletMatching(ts, content);
        }

        string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        bool TempletMatching(List<string> templet, string content)
        {
            //Regex regex = new Regex(@"[~!@#\$%\^&\(\)\+=\|\\\}\]\{\[:;<,>\?\/""`_\-'.￥……——、“《》【】‘。”’ ]", RegexOptions.IgnorePatternWhitespace);
            Regex regex = new Regex(@"[^\u4E00-\u9FFFa-z0-9*]");
            content = ToDBC(content).ToLower();
            content = regex.Replace(content, "");

            if (content.Length == 0)
            {
                return false;
            }
            if (templet.Count > 0)
            {
                string[] tc;
                string tv;
                int ci = 0;
                foreach (var v in templet)
                {
                    tv = ToDBC(v).ToLower();
                    tv = regex.Replace(tv, "");
                    tc = tv.Split('*');
                    ci = 0;
                    for (int i = 0; i < tc.Length; i++)
                    {
                        if (i == 0 && tc[0] != "")
                        {
                            if (content.Length < tc[0].Length) break;
                            string t = content.Substring(0, tc[0].Length);
                            if (string.Compare(t, tc[0]) != 0) break;
                        }
                        ci = content.IndexOf(tc[i], ci);
                        if (ci < 0) break;
                        ci += tc[i].Length;
                        if (i == tc.Length - 1)
                        {
                            if (ci == content.Length)
                            {
                                return true;
                            }
                            if (tc[tc.Length - 1] == "")
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        #endregion

        #region 中呼端管理平台（账号管理）

        /// <summary>
        /// 添加账号
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult AddSysAccount(SMS.Model.SysAccount account)
        {
            bool ok = false;
            try
            {
                account.PassWord = DESEncrypt.Encrypt(account.PassWord);
                ok = DAL.SysAccount.Add(account);

            }
            catch
            {
                return new SMS.Model.RPCResult(false, "添加账号失败！");
            }
            if (ok)
            {
                return new SMS.Model.RPCResult(ok, "添加账号成功！");
            }
            else
            {
                return new SMS.Model.RPCResult(false, "添加账号失败！");
            }

        }

        /// <summary>
        /// 更新账号
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult UpdateSysAccount(SMS.Model.SysAccount account)
        {
            bool ok = false;
            try
            {
                ok = DAL.SysAccount.Update(account);

            }
            catch
            {
                return new SMS.Model.RPCResult(false, "修改账号失败！");
            }
            if (ok)
            {
                return new SMS.Model.RPCResult(ok, "修改账号成功！");
            }
            else
            {
                return new SMS.Model.RPCResult(false, "修改账号失败！");
            }

        }

        #endregion
    }
}