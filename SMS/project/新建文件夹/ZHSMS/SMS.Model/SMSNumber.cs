using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    /// <summary>
    /// 短信号码
    /// </summary>
    [Serializable]
    public class SMSNumber : BaseModel
    {
        /// <summary>
        /// 短信ID
        /// </summary>
        public String SMSID { get; set; }

        /// <summary>
        /// 号码，逗号隔开，最多5000,
        /// </summary>
        public String Numbers { get; set; }

        /// <summary>
        /// 号码个数，最多5000
        /// </summary>
        public int NumberCount { get; set; }

        /// <summary>
        /// 运营商，这一批的号码为同一个运营商
        /// </summary>
        public OperatorType Operator { get; set; }

        public DateTime? SendTime { get; set; }

    }
}
