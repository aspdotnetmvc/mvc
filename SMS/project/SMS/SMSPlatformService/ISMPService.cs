using Newtonsoft.Json;
using SMS.Model;
using SMSPlatform.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SMSPlatform
{
    public partial class SMSPlatformService
    {
        #region ISMP
        /// <summary>
        /// 添加企业
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult ISMPAddEnterprise(SMS.Model.EnterpriseUser user)
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
                SMS.Model.RPCResult<Guid> r = SMSProxy.GetSMSService().CreateAccount(account);
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
        /// 获取所有企业，包括待审核的 
        /// 用于检查spNumber 是否已被使用等
        /// </summary>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<SMS.Model.EnterpriseUser>> ISMPGetAllEnterprise()
        {
            try
            {
                List<SMS.Model.EnterpriseUser> list = AccountServer.Instance.GetAccounts();
                return new SMS.Model.RPCResult<List<SMS.Model.EnterpriseUser>>(true, list, "");
            }
            catch
            {
                return new SMS.Model.RPCResult<List<SMS.Model.EnterpriseUser>>(false, null, "获取企业失败");
            }
        }
        public RPCResult<List<EnterpriseUser>> ISMPGetEnterpriseBySMSType(SMSType smstype)
        {
            try
            {
                List<SMS.Model.EnterpriseUser> list = AccountServer.Instance.GetAccounts().Where(eu => eu.SMSType == smstype).ToList();
                return new SMS.Model.RPCResult<List<SMS.Model.EnterpriseUser>>(true, list, "");
            }
            catch
            {
                return new SMS.Model.RPCResult<List<SMS.Model.EnterpriseUser>>(false, null, "获取企业失败");
            }
        }
        public SMS.Model.RPCResult<List<SMS.Model.Keywords>> GetAllKeywords()
        {
            var list = SMSProxy.GetSMSService().GetAllKeywords(0, 100000000);
            return new SMS.Model.RPCResult<List<SMS.Model.Keywords>>(true, list.Value.Values.FirstOrDefault(), "");
        }

        /// <summary>
        /// 获取短信统计
        /// </summary>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult<List<string>> GetSMSStatisticsAll(DateTime BeginDate, DateTime EndDate)
        {
            var Enterprises = AccountServer.Instance.GetAccounts();
            //取当前余额  
            var Balance = SMSProxy.GetSMSService().GetAccounts().Value;
            //获取指定时间范围内，充值金额
            var ChargeStatics = ChargeRecordDB.GetChargeStatics(BeginDate.Date, EndDate.AddDays(1).Date);
            //获取
            var ReportStatistics = SMSProxy.GetSMSService().GetStatisticsReportAll(BeginDate.Date, EndDate.Date).Value;

            var qry = from e in Enterprises
                      join b in Balance
                      on e.AccountID equals b.AccountID into EnterpriseBalance
                      from eb in EnterpriseBalance.DefaultIfEmpty()
                      join r in
                          (from t in ReportStatistics group t by new { t.AccountID, t.Channel } into g select new { Account = g.Key.AccountID, Channel = g.Key.Channel, SendCount = g.Sum(t => t.SendCount), Succeed = g.Sum(t => t.SuccessCount) })
                      on e.AccountID equals r.Account into EnterpriseReport
                      from er in EnterpriseReport.DefaultIfEmpty()
                      join c in ChargeStatics
                      on e.AccountCode equals c.Enterprese into EnterpriseCharge
                      from ec in EnterpriseCharge.DefaultIfEmpty()
                      select Newtonsoft.Json.JsonConvert.SerializeObject(
                      new
                     {
                         EnterpriseCode = e.AccountCode,
                         AccountId = e.AccountID,
                         Channel = e.Channel,
                         ChargeCount = ec == null ? 0 : ec.SMSCount,
                         ChargeMoney = ec == null ? 0 : ec.TotalMoney,
                         SendCount = er == null ? 0 : er.SendCount,
                         Succeed = er == null ? 0 : er.Succeed,
                         Balance = eb == null ? 0 : eb.SMSNumber
                     });
            return new SMS.Model.RPCResult<List<string>>(true, qry.ToList(), "");
        }


        #endregion

        #region 企业黑名单
        public SMS.Model.RPCResult AddEnterpriseBlackList(string EnterpriseCode, List<string> Numbers)
        {
            var ok = EnterpriseBlackListDB.AddEnterpriseBlackList(EnterpriseCode, Numbers);
            if (ok)
            {
                //添加缓存
                EnterpriseBlackList.Instance.AddBlackNumbers(EnterpriseCode, Numbers);

                return new SMS.Model.RPCResult(ok, "添加黑名单成功！");
            }
            else
            {
                return new SMS.Model.RPCResult(ok, "添加黑名单失败");
            }
        }
        public SMS.Model.RPCResult DeleteEnterpriseBlackList(string EnterpriseCode, List<string> Numbers)
        {
            var ok = EnterpriseBlackListDB.DelEnterpriseBlackList(EnterpriseCode, Numbers);
            if (ok)
            {
                //删除缓存
                EnterpriseBlackList.Instance.DeleteBlackNumbers(EnterpriseCode, Numbers);

                return new SMS.Model.RPCResult(ok, "删除黑名单成功！");
            }
            else
            {
                return new SMS.Model.RPCResult(ok, "删除黑名单失败");
            }
        }

        public SMS.Model.RPCResult<List<string>> GetEnterpriseBlackList(string EnterpriseCode)
        {
            var list = EnterpriseBlackListDB.getEnterpriseBlackList(EnterpriseCode);
            return new SMS.Model.RPCResult<List<string>>(true, list, "");
        }

        #endregion

        #region 发送短信

        ///// <summary>
        ///// 发送短信
        ///// 返回值是sms，包含SerialNumber
        ///// 增加了判断短信内容是否包含签名
        ///// </summary>
        ///// <param name="account"></param>
        ///// <param name="password"></param>
        ///// <param name="smsContent"></param>
        ///// <param name="wapUrl"></param>
        ///// <param name="numbers"></param>
        ///// <param name="isSMSTimer"></param>
        ///// <param name="smsTimer"></param>
        ///// <returns></returns>
        //public SMS.Model.RPCResult<PlainSMS> DoSendSMS(string account, string password, string smsContent, string wapUrl, List<string> numbers, bool isSMSTimer, DateTime smsTimer)
        //{
        //    PlainSMS sms = new PlainSMS();
        //    try
        //    {
        //        if (string.IsNullOrEmpty(account))
        //        {
        //            return new SMS.Model.RPCResult<PlainSMS>(false, sms, "帐号不能为空");
        //        }
        //        if (numbers.Count == 0)
        //        {
        //            return new SMS.Model.RPCResult<PlainSMS>(false, sms, "短信接收号码不能为空");
        //        }
        //        if (numbers.Count > 5000)
        //        {
        //            return new SMS.Model.RPCResult<PlainSMS>(false, sms, "接收短信的号码过多，应少于5000个号码");
        //        }
        //        #region  by lmw 2016-03-10 企业黑名单过滤

        //        string numberinblacklist = EnterpriseBlackList.Instance.CheckNumber(account, numbers);

        //        if (!string.IsNullOrWhiteSpace(numberinblacklist))
        //        {
        //            // return new SMS.Model.RPCResult<PlainSMS>(false, sms, "号码中含有在黑名单号码：" + numberinblacklist);

        //            var blackNumbers = EnterpriseBlackList.Instance.GetEnterpriseBlackNumbers(account);
        //            numbers = (from n in numbers where !blackNumbers.Any(b => b == n) select n).ToList();
        //        }
        //        if (numbers.Count == 0)
        //        {
        //            return new SMS.Model.RPCResult<PlainSMS>(false, sms, "号码全部在黑名单中：" + numberinblacklist);
        //        }

        //        #endregion

        //        if (string.IsNullOrEmpty(smsContent)) return new SMS.Model.RPCResult<PlainSMS>(false, sms, "短信内容是空的");
        //        try
        //        {
        //            Model.EnterpriseUser user = AccountServer.Instance.GetAccount(account);
        //            if (user == null)
        //            {
        //                return new SMS.Model.RPCResult<PlainSMS>(false, sms, "用户不存在");
        //            }
        //            if (!user.Enabled)
        //            {
        //                return new SMS.Model.RPCResult<PlainSMS>(false, sms, "此帐户已被禁止发送短信");
        //            }
        //            if (password != user.Password) return new SMS.Model.RPCResult<PlainSMS>(false, sms, "帐号或密码不正确");
        //            if (string.IsNullOrEmpty(user.SPNumber)) return new SMS.Model.RPCResult<PlainSMS>(false, sms, "企业SPNumber不能为空");
        //            if (isSMSTimer)
        //            {
        //                if (DateTime.Compare(smsTimer, DateTime.Now.AddMinutes(5)) < 0)
        //                {
        //                    return new SMS.Model.RPCResult<PlainSMS>(false, sms, "定时短信发送的时间应在当前时间之后");
        //                }
        //            }

        //            sms.Number = numbers;
        //            sms.SendTime = DateTime.Now;
        //            sms.Content = smsContent;
        //            sms.WapURL = wapUrl;
        //            if (isSMSTimer)
        //            {
        //                sms.SMSTimer = smsTimer;
        //            }
        //            sms.Account = user.AccountID;
        //            sms.SPNumber = user.SPNumber;
        //            sms.Level = (SMS.Model.LevelType)user.Level;
        //            sms.StatusReport = (SMS.Model.StatusReportType)user.StatusReport;
        //            sms.Filter = (SMS.Model.FilterType)user.Filter;
        //            sms.Signature = "";
        //            if (!string.IsNullOrEmpty(user.Signature))
        //            {
        //                sms.Signature = "【" + user.Signature + "】";
        //            }
        //            else
        //            {
        //                //判断短信本身是否含有signature.
        //                string regex = @"^【.+】|【.+】$";

        //                if (!Regex.IsMatch(sms.Content, regex))
        //                {
        //                    return new SMS.Model.RPCResult<PlainSMS>(false, sms, "短信不包含签名");
        //                }
        //            }
        //            sms.Channel = user.Channel;
        //            //计算短信优先级
        //            if ((int)sms.Level >= 7)
        //            {
        //                return new SMS.Model.RPCResult<PlainSMS>(false, sms, "定义的短信级别最高为LEVEL6");
        //            }
        //            sms.Level += (ushort)user.Priority;
        //            if (user.Audit == SMS.Model.AccountAuditType.Auto)
        //            {
        //                sms.Audit = SMS.Model.AuditType.Auto;
        //            }
        //            else if (user.Audit == SMS.Model.AccountAuditType.Manual)
        //            {
        //                bool match = SMSTempletMatching(account, sms.Content);
        //                if (match)
        //                {
        //                    sms.Audit = SMS.Model.AuditType.Auto;
        //                }
        //                else
        //                {
        //                    sms.Audit = SMS.Model.AuditType.Manual;
        //                }
        //            }

        //            var r = ZHSMSProxy.GetPretreatmentService().SendSMS(sms);
        //            sms.SerialNumber =Guid.Parse r.Value.Message.ID;
        //            if (r.Success && !string.IsNullOrWhiteSpace(numberinblacklist))
        //            {
        //                return new SMS.Model.RPCResult<PlainSMS>(r.Success, sms, "发送成功,以下在黑名单号码未发送：" + numberinblacklist);
        //            }
        //            else
        //            {

        //                return new SMS.Model.RPCResult<PlainSMS>(r.Success, sms, r.Message);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return new SMS.Model.RPCResult<PlainSMS>(false, sms, "短信出现异常");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new SMS.Model.RPCResult<PlainSMS>(false, sms, "发送短信出现错误");
        //    }
        //}


        #endregion

        #region 上行短信处理
        /// <summary>
        /// 上行短信处理（自动把内容为T/N 的号码加入企业黑名单
        /// </summary>
        /// <param name="sms"></param>
        /// <returns></returns>
        public SMS.Model.RPCResult ProcessMOSMS(string message)
        {
            try
            {
                var sms = JsonConvert.DeserializeObject<MOSMS>(message);
                if (sms != null)
                {
                    if (sms.Message == "T" || sms.Message == "N" || sms.Message == "t" || sms.Message == "n")
                    {
                        //根据spNumber 获取企业
                        var eu = AccountServer.Instance.GetAccounts().FirstOrDefault(e => e.SPNumber == sms.SPNumber);
                        if (eu != null)
                        {
                            AddEnterpriseBlackList(eu.AccountCode, new List<string>() { sms.UserNumber });
                        }
                    }
                }
                return new SMS.Model.RPCResult(true, "处理完成！");
            }
            catch (Exception ex)
            {
                return new SMS.Model.RPCResult(false, "处理上行短信发生异常！");
            }
        }
        #endregion
    }
}
