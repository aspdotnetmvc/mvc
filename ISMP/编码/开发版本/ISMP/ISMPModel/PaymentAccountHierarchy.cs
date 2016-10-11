using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    /// <summary>
    /// 资金账户从属
    /// </summary>
    [Serializable]
    public class PaymentAccountHierarchy:PaymentAccount
    {
        /// <summary>
        /// 关联ID
        /// </summary>
        public string RelatedId { get; set; }
        /// <summary>
        /// 经销商AccountId
        /// </summary>
        public string AgentAccountId { get; set; }
        /// <summary>
        /// 经销商Id
        /// </summary>
        public string AgentId { get; set; }
        /// <summary>
        /// 经销商名称
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 产品Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
    }


    /// <summary>
    /// 资金账户费用预警设置
    /// </summary>
    [Serializable]
    public class AgentAlertConfig : PaymentAccountHierarchy
    {
        public AgentAlertConfig()
        {
            IsBalanceAlert = true;
            IsSendBalanceAlert = false;
            BalanceAlert_Money = 0.000M;
            BalanceAlert_Percent = 0.100M;
            IsExpireAlert = true;
            IsSendExpireAlert = false;
            ExpireAlert_Day = 30;
        }
        /// <summary>
        /// 是否余额预警提醒
        /// </summary>
        public bool IsBalanceAlert { get; set; }
        /// <summary>
        /// 是否已发送余额预警提醒
        /// </summary>
        public bool IsSendBalanceAlert { get; set; }
        /// <summary>
        /// 余额预警阀值_金额
        /// </summary>
        public decimal BalanceAlert_Money { get; set; }
        /// <summary>
        /// 余额预警阀值_百分比（除以100后）
        /// </summary>
        public decimal BalanceAlert_Percent { get; set; }
        /// <summary>
        /// 是否到期提醒
        /// </summary>
        public bool IsExpireAlert { get; set; }
        /// <summary>
        /// 是否已发送到期提醒
        /// </summary>
        public bool IsSendExpireAlert { get; set; }
        /// <summary>
        /// 到期提醒_天数
        /// </summary>
        public int ExpireAlert_Day { get; set; }
    }
}
