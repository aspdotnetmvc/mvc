using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    /// <summary>
    /// 短信
    /// </summary>
    [Serializable]
    public class SMSMessage:BaseModel
    {
        /// <summary>
        /// 企业账号
        /// </summary>
        public String AccountID { get; set; }

        /// <summary>
        /// 短信内容
        /// </summary>
        public String Content { get; set; }

        /// <summary>
        /// 短信签名
        /// </summary>
        public String Signature { get; set; }

        /// <summary>
        /// 号码个数
        /// </summary>
        public int NumberCount { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime? SendTime { get; set; }

        /// <summary>
        /// 短信拆分计费条数
        /// </summary>
        public int SplitNumber { get; set; }

        /// <summary>
        /// 总扣费条数
        /// </summary>
        public int FeeTotalCount { get; set; }

        /// <summary>
        /// 返费条数
        /// </summary>
        public int FeeBack { get; set; }

        /// <summary>
        /// 发送失败条数
        /// </summary>
        public int FailureCount { get; set; }

        /// <summary>
        /// 返费原因  （发送失败，审核失败）
        /// </summary>
        public String FeeBackReason { get; set; }

        /// <summary>
        /// 审核结果
        /// </summary>
        public int AuditResult { get; set; }
        /// <summary>
        /// 审核失败原因
        /// </summary>
        public string AuditFailureCase { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditTime { get; set; }

        /// <summary>
        /// 审核类型  1，免审，0，人工审核，2，模板匹配成功
        /// </summary>
        public AuditType AuditType { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public String AuditAccountLoginName { get; set; }

        /// <summary>
        /// 发送通道
        /// </summary>
        public String Channel { get; set; }

       /// <summary>
       /// 短信优先级 
       /// 根据号码个数和企业优先级判断，企业分 三级优先级，短信分三级，组合起来即发送优先级
       /// </summary>
        public int SMSLevel { get; set; }
        
      
        public int StatusReportType { get; set; }
        public int FilterType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String SPNumber { get; set; }

        /// <summary>
        /// wap地址
        /// </summary>
        public String WapURL { get; set; }
        /// <summary>
        /// 定时发送短信定时时间
        /// </summary>
        public DateTime? SMSTimer { get; set; }

        /// <summary>
        /// 0，行业短信，1，商业短信
        /// </summary>
        public int SMSType { get; set; }
        /// <summary>
        /// 短信状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 短信来源，平台，http，webservice 接口
        /// </summary>
        public string Source { get; set; }
       
    }
}
