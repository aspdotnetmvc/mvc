using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSModel
{
    [Serializable]
    public class ChargeStatics
    {
        /// <summary>
        /// 企业代码
        /// </summary>
        public string Enterprese { get; set; }
        /// <summary>
        /// 发送短信总数
        /// </summary>
        public long SendCount
        {
            get
            {
                return SMSCount - RemainSMSNumber < 0 ? 0 : SMSCount - RemainSMSNumber;
            }
        }
        /// <summary>
        /// 企业充值总短信数
        /// </summary>
        public long SMSCount { get; set; }
        /// <summary>
        /// 企业充值总金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 企业剩余短信条数
        /// </summary>
        public int RemainSMSNumber { get; set; }
        /// <summary>
        /// 短信网关
        /// </summary>
        public string GateWay { get; set; }
    }
}
