using ISMPModel;
using System;

namespace ISMPModel
{
    /// <summary>
    /// 运营客服
    /// </summary>
    [Serializable]
    public class ChannelServant : Employee
    {
        public ChannelServant()
            : base()
        {
            base.IsChannelServant = true;
        }

        /// <summary>
        /// 编码，用于查询上下级关系每级5位。
        /// </summary>
        public String ChannelServantCode { get; set; }
        /// <summary>
        /// 运营客服Id
        /// </summary>
        public string ChannelServantId
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
        public ChannelServant Parent
        {
            get;
            set;
        }
    }
}
