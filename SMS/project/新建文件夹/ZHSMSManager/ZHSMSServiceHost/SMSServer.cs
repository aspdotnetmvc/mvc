using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thrift.Server;
using Thrift.Transport;

namespace ZHSMSServiceHost
{
    class SMSServer : ACSMSService.Iface
    {
        TServer server;
        public void Start(int port)
        {
            TServerSocket serverTransport = new TServerSocket(port, 0, false);
            ACSMSService.Processor processor = new ACSMSService.Processor(new SMSServer());
            server = new TThreadPoolServer(processor, serverTransport);
            server.Serve();
        }

        public void Stop()
        {
            server.Stop();
        }

        //创建账号
        public RPCResult CreateAccount()
        {
            try
            {
                SMSModel.Account account = new SMSModel.Account();
                account.AccountID = "";
                account.SMSNumber = 0;
                SMSModel.RPCResult<Guid> r = ZHSMSProxy.GetPretreatmentService().CreateAccount(account);
                if (!r.Success) return RPCResultHelper.BuildFailureResult(r.Message);
                return RPCResultHelper.BuildSucessResult(r.Value.ToString());
            }
            catch
            {
                return RPCResultHelper.BuildFailureResult("账号创建失败");
            }
        }

        //删除账号
        public RPCResult DelAccount(string accountID)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return RPCResultHelper.BuildFailureResult("账号不能为空");
                SMSModel.RPCResult r = ZHSMSProxy.GetPretreatmentService().DelAccount(accountID);
                if (!r.Success) return RPCResultHelper.BuildFailureResult(r.Message);
                return RPCResultHelper.BuildSucessResult();
            }
            catch
            {
                return RPCResultHelper.BuildFailureResult("账号删除失败");
            }
        }

        //获取账号
        public Account GetAccount(string accountID)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return null;
                SMSModel.RPCResult<SMSModel.Account> r = ZHSMSProxy.GetPretreatmentService().GetAccount(accountID);
                if (r.Success)
                {
                    Account a = new Account();
                    a.AccountID = r.Value.AccountID;
                    a.SMSNumber = r.Value.SMSNumber;
                    return a;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        //充值
        public RPCResult AccountPrepaid(string accountID, int quantity, string operatorAccount)
        {
            try
            {
                if (quantity == 0) return RPCResultHelper.BuildSucessResult();
                SMSModel.RPCResult r = ZHSMSProxy.GetPretreatmentService().AccountPrepaid(accountID, (uint)quantity, operatorAccount);
                if (r.Success) return RPCResultHelper.BuildSucessResult();
                return RPCResultHelper.BuildFailureResult(r.Message);
            }
            catch
            {
                return RPCResultHelper.BuildFailureResult("账号充值失败");
            }
        }

        //充值记录
        public RPCPrepaidRecordListResult GetPrepaidRecord(string beginTime, string endTime, int pSize, int pIndex)
        {
            RPCPrepaidRecordListResult result = new RPCPrepaidRecordListResult();
            result.Message = "";
            result.Records = new List<PrepaidRecord>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            DateParse dp = getDateParse(beginTime, endTime);
            if (!dp.Result)
            {
                result.Message = dp.Message;
                result.Success = false;
                return result;
            }
            beginTime = dp.BeginTime;
            endTime = dp.EndTime;
            if (DateTime.Compare(DateTime.Parse(endTime).AddMonths(-1), DateTime.Parse(beginTime)) > 0)
            {
                result.Message = "时间范围不允许超过1个月";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<SMSModel.PrepaidRecord>> rt = ZHSMSProxy.GetPretreatmentService().GetPrepaidRecord(DateTime.Parse(beginTime), DateTime.Parse(endTime));
            if (rt.Success)
            {
                List<SMSModel.PrepaidRecord> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.PrepaidRecord>.GetList(list, pSize, pIndex);
                    foreach (var v in list)
                    {
                        result.Records.Add(SMSModelToPrepaidRecord(v));
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        //扣费
        public RPCResult AccountDeductSMSCharge(string accountID, int quantity)
        {
            try
            {
                if (quantity == 0) return RPCResultHelper.BuildSucessResult();
                SMSModel.RPCResult rt = ZHSMSProxy.GetPretreatmentService().AccountDeductSMSCharge(accountID, quantity);
                if (rt.Success) return RPCResultHelper.BuildSucessResult();
                return RPCResultHelper.BuildFailureResult(rt.Message);
            }
            catch
            {
                return RPCResultHelper.BuildFailureResult("账号扣费失败");
            }
        }

        //发送短信
        public RPCResult SendSMS(SMS sms, bool isSMSTimer)
        {
            try
            {
                if (string.IsNullOrEmpty(sms.Account))
                {
                    return RPCResultHelper.BuildFailureResult("帐号不能为空");
                }
                if (string.IsNullOrEmpty(sms.Signature))
                {
                    return RPCResultHelper.BuildFailureResult("短信签名不能为空");
                }
                if (sms.Number.Count == 0)
                {
                    return RPCResultHelper.BuildFailureResult("短信接收号码不能为空");
                }
                if (sms.Number.Count > 1000)
                {
                    return RPCResultHelper.BuildFailureResult("接收短信的号码过多，最多发送1000个号码");
                }
                if (string.IsNullOrEmpty(sms.Content)) return RPCResultHelper.BuildFailureResult("短信内容不能为空");
                SMSModel.SMS s = new SMSModel.SMS();
                if (isSMSTimer)
                {
                    DateTime time;
                    try
                    {
                        if (string.IsNullOrEmpty(sms.SMSTimer)) return RPCResultHelper.BuildFailureResult("定时发送时间不能为空");
                        time = DateTime.Parse(sms.SMSTimer);
                    }
                    catch
                    {
                        return RPCResultHelper.BuildFailureResult("定时发送时间格式不正确");
                    }
                    if (DateTime.Compare(time, DateTime.Now.AddMinutes(5)) < 0)
                    {
                        return RPCResultHelper.BuildFailureResult("定时短信发送的时间应在当前时间之后");
                    }
                    s.SMSTimer = time;
                }
                s.Number = sms.Number;
                s.SendTime = DateTime.Now;
                s.Content = sms.Content;
                s.WapURL = sms.WapURL;
                s.Account = sms.Account;
                s.SPNumber = sms.SPNumber;
                s.Level = (SMSModel.LevelType)sms.Level;
                s.StatusReport = (SMSModel.StatusReportType)sms.StatusReport;
                s.Filter = (SMSModel.FilterType)sms.Filter;
                s.Signature = sms.Signature;
                s.Channel = sms.Channel;
                s.Audit = (SMSModel.AuditType)sms.Audit;
                s.LinkID = sms.LinkID;
                SMSModel.RPCResult<Guid> r = ZHSMSProxy.GetPretreatmentService().SendSMS(s);
                if (r.Success) return RPCResultHelper.BuildSucessResult(r.Value.ToString());
                return RPCResultHelper.BuildFailureResult(r.Message);
            }
            catch
            {
                return RPCResultHelper.BuildFailureResult("发送短信出现错误");
            }
        }

        //发送短信
        public RPCResult SendSGIPSubmit(SMS sms, SGIPSubmit submit, bool isSMSTimer)
        {
            try
            {
                if (submit == null)
                {
                    return RPCResultHelper.BuildFailureResult("SGIPSUBMIT没有设置.");
                }
                if (string.IsNullOrEmpty(sms.Account))
                {
                    return RPCResultHelper.BuildFailureResult("帐号不能为空");
                }
                if (string.IsNullOrEmpty(sms.Signature))
                {
                    return RPCResultHelper.BuildFailureResult("短信签名不能为空");
                }
                if (sms.Number.Count == 0)
                {
                    return RPCResultHelper.BuildFailureResult("短信接收号码不能为空");
                }
                if (sms.Number.Count > 1000)
                {
                    return RPCResultHelper.BuildFailureResult("接收短信的号码过多，最多发送1000个号码");
                }
                if (string.IsNullOrEmpty(sms.Content)) return RPCResultHelper.BuildFailureResult("短信内容不能为空");
                SMSModel.SMS s = new SMSModel.SMS();
                SMSModel.SGIPSUBMIT sp = new SMSModel.SGIPSUBMIT();
                if (isSMSTimer)
                {
                    DateTime time;
                    try
                    {
                        if (string.IsNullOrEmpty(sms.SMSTimer)) return RPCResultHelper.BuildFailureResult("定时发送时间不能为空");
                        time = DateTime.Parse(sms.SMSTimer);
                    }
                    catch
                    {
                        return RPCResultHelper.BuildFailureResult("定时发送时间格式不正确");
                    }
                    if (DateTime.Compare(time, DateTime.Now.AddMinutes(5)) < 0)
                    {
                        return RPCResultHelper.BuildFailureResult("定时短信发送的时间应在当前时间之后");
                    }
                    s.SMSTimer = time;
                }
                sp.AgentFlag = (uint)submit.AgentFlag;
                sp.Pk_total = (uint)submit.Pk_total;
                sp.Pk_number = (uint)submit.Pk_number;
                sp.WapURL = submit.WapURL;
                sp.SPNumber = submit.SPNumber;
                sp.ChargeNumber = submit.ChargeNumber;
                sp.UserCount = (uint)submit.UserCount;
                sp.UserNumber = submit.UserNumber;
                sp.CorpId = submit.CorpId;
                sp.ServiceType = submit.ServiceType;
                sp.FeeType = (uint)submit.FeeType;
                sp.FeeValue = submit.FeeValue;
                sp.GivenValue = submit.GivenValue;
                sp.MorelatetoMTFlag = (uint)submit.MorelatetoMTFlag;
                sp.Priority = (uint)submit.Priority;
                sp.ExpireTime = submit.ExpireTime;
                sp.ScheduleTime = submit.ScheduleTime;
                sp.ReportFlag = (uint)submit.ReportFlag;
                sp.TP_pid = (uint)submit.TP_pid;
                sp.TP_udhi = (uint)submit.TP_udhi;
                sp.MessageCoding = (uint)submit.MessageCoding;
                sp.MessageType = (uint)submit.MessageType;
                sp.MessageLength = (uint)submit.MessageLength;
                sp.MessageContent = submit.MessageContent;
                sp.LinkID = submit.LinkID;

                s.Number = sms.Number;
                s.SendTime = DateTime.Now;
                s.Content = sms.Content;
                s.WapURL = sms.WapURL;
                s.Account = sms.Account;
                s.SPNumber = sms.SPNumber;
                s.Level = (SMSModel.LevelType)sms.Level;
                s.StatusReport = (SMSModel.StatusReportType)sms.StatusReport;
                s.Filter = (SMSModel.FilterType)sms.Filter;
                s.Signature = sms.Signature;
                s.Channel = sms.Channel;
                s.Extend = sp;
                s.Audit = (SMSModel.AuditType)sms.Audit;
                s.LinkID = sms.LinkID;
                SMSModel.RPCResult<Guid> r = ZHSMSProxy.GetPretreatmentService().SendSMS(s);
                if (r.Success) return RPCResultHelper.BuildSucessResult(r.Value.ToString());
                return RPCResultHelper.BuildFailureResult(r.Message);
            }
            catch
            {
                return RPCResultHelper.BuildFailureResult("发送短信出现错误");
            }
        }

        //获取预处理短信列表（最多100条）
        public RPCSMSListResult GetSMSByAccount(string accountID, int pSize, int pIndex)
        {
            RPCSMSListResult result = new RPCSMSListResult();
            result.Message = "";
            result.SMSList = new List<SMS>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;
            if (string.IsNullOrEmpty(accountID))
            {
                result.Message = "帐号是空的";
                result.Success = false;
                return result;
            }
            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<SMSModel.SMS>> rt = ZHSMSProxy.GetPretreatmentService().GetSMSByAccount(accountID);
            if (rt.Success)
            {
                List<SMSModel.SMS> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.SMS>.GetList(list, pSize, pIndex);
                    foreach (SMSModel.SMS sms in list)
                    {
                        result.SMSList.Add(SMSModelToSMS(sms));
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }


        public RPCSMSListResult GetReviewSMS(int pSize, int pIndex)
        {
            RPCSMSListResult result = new RPCSMSListResult();
            result.Message = "";
            result.SMSList = new List<SMS>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;
            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<Dictionary<int, List<SMSModel.SMS>>> rt = ZHSMSProxy.GetPretreatmentService().GetReviewSMS(pIndex,pSize);
            if (rt.Success)
            {
                Dictionary<int, List<SMSModel.SMS>> dic = rt.Value;
                if (dic != null && dic.Count > 0)
                {
                    foreach (var v in dic)
                    {
                        result.Total = v.Key;
                        result.PageCount = GetTotalPage(result.Total, pSize);
                        foreach (SMSModel.SMS sms in v.Value)
                        {
                            result.SMSList.Add(SMSModelToSMS(sms));
                        }
                        break;
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }


        // 获取预处理短信总数
        public int GetSMSCountByAccount(string accountID)
        {
            if (string.IsNullOrEmpty(accountID)) return 0;
            SMSModel.RPCResult<int> rt = ZHSMSProxy.GetPretreatmentService().GetSMSCountByAccount(accountID);
            if (rt.Success) return rt.Value;
            return 0;
        }

        // 设置未发送短信级别
        public RPCResult SetSMSLevel(string serialNumber, LevelType level, string operatorAccount)
        {
            SMSModel.RPCResult rt = ZHSMSProxy.GetPretreatmentService().SetSMSLevel(Guid.Parse(serialNumber), (SMSModel.LevelType)level, operatorAccount);
            if (rt.Success) return RPCResultHelper.BuildSucessResult();
            return RPCResultHelper.BuildFailureResult(rt.Message);
        }

        // 审核短信
        public RPCResult AuditSMS(string serialNumber, bool audit, string operatorAccount, string resultCase)
        {
            SMSModel.RPCResult rt = ZHSMSProxy.GetPretreatmentService().AuditSMS(Guid.Parse(serialNumber), audit, operatorAccount, resultCase);
            if (rt.Success) return RPCResultHelper.BuildSucessResult();
            return RPCResultHelper.BuildFailureResult(rt.Message);
        }

        // 删除待发送的短信
        public RPCResult DelSMS(List<string> serialNumbers)
        {
            List<Guid> list = new List<Guid>();
            foreach (string s in serialNumbers)
            {
                list.Add(Guid.Parse(s));
            }
            SMSModel.RPCResult rt = ZHSMSProxy.GetPretreatmentService().DelSMS(list);
            if (rt.Success) return RPCResultHelper.BuildSucessResult();
            return RPCResultHelper.BuildFailureResult(rt.Message);
        }

        // 根据企业获取待审核的短信
        public RPCSMSListResult GetSMSAuditByAccount(string accountID, string beginTime, string endTime, int pSize, int pIndex)
        {
            RPCSMSListResult result = new RPCSMSListResult();
            result.Message = "";
            result.SMSList = new List<SMS>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (string.IsNullOrEmpty(accountID))
            {
                result.Message = "帐号是空的";
                result.Success = false;
                return result;
            }
            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            DateParse dp = getDateParse(beginTime, endTime);
            if (!dp.Result)
            {
                result.Message = dp.Message;
                result.Success = false;
                return result;
            }
            beginTime = dp.BeginTime;
            endTime = dp.EndTime;
            if (DateTime.Compare(DateTime.Parse(endTime).AddMonths(-1), DateTime.Parse(beginTime)) > 0)
            {
                result.Message = "时间范围不允许超过1个月";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<SMSModel.SMS>> rt = ZHSMSProxy.GetPretreatmentService().GetSMSAuditByAccount(accountID, DateTime.Parse(beginTime), DateTime.Parse(endTime));
            if (rt.Success)
            {
                List<SMSModel.SMS> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.SMS>.GetList(list, pSize, pIndex);
                    foreach (SMSModel.SMS sms in list)
                    {
                        result.SMSList.Add(SMSModelToSMS(sms));
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 根据时间获取全部审核的短信
        public RPCSMSListResult GetSMSByAudit(string beginTime, string endTime, int pSize, int pIndex)
        {
            RPCSMSListResult result = new RPCSMSListResult();
            result.Message = "";
            result.SMSList = new List<SMS>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            DateParse dp = getDateParse(beginTime, endTime);
            if (!dp.Result)
            {
                result.Message = dp.Message;
                result.Success = false;
                return result;
            }
            beginTime = dp.BeginTime;
            endTime = dp.EndTime;
            if (DateTime.Compare(DateTime.Parse(endTime).AddMonths(-1), DateTime.Parse(beginTime)) > 0)
            {
                result.Message = "时间范围不允许超过1个月";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<SMSModel.SMS>> rt = ZHSMSProxy.GetPretreatmentService().GetSMSByAudit(DateTime.Parse(beginTime), DateTime.Parse(endTime));
            if (rt.Success)
            {
                List<SMSModel.SMS> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.SMS>.GetList(list, pSize, pIndex);
                    foreach (SMSModel.SMS sms in list)
                    {
                        result.SMSList.Add(SMSModelToSMS(sms));
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 根据企业获取审核失败短信
        public RPCFailureSMSListResult GetSMSByAuditFailure(string accountID, int pSize, int pIndex)
        {
            RPCFailureSMSListResult result = new RPCFailureSMSListResult();
            result.Message = "";
            result.SMSList = new List<FailureSMS>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (string.IsNullOrEmpty(accountID))
            {
                result.Message = "帐号是空的";
                result.Success = false;
                return result;
            }
            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<SMSModel.FailureSMS>> rt = ZHSMSProxy.GetPretreatmentService().GetSMSByAuditFailure(accountID);
            if (rt.Success)
            {
                List<SMSModel.FailureSMS> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.FailureSMS>.GetList(list, pSize, pIndex);
                    foreach (SMSModel.FailureSMS sms in list)
                    {
                        result.SMSList.Add(SMSModelToFailureSMS(sms));
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 获取全部审核失败短信
        public RPCFailureSMSListResult GetSMSByAuditFailures(string beginTime, string endTime, int pSize, int pIndex)
        {
            RPCFailureSMSListResult result = new RPCFailureSMSListResult();
            result.Message = "";
            result.SMSList = new List<FailureSMS>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            DateParse dp = getDateParse(beginTime, endTime);
            if (!dp.Result)
            {
                result.Message = dp.Message;
                result.Success = false;
                return result;
            }
            beginTime = dp.BeginTime;
            endTime = dp.EndTime;
            if (DateTime.Compare(DateTime.Parse(endTime).AddMonths(-1), DateTime.Parse(beginTime)) > 0)
            {
                result.Message = "时间范围不允许超过1个月";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<SMSModel.FailureSMS>> rt = ZHSMSProxy.GetPretreatmentService().GetSMSByAuditFailure(DateTime.Parse(beginTime), DateTime.Parse(endTime));
            if (rt.Success)
            {
                List<SMSModel.FailureSMS> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.FailureSMS>.GetList(list, pSize, pIndex);
                    foreach (SMSModel.FailureSMS sms in list)
                    {
                        result.SMSList.Add(SMSModelToFailureSMS(sms));
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 确认删除审核失败的短信
        public RPCResult AffirmAuditFailureSMS(List<string> serialNumbers)
        {
            try
            {
                if (serialNumbers == null || serialNumbers.Count == 0) return RPCResultHelper.BuildFailureResult("要删的短信不能为空");
                foreach (var serialNumber in serialNumbers)
                {
                    ZHSMSProxy.GetPretreatmentService().AffirmAuditFailureSMS(Guid.Parse(serialNumber));

                } return RPCResultHelper.BuildSucessResult();
            }
            catch
            {
                return RPCResultHelper.BuildFailureResult("删除失败");
            }
        }

        // 添加黑名单
        public RPCResult AddBlacklist(List<string> numbers)
        {
            SMSModel.RPCResult rt = ZHSMSProxy.GetPretreatmentService().AddBlacklist(numbers);
            if (rt.Success) return RPCResultHelper.BuildSucessResult();
            return RPCResultHelper.BuildFailureResult(rt.Message);
        }

        // 删除黑名单
        public RPCResult DelBlacklist(List<string> numbers)
        {
            SMSModel.RPCResult rt = ZHSMSProxy.GetPretreatmentService().DelBlacklist(numbers);
            if (rt.Success) return RPCResultHelper.BuildSucessResult();
            return RPCResultHelper.BuildFailureResult(rt.Message);
        }

        // 获取黑名单
        public RPCListResult GetBlacklist(string phoneNumber,int pSize, int pIndex)
        {
            RPCListResult result = new RPCListResult();
            result.Message = "";
            result.Lists = new List<string>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;
            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<string>> rt = ZHSMSProxy.GetPretreatmentService().GetBlacklist();
            if (rt.Success)
            {
                List<string> list = rt.Value;
                
                if (list != null && list.Count > 0)
                {
                    if (!string.IsNullOrEmpty(phoneNumber))
                    {
                        list = list.Where(c => c.Contains(phoneNumber)).ToList<string>();
                    }
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<string>.GetList(list, pSize, pIndex);
                    result.Lists = list;
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 添加敏感词组
        public RPCResult AddKeywordsGroup(string keyGroup, string remark)
        {
            if (keyGroup == "-1") return RPCResultHelper.BuildFailureResult("已存在此敏感词组");
            SMSModel.RPCResult rt = ZHSMSProxy.GetPretreatmentService().AddKeywordsGroup(keyGroup, remark);
            if (rt.Success) return RPCResultHelper.BuildSucessResult();
            return RPCResultHelper.BuildFailureResult(rt.Message);
        }

        // 添加敏感词类别
        public RPCResult AddKeywordsType(string type, string remark)
        {
            if (type == "-1") return RPCResultHelper.BuildFailureResult("已存在此敏感词类别");
            SMSModel.RPCResult rt = ZHSMSProxy.GetPretreatmentService().AddKeywordsType(type, remark);
            if (rt.Success) return RPCResultHelper.BuildSucessResult();
            return RPCResultHelper.BuildFailureResult(rt.Message);
        }

        // 获取敏感词类别
        public RPCDictionaryResult GetKeywordsTypes(int pSize, int pIndex)
        {
            RPCDictionaryResult result = new RPCDictionaryResult();
            result.Message = "";
            result.Dictionarys = new List<DictionaryC>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<Dictionary<string, string>> rt = ZHSMSProxy.GetPretreatmentService().GetKeywordsTypes();
            if (rt.Success)
            {
                Dictionary<string, string> li = rt.Value;
                List<DictionaryC> list = new List<DictionaryC>();
                foreach (var v in li)
                {
                    DictionaryC dic = new DictionaryC();
                    dic.Key = v.Key;
                    dic.Value = v.Value;
                    list.Add(dic);
                }
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<DictionaryC>.GetList(list, pSize, pIndex);
                    foreach (DictionaryC di in list)
                    {
                        result.Dictionarys.Add(di);
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 获取关键词组
        public RPCDictionaryResult GetKeyGroups(int pSize, int pIndex)
        {
            RPCDictionaryResult result = new RPCDictionaryResult();
            result.Message = "";
            result.Dictionarys = new List<DictionaryC>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<Dictionary<string, string>> rt = ZHSMSProxy.GetPretreatmentService().GetKeyGroups();
            if (rt.Success)
            {
                Dictionary<string, string> li = rt.Value;
                List<DictionaryC> list = new List<DictionaryC>();
                foreach (var v in li)
                {
                    DictionaryC dic = new DictionaryC();
                    dic.Key = v.Key;
                    dic.Value = v.Value;
                    list.Add(dic);
                }
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<DictionaryC>.GetList(list, pSize, pIndex);
                    foreach (DictionaryC di in list)
                    {
                        result.Dictionarys.Add(di);
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        public RPCDictionaryResult GetKeywordsTypesByWord(string typeName, int pSize, int pIndex)
        {
            RPCDictionaryResult result = new RPCDictionaryResult();
            result.Message = "";
            result.Dictionarys = new List<DictionaryC>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<Dictionary<string, string>> rt = ZHSMSProxy.GetPretreatmentService().GetKeywordsTypes();
            if (rt.Success)
            {
                Dictionary<string, string> li = rt.Value;
                li = li.Where(c => c.Value.Contains(typeName)).ToDictionary(c => c.Key, c => c.Value);
                List<DictionaryC> list = new List<DictionaryC>();
                foreach (var v in li)
                {
                    DictionaryC dic = new DictionaryC();
                    dic.Key = v.Key;
                    dic.Value = v.Value;
                    list.Add(dic);
                }
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<DictionaryC>.GetList(list, pSize, pIndex);
                    foreach (DictionaryC di in list)
                    {
                        result.Dictionarys.Add(di);
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        public RPCDictionaryResult GetKeyGroupsByWord(string groupName, int pSize, int pIndex)
        {
            RPCDictionaryResult result = new RPCDictionaryResult();
            result.Message = "";
            result.Dictionarys = new List<DictionaryC>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<Dictionary<string, string>> rt = ZHSMSProxy.GetPretreatmentService().GetKeyGroups();
            if (rt.Success)
            {
                Dictionary<string, string> li = rt.Value;
                li = li.Where(c => c.Value.Contains(groupName)).ToDictionary(c => c.Key, c => c.Value);
                List<DictionaryC> list = new List<DictionaryC>();
                foreach (var v in li)
                {
                    DictionaryC dic = new DictionaryC();
                    dic.Key = v.Key;
                    dic.Value = v.Value;
                    list.Add(dic);
                }
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<DictionaryC>.GetList(list, pSize, pIndex);
                    foreach (DictionaryC di in list)
                    {
                        result.Dictionarys.Add(di);
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }


        // 根据词获取敏感词（模糊查询）
        public RPCKeywordsListResult GetKeywordsByKeyword(string keyword, int pSize, int pIndex)
        {
            RPCKeywordsListResult result = new RPCKeywordsListResult();
            result.Message = "";
            result.Words = new List<Keywords>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<SMSModel.Keywords>> rt = ZHSMSProxy.GetPretreatmentService().GetKeywordsByKeyword(keyword);
            if (rt.Success)
            {
                List<SMSModel.Keywords> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.Keywords>.GetList(list, pSize, pIndex);
                    foreach (SMSModel.Keywords word in list)
                    {
                        result.Words.Add(SMSModelToKeywords(word));
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 获取敏感词列表
        public RPCKeywordsListResult GetAllKeywords(int pSize, int pIndex)
        {
            RPCKeywordsListResult result = new RPCKeywordsListResult();
            result.Message = "";
            result.Words = new List<Keywords>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<Dictionary<int, List<SMSModel.Keywords>>> rt = ZHSMSProxy.GetPretreatmentService().GetAllKeywords(pIndex, pSize);
            if (rt.Success)
            {
                Dictionary<int, List<SMSModel.Keywords>> dic = rt.Value;
                if (dic != null && dic.Count > 0)
                {
                    foreach (var v in dic)
                    {
                        result.Total = v.Key;
                        result.PageCount = GetTotalPage(result.Total, pSize);
                        foreach (SMSModel.Keywords word in v.Value)
                        {
                            result.Words.Add(SMSModelToKeywords(word));
                        }
                        break;
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }


        public RPCKeywordsListResult GetKeywordsByEnable(bool enabled, int pSize, int pIndex)
        {
            RPCKeywordsListResult result = new RPCKeywordsListResult();
            result.Message = "";
            result.Words = new List<Keywords>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<Dictionary<int, List<SMSModel.Keywords>>> rt = ZHSMSProxy.GetPretreatmentService().GetKeywordsByEnable(enabled, pIndex, pSize);
            if (rt.Success)
            {
                Dictionary<int, List<SMSModel.Keywords>> dic = rt.Value;
                if (dic != null && dic.Count > 0)
                {
                    foreach (var v in dic)
                    {
                        result.Total = v.Key;
                        result.PageCount = GetTotalPage(result.Total, pSize);
                        foreach (SMSModel.Keywords word in v.Value)
                        {
                            result.Words.Add(SMSModelToKeywords(word));
                        }
                        break;
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }


        //根据组合条件查询敏感词
        public RPCKeywordsListResult GetKeywordsByCompositionCondition(string groupName, string typeName, int enabled, int pSize, int pIndex)
        {
            RPCKeywordsListResult result = new RPCKeywordsListResult();
            result.Message = "";
            result.Words = new List<Keywords>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<Dictionary<int, List<SMSModel.Keywords>>> rt = ZHSMSProxy.GetPretreatmentService().GetKeywordsByCompositionCondition(groupName, typeName, enabled, pSize, pIndex);
            if (rt.Success)
            {
                Dictionary<int, List<SMSModel.Keywords>> dic = rt.Value;
                if (dic != null && dic.Count > 0)
                {
                    foreach (var v in dic)
                    {
                        result.Total = v.Key;
                        result.PageCount = GetTotalPage(result.Total, pSize);
                        foreach (SMSModel.Keywords word in v.Value)
                        {
                            result.Words.Add(SMSModelToKeywords(word));
                        }
                        break;
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 添加关键词
        public RPCResult AddKeywords(string keyGroup, List<Keywords> keywords)
        {
            if (string.IsNullOrEmpty(keyGroup)) return RPCResultHelper.BuildFailureResult("请给敏感词指定一个词组");
            if (keywords == null || keywords.Count == 0) return RPCResultHelper.BuildFailureResult("请填写要添加的敏感词");
            List<SMSModel.Keywords> list = new List<SMSModel.Keywords>();
            foreach (var v in keywords)
            {
                SMSModel.Keywords k = new SMSModel.Keywords();
                k.Words = v.Words;
                k.ReplaceKeywords = v.ReplaceKeywords;
                k.KeywordsType = v.KeywordsType;
                k.KeyGroup = v.KeyGroup;
                k.Enable = v.Enable;
                list.Add(k);
            }
            SMSModel.RPCResult rt = ZHSMSProxy.GetPretreatmentService().AddKeywords(keyGroup, list);
            if (rt.Success) return RPCResultHelper.BuildSucessResult();
            return RPCResultHelper.BuildFailureResult(rt.Message);
        }

        // 删除关键词
        public RPCResult DelKeywords(string keyGroup, List<string> keywords)
        {
            if (string.IsNullOrEmpty(keyGroup)) return RPCResultHelper.BuildFailureResult("敏感词词组不能为空");
            if (keywords == null || keywords.Count == 0) return RPCResultHelper.BuildFailureResult("请指定要删除的敏感词");
            SMSModel.RPCResult rt = ZHSMSProxy.GetPretreatmentService().DelKeywords(keyGroup, keywords);
            if (rt.Success) return RPCResultHelper.BuildSucessResult();
            return RPCResultHelper.BuildFailureResult(rt.Message);
        }

        //删除敏感词组
        public RPCResult DelKeywordsGroup(string keyGroup)
        {
            try
            {
                if (string.IsNullOrEmpty(keyGroup)) return RPCResultHelper.BuildFailureResult("敏感词词组不能为空");
                SMSModel.RPCResult rt = ZHSMSProxy.GetPretreatmentService().DelKeywordGroup(keyGroup);
                if (rt.Success) return RPCResultHelper.BuildSucessResult();
                return RPCResultHelper.BuildFailureResult(rt.Message);
            }
            catch
            {
                return RPCResultHelper.BuildFailureResult("删除失败");
            }
        }

        // 获取关键词组内的关键词
        public RPCKeywordsListResult GetKeywords(string keyGroup, int pSize, int pIndex)
        {
            RPCKeywordsListResult result = new RPCKeywordsListResult();
            result.Message = "";
            result.Words = new List<Keywords>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (string.IsNullOrEmpty(keyGroup))
            {
                result.Message = "敏感词词组不能为空";
                result.Success = false;
                return result;
            }
            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<SMSModel.Keywords>> rt = ZHSMSProxy.GetPretreatmentService().GetKeywords(keyGroup);
            if (rt.Success)
            {
                List<SMSModel.Keywords> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.Keywords>.GetList(list, pSize, pIndex);
                    foreach (SMSModel.Keywords word in list)
                    {
                        result.Words.Add(SMSModelToKeywords(word));
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 根据类型获取关键词
        public RPCKeywordsListResult GetKeywordsByType(string type, int pSize, int pIndex)
        {
            RPCKeywordsListResult result = new RPCKeywordsListResult();
            result.Message = "";
            result.Words = new List<Keywords>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (string.IsNullOrEmpty(type))
            {
                result.Message = "敏感词类型不能为空";
                result.Success = false;
                return result;
            }
            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<SMSModel.Keywords>> rt = ZHSMSProxy.GetPretreatmentService().GetKeywordsByType(type);
            if (rt.Success)
            {
                List<SMSModel.Keywords> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.Keywords>.GetList(list, pSize, pIndex);
                    foreach (SMSModel.Keywords word in list)
                    {
                        result.Words.Add(SMSModelToKeywords(word));
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 敏感词状态启用与否
        public RPCResult KeywordsEnabled(string keyGroup, string keywords, bool enabled)
        {
            SMSModel.RPCResult rt = ZHSMSProxy.GetPretreatmentService().KeywordsEnabled(keyGroup, keywords, enabled);
            if (rt.Success) return RPCResultHelper.BuildSucessResult();
            return RPCResultHelper.BuildFailureResult(rt.Message);
        }

        // 添加关键词组与网关绑定信息
        public RPCResult AddkeyGroupGatewayBind(string keyGroup, string gateway)
        {
            SMSModel.RPCResult rt = ZHSMSProxy.GetPretreatmentService().AddkeyGroupGatewayBind(keyGroup, gateway);
            if (rt.Success) return RPCResultHelper.BuildSucessResult();
            return RPCResultHelper.BuildFailureResult(rt.Message);
        }

        // 获取网关绑定的关键词组
        public RPCResult GetKeyGroupGatewayBinds(string gateway)
        {
            SMSModel.RPCResult<string> rt = ZHSMSProxy.GetPretreatmentService().GetKeyGroupGatewayBinds(gateway);
            if (rt.Success) return RPCResultHelper.BuildSucessResult(rt.Value);
            return RPCResultHelper.BuildFailureResult(rt.Message);
        }

        // 添加网关配置
        public RPCResult AddGatewayConfig(GatewayConfiguration config)
        {
            if (config != null)
            {
                SMSModel.GatewayConfiguration con = new SMSModel.GatewayConfiguration();
                con.Gateway = config.Gateway;
                con.HandlingAbility = config.HandlingAbility;
                con.Operators = config.Operators;
                SMSModel.RPCResult rt = ZHSMSProxy.GetPretreatmentService().AddGatewayConfig(con);
                if (rt.Success) return RPCResultHelper.BuildSucessResult();
                return RPCResultHelper.BuildFailureResult(rt.Message);
            }
            return RPCResultHelper.BuildFailureResult("网关配置不能为空");
        }

        // 更新网关配置
        public RPCResult UpdateGatewayConfig(GatewayConfiguration config)
        {
            if (config != null)
            {
                SMSModel.GatewayConfiguration con = new SMSModel.GatewayConfiguration();
                con.Gateway = config.Gateway;
                con.HandlingAbility = config.HandlingAbility;
                con.Operators = config.Operators;
                SMSModel.RPCResult rt = ZHSMSProxy.GetPretreatmentService().UpdateGatewayConfig(con);
                if (rt.Success)
                {
                    return RPCResultHelper.BuildSucessResult();
                }
                else
                {
                    return RPCResultHelper.BuildFailureResult(rt.Message);
                }
            }
            return RPCResultHelper.BuildFailureResult("网关配置不能为空");
        }

        // 获取网关配置
        public GatewayConfiguration GetGatewayConfig(string gateway)
        {
            if (string.IsNullOrEmpty(gateway)) return null;
            SMSModel.RPCResult<SMSModel.GatewayConfiguration> rt = ZHSMSProxy.GetPretreatmentService().GetGatewayConfig(gateway);
            if (rt.Success)
            {
                SMSModel.GatewayConfiguration config = rt.Value;
                GatewayConfiguration con = new GatewayConfiguration();
                con.Gateway = config.Gateway;
                con.HandlingAbility = config.HandlingAbility;
                con.Operators = config.Operators;
                return con;
            }
            return null;
        }

        // 获取网关配置信息
        public RPCGatewayConfigListResult GetGatewayConfigs(int pSize, int pIndex)
        {
            RPCGatewayConfigListResult result = new RPCGatewayConfigListResult();
            result.Message = "";
            result.Configs = new List<GatewayConfiguration>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<SMSModel.GatewayConfiguration>> rt = ZHSMSProxy.GetPretreatmentService().GetGatewayConfigs();
            if (rt.Success)
            {
                List<SMSModel.GatewayConfiguration> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.GatewayConfiguration>.GetList(list, pSize, pIndex);
                    foreach (SMSModel.GatewayConfiguration config in list)
                    {
                        GatewayConfiguration con = new GatewayConfiguration();
                        con.Gateway = config.Gateway;
                        con.HandlingAbility = config.HandlingAbility;
                        con.Operators = config.Operators;
                        result.Configs.Add(con);
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 删除网关配置
        public RPCResult DelGatewayConfig(string gateway)
        {
            try
            {
                if(string.IsNullOrEmpty(gateway)) return  RPCResultHelper.BuildFailureResult("要删除的网关是空的");
                SMSModel.RPCResult rt = ZHSMSProxy.GetPretreatmentService().DelGatewayConfig(gateway);
                if(rt.Success)return RPCResultHelper.BuildSucessResult();
                return RPCResultHelper.BuildFailureResult(rt.Message);
            }
            catch
            {
                return RPCResultHelper.BuildFailureResult("删除失败");
            }
        }

        // 添加通道
        public RPCResult AddChannel(Channel channel)
        {
            if (channel != null)
            {
                SMSModel.Channel ch = new SMSModel.Channel();
                ch.ChannelID = channel.ChannelID;
                ch.ChannelName = channel.ChannelName;
                ch.Remark = channel.Remark;
                SMSModel.RPCResult rt = ZHSMSProxy.GetPretreatmentService().AddChannel(ch);
                if (rt.Success) return RPCResultHelper.BuildSucessResult();
                return RPCResultHelper.BuildFailureResult(rt.Message);
            }
            return RPCResultHelper.BuildFailureResult("通道不能为空");
        }

        // 更新通道
        public RPCResult UpdateChannel(Channel channel)
        {
            if (channel != null)
            {
                SMSModel.Channel ch = new SMSModel.Channel();
                ch.ChannelID = channel.ChannelID;
                ch.ChannelName = channel.ChannelName;
                ch.Remark = channel.Remark;
                SMSModel.RPCResult rt = ZHSMSProxy.GetPretreatmentService().UpdateChannel(ch);
                if (rt.Success) return RPCResultHelper.BuildSucessResult();
                return RPCResultHelper.BuildFailureResult(rt.Message);
            }
            return RPCResultHelper.BuildFailureResult("通道不能为空");
        }

        // 获取所有通道
        public RPCChannelListResult GetChannels(int pSize, int pIndex)
        {
            RPCChannelListResult result = new RPCChannelListResult();
            result.Message = "";
            result.Channels = new List<Channel>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<SMSModel.Channel>> rt = ZHSMSProxy.GetPretreatmentService().GetChannels();
            if (rt.Success)
            {
                List<SMSModel.Channel> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.Channel>.GetList(list, pSize, pIndex);
                    foreach (var v in list)
                    {
                        Channel channel = new Channel();
                        channel.ChannelID = v.ChannelID;
                        channel.ChannelName = v.ChannelName;
                        channel.Remark = v.Remark;
                        result.Channels.Add(channel);
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 获取通道
        public Channel GetChannel(string channel)
        {
            if (string.IsNullOrEmpty(channel)) return null;
            SMSModel.RPCResult<SMSModel.Channel> rt = ZHSMSProxy.GetPretreatmentService().GetChannel(channel);
            if (rt.Success)
            {
                SMSModel.Channel c = rt.Value;
                Channel ch = new Channel();
                ch.ChannelID = c.ChannelID;
                ch.ChannelName = c.ChannelName;
                ch.Remark = c.Remark;
                return ch;
            }
            return null;
        }

        // 删除通道
        public RPCResult DelChannel(string channel)
        {
            try
            {
                if (string.IsNullOrEmpty(channel)) return RPCResultHelper.BuildFailureResult("要删除的通道不能为空");
                SMSModel.RPCResult rt = ZHSMSProxy.GetPretreatmentService().DelChannel(channel);
                if(rt.Success) return RPCResultHelper.BuildSucessResult();
                return RPCResultHelper.BuildFailureResult(rt.Message);
            }
            catch
            {
                return RPCResultHelper.BuildFailureResult("删除通道失败");
            }
        }

        // 添加通道绑定网关信息
        public RPCResult AddChannelGatewayBind(string channel, List<string> gateways)
        {
            if (string.IsNullOrEmpty(channel)) return RPCResultHelper.BuildFailureResult("要绑定的通道不能为空");
            if (gateways == null || gateways.Count == 0) return RPCResultHelper.BuildFailureResult("要绑定的网关不能为空");
            SMSModel.RPCResult rt = ZHSMSProxy.GetPretreatmentService().AddChannelGatewayBind(channel, gateways);
            if (rt.Success) return RPCResultHelper.BuildSucessResult();
            return RPCResultHelper.BuildFailureResult(rt.Message);
        }

        // 获取通道绑定的网关
        public RPCListResult GetGatewaysByChannel(string channel, int pSize, int pIndex)
        {
            RPCListResult result = new RPCListResult();
            result.Message = "";
            result.Lists = new List<string>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;
            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<string>> rt = ZHSMSProxy.GetPretreatmentService().GetGatewaysByChannel(channel);
            if (rt.Success)
            {
                List<string> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<string>.GetList(list, pSize, pIndex);
                    result.Lists = list;
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 获取短信审核记录列表
        public RPCAuditRecordListResult GetAuditRecords(string beginTime, string endTime, int pSize, int pIndex)
        {
            RPCAuditRecordListResult result = new RPCAuditRecordListResult();
            result.Message = "";
            result.Records = new List<AuditRecord>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            DateParse dp = getDateParse(beginTime, endTime);
            if (!dp.Result)
            {
                result.Message = dp.Message;
                result.Success = false;
                return result;
            }
            beginTime = dp.BeginTime;
            endTime = dp.EndTime;
            if (DateTime.Compare(DateTime.Parse(endTime).AddMonths(-1), DateTime.Parse(beginTime)) > 0)
            {
                result.Message = "时间范围不允许超过1个月";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<SMSModel.AuditRecord>> rt = ZHSMSProxy.GetPretreatmentService().GetAuditRecords(DateTime.Parse(beginTime), DateTime.Parse(endTime));
            if (rt.Success)
            {
                List<SMSModel.AuditRecord> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.AuditRecord>.GetList(list, pSize, pIndex);
                    foreach (var v in list)
                    {
                        result.Records.Add(SMSModelToAuditRecord(v));
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 获取短信审核记录
        public AuditRecord GetAuditRecord(string serialNumber)
        {
            if (string.IsNullOrEmpty(serialNumber)) return null;
            SMSModel.RPCResult<SMSModel.AuditRecord> rt = ZHSMSProxy.GetPretreatmentService().GetAuditRecord(Guid.Parse(serialNumber));
            if (rt.Success)
            {
                AuditRecord r = SMSModelToAuditRecord(rt.Value);
                return r;
            }
            return null;
        }

        // 获取短信级别调整记录
        public RPCLevelModifyRecordListResult GetLevelModifyRecords(string beginTime, string endTime, int pSize, int pIndex)
        {
            RPCLevelModifyRecordListResult result = new RPCLevelModifyRecordListResult();
            result.Message = "";
            result.Records = new List<LevelModifyRecord>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            DateParse dp = getDateParse(beginTime, endTime);
            if (!dp.Result)
            {
                result.Message = dp.Message;
                result.Success = false;
                return result;
            }
            beginTime = dp.BeginTime;
            endTime = dp.EndTime;
            if (DateTime.Compare(DateTime.Parse(endTime).AddMonths(-1), DateTime.Parse(beginTime)) > 0)
            {
                result.Message = "时间范围不允许超过1个月";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<SMSModel.LevelModifyRecord>> rt = ZHSMSProxy.GetPretreatmentService().GetLevelModifyRecords(DateTime.Parse(beginTime), DateTime.Parse(endTime));
            if (rt.Success)
            {
                List<SMSModel.LevelModifyRecord> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.LevelModifyRecord>.GetList(list, pSize, pIndex);
                    foreach (var v in list)
                    {
                        result.Records.Add(SMSModelToLevelModifyRecord(v));
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 短信优先级调整记录
        public RPCLevelModifyRecordListResult GetLevelModifyRecord(string serialNumber, int pSize, int pIndex)
        {
            RPCLevelModifyRecordListResult result = new RPCLevelModifyRecordListResult();
            result.Message = "";
            result.Records = new List<LevelModifyRecord>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (string.IsNullOrEmpty(serialNumber))
            {
                result.Message = "所查询的短信业务号不能为空";
                result.Success = false;
                return result;
            }
            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<SMSModel.LevelModifyRecord>> rt = ZHSMSProxy.GetPretreatmentService().GetLevelModifyRecord(Guid.Parse(serialNumber));
            if (rt.Success)
            {
                List<SMSModel.LevelModifyRecord> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.LevelModifyRecord>.GetList(list, pSize, pIndex);
                    foreach (var v in list)
                    {
                        result.Records.Add(SMSModelToLevelModifyRecord(v));
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 根据业务单获取短信状态明细
        public RPCStatusReportListResult GetSMSStatusReport(string serialNumber, string sendTime, int pSize, int pIndex)
        {
            RPCStatusReportListResult result = new RPCStatusReportListResult();
            result.Message = "";
            result.Records = new List<StatusReport>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (string.IsNullOrEmpty(serialNumber))
            {
                result.Message = "所查询的短信业务号不能为空";
                result.Success = false;
                return result;
            }
            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            if (!IsDateTime(sendTime))
            {
                result.Message = "时间格式不正确，应为yyyy-MM-dd HH:mm:ss";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<SMSModel.StatusReport>> rt = ZHSMSProxy.GetStatusReportService().GetSMSStatusReport(Guid.Parse(serialNumber), DateTime.Parse(sendTime));
            if (rt.Success)
            {
                List<SMSModel.StatusReport> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.StatusReport>.GetList(list, pSize, pIndex);
                    foreach (var v in list)
                    {
                        result.Records.Add(SMSModelToStatusReport(v));
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 查看短信统计报告（缓存无，数据库获取）
        public ReportStatistics GetReportStatistics(string serialNumber, string sendTime)
        {
            if (string.IsNullOrEmpty(serialNumber)) return null;
            if (!IsDateTime(sendTime)) return null;
            SMSModel.RPCResult<SMSModel.ReportStatistics> rt = ZHSMSProxy.GetStatusReportService().GetReportStatistics(Guid.Parse(serialNumber), DateTime.Parse(sendTime));
            if (rt.Success)
            {
                return SMSModelToReportStatistics(rt.Value);
            }
            return null;
        }

        // 根据企业获取短信原始记录
        public RPCSMSListResult GetSMSRecordByAccount(string account, string beginTime, string endTime, int pSize, int pIndex)
        {
            RPCSMSListResult result = new RPCSMSListResult();
            result.Message = "";
            result.SMSList = new List<SMS>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (string.IsNullOrEmpty(account))
            {
                result.Message = "企业账号不能为空";
                result.Success = false;
                return result;
            }
            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            DateParse dp = getDateParse(beginTime, endTime);
            if (!dp.Result)
            {
                result.Message = dp.Message;
                result.Success = false;
                return result;
            }
            beginTime = dp.BeginTime;
            endTime = dp.EndTime;
            if (DateTime.Compare(DateTime.Parse(endTime).AddMonths(-1), DateTime.Parse(beginTime)) > 0)
            {
                result.Message = "时间范围不允许超过1个月";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<SMSModel.SMS>> rt = ZHSMSProxy.GetStatusReportService().GetSMSRecordByAccount(account, DateTime.Parse(beginTime), DateTime.Parse(endTime));
            if (rt.Success)
            {
                List<SMSModel.SMS> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.SMS>.GetList(list, pSize, pIndex);
                    foreach (SMSModel.SMS sms in list)
                    {
                        result.SMSList.Add(SMSModelToSMS(sms));
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 所有短信统计（发送总数，失败总数）
        public SMSSendResult GetSMSStatistics(string beginTime, string endTime)
        {
            SMSSendResult sr = new SMSSendResult();
            sr.FailureCount = 0;
            sr.SendCount = 0;
            sr.Message = "";
            sr.Success = true;
            DateParse dp = getDateParse(beginTime, endTime);
            if (!dp.Result)
            {
                sr.Message = dp.Message;
                sr.Success = false;
                return sr;
            }
            SMSModel.RPCResult<string[,]> rt = ZHSMSProxy.GetStatusReportService().GetSMSStatistics(DateTime.Parse(beginTime), DateTime.Parse(endTime));
            if (rt.Success)
            {
                sr.SendCount = int.Parse(rt.Value[0, 0]);
                sr.FailureCount = int.Parse(rt.Value[0, 1]);
            }
            return sr;
        }

        // 获取企业短信统计信息（3个月以内的统计，数据库不包含内存）
        public SMSSendResult GetSMSStatisticsByAccount(string accountID, string beginTime, string endTime)
        {
            SMSSendResult sr = new SMSSendResult();
            sr.FailureCount = 0;
            sr.SendCount = 0;
            sr.Message = "";
            sr.Success = true;
            if (string.IsNullOrEmpty(accountID))
            {
                sr.Message = "企业账号不能为空";
                sr.Success = false;
                return sr;
            }
            DateParse dp = getDateParse(beginTime, endTime);
            if (!dp.Result)
            {
                sr.Message = dp.Message;
                sr.Success = false;
                return sr;
            }
            if (DateTime.Compare(DateTime.Parse(endTime).AddMonths(-3), DateTime.Parse(beginTime)) > 0)
            {
                sr.Message = "时间范围不允许超过3个月";
                sr.Success = false;
                return sr;
            }
            SMSModel.RPCResult<string[,]> rt = ZHSMSProxy.GetStatusReportService().GetSMSStatisticsByAccount(accountID, DateTime.Parse(beginTime), DateTime.Parse(endTime));
            if (rt.Success)
            {
                sr.SendCount = int.Parse(rt.Value[0, 0]);
                sr.FailureCount = int.Parse(rt.Value[0, 1]);
            }
            return sr;
        }

        //获取各企业的短信统计（发送数，失败数）
        public RPCAccountSMSStatisticsListResult GetAccountSMSStatistics(string beginTime, string endTime, int pSize, int pIndex)
        {
            RPCAccountSMSStatisticsListResult result = new RPCAccountSMSStatisticsListResult();
            result.Message = "";
            result.Statistics = new List<SMSStatistics>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;
            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            DateParse dp = getDateParse(beginTime, endTime);
            if (!dp.Result)
            {
                result.Message = dp.Message;
                result.Success = false;
                return result;
            }
            if (DateTime.Compare(DateTime.Parse(endTime).AddMonths(-3), DateTime.Parse(beginTime)) > 0)
            {
                result.Message = "时间范围不允许超过3个月";
                result.Success = false;
                return result;
            }
            if (DateTime.Compare(DateTime.Now.AddYears(-1), DateTime.Parse(beginTime)) > 0)
            {
                result.Message = "1年以前的记录不能统计";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<SMSModel.Account>> rt = ZHSMSProxy.GetPretreatmentService().GetAccounts();
            if (rt.Success)
            {
                List<SMSModel.Account> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.Account>.GetList(list, pSize, pIndex);
                    foreach (SMSModel.Account account in list)
                    {
                        SMSStatistics s = new SMSStatistics();
                        s.AccountID = account.AccountID;
                        SMSModel.RPCResult<string[,]> r = ZHSMSProxy.GetStatusReportService().GetSMSStatisticsByAccount(s.AccountID, DateTime.Parse(beginTime), DateTime.Parse(endTime));
                        if (r.Success)
                        {
                            s.SendCount = int.Parse(r.Value[0, 0]);
                            s.FailureCount = int.Parse(r.Value[0, 1]);
                        }
                        else
                        {
                            s.SendCount = 0;
                            s.FailureCount = 0;
                        }
                        result.Statistics.Add(s);
                    }
                    return result;
                }
                return result;
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 根据短信号码获取上行短信
        public RPCMOSMSListResult GetMOSMS(string spNumber, string beginTime, string endTime, int pSize, int pIndex)
        {
            RPCMOSMSListResult result = new RPCMOSMSListResult();
            result.Message = "";
            result.MOSMSList = new List<MOSMS>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (string.IsNullOrEmpty(spNumber))
            {
                result.Message = "短信号码不能为空";
                result.Success = false;
                return result;
            }
            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            DateParse dp = getDateParse(beginTime, endTime);
            if (!dp.Result)
            {
                result.Message = dp.Message;
                result.Success = false;
                return result;
            }
            beginTime = dp.BeginTime;
            endTime = dp.EndTime;
            if (DateTime.Compare(DateTime.Parse(endTime).AddMonths(-1), DateTime.Parse(beginTime)) > 0)
            {
                result.Message = "时间范围不允许超过1个月";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<SMSModel.MOSMS>> rt = ZHSMSProxy.GetStatusReportService().GetMOSMS(spNumber, DateTime.Parse(beginTime), DateTime.Parse(endTime));
            if (rt.Success)
            {
                List<SMSModel.MOSMS> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.MOSMS>.GetList(list, pSize, pIndex);
                    foreach (SMSModel.MOSMS sms in list)
                    {
                        result.MOSMSList.Add(SMSModelToMOSMS(sms));
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 获取账号统计报告（数据库获取，缓存忽略，直客端使用）
        public RPCReportStatisticListResult GetStatisticsReportByAccount(string account, string beginTime, string endTime, int pSize, int pIndex)
        {
            RPCReportStatisticListResult result = new RPCReportStatisticListResult();
            result.Message = "";
            result.Reports = new List<ReportStatistics>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (string.IsNullOrEmpty(account))
            {
                result.Message = "企业账号不能为空";
                result.Success = false;
                return result;
            }
            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            DateParse dp = getDateParse(beginTime, endTime);
            if (!dp.Result)
            {
                result.Message = dp.Message;
                result.Success = false;
                return result;
            }
            beginTime = dp.BeginTime;
            endTime = dp.EndTime;
            if (DateTime.Compare(DateTime.Parse(endTime).AddMonths(-1), DateTime.Parse(beginTime)) > 0)
            {
                result.Message = "时间范围不允许超过1个月";
                result.Success = false;
                return result;
            }

            SMSModel.RPCResult<List<SMSModel.ReportStatistics>> rt = ZHSMSProxy.GetStatusReportService().GetStatisticsReportByAccount(account, DateTime.Parse(beginTime), DateTime.Parse(endTime));
            if (rt.Success)
            {
                List<SMSModel.ReportStatistics> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.ReportStatistics>.GetList(list, pSize, pIndex);
                    foreach (SMSModel.ReportStatistics rs in list)
                    {
                        result.Reports.Add(SMSModelToReportStatistics(rs));
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 获取账号统计报告(数据库和缓存获取，管理平台使用)
        public RPCReportStatisticListResult GetStatisticsReportAllByAccount(string account, string beginTime, string endTime, int pSize, int pIndex)
        {
            RPCReportStatisticListResult result = new RPCReportStatisticListResult();
            result.Message = "";
            result.Reports = new List<ReportStatistics>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (string.IsNullOrEmpty(account))
            {
                result.Message = "企业账号不能为空";
                result.Success = false;
                return result;
            }
            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            DateParse dp = getDateParse(beginTime, endTime);
            if (!dp.Result)
            {
                result.Message = dp.Message;
                result.Success = false;
                return result;
            }
            beginTime = dp.BeginTime;
            endTime = dp.EndTime;
            if (DateTime.Compare(DateTime.Parse(endTime).AddMonths(-1), DateTime.Parse(beginTime)) > 0)
            {
                result.Message = "时间范围不允许超过1个月";
                result.Success = false;
                return result;
            }

            SMSModel.RPCResult<List<SMSModel.ReportStatistics>> rt = ZHSMSProxy.GetStatusReportService().GetStatisticsReportAllByAccount(account, DateTime.Parse(beginTime), DateTime.Parse(endTime));
            if (rt.Success)
            {
                List<SMSModel.ReportStatistics> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.ReportStatistics>.GetList(list, pSize, pIndex);
                    foreach (SMSModel.ReportStatistics rs in list)
                    {
                        result.Reports.Add(SMSModelToReportStatistics(rs));
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 根据帐号获取未过期的短信统计报告（缓存）
        public RPCReportStatisticListResult GetDirectStatisticReportByAccount(string account, int pSize, int pIndex)
        {
            RPCReportStatisticListResult result = new RPCReportStatisticListResult();
            result.Message = "";
            result.Reports = new List<ReportStatistics>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (string.IsNullOrEmpty(account))
            {
                result.Message = "企业账号不能为空";
                result.Success = false;
                return result;
            }
            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }

            SMSModel.RPCResult<List<SMSModel.ReportStatistics>> rt = ZHSMSProxy.GetStatusReportService().GetDirectStatisticReportByAccount(account);
            if (rt.Success)
            {
                List<SMSModel.ReportStatistics> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.ReportStatistics>.GetList(list, pSize, pIndex);
                    foreach (SMSModel.ReportStatistics rs in list)
                    {
                        result.Reports.Add(SMSModelToReportStatistics(rs));
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 根据业务号获取短信统计报告（内存）
        public ReportStatistics GetDirectReportStatistics(string serialNumber)
        {
            if (string.IsNullOrEmpty(serialNumber)) return null;
            SMSModel.RPCResult<SMSModel.ReportStatistics> rt = ZHSMSProxy.GetStatusReportService().GetDirectReportStatistics(Guid.Parse(serialNumber));
            if (rt.Success)
            {
                return SMSModelToReportStatistics(rt.Value);
            }
            return null;

        }

        // 根据业务号获取短信状态明细（内存）
        public RPCStatusReportListResult GetDirectStatusReport(string serialNumber, int pSize, int pIndex)
        {
            RPCStatusReportListResult result = new RPCStatusReportListResult();
            result.Message = "";
            result.Records = new List<StatusReport>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (string.IsNullOrEmpty(serialNumber))
            {
                result.Message = "短信业务号不能为空";
                result.Success = false;
                return result;
            }
            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<SMSModel.StatusReport>> rt = ZHSMSProxy.GetStatusReportService().GetDirectStatusReport(Guid.Parse(serialNumber));
            if (rt.Success)
            {
                List<SMSModel.StatusReport> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.StatusReport>.GetList(list, pSize, pIndex);
                    foreach (SMSModel.StatusReport rs in list)
                    {
                        result.Records.Add(SMSModelToStatusReport(rs));
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }

        // 获取企业的上行短信（内存）
        public RPCMOSMSListResult GetDirectMOSmsBySPNumber(string spNumber, int pSize, int pIndex)
        {
            RPCMOSMSListResult result = new RPCMOSMSListResult();
            result.Message = "";
            result.MOSMSList = new List<MOSMS>();
            result.Success = true;
            result.Total = 0;
            result.PageCount = 0;

            if (string.IsNullOrEmpty(spNumber))
            {
                result.Message = "短信号码不能为空";
                result.Success = false;
                return result;
            }
            if (pIndex < 0) pIndex = 0;
            if (pSize <= 0)
            {
                result.Message = "请求每页的记录条数应大于0";
                result.Success = false;
                return result;
            }
            SMSModel.RPCResult<List<SMSModel.MOSMS>> rt = ZHSMSProxy.GetStatusReportService().GetDirectMOSmsBySPNumber("", spNumber);
            if (rt.Success)
            {
                List<SMSModel.MOSMS> list = rt.Value;
                if (list != null && list.Count > 0)
                {
                    result.Total = list.Count;
                    result.PageCount = GetTotalPage(list.Count, pSize);
                    list = ResultList<SMSModel.MOSMS>.GetList(list, pSize, pIndex);
                    foreach (SMSModel.MOSMS sms in list)
                    {
                        result.MOSMSList.Add(SMSModelToMOSMS(sms));
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                result.Message = rt.Message;
                result.Success = false;
                return result;
            }
        }


        #region 私有方法

        bool IsDateTime(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                DateTime dateTime;
                if (DateTime.TryParse(str, out dateTime))
                    return true;
            }
            return false;
        }

        class DateParse
        {
            public string BeginTime { get; set; }
            public string EndTime { get; set; }
            public bool Result { get; set; }
            public string Message { get; set; }
        }

        DateParse getDateParse(string beginTime, string endTime)
        {
            DateParse dp = new DateParse();
            try
            {
                if (!IsDateTime(beginTime))
                {
                    dp.Message = "查询的开始时间格式不正确，应为yyyy-MM-dd HH:mm:ss";
                    dp.Result = false;
                    return dp;
                }
                if (!IsDateTime(endTime))
                {
                    dp.Message = "查询的结束时间格式不正确，应为yyyy-MM-dd HH:mm:ss";
                    dp.Result = false;
                    return dp;
                }
                if (DateTime.Compare(DateTime.Parse(beginTime), DateTime.Parse(endTime)) > 0)
                {
                    string dt = beginTime;
                    beginTime = endTime;
                    endTime = dt;
                }
                if (DateTime.Compare(DateTime.Parse(endTime), DateTime.Now) > 0)
                {
                    endTime = DateTime.Now.ToString();
                }

                dp.Result = true;
                dp.BeginTime = DateTime.Parse(beginTime).ToString("yyyy-MM-dd HH:mm:ss");
                dp.EndTime = DateTime.Parse(endTime).ToString("yyyy-MM-dd HH:mm:ss");
                dp.Message = "";

            }
            catch
            {
                dp.Message = "时间类型输入错误";
                dp.Result = false;
            }
            return dp;
        }

        int GetTotalPage(int count, int pageSize)
        {
            if (count == 0)
            {
                return 0;
            }
            else
            {
                return count % pageSize == 0 ? (count / pageSize) : (count / pageSize + 1);
            }
        }

        SMS SMSModelToSMS(SMSModel.SMS sms)
        {
            SMS s = new SMS();
            s.Account = sms.Account;
            s.Audit = (AuditType)sms.Audit;
            s.Channel = sms.Channel;
            s.Content = sms.Content;
            s.Filter = (FilterType)sms.Filter;
            s.Level = (LevelType)sms.Level;
            s.LinkID = sms.LinkID;
            s.Number = sms.Number;
            s.NumberCount = sms.NumberCount;
            s.SendTime = sms.SendTime.ToString();
            s.SerialNumber = sms.SerialNumber.ToString();
            s.Signature = sms.Signature;
            s.SMSTimer = sms.SMSTimer.ToString();
            s.SPNumber = sms.SPNumber;
            s.StatusReport = (StatusReportType)sms.StatusReport;
            s.WapURL = sms.WapURL;
            return s;
        }

        PrepaidRecord SMSModelToPrepaidRecord(SMSModel.PrepaidRecord record)
        {
            PrepaidRecord p = new PrepaidRecord();
            p.Operatorer = record.AccountID;
            p.PrepaidAccount = record.PrepaidAccount;
            p.PrepaidTime = record.PrepaidTime.ToString();
            p.Quantity = (int)record.Quantity;
            return p;
        }

        FailureSMS SMSModelToFailureSMS(SMSModel.FailureSMS sms)
        {
            FailureSMS fs = new FailureSMS();
            SMS s = new SMS();
            s.Account = sms.Account;
            s.Audit = (AuditType)sms.Audit;
            s.Channel = sms.Channel;
            s.Content = sms.Content;
            s.Filter = (FilterType)sms.Filter;
            s.Level = (LevelType)sms.Level;
            s.LinkID = sms.LinkID;
            s.Number = sms.Number;
            s.NumberCount = sms.NumberCount;
            s.SendTime = sms.SendTime.ToString();
            s.SerialNumber = sms.SerialNumber.ToString();
            s.Signature = sms.Signature;
            s.SMSTimer = sms.SMSTimer.ToString();
            s.SPNumber = sms.SPNumber;
            s.StatusReport = (StatusReportType)sms.StatusReport;
            s.WapURL = sms.WapURL;
            fs.Sms = s;
            fs.AuditUser = sms.AuditUser;
            fs.AuditTime = sms.AuditTime;
            fs.FailureCase = sms.FailureCase;
            return fs;
        }

        Keywords SMSModelToKeywords(SMSModel.Keywords keyword)
        {
            Keywords k = new Keywords();
            k.Enable = keyword.Enable;
            k.KeyGroup = keyword.KeyGroup;
            k.KeywordsType = keyword.KeywordsType;
            k.ReplaceKeywords = keyword.ReplaceKeywords;
            k.Words = keyword.Words;
            return k;
        }

        AuditRecord SMSModelToAuditRecord(SMSModel.AuditRecord record)
        {
            AuditRecord r = new AuditRecord();
            r.Auditer = record.AccountID;
            r.AuditTime = record.AuditTime.ToString();
            r.Content = record.Content;
            r.Result = record.Result;
            r.SendTime = record.SendTime.ToString();
            r.SerialNumber = record.SerialNumber.ToString();
            return r;
        }

        LevelModifyRecord SMSModelToLevelModifyRecord(SMSModel.LevelModifyRecord record)
        {
            LevelModifyRecord r = new LevelModifyRecord();
            r.Modifyer = record.AccountID;
            r.ModifyContent = record.ModifyContent;
            r.ModifyTime = record.ModifyTime.ToString();
            r.Content = record.Content;
            r.SendTime = record.SendTime.ToString();
            r.SerialNumber = record.SerialNumber.ToString();
            return r;
        }

        StatusReport SMSModelToStatusReport(SMSModel.StatusReport record)
        {
            StatusReport sr = new StatusReport();
            sr.Describe = record.Describe;
            sr.Gateway = record.Gateway;
            SMS s = SMSModelToSMS(record.Message);
            sr.Message = s;
            sr.ResponseTime = record.ResponseTime.ToString();
            sr.Serial = record.Serial;
            sr.SplitNumber = record.SplitNumber;
            sr.SplitTotal = record.SplitTotal;
            sr.StatusCode = record.StatusCode;
            sr.Succeed = record.Succeed;
            return sr;
        }

        ReportStatistics SMSModelToReportStatistics(SMSModel.ReportStatistics record)
        {
            ReportStatistics sr = new ReportStatistics();
            sr.Account = record.Account;
            sr.BeginSendTime = record.BeginSendTime.ToString();
            sr.FailureCount = record.FailureCount;
            sr.LastResponseTime = record.LastResponseTime.ToString();
            sr.Numbers = record.Numbers;
            sr.SendCount = record.SendCount;
            sr.SendTime = record.SendTime.ToString();
            sr.SerialNumber = record.SerialNumber.ToString();
            sr.SMSContent = record.SMSContent;
            sr.SplitNumber = record.SplitNumber;
            sr.Succeed = record.Succeed;
            sr.Telephones = record.Telephones;
            return sr;
        }

        MOSMS SMSModelToMOSMS(SMSModel.MOSMS sms)
        {
            MOSMS mo = new MOSMS();
            mo.Gateway = sms.Gateway;
            mo.LinkID = sms.LinkID;
            mo.Message = sms.Message;
            mo.ReceiveTime = sms.ReceiveTime.ToString();
            mo.Serial = sms.Serial;
            mo.Service = sms.Service;
            mo.SPNumber = sms.SPNumber;
            mo.UserNumber = sms.UserNumber;
            return mo;
        }

        #endregion

    }
}