using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GatewayInterface
{
    /// <summary>
    /// SMS 事件类型。
    /// </summary>
    public enum SMS_Event
    {
        /// <summary>
        /// 请求连接。
        /// </summary>
        SP_CONNECT,
        /// <summary>
        /// 终止连接。
        /// </summary>
        SP_DISCONNECT,
        /// <summary>
        /// 提交短信。
        /// </summary>
        SUBMIT,
        /// <summary>
        /// 提交短信应答。
        /// </summary>
        SUBMIT_RESPONSE,
        /// <summary>
        /// 短信下发。
        /// </summary>
        DELIVER,
        /// <summary>
        /// 下发短信应答。
        /// </summary>
        DELIVER_RESPONSE,
        /// <summary>
        /// 激活测试。
        /// </summary>
        ACTIVE_TEST,
        /// <summary>
        /// 激活测试应答。
        /// </summary>
        ACTIVE_TEST_RESPONSE,
        /// <summary>
        /// SUBMIT 状态报告。
        /// </summary>
        REPORT,
        /// <summary>
        /// 请求连接错误。
        /// </summary>
        SP_CONNECT_ERROR,
        /// <summary>
        /// 终止连接错误。
        /// </summary>
        SP_DISCONNECT_ERROR,
        /// <summary>
        /// 提交短信错误。
        /// </summary>
        SUBMIT_ERROR,
        /// <summary>
        /// 提交短信应答错误。
        /// </summary>
        SUBMIT_RESPONSE_ERROR,
        /// <summary>
        /// 短信下发错误。
        /// </summary>
        DELIVER_ERROR,
        /// <summary>
        /// 下发短信错误。
        /// </summary>
        DELIVER_RESPONSE_ERROR,
        /// <summary>
        /// 激活测试错误。
        /// </summary>
        ACTIVE_TEST_ERROR,
        /// <summary>
        /// 激活测试应答错误。
        /// </summary>
        ACTIVE_TEST_RESPONSE_ERROR,
        /// <summary>
        /// 未知错误。
        /// </summary>
        UNKNOW_ERROR
    }
}
