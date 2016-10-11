using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPInterface
{
    /// <summary>
    /// 自充值限制类型
    /// </summary>
    [Serializable]
    public enum AutoRechargeLimitType
    {
        /// <summary>
        /// 不限制
        /// </summary>
        UnLimit = 0,
        /// <summary>
        /// 金额限制
        /// </summary>
        MoneyLimit = 1,
        /// <summary>
        /// 套餐限制
        /// </summary>
        PackageLimit = 2
    }
    /// <summary>
    /// 自充值限制
    /// </summary>
    [Serializable]
    public class AutoRechargeLimit
    {
        /// <summary>
        /// 限制类型
        /// </summary>
        public AutoRechargeLimitType AutoRechargeLimitType { get; set; }
        /// <summary>
        /// 金额限制列表，限制类型为MoneyLimit时，不可为空
        /// </summary>
        public List<decimal> MoneyList { get; set; }
        /// <summary>
        /// 套餐限制列表，限制类型为PackageLimit时，不可为空
        /// </summary>
        public List<AutoRechargePackage> PackageList { get; set; }
        /// <summary>
        /// 经销商折扣，不限制和金额限制时必须有，换算关系为：经销商成本=上述充值金额*AgentDiscount
        /// </summary>
        public decimal AgentDiscount { get; set; }
        /// <summary>
        /// 产品支付类型
        /// </summary>
        public string ProductPayType { get; set; }
    }
    /// <summary>
    /// 自充值限制
    /// </summary>
    [Serializable]
    public class AutoRechargePackage
    {
        /// <summary>
        /// 套餐ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 套餐金额（企业支付金额）
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        /// 经销商成本
        /// </summary>
        public decimal AgentCost { get; set; }
        /// <summary>
        /// 套餐描述
        /// </summary>
        public string Description { get; set; }
    }
}
