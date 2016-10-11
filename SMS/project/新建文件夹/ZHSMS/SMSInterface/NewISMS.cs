using SMS.Model;
using SMS.DTO;
using Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    public partial interface ISMS : IMonitoring
    {
        #region  SMS

        RPC_Result<List<NumSect>> GetNumSect();

        RPC_Result<SMSDTO> SendSMS(SMSDTO sms);
        /// <summary>
        /// 获取待审核短信
        /// </summary>
        /// <param name="qp"></param>
        /// <returns></returns>
        RPC_Result<QueryResult<SMSMessage>> GetSMSForAudit(QueryParams qp);
        /// <summary>
        /// 审核短信 -失败
        /// </summary>
        /// <param name="AuditAccountLoginName"></param>
        /// <param name="SMSIDList"></param>
        /// <param name="FailureCase"></param>
        /// <returns></returns>
        RPC_Result AuditSMSFailure(string AuditAccountLoginName, List<string> SMSIDList, string FailureCase);
        /// <summary>
        /// 审核短信 - 成功
        /// </summary>
        /// <param name="AuditAccountLoginName"></param>
        /// <param name="SMSIDList"></param>
        /// <returns></returns>
        RPC_Result AuditSMSSuccess(string AuditAccountLoginName, List<string> SMSIDList,string SendChannel);

        /// <summary>
        /// 查询短信审核记录
        /// </summary>
        /// <param name="qp"></param>
        /// <returns></returns>
        RPC_Result<QueryResult<SMSMessage>> GetSMSByAudit(QueryParams qp);

        /// <summary>
        /// 查询短信发送记录
        /// </summary>
        /// <param name="qp"></param>
        /// <returns></returns>
        RPC_Result<QueryResult<SMSMessage>> GetSMSList(QueryParams qp);
        #endregion

        #region 审核失败原因
        /// <summary>
        /// 获取审核失败原因列表
        /// </summary>
        /// <returns></returns>
        RPC_Result<List<AuditFailureReason>> GetAuditFailureReasonList();

        #endregion
    }
}
