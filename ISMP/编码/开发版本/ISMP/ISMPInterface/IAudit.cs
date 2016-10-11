using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPInterface
{
    /// <summary>
    /// 审核
    /// </summary>
    public interface  IAudit
    {
        /// <summary>
        /// 审核ID
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        string ProductId { get; set; }

        /// <summary>
        /// 审核类型
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// 审核描述
        /// </summary>
        string Description { get; set; }

    }

    /// <summary>
    /// 审核结果
    /// </summary>
    public interface IAuditResult:IAudit
    {
        bool Status { get; set; }
        string Message { get; set; }
    }

}
