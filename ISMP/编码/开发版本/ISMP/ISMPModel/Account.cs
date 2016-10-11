using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISMPModel
{
    [Serializable]
    public enum AccountRegisterType
    {
        UserName = 1,
        Phone = 2,
        EMail = 3
    }
    [Serializable]
    public enum AccountState
    {
        Normal = 1,
        Disable = 0
    }
    [Serializable]
    public enum LinkType
    {
        Employee = 1,//员工
        Agent = 2,//经销商
        Enterprise = 3 ,//企业
        AgentEmployee=4 //经销商员工
    }
    /// <summary>
    /// 账号表（销售经理账号，经销商，企业） 
    /// </summary>
    [Serializable]
    public class Account
    {
        public Account()
        {
            Password = "123456";//设置默认密码
            IsSmsLogin = false;
            State = AccountState.Normal;
        }
        /// <summary>
        /// 系统生成的ID
        /// </summary>
        public String AccountId { get; set; }

        /// <summary>
        /// 登录名，需验证唯一性
        /// </summary>
        public String LoginName { get; set; }

        /// <summary>
        /// 注册类型（手机，邮箱，400号码，用户名注册 等）
        /// </summary>
        public AccountRegisterType RegisterType { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime? RegisterTime { get; set; }
        /// <summary>
        /// 密码（加密后）
        /// </summary>
        public String Password { get; set; }

        /// <summary>
        /// 状态（正常，禁用）
        /// </summary>
        public AccountState State { get; set; }

        /// <summary>
        /// 关联员工（Employee），经销商 Agent,终端客户Enterprise 的ID
        /// </summary>
        public String LinkId { get; set; }

        /// <summary>
        /// 账号身份类型（Employee，Agent,Enterprise)
        /// </summary>
        public LinkType LinkType { get; set; }


        /// <summary>
        /// 是否登录时短信验证
        /// </summary>
        public bool IsSmsLogin { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 最后登录Ip
        /// </summary>
        public String LastLoginIp { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 删除日期
        /// </summary>
        public DateTime? DeleteTime { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public List<Role> Roles { get; set; }
        /// <summary>
        /// 权限
        /// </summary>
        public List<Permission> Permissions { get; set; }
    }

    /// <summary>
    /// Account 视图
    /// </summary>
    [Serializable]
    public class AccountView : Account
    {
        public AccountView()
            : base()
        {

        }
        /// <summary>
        /// 对应的用户(员工，经销商，企业)名字
        /// </summary>
        public String UserName { get; set; }

    }
}
