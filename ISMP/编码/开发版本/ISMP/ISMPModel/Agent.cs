using System;
using System.Text;
namespace ISMPModel
{
    /// <summary>
    /// 经销商状态
    /// </summary>
    [Serializable]
    public enum AgentState
    {
        /// <summary>
        /// 停用
        /// </summary>
        Disable = 1,
        /// <summary>
        /// 启用
        /// </summary>
        Enable = 2,
        /// <summary>
        /// 注册审核中
        /// </summary>
        Auditing = 3,
        /// <summary>
        /// 注册审核失败
        /// </summary>
        AuditFail = 5,
        /// <summary>
        /// 冻结
        /// </summary>
        Frozen = 12
    }

    /// <summary>
    /// 经销商信息
    /// </summary>
    [Serializable]
    public class Agent:Account,IContactInformation
    {
        /// <summary>
        /// 
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// 经销商名称
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 经销商层级
        /// </summary>
        public Int32 Level { get; set; }
        /// <summary>
        /// 经销商编码，用于层级，4位表示一级，16进制
        /// </summary>
        public String LevelCode { get; set; }

        /// <summary>
        /// 上级经销商,只给Code 赋值
        /// </summary>
        public Agent Parent { get; set; }

        /// <summary>
        /// 经销商状态
        /// </summary>
        public AgentState Status { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public String Contact { get; set; }

        /// <summary>
        /// 联系人性别
        /// </summary>
        public String Sex { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public String Telephone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        public String QQ { get; set; }

        /// <summary>
        /// 微信
        /// </summary>
        public String WeChat { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public String Mobile { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        public String PostCode { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public String ProvinceCode { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        public String CityCode { get; set; }
        /// <summary>
        /// 联系地址
        /// </summary>
        public String Address { get; set; }

        /// <summary>
        /// Logo 存放路径
        /// </summary>
        public String LogoPath { get; set; }

        /// <summary>
        /// 经销商网站
        /// </summary>
        public String WebSite { get; set; }

        /// <summary>
        /// 经销商人数范围
        /// </summary>
        public String Scale { get; set; }

    }
}