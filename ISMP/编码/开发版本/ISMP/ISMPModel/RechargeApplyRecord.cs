using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    /// <summary>
    /// 充值申请记录状态
    /// </summary>
    [Serializable]
    public enum RechargeApplyStatus
    {
        /// <summary>
        /// 未审核
        /// </summary>
        UnAudit = 0,
        /// <summary>
        /// 审核成功
        /// </summary>
        AuditSuccess = 1,
        /// <summary>
        /// 审核失败
        /// </summary>
        AuditFail = 2,
        /// <summary>
        /// 无需审核
        /// </summary>
        AutoAudit = 3
    }
    /// <summary>
    /// 充值申请记录状态
    /// </summary>
    [Serializable]
    public enum RechargeOprateType
    {
        /// <summary>
        /// 普通充值
        /// </summary>
        NormalRecharge = 0,
        /// <summary>
        /// 预充值
        /// </summary>
        LoanRecharge = 1
    }
    /// <summary>
    /// 充值申请记录
    /// </summary>
    [Serializable]
    public class RechargeApplyRecord
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 充值目标AccountId
        /// </summary>
        public string AgentAccountId { get; set; }
        /// <summary>
        /// 充值目标名称
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 充值产品Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 充值产品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 充值账户Id（上级）
        /// </summary>
        public string PaymentAccountId { get; set; }
        /// <summary>
        /// 充值账户名称（上级）
        /// </summary>
        public string PaymentAccountName { get; set; }
        /// <summary>
        /// 充值类型
        /// </summary>
        public RechargeDeductType Type { get; set; }
        /// <summary>
        /// 充值操作类型
        /// </summary>
        public RechargeOprateType OprateType { get; set; }
        /// <summary>
        /// 充值金额
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        /// 折扣比例，充值时会将输入金额乘以该值来扣除和充入
        /// </summary>
        public decimal Discount { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public String Description { get; set; }
        /// <summary>
        /// 凭证文件名
        /// </summary>
        public String Proof { get; set; }
        /// <summary>
        /// 系统自动生成的消息
        /// </summary>
        public String Msg { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime? ApplyTime { get; set; }
        /// <summary>
        /// 申请者AccountId
        /// </summary>
        public string ApplyWorkerAccountId { get; set; }
        /// <summary>
        /// 申请者名称
        /// </summary>
        public string ApplyWorkerName { get; set; }
        /// <summary>
        /// 处理时间（审核时间）
        /// </summary>
        public DateTime? DealTime { get; set; }
        /// <summary>
        /// 处理者AccountId
        /// </summary>
        public string DealWorkerAccountId { get; set; }
        /// <summary>
        /// 处理者名称
        /// </summary>
        public string DealWorkerName { get; set; }
        /// <summary>
        /// 记录状态
        /// </summary>
        public RechargeApplyStatus Status { get; set; }
        /// <summary>
        /// 该充值审核最后实际充值的流水号
        /// </summary>
        public string AgentRechargeRecordId { get; set; }
    }
}
