using System;
using System.Text;
namespace ISMPModel
{
    /// <summary>
    /// 经销商配置产品状态
    /// </summary>
    [Serializable]
    public enum AgentProductStatus
    {
        /// <summary>
        /// 启用
        /// </summary>
        Enable = 2,
        /// <summary>
        /// 审核中
        /// </summary>
        Auditing = 3,
        /// <summary>
        /// 审核失败
        /// </summary>
        AuditFail = 5
    }
    /// <summary>
    /// 经销商配置产品
    /// </summary>
    [Serializable]
    public class AgentProductConfig : Agent
    {
        /// <summary>
        /// 
        /// </summary>
        public String RelatedId { get; set; }

        /// <summary>
        /// 经销商Id
        /// </summary>
        public String AgentId { get; set; }

        /// <summary>
        /// 产品Id
        /// </summary>
        public String ProductId { get; set; }

        /// <summary>
        /// 经销商状态
        /// </summary>
        public AgentProductStatus ProductStatus { get; set; }

    }
}