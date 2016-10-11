using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    /// <summary>
    /// 充值扣费类型
    /// </summary>
    [Serializable]
    public enum RechargeDeductType
    {
        /// <summary>
        /// 系统扣款
        /// </summary>
        SystemDeduct = 1,
        /// <summary>
        /// 系统赠款
        /// </summary>
        SystemGrant = 2,
        /// <summary>
        /// 经销商新开
        /// </summary>
        AgentNew = 11,
        /// <summary>
        /// 经销商续费
        /// </summary>
        AgentRenew = 12,
        /// <summary>
        /// 企业产品新开
        /// </summary>
        EnterpriseNew = 21,
        /// <summary>
        /// 企业产品续费
        /// </summary>
        EnterpriseRenew = 22
    }
    /// <summary>
    /// 产品资金账户状态
    /// </summary>
    [Serializable]
    public enum PaymentAccountStatus
    {
        /// <summary>
        /// 冻结
        /// </summary>
        Frozen = 0,
        /// <summary>
        /// 启用
        /// </summary>
        Normal = 1
    }
    /// <summary>
    /// 资金账户
    /// </summary>
    [Serializable]
    public class PaymentAccount
    {
        /// <summary>
        /// 账户的ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 账户名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 账户的标识，创建时自动生成，子账户继承，一代继承自模版
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// 账户金额
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        /// 是否可转移
        /// </summary>
        public sbyte IsTransfer { get; set; }
        /// <summary>
        /// 是否可充值
        /// </summary>
        public sbyte IsRecharge { get; set; }
        /// <summary>
        /// 有效日期
        /// </summary>
        public DateTime? EffectiveDate { get; set; }
        /// <summary>
        /// 支付类型
        /// </summary>
        public List<string> PaymentType { get; set; }
        public string PaymentType_String
        {
            get { return (PaymentType == null?"":string.Join(",", PaymentType.ToArray())); }
            set { PaymentType = (value == null ? null : new List<string>(value.Split(','))); }
        }
        /// <summary>
        /// 支付优先级,越高越先使用
        /// </summary>
        public int PaymentPriority { get; set; }
        /// <summary>
        /// 折扣比例，充值时会将输入金额乘以该值来扣除和充入
        /// </summary>
        public decimal Discount { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public String Description { get; set; }
        /// <summary>
        /// 是否删除:0否，1是
        /// </summary>
        public sbyte IsDelete { get; set; }

        /// <summary>
        /// 状态:1自定义账户, 2系统现金账户,3系统赠款账户,4系统保证金账户
        /// </summary>
        public sbyte Type { get; set; }
        /// <summary>
        /// 账户状态
        /// </summary>
        public int Status { get; set; }
    }
}
