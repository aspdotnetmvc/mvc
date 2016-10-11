using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    public class PaymentAccountTemplate
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

        public string Description { get; set; }

        /// <summary>
        /// 是否可转移
        /// </summary>
        public sbyte IsTransfer { get; set; }
        /// <summary>
        /// 是否可充值
        /// </summary>
        public sbyte IsRecharge { get; set; }

        /// <summary>
        /// 状态:1自定义账户, 2系统现金账户,3系统赠款账户,4系统保证金账户
        /// </summary>
        public sbyte Type { get; set; }

        /// <summary>
        /// 支付优先级,越高越先使用
        /// </summary>
        public int PaymentPriority { get; set; }
    }
}
