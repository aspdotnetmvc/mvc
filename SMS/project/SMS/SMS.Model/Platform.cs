using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    public enum PlatformCode : ushort
    {
        SYS = 1000,
        CMPP = 2000,
        SGIP = 3000,
        SMGP = 4000,
    }

    public enum SystemCode : ushort
    {
        /// <summary>
        /// 准备发送
        /// </summary>
        SendReady =1,
        /// <summary>
        /// 提交网关成功
        /// </summary>
        SubmitSuccess = 10,
        /// <summary>
        /// 发送失败
        /// </summary>
        SendFailure =20,
        /// <summary>
        /// 黑名单号码
        /// </summary>
        BlacklistNumber = 21,
        /// <summary>
        /// 无可用网关
        /// </summary>
        NoUseGateway = 22,
        /// <summary>
        /// 无可用通道
        /// </summary>
        NoUseChannel = 23,
        /// <summary>
        /// 无效运行商号段
        /// </summary>
        VoidOperators = 24,
        /// <summary>
        /// 发送号码错误
        /// </summary>
        NumberWrong = 25,
        /// <summary>
        /// 发送超时
        /// </summary>
        SendTimeOut = 26,
        /// <summary>
        /// 包含非法关键字
        /// </summary>
        IllegalKeyword = 27,
        /// <summary>
        /// 序列化失败
        /// </summary>
        SerializationFailure = 28,
        /// <summary>
        /// 报告返回
        /// </summary>
        ReportBack = 100,
        /// <summary>
        /// 报告超时
        /// </summary>
        ReportTimeOut = 101,
        /// <summary>
        /// 停机
        /// </summary>
        Downtime = 102,
        /// <summary>
        /// 空号
        /// </summary>
        DeadNumber = 103,
    }
}
