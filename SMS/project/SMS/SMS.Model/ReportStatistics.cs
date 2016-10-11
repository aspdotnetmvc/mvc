using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SMS.Model
{
    [Serializable]
    public class ReportStatistics:SMSMessage
    {
        public ReportStatistics()
        {
            SendCount=0;
        }
        public ReportStatistics(SMSMessage sms)
        {
            this.ID = sms.ID;
            this.AccountID = sms.AccountID;
            this.Channel = sms.Channel;
            this.Content = sms.Content;
            this.FailureCount = sms.FailureCount;
            this.FeeBack = sms.FeeBack;
            this.FeeBackReason = sms.FeeBackReason;
            this.FeeTotalCount = sms.FeeTotalCount;
            this.FilterType = sms.FilterType;
            this.NumberCount = sms.NumberCount;
            this.SendCount = sms.NumberCount;
            this.SendTime = sms.SendTime;
            this.Signature = sms.Signature;
            this.SMSLevel = sms.SMSLevel;
            this.SMSTimer = sms.SMSTimer;
            this.SMSType = sms.SMSType;
            this.Source = sms.Source;
            this.SplitNumber = sms.SplitNumber;
            this.SPNumber = sms.SPNumber;
            this.StatusReportType = sms.StatusReportType;
        }
        /// <summary>
        /// 发送条数
        /// </summary>
        public int SendCount { get; set; }
        public int SuccessCount {
            get
            {
                return SendCount - FailureCount;
            }
        }
    }
}
