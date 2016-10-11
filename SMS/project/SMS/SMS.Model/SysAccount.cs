using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    [Serializable]
    public class SysAccount
    {
        /// <summary>
        /// 工号
        /// </summary>
        public string UserCode { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public List<string> Roles { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 状态 true:可登陆，false：不可登录
        /// </summary>
        public bool Status { get; set; }
    }
}
