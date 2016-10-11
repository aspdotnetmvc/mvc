using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMPP
{
    #region COMMAND_ID 定义
    public enum CMPP_COMMAND : uint
    {
        /// <summary>
        /// 请求连接。
        /// </summary>
        CMD_CONNECT = 0x00000001,
        /// <summary>
        /// 请求连接应答。
        /// </summary>
        CMD_CONNECT_RESP = 0x80000001,
        /// <summary>
        /// 终止连接。
        /// </summary>
        CMD_TERMINATE = 0x00000002,
        /// <summary>
        /// 终止连接应答。
        /// </summary>
        CMD_TERMINATE_RESP = 0x80000002,
        /// <summary>
        /// 提交短信。
        /// </summary>
        CMD_SUBMIT = 0x00000004,
        /// <summary>
        /// 提交短信应答。
        /// </summary>
        CMD_SUBMIT_RESP = 0x80000004,
        /// <summary>
        /// 短信下发。
        /// </summary>
        CMD_DELIVER = 0x00000005,
        /// <summary>
        /// 下发短信应答。
        /// </summary>
        CMD_DELIVER_RESP = 0x80000005,
        /// <summary>
        /// 发送短信状态查询
        /// </summary>
        CMPP_QUERY = 0x00000006,
        /// <summary>
        /// 发送短信状态查询应答
        /// </summary>
        CMPP_QUERY_RESP = 0x80000006,
        /// <summary>
        /// 删除短信。
        /// </summary>
        CMD_CANCEL = 0x00000007,
        /// <summary>
        /// 删除短信应答。
        /// </summary>
        CMD_CANCEL_RESP = 0x80000007,
        /// <summary>
        /// 激活测试。
        /// </summary>
        CMD_ACTIVE_TEST = 0x00000008,
        /// <summary>
        /// 激活测试应答。
        /// </summary>
        CMD_ACTIVE_TEST_RESP = 0x80000008,
        /// <summary>
        /// 网络故障。
        /// </summary>
        CMD_ERROR = 0xFFFFFFFF,
    }
    #endregion
}
