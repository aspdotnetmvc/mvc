using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPInterface
{
    /// <summary>
    /// 企业预警
    /// </summary>
    [Serializable]
    public class EnterpriseAlert
    {
        /// <summary>
        /// 企业AccountId
        /// </summary>
        public string EnterpriseAccountId { get; set; }
        /// <summary>
        /// 余量
        /// </summary>
        public decimal Balance { get; set; }
        /// <summary>
        /// 余量单位
        /// </summary>
        public string UnitName { get; set; }
        /// <summary>
        /// 到期时间，为null长期有效
        /// </summary>
        public DateTime? ExpireDateTime { get; set; }
    }
    /// <summary>
    /// 企业预警详细
    /// </summary>
    [Serializable]
    public class EnterpriseAlertDetail : EnterpriseAlert
    {
        /// <summary>
        /// 企业账号
        /// </summary>
        public string EnterpriseLoginName { get; set; }
        /// <summary>
        /// 企业名称
        /// </summary>
        public string EnterpriseName { get; set; }
        /// <summary>
        /// 产品Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 企业直属代理商名称
        /// </summary>
        public string SeniorAgentName { get; set; }
    }
}
