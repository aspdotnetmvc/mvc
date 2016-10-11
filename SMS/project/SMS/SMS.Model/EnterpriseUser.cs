
using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    [Serializable]
    public class EnterpriseUser
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string AccountID { get; set; }
        /// <summary>
        /// 企业帐号
        /// </summary>
        public string AccountCode { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }  
        /// <summary>
        /// 优先级(数字越大越高)
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 注册日期
        /// </summary>
        public DateTime RegisterDate { get; set; }
       
        /// <summary>
        /// 企业代码
        /// </summary>
        public string SPNumber { get; set; }
        /// <summary>
        /// 是否启用 true:1 false:0
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// 是否是代理商
        /// </summary>
        public bool IsAgent { get; set; }
        /// <summary>
        /// 上级企业代码，无则-1标识
        /// </summary>
        public string ParentAccountCode { get; set; }
        /// <summary>
        /// 审核方式
        /// </summary>
        public AccountAuditType Audit { get; set; }
     
        /// <summary>
        /// 企业名称
        /// </summary>
        public string Name { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        /// <summary>
        /// 密钥
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// 是否对外开放接口
        /// </summary>
        public bool IsOpen { get; set; }
        /// <summary>
        /// 对外开放接口帐号密码
        /// </summary>
        public string AppPassword { get; set; }


        #region SMSSetting

        /// <summary>
        /// 商业 行业 四大类
        /// </summary>
        public SMSType SMSType { get; set; }
        /// <summary>
        /// 状态报告接收方式
        /// </summary>
        public StatusReportType StatusReport
        {
            get;
            set;
        }
        /// <summary>
        /// 发送短信的通道
        /// </summary>
        public string Channel { get; set; }


        /// <summary>
        /// 过滤方式
        /// </summary>
        public ushort FilterType { get; set; }

        /// <summary>
        /// 短信签名
        /// </summary>
        public string Signature { get; set; }
        #endregion
         

    }
}
