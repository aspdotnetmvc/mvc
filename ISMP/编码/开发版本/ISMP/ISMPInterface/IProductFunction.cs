using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPInterface
{
    public enum FunctionType
    {
        /// <summary>
        /// 产品激活接口，用于激活经销商产品配置
        /// </summary>
        Activate,
        /// <summary>
        /// 销售接口
        /// </summary>
        Sale,
        /// <summary>
        /// 充值接口
        /// </summary>
        Recharge,
        /// <summary>
        /// 指令接口，向产品发送指令
        /// </summary>
        Command,
        /// <summary>
        /// 审核接口，调用产品审核页面
        /// </summary>
        Audit,
        /// <summary>
        /// 审核编辑接口，审核失败后调用进行编辑重提
        /// </summary>
        AuditEdit,
        /// <summary>
        /// 明细接口，用于获取产品展示的明细项(实例 IProductDetail 类型)
        /// </summary>
        Detail,
        /// <summary>
        /// 融合套餐获取接口，用于获取产品的融合套餐(实例 IProductDetail 类型)
        /// </summary>
        Package,
        /// <summary>
        /// 融合套餐配置接口，用于配置产品的融合套餐
        /// </summary>
        PackageConfig,
        /// <summary>
        /// 销售验证表单、提交表单接口
        /// </summary>
        SaleCheckSubmit,
        /// <summary>
        /// 拉取产品状态接口
        /// </summary>
        Status,
        /// <summary>
        /// 用于ISMP首页获取统计信息接口
        /// </summary>
        HomePageStatistics,
        /// <summary>
        /// 企业自充值可选金额或套餐查询
        /// </summary>
        AutoRechargeAsk,
        /// <summary>
        /// 企业自充值
        /// </summary>
        AutoRecharge,
        /// <summary>
        /// 企业预警
        /// </summary>
        Alert,
        /// <summary>
        /// 修改企业归属
        /// </summary>
        ChangeOwnership,
    }
    public enum FunctionRole
    {
        /// <summary>
        /// 系统
        /// </summary>
        System,
        /// <summary>
        /// 经销商
        /// </summary>
        Agent,
        /// <summary>
        /// 企业
        /// </summary>
        Enterprise,

    }
    /// <summary>
    /// 产品功能接口
    /// </summary>
    public interface IProductFunction
    {
        /// <summary>
        /// ID
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// URL
        /// </summary>
        string Url { get; set; }
        /// <summary>
        /// 接口类型
        /// </summary>
        FunctionType FunctionType { get; set; }
        /// <summary>
        /// 接口角色
        /// </summary>
        FunctionRole FunctionRole { get; set; }
    }
}
