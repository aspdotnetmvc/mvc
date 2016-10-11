using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSModel
{
    /// <summary>
    /// 短信类型
    /// </summary>
    [Serializable]
    public class SMS
    {
        public SMS()
        {
            //默认是true
            IsSplit = true;
        }

        public SMS(List<string> numbers, string msgContent, LevelType msgLevel, DateTime sendTime, StatusReportType statusReport)
        {
            Number = numbers;
            Content = msgContent;
            Level = msgLevel;
            SendTime = sendTime;
            StatusReport = statusReport;
            IsSplit = true;
        }

        StatusReportType _statusReport = 0;
        LevelType _level = LevelType.Level2;
        AuditType _audit = 0;
        FilterType _filter = 0;

        //帐号
        public string Account { get; set; }

        //业务流水号
        public Guid SerialNumber { get; set; }

        //发送号码列表
        public List<string> Number {get;set;}
        /// <summary>
        /// 发送号码个数
        /// </summary>
        public int NumberCount { get; set; }
        //短信内容
        public string Content {get;set;}

        /// <summary>
        /// 状态报告接收方式
        /// </summary>
        public StatusReportType StatusReport
        {
            get { return _statusReport; }
            set { _statusReport = value; }
        }

        //等级（0-6，数字越高优先级越高）；默认2
        public LevelType Level
        {
            get { return _level; }
            set { _level = value; }
        }

        //发送时间
        public DateTime SendTime {get;set;}
        /// <summary>
        /// 定时短信
        /// </summary>
        public DateTime SMSTimer { get; set; }
        //短信内容审核方式
        public AuditType Audit
        {
            get { return _audit; }
            set { _audit = value; }
        }

        //短信内容过滤方式
        public FilterType Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }
        /// <summary>
        /// 短信发送通道
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// 指定网关
        /// </summary>
        public bool IsSplit { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string Signature { get; set; }
        /// <summary>
        /// SPNumber
        /// </summary>
        public string SPNumber { get; set; }
        /// <summary>
        /// WapPush URL
        /// </summary>
        public string WapURL { get; set; }
        /// <summary>
        /// LinkID
        /// </summary>
        public string LinkID { get; set; }
        /// <summary>
        /// 扩展字段
        /// </summary>
        public object Extend {get;set;}
        /// <summary>
        /// 审核失败原因
        /// </summary>
        public string FailureCase { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        public string AuditUser { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public string AuditTime { get; set; }

    }
}
