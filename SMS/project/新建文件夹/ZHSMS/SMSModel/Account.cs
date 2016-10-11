using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSModel
{
    [Serializable]
    public class Account
    {
        //帐号
        public string AccountID { get; set; }
        //密码
        //public string Password { get; set; }
        //优先级(数字越大越高)
        //public AccountPriorityType Priority { get; set; }
        //短信条数
        public int SMSNumber { get; set; }
        ////注册日期
        //public DateTime RegisterDate { get; set; }
        ////审核方式
        //public AccountAuditType Audit { get; set; }
        ///// <summary>
        ///// 是否启用 true:1 false:0
        ///// </summary>
        //public bool Enabled { get; set; }
        ///// <summary>
        ///// 企业代码
        ///// </summary>
        //public string SPNumber { get; set; }
    }
}
