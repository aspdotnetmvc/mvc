using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    [Serializable]
    public class AuditEnterprise
    {
        /// <summary>
        /// 企业代码
        /// </summary>
        public string EnterpriseCode { get; set; }
        /// <summary>
        /// 企业名称
        /// </summary>
        public string EnterpriseName { get; set; }
        /// <summary>
        /// 代理商终端用户开户时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 审核结果 true：已审 false ：未审
        /// </summary>
        public bool AuditResult { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        public string Auditor { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditTime { get; set; }
        /// <summary>
        /// 审核企业的结果 0：不通过，1：通过
        /// </summary>
        public bool EnterpriseResult { get; set; }
        /// <summary>
        /// 审核备注
        /// </summary>
        public string AuditRemark { get; set; }
    }
}
