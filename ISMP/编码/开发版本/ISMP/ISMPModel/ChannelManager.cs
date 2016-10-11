using ISMPModel;
using System;

namespace ISMPModel
{
    /// <summary>
    /// 销售经理
    /// </summary>
    [Serializable]
    public class ChannelManager : Employee
    {
        public ChannelManager()
            : base()
        {
            base.IsChannelManager = true;
        }

        /// <summary>
        /// 编码，用于查询上下级关系每级5位。
        /// </summary>
        public String ChannelManagerCode { get; set; }
        /// <summary>
        /// 销售经理Id
        /// </summary>
        public string ChannelManagerId
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
        /// 上级渠道
        /// </summary>
        public ChannelManager Parent
        {
            get;
            set;
        }
    }
}
