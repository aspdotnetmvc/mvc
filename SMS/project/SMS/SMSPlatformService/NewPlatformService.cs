using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SMSPlatform
{
    public partial class SMSPlatformService
    {
        #region  SMS

        public RPC_Result<List<NumSect>> GetNumSect()
        {
            return SMSProxy.GetSMSService().GetNumSect();
        }

        public RPC_Result<SMS.DTO.SMSDTO> SendSMS(string LoginName, string Password, SMS.DTO.SMSDTO sms,string Source)
        {
            if (string.IsNullOrWhiteSpace(LoginName))
            {
                return new RPC_Result<SMS.DTO.SMSDTO>(false, sms, "用户名不能为空");
            }
            //权限验证
            var account = AccountServer.Instance.GetAccount(LoginName);
            if (account == null)
            {
                return new RPC_Result<SMS.DTO.SMSDTO>(false, sms, "用户不存在");
            }

            if (Password != account.Password)
            {
                return new RPC_Result<SMS.DTO.SMSDTO>(false, sms, "密码不正确");
            }

            int Priority = SMS.Util.SMSHelper.GetPriorityByNumberCount(sms.Message.NumberCount);
            sms.Message.Source = Source;
            sms.Message.AccountID = account.AccountID;
            sms.Message.Channel = account.Channel;
            sms.Message.SMSLevel = Priority + account.Priority;
            sms.Message.SPNumber = account.SPNumber;
            sms.Message.StatusReportType = (int)account.StatusReport;
            if (account.Audit == SMS.Model.AccountAuditType.NonAudit)
            {
                sms.Message.AuditType = AuditType.Auto;
                sms.Message.Status = "待发送";
            }
            else if (account.Audit == SMS.Model.AccountAuditType.SingleNonAudit&& sms.Message.NumberCount==1)
            {
                sms.Message.AuditType = AuditType.Auto;
                sms.Message.Status = "待发送";
            }
            else if (account.Audit == SMS.Model.AccountAuditType.InterfaceNonAudit && sms.Message.Source!="平台")
            {
                sms.Message.AuditType = AuditType.Auto;
                sms.Message.Status = "待发送";
            }
            else
            {
                //判断是否匹配模板
                bool match = SMSTempletMatching(LoginName, sms.Message.Content);
                if (match)
                {
                    sms.Message.AuditType = AuditType.Template;
                    sms.Message.Status = "待发送";
                }
                else
                {
                    sms.Message.AuditType = AuditType.Manual;
                    sms.Message.Status = "待审核";
                }
            }

            return SMSProxy.GetSMSService().SendSMS(sms);
        }


        /// <summary>
        /// 获取待审核短信
        /// </summary>
        /// <param name="qp"></param>
        /// <returns></returns>
        public RPC_Result<QueryResult<SMSMessage>> GetSMSForAudit(QueryParams qp)
        {
            return SMSProxy.GetSMSService().GetSMSForAudit(qp);
        }
        /// <summary>
        /// 审核短信 -失败
        /// </summary>
        /// <param name="AuditAccountLoginName"></param>
        /// <param name="SMSIDList"></param>
        /// <param name="FailureCase"></param>
        /// <returns></returns>
        public RPC_Result AuditSMSFailure(string AuditAccountLoginName, List<string> SMSIDList, string FailureCase)
        {
            return SMSProxy.GetSMSService().AuditSMSFailure(AuditAccountLoginName, SMSIDList, FailureCase);
        }
        /// <summary>
        /// 审核短信 - 成功
        /// </summary>
        /// <param name="AuditAccountLoginName"></param>
        /// <param name="SMSIDList"></param>
        /// <returns></returns>
        public RPC_Result AuditSMSSuccess(string AuditAccountLoginName, List<string> SMSIDList, string SendChannel)
        {
            return SMSProxy.GetSMSService().AuditSMSSuccess(AuditAccountLoginName, SMSIDList, SendChannel);
        }

        /// <summary>
        /// 查询短信审核记录
        /// </summary>
        /// <param name="qp"></param>
        /// <returns></returns>
        public RPC_Result<QueryResult<SMSMessage>> GetSMSByAudit(QueryParams qp)
        {
            return SMSProxy.GetSMSService().GetSMSByAudit(qp);
        }

        /// <summary>
        /// 查询短信发送记录
        /// </summary>
        /// <param name="qp"></param>
        /// <returns></returns>
        public RPC_Result<QueryResult<SMSMessage>> GetSMSList(QueryParams qp)
        {
            return SMSProxy.GetSMSService().GetSMSList(qp);
        }

        /// <summary>
        /// 查询短信发送记录
        /// </summary>
        /// <param name="qp"></param>
        /// <returns></returns>
        public RPC_Result<QueryResult<SMSMessage>> GetSMSListByAccountID(string AccountID, QueryParams qp)
        {
            qp.add("AccountID", AccountID);
            return SMSProxy.GetSMSService().GetSMSList(qp);
        }


        /// <summary>
        /// 根据短信ID 获取号码
        /// </summary>
        /// <param name="SMSID"></param>
        /// <returns></returns>
        public RPC_Result<string> GetSMSNumbersBySMSID(string SMSID)
        {
            return SMSProxy.GetSMSService().GetSMSNumbersBySMSID(SMSID);
        }
        #endregion
        #region 审核失败原因
        /// <summary>
        /// 获取审核失败原因列表
        /// </summary>
        /// <returns></returns>
        public RPC_Result<List<AuditFailureReason>> GetAuditFailureReasonList()
        {
            return SMSProxy.GetSMSService().GetAuditFailureReasonList();
        }

        #endregion

        #region 企业设置
        public RPC_Result UpdateEnterpriseChannel(string enterprise, string channel)
        {
            var eu = AccountServer.Instance.GetAccount(enterprise);
            eu.Channel = channel;
            AccountServer.Instance.UpdateAccountSMS(eu);
            return new RPC_Result(true, "");
        }

        #endregion
    }
}
