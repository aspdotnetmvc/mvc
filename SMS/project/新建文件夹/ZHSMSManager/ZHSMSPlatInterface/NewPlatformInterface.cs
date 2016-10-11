using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMS.Model;


namespace SMSPlatInterface
{
    public partial interface ISMService
    {
        #region  SMS
        /// <summary>
        /// 获取NumSect 用来给号码分组
        /// </summary>
        /// <returns></returns>
        RPC_Result<List<NumSect>> GetNumSect();
        /// <summary>
        /// 新的发送短信逻辑
        /// </summary>
        /// <param name="sms"></param>
        /// <returns></returns>
        RPC_Result<SMS.DTO.SMSDTO> SendSMS(string LoginName,string Password,SMS.DTO.SMSDTO sms,string Source);

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

        /// <summary>
        /// 查询短信发送记录
        /// </summary>
        /// <param name="qp"></param>
        /// <returns></returns>
        RPC_Result<QueryResult<SMSMessage>> GetSMSListByAccountID(string AccountID,QueryParams qp);
        #endregion

        #region
        /// <summary>
        /// 获取审核失败原因列表
        /// </summary>
        /// <returns></returns>
        RPC_Result<List<AuditFailureReason>> GetAuditFailureReasonList();
        
        #endregion


        #region 企业设置
        RPC_Result UpdateEnterpriseChannel(string enterprise, string channel);
        
        #endregion
    }
}
