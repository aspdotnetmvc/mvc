using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    [Serializable]
    public class ChargeRecord
    {
        /// <summary>
        /// 操作者
        /// </summary>
        public string OperatorAccount { get; set; }
        /// <summary>
        /// 操作帐户
        /// </summary>
        public string PrepaidAccount { get; set; }
        /// <summary>
        /// 当时费率
        /// </summary>
        public decimal? ThenRate { get; set; }
        /// <summary>
        /// 充值方式 0:充金额 1:充条数
        /// </summary>
        public ushort PrepaidType { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal? Money { get; set; }
        /// <summary>
        /// 短信条数
        /// </summary>
        public int? SMSCount { get; set; }
        /// <summary>
        /// 充值前剩余条数（不包含发送失败返充的数量）
        /// </summary>
        public int? RemainSMSCount { get; set; }
        /// <summary>
        /// 充值时间
        /// </summary>
        public DateTime? PrepaidTime { get; set; }
        /// <summary>
        /// 充值标识 0：平台充值 1：直客端代理商充值
        /// </summary>
        public ushort ChargeFlag { get; set; }
        /// <summary>
        /// 充值备注
        /// </summary>
        public string Remark { get; set; }
    }
}
