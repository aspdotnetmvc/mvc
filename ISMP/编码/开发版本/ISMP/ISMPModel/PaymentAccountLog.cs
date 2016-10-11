using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
     [Serializable]
    public enum PaymentAccountDealType
    {
        ExpiredClear = 0 //逾期清零
    }
    [Serializable]
    public class PaymentAccountLog
    {
        /// <summary>
        /// 账户的ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 账户名称
        /// </summary>
        public string PaymentAccountId { get; set; }

        /// <summary>
        /// 账户金额
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime DealTime { get; set; }
        /// <summary>
        /// 处理人AccountId
        /// </summary>
        public String DealAccountId { get; set; }
        /// <summary>
        /// 处理人名称
        /// </summary>
        public String DealName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public String Msg { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public PaymentAccountDealType ActionType { get; set; }
    }
}
