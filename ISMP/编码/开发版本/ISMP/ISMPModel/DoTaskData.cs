using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    //执行任务数据
    public interface IDoTaskData
    {
        
    }
    /// <summary>
    /// 审核信息
    /// </summary>
    [Serializable]
    public class AuditInfo : IDoTaskData
    {
        /// <summary>
        /// 审核人账户ID
        /// </summary>
        public String AuditorAccountId { get; set; }
        /// <summary>
        /// 审核人账户名称
        /// </summary>
        public String AuditorLoginName { get; set; }
        /// <summary>
        /// 审核人名称
        /// </summary>
        public String AuditorName { get; set; }
        /// <summary>
        /// 审核项Id
        /// </summary>
        public String AuditId { get; set; }
        /// <summary>
        /// 审核结果
        /// </summary>
        public sbyte AuditSuccess { get; set; }
        /// <summary>
        /// 审核备注
        /// </summary>
        public String AuditMsg { get; set; }
        /// <summary>
        /// 审核表中所记录的Identifier
        /// </summary>
        public String Identifier { get; set; }
    }
    // 启用经销商
    [Serializable]
    public class EnableAgents : AuditInfo
    {
        public String AgentId { get; set; }
        public String AgentName { get; set; }
    }
    // 经销商充值
    [Serializable]
    public class AgentsRecharge : AuditInfo
    {

    }

    // 经销商（或企业证件）更新或新增
    [Serializable]
    public class AddOrUpdateCredentials : AuditInfo
    {
        /// <summary>
        /// 证件表Id
        /// </summary>
        public string CredentialsId { get; set; }
        /// <summary>
        /// credentials表AccountId
        /// </summary>
        public string AccountId { get; set; }
        /// <summary>
        /// 同Account表
        /// </summary>
        public string LinkType { get; set; }
        /// <summary>
        /// 证件类型Id
        /// </summary>
        public string CredentialsTypeId { get; set; }
        /// <summary>
        /// 证件类型名称
        /// </summary>
        public string CredentialsTypeName { get; set; }
        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
    }
    // 企业实名认证
    [Serializable]
    public class EnterpriseCertification : AuditInfo
    {
        /// <summary>
        /// 审核证件的类型ID列表
        /// </summary>
        public List<string> CredentialType;
    }

}
