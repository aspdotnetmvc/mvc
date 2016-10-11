using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    /// <summary>
    /// 企业层级结构
    /// </summary>
    [Serializable]
    public class EnterpriseHierarchy : Enterprise
    {
        /// <summary>
        /// 所属销售经理Code
        /// </summary>
        public String ChannelManagerCode { get; set; }

        /// <summary>
        /// 所属客服Code
        /// </summary>
        public String ChannelServantCode { get; set; }

        /// <summary>
        /// 所属经销商Code
        /// </summary>
        public String AgentCode { get; set; }
    }
}
