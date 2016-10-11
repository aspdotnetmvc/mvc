using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    [Serializable]
    public enum ProductAuditStatus
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
        AuditFail = 2
    }

    [Serializable]
    public class ProductAuditRecord
    {
        /// <summary>
        /// 审核Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 产品Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 产品审核记录标示
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// 审核类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 状态（0未审核1审核成功2审核失败）
        /// </summary>
        public ProductAuditStatus Status { get; set; }
        /// <summary>
        /// 审核对象（企业或经销商）AccountId
        /// </summary>
        public string AuditTargetAccountId { get; set; }
        /// <summary>
        /// 审核人AccountId
        /// </summary>
        public string AuditorAccountId { get; set; }
        /// <summary>
        /// 审核人名称
        /// </summary>
        public string Auditor { get; set; }
        /// <summary>
        /// 申请人AccountId
        /// </summary>
        public string ApplyAccountId { get; set; }
        /// <summary>
        /// 申请人名称
        /// </summary>
        public string ApplyName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime AuditDate { get; set; }
        /// <summary>
        /// 审核备注
        /// </summary>
        public string AuditMessage { get; set; }
    }
}
