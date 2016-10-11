using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPInterface
{
    /// <summary>
    /// 产品接口调用参数
    /// </summary>
    [Serializable]
    public class FunctionParameter
    {
        /// <summary>
        /// 经销商账号Id
        /// </summary>
        public string AgentAccountId { get; set; }
        /// <summary>
        /// 经销商账号
        /// </summary>
        public string AgentLoginName { get; set; }
        /// <summary>
        /// 经销商名称
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 企业账号Id
        /// </summary>
        public string EnterpriseAccountId { get; set; }
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
        /// 当前用户账号Id
        /// </summary>
        public string AccountId { get; set; }
        /// <summary>
        /// 当前用户账号
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 当前用户名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 当前用户类型
        /// </summary>
        public UserType UserType { get; set; }
        /// <summary>
        /// 当前用户的IP 地址
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// 菜单列表
        /// </summary>
        public List<IPermission> Menu { get; set; }
        /// <summary>
        /// 页面权限列表
        /// </summary>
        public Dictionary<string, Dictionary<string, IPermission>> PageList { get; set; }
        /// <summary>
        /// 实际登录(实际操作)用户账号Id
        /// </summary>
        public string OperatorAccountId { get; set; }
        /// <summary>
        /// 实际登录(实际操作)账号
        /// </summary>
        public string OperatorLoginName { get; set; }
        /// <summary>
        /// 实际登录(实际操作)名称
        /// </summary>
        public string OperatorName { get; set; }
    }

    /// <summary>
    /// 产品售卖接口
    /// </summary>
    [Serializable]
    public class SaleParameter : FunctionParameter
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 套餐包Id
        /// </summary>
        public string  PackageId { get; set; }
        /// <summary>
        /// 套餐包成本
        /// </summary>
        public decimal Cost { get; set; }
    }

    /// <summary>
    /// 经销商充值
    /// </summary>
    [Serializable]
    public class AgentRechargeParameter : FunctionParameter
    {
        /// <summary>
        /// 目的账号
        /// </summary>
        public string destAccount;
    }

    /// <summary>
    /// 经销商配置产品
    /// </summary>
    [Serializable]
    public class AgentProductParameter : FunctionParameter
    {
        /// <summary>
        /// 直属上级经销商账号Id
        /// </summary>
        public string SeniorAgentAccountId { get; set; }
        /// <summary>
        /// 直属上级经销商名称
        /// </summary>
        public string SeniorAgentName { get; set; }
    }

    [Serializable]
    public class AuditProductParameter : FunctionParameter
    {
        /// <summary>
        /// 审核记录的标识
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// 审核记录的类型
        /// </summary>
        public string AuditType { get; set; }
        /// <summary>
        /// 审核对象（企业或经销商）AccountId
        /// </summary>
        public string AuditTargetAccountId { get; set; }
        /// <summary>
        /// 是否是审核后查看详情，如果是删除操作按钮
        /// </summary>
        public bool IsDisplayDetail { get; set; }
        
    }

    /// <summary>
    /// 企业产品充值
    /// </summary>
    [Serializable]
    public class EnterpriseProductRechargeParameter : FunctionParameter
    {
        /// <summary>
        /// 是否赠送
        /// </summary>
        public bool IsGrant { get; set; }
        /// <summary>
        /// 赠款原因列表
        /// </summary>
        public List<IDictionary<string, object>> GrantReason { get; set; }
    }

    /// <summary>
    /// 企业自助充值
    /// </summary>
    [Serializable]
    public class EnterpriseAutoRechargeParameter : FunctionParameter
    {
        /// <summary>
        /// 充值金额
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        /// 套餐ID
        /// </summary>
        public string PackageId { get; set; }
        /// <summary>
        /// 是否套餐
        /// </summary>
        public bool IsPackage { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// 企业预警
    /// </summary>
    [Serializable]
    public class EnterpriseAlertParameter : FunctionParameter
    {
        /// <summary>
        /// 企业AccountId列表，逗号分割的字符串
        /// </summary>
        public string EnterpriseAccountIdList_String { get; set; }
        /// <summary>
        /// 是否只查询达到预警值的项，否则查询全部
        /// </summary>
        public bool IsOnlyAlertItem { get; set; }
        /// <summary>
        /// 是否费用预警，否则到期预警
        /// </summary>
        public bool IsBalanceAlert { get; set; }
        /// <summary>
        /// 是否分页查询
        /// </summary>
        public bool IsPage { get; set; }
        /// <summary>
        /// 如果分页，页码，从1开始
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// 如果分页，分页的行数
        /// </summary>
        public int rows { get; set; }
        /// <summary>
        /// 其它参数
        /// </summary>
        public Dictionary<string, object> OtherParameter { get; set; }
    }
}
