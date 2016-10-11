using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPInterface
{
    /// <summary>
    /// 用户类型
    /// </summary>
    [Serializable]
    public enum UserType
    {
        Employee = 1,
        ChannelManager = 2,
        ChannelServant = 3,
        Agent = 10,
        AgentEmployee = 11,
        Enterprise = 20
    }
}
