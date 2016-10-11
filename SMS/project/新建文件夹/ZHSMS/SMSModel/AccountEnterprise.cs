using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSModel
{
    public class AccountEnterprise
    {
        public string UserCode { get; set; }
        public string EnterpriseCode { get; set; }
    }

    public class EnterpriseManage
    {
        // 记录编号
        public Int32 ID { get; set; }
        // 企业代码
        public string EnterpriseCode { get; set; }
        // 给该企业指定的渠道经理
        public string ChannelManager { get; set; }
        // 给该企业指定的渠道客服
        public string CSManager { get; set; }
        // 保留字段
        public string Reserve { get; set; }
    }
}
