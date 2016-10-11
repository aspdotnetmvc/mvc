using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    /// <summary>
    /// 支付宝结果
    /// </summary>
    [Serializable]
    public enum AliPayStatus
    {
        /// <summary>
        /// 未支付
        /// </summary>
        UnPay = 0,
        /// <summary>
        /// 已支付
        /// </summary>
        PaySuccess = 1,
        /// <summary>
        /// 支付失败
        /// </summary>
        PayFail = 2
    }
    /// <summary>
    /// ISMP中对自充值的处理结果
    /// </summary>
    [Serializable]
    public enum AliPayDealStatus
    {
        /// <summary>
        /// 未处理
        /// </summary>
        UnDeal = 0,
        /// <summary>
        /// 已处理
        /// </summary>
        DealSuccess = 1,
        /// <summary>
        /// 处理失败
        /// </summary>
        DealFail = 2
    }
    /// <summary>
    /// 支付宝自充值申请
    /// </summary>
    [Serializable]
    public class AliPayRechargeApply
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 订单名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        /// 订单描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 收款方（被扣费方）在ISMP中AccountId
        /// </summary>
        public string DeductAccountId { get; set; }
        /// <summary>
        /// 收款方收款时使用的AliPayPID
        /// </summary>
        public string AliPayPID_DA { get; set; }
        /// <summary>
        /// 收款方支付宝账号，可以是Email或手机号码。
        /// </summary>
        public string AliPayAccount_DA { get; set; }
        /// <summary>
        /// 付款方（充值方）在ISMP中AccountId
        /// </summary>
        public string RechargeAccountId { get; set; }
        /// <summary>
        /// 付款方（充值方）充值产品Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 充值产品套餐Id
        /// </summary>
        public string PackageId { get; set; }
        /// <summary>
        /// 是否套餐
        /// </summary>
        public bool IsPackage { get; set; }
        /// <summary>
        /// 操作人的AccountId
        /// </summary>
        public string ApplyAccountId { get; set; }
        /// <summary>
        /// 操作人名称
        /// </summary>
        public string ApplyName { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime? ApplyTime { get; set; }
        /// <summary>
        /// 0未支付，1已支付，2支付失败
        /// </summary>
        public AliPayStatus AliPayStatus { get; set; }
        /// <summary>
        /// 支付结果状态时间
        /// </summary>
        public DateTime? AliPayStatusTime { get; set; }
        /// <summary>
        /// 0未处理，1已处理，2处理失败
        /// </summary>
        public AliPayDealStatus DealStatus { get; set; }
        /// <summary>
        /// ISMP处理结果状态时间
        /// </summary>
        public DateTime? DealStatusTime { get; set; }
    }
    /// <summary>
    /// 收款人第三方收款账户（可扩展列）
    /// </summary>
    [Serializable]
    public class BeneficiaryAccount
    {
        /// <summary>
        /// 用户AccountId
        /// </summary>
        public string AccountId { get; set; }
        /// <summary>
        /// 支付宝PID
        /// </summary>
        public string AliPayPID { get; set; }
    }
    /// <summary>
    /// 支付宝自充值返回结果记录
    /// </summary>
    [Serializable]
    public class AliPayRechargeApplyReturn
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 订单名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public string Money { get; set; }
        /// <summary>
        /// 订单描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 收款方收款时使用的AliPayPID
        /// </summary>
        public string AliPayPID_DA { get; set; }
        /// <summary>
        /// 收款方支付宝账号，可以是Email或手机号码。
        /// </summary>
        public string AliPayAccount_DA { get; set; }
        /// <summary>
        /// 付款方（充值方）收款时使用的AliPayPID
        /// </summary>
        public string AliPayPID_RA { get; set; }
        /// <summary>
        /// 付款方（充值方）支付宝账号，可以是Email或手机号码。
        /// </summary>
        public string AliPayAccount_RA { get; set; }
        /// <summary>
        /// 支付宝返回交易流水号
        /// </summary>
        public string AliPayTradeId { get; set; }
        /// <summary>
        /// 支付宝返回的交易状态
        /// </summary>
        public string AliPayTradeStatus { get; set; }
        /// <summary>
        /// 交易结果返回时间
        /// </summary>
        public DateTime? ReturnTime { get; set; }
    }
}
