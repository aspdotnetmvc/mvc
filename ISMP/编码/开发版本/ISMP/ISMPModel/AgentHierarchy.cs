using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    [Serializable]
    public class AgentHierarchy:Agent
    {
        /// <summary>
        /// 经销商Id
        /// </summary>
        public string AgentId
        {
            get
            {
                return base.Id;
            }
            set
            {
                base.Id = value;
            }
        }
        /// <summary>
        /// 经销商编码
        /// </summary>
        public string AgentCode
        {
            get
            {
                return base.LevelCode;
            }
            set
            {
                base.LevelCode = value;
            }
        }
        /// <summary>
        /// 所属销售Code
        /// </summary>
        public String ChannelManagerCode { get; set; }
        /// <summary>
        /// 所属客服Code
        /// </summary>
        public String ChannelServantCode { get; set; }

    }
}
