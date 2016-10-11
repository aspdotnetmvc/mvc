using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISMPModel;
using Newtonsoft.Json;
using ISMPInterface;

namespace ISMP_SMS.Model
{
    /// <summary>
    /// ISMP 中登录的用户
    /// </summary>
    [Serializable]
    public class ISMPUser
    {
        /// <summary>
        /// 账号Id
        /// </summary>
        public string AccountId { get; set; }
        /// <summary>
        /// ISMP 的LoginName
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 当前用户的Name
        /// </summary>
        public string Name { get; set; }
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

        /// <summary>
        /// 账号类型(员工，代理商，企业）
        /// </summary>
        public UserType UserType
        {
            get;
            set;
        }

        /// <summary>
        /// 发送短信用的密码,（当前用户是企业时有用）
        /// </summary>
        public string SMSPassword { get; set; }

        #region 暂时未用到
        /// <summary>
        /// 当前用户手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 当前用户Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 当前用户的IP 地址
        /// </summary>
        public string IPAddress
        {
            get
            {
                var context = HttpContext.Current;
                string ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ip))
                {
                    ip = context.Request.ServerVariables["REMOTE_ADDR"];
                }
                if (string.IsNullOrEmpty(ip))
                {
                    ip = context.Request.UserHostAddress;
                }
                if (string.IsNullOrEmpty(ip))
                {
                    ip = "0.0.0.0";
                }
                return ip;
            }
        }
     
        /// <summary>
        /// 对应员工，代理商，企业 的Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 上下级code
        /// </summary>
        public String AccountCode { get; set; }
        public int Level { get; set; }
        public DateTime? LastLoginTime { get; set; }
        /// <summary>
        /// 角色列表
        /// </summary>
        public List<Role> Roles { get; set; }
        /// <summary>
        /// 菜单列表
        /// </summary>
        public List<Permission> Menu { get; set; }
        /// <summary>
        /// 页面权限列表
        /// </summary>
        public Dictionary<string, Dictionary<string, Permission>> PageList { get; set; }

        /// <summary>
        /// 首页的配置
        /// </summary>
        public List<HomeConfig> HomeConfigList { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(new
            {
                AccountId = this.AccountId,
                AccountCode = this.AccountCode,
                UserType = this.UserType.ToString(),
                Name = this.Name,
                UserId = this.UserId,
                isFirstLogin = (this.LastLoginTime == null) ? true : false,
                HomeConfig=this.HomeConfigList
            });
        }
        #endregion

    }
}