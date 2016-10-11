using LogClient;
using SMS.DB;
using SMS.Model;
using SMS.Util;
using System;
using System.Collections.Generic;

namespace SMSService
{
    public partial class SMSService : MarshalByRefObject, ISMS
    {
        #region  SMS
        /// <summary>
        /// 获取NumSect 分组信息
        /// </summary>
        /// <returns></returns>
        public RPC_Result<List<NumSect>> GetNumSect()
        {
            if (!MyCache.Instance.ContainsKey(MyCacheKey.NumSect))
            {
                var list = SMS.DB.SMSDAL.GetNumSectList();
                MyCache.Instance.Set(MyCacheKey.NumSect, list);
            }
            var o = MyCache.Instance.Get(MyCacheKey.NumSect);
            var numsectlist = (List<NumSect>)o;
            return new RPC_Result<List<NumSect>>(true, numsectlist, "");
        }
        /// <summary>
        /// 提交短信，并扣费
        /// </summary>
        /// <param name="sms"></param>
        /// <returns></returns>
        public RPC_Result<SMS.DTO.SMSDTO> SendSMS(SMS.DTO.SMSDTO sms)
        {
            try
            {
                Account account = AccountDB.GetAccount(sms.Message.AccountID);//AccountServer.Instance.GetAccount(sms.Message.AccountID);
                if (account == null)
                {
                    LogHelper.LogWarn("Pretreatment", "PretreatmentService.SendSMS", "系统中不存在此用户 -> {Account=" + sms.Message.AccountID + "}");
                    MessageTools.MessageHelper.Instance.WirteInfo("提交短信失败，PretreatmentService.SendSMS，系统中不存在此用户 -> {Account=" + sms.Message.AccountID + "}");
                    return new RPC_Result<SMS.DTO.SMSDTO>(false, sms, "用户不存在");
                }
                if (account.SMSNumber < sms.Message.NumberCount)
                {
                    return new RPC_Result<SMS.DTO.SMSDTO>(false, sms, "总共要发 " + sms.Message.FeeTotalCount + " 条短信，账号余额不足！");
                }
                if (string.IsNullOrEmpty(sms.Message.SPNumber))
                {
                    sms.Message.SPNumber = defaultSPNumber.Split(',')[0];
                }

                SMS.DB.SMSDAL.AddSMS(sms);
                account = AccountDB.GetAccount(sms.Message.AccountID);
            
                return new RPC_Result<SMS.DTO.SMSDTO>(true, sms, "短信已提交成功，扣费:" + sms.Message.FeeTotalCount + "条,剩余" + account.SMSNumber + "条");
            }
            catch (Exception ex)
            {
                MessageTools.MessageHelper.Instance.WirteError("发送短信时发生了异常", ex);
                return new RPC_Result<SMS.DTO.SMSDTO>(false, sms, "发送短信时发生了异常");
            }
        }
        /// <summary>
        /// 获取待审核短信
        /// </summary>
        /// <param name="qp"></param>
        /// <returns></returns>
        public RPC_Result<QueryResult<SMSMessage>> GetSMSForAudit(QueryParams qp)
        {
            try
            {
                var qr = SMS.DB.SMSDAL.GetSMSForAudit(qp);
                return new RPC_Result<QueryResult<SMSMessage>>(true, qr, "");
            }
            catch (Exception ex)
            {
                MessageTools.MessageHelper.Instance.WirteError("获取待审核短信时发生了异常", ex);
                return new RPC_Result<QueryResult<SMSMessage>>(false, null, "获取待审核短信时发生了异常");
            }
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
            try
            {

                SMS.DB.SMSDAL.AuditSMSFailure(AuditAccountLoginName, SMSIDList, FailureCase);

                return new RPC_Result(true, "审核成功！", 0);
            }
            catch (OperateException oe)
            {
                return new RPC_Result(false, oe.Message, oe.ErrorCode);
            }
            catch (Exception ex)
            {
                return new RPC_Result(false, "审核时发生了异常", 0);
            }
        }
        /// <summary>
        /// 审核短信 - 成功
        /// </summary>
        /// <param name="AuditAccountLoginName"></param>
        /// <param name="SMSIDList"></param>
        /// <returns></returns>
        public RPC_Result AuditSMSSuccess(string AuditAccountLoginName, List<string> SMSIDList, string SendChannel)
        {
            try
            {
                SMS.DB.SMSDAL.AuditSMSSuccess(AuditAccountLoginName, SMSIDList, SendChannel);

                return new RPC_Result(true, "操作完成！", 0);
            }
            catch (OperateException oe)
            {
                return new RPC_Result(false, oe.Message, oe.ErrorCode);
            }
            catch (Exception ex)
            {
                MessageTools.MessageHelper.Instance.WirteError("审核时发生了异常", ex);
                return new RPC_Result(false, "审核时发生了异常", 0);
            }
        }
        /// <summary>
        /// 查询短信审核记录
        /// </summary>
        /// <param name="qp"></param>
        /// <returns></returns>
        public RPC_Result<QueryResult<SMSMessage>> GetSMSByAudit(QueryParams qp)
        {
            try
            {
                var r = SMS.DB.SMSDAL.GetSMSByAudit(qp);
                return new RPC_Result<QueryResult<SMSMessage>>(true, r, "");
            }

            catch (Exception ex)
            {
                MessageTools.MessageHelper.Instance.WirteError("获取审核短信记录时发生了异常", ex);
                return new RPC_Result<QueryResult<SMSMessage>>(false, null, "审核时发生了异常");
            }
        }

        /// <summary>
        /// 查询短信发送记录
        /// </summary>
        /// <param name="qp"></param>
        /// <returns></returns>
        public RPC_Result<QueryResult<SMSMessage>> GetSMSList(QueryParams qp)
        {
            try
            {
                var r = SMS.DB.SMSDAL.GetSMSList(qp);
                return new RPC_Result<QueryResult<SMSMessage>>(true, r, "");
            }

            catch (Exception ex)
            {
                MessageTools.MessageHelper.Instance.WirteError("查询短信发送记录时发生了异常", ex);
                return new RPC_Result<QueryResult<SMSMessage>>(false, null, "查询短信发送记录时发生了异常");
            }
        }
        #endregion

        #region 审核失败原因
        /// <summary>
        /// 获取审核失败原因列表
        /// </summary>
        /// <returns></returns>
        public RPC_Result<List<AuditFailureReason>> GetAuditFailureReasonList()
        {
            try
            {
                var list = SMSDAL.GetAuditFailureReasonList();
                return new RPC_Result<List<AuditFailureReason>>(true, list, "");
            }
               
            catch (Exception ex)
            {
                MessageTools.MessageHelper.Instance.WirteError("获取审核失败原因时发生了异常", ex);
                return new RPC_Result<List<AuditFailureReason>>(false, null, "获取审核失败原因时发生了异常");
            }
        }

        #endregion
    }
}
