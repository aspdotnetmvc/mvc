using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGIP
{
    /// <summary>
    /// 状态字典
    /// </summary>
    public class StateDictionary
    {
        /// <summary>
        /// Status 请求返回结果。响应包用来向请求包返回成功信息或者失败原因。
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static string StateRespDictionary(uint state)
        {
            string result = "结果：";
            switch (state)
            {
                case 0: result += "无错误，命令正确接收"; break;
                case 1: result += "非法登录，如登录名、口令出错、登录名与口令不符等。"; break;
                case 2: result += "重复登录，如在同一TCP/IP连接中连续两次以上请求登录。"; break;
                case 3: result += "连接过多，指单个节点要求同时建立的连接数过多。"; break;
                case 4: result += "登录类型错，指bind命令中的logintype字段出错。"; break;

                case 5: result += "参数格式错，指命令中参数值与参数类型不符或与协议规定的范围不符。"; break;
                case 6: result += "非法手机号码，协议中所有手机号码字段出现非86130号码或手机号码前未加“86”时都应报错。"; break;
                case 7: result += "消息ID错"; break;
                case 8: result += "信息长度错"; break;
                case 9: result += "非法序列号，包括序列号重复、序列号格式错误等"; break;
                case 10: result += "非法操作GNS"; break;
                case 11: result += "节点忙，指本节点存储队列满或其他原因，暂时不能提供服务的情况"; break;

                case 21: result += "目的地址不可达，指路由表存在路由且消息路由正确但被路由的节点暂时不能提供服务的情况"; break;
                case 22: result += "路由错，指路由表存在路由但消息路由出错的情况，如转错SMG等"; break;
                case 23: result += "路由不存在，指消息路由的节点在路由表中不存在"; break;
                case 24: result += "计费号码无效，鉴权不成功时反馈的错误信息"; break;
                case 25: result += "用户不能通信（如不在服务区、未开机等情况）"; break;
                case 26: result += "手机内存不足"; break;
                case 27: result += "手机不支持短消息"; break;
                case 28: result += "手机接收短消息出现错误"; break;
                case 29: result += "不知道的用户"; break;
                case 30: result += "不提供此功能"; break;
                case 31: result += "非法设备"; break;
                case 32: result += "系统失败"; break;
                case 33: result += "短信中心队列满"; break;

                default: result = "其它错误码" + state.ToString(); break;
            }
            return result;
        }
    }

    /// <summary>
    /// 计费类别定义
    /// </summary>
    public enum FeeTypes : byte
    {
        /// <summary>
        /// 0	“短消息类型”为“发送”，对“计费用户号码”不计信息费，此类话单仅用于核减SP对称的信道费
        /// </summary>
        FreeSend = 0,
        /// <summary>
        /// 1	对“计费用户号码”免费
        /// </summary>
        Free = 1,
        /// <summary>
        /// 2	对“计费用户号码”按条计信息费
        /// </summary>
        RowNumFee = 2,
        /// <summary>
        /// 3	对“计费用户号码”按包月收取信息费
        /// </summary>
        MonthFee = 3,
        /// <summary>
        /// 4	对“计费用户号码”的收费是由SP实现
        /// </summary>
        SpFee = 4,
    }
    /// <summary>
    /// Report 状态与短消息状态的映射
    /// </summary>
    public enum ReportStatus : uint
    {
        /// <summary>
        /// 0，发送成功	DELIVERED
        /// </summary>
        Delivered = 0,
        /// <summary>
        /// 1，等待发送	ENROUTE，ACCEPTED
        /// </summary>
        Accepted = 1,
        /// <summary>
        /// 2，发送失败	EXPIRED，DELETED，UNDELIVERABLE，UNKNOWN，REJECTED
        /// </summary>
        Error = 2,
    }
    public enum ErrorCodes : byte
    {

        /// <summary>
        /// 0	无错误，命令正确接收
        /// </summary>
        Success = 0,
        /// <summary>
        /// 1	非法登录，如登录名、口令出错、登录名与口令不符等。
        /// </summary>
        LoginError = 1,
        /// <summary>
        /// 2	重复登录，如在同一TCP/IP连接中连续两次以上请求登录。
        /// </summary>
        Relogon = 2,
        /// <summary>
        /// 3	连接过多，指单个节点要求同时建立的连接数过多。
        /// </summary>
        ConnectionFull = 3,
        /// <summary>
        /// 4	登录类型错，指bind命令中的logintype字段出错。
        /// </summary>
        ErrorLoginType = 4,
        /// <summary>
        /// 5	参数格式错，指命令中参数值与参数类型不符或与协议规定的范围不符。
        /// </summary>
        ParameterError = 5,
        /// <summary>
        /// 6	非法手机号码，协议中所有手机号码字段出现非86130号码或手机号码前未加“86”时都应报错。
        /// </summary>
        TelnumberError = 6,
        /// <summary>
        /// 7	消息ID错
        /// </summary>
        MsgIDError = 7,
        /// <summary>
        /// 8	信息长度错
        /// </summary>
        PackageLengthError = 8,
        /// <summary>
        /// 9	非法序列号，包括序列号重复、序列号格式错误等
        /// </summary>
        SequenceError = 9,
        /// <summary>
        /// 10	非法操作GNS
        /// </summary>
        GnsOperationError = 10,
        /// <summary>
        /// 11	节点忙，指本节点存储队列满或其他原因，暂时不能提供服务的情况
        /// </summary>
        NodeBusy = 11,
        /// <summary>
        /// 21	目的地址不可达，指路由表存在路由且消息路由正确但被路由的节点暂时不能提供服务的情况
        /// </summary>
        NodeCanNotReachable = 21,
        /// <summary>
        /// 22	路由错，指路由表存在路由但消息路由出错的情况，如转错SMG等
        /// </summary>
        RouteError = 22,
        /// <summary>
        /// 23	路由不存在，指消息路由的节点在路由表中不存在
        /// </summary>
        RoutNodeNotExisted = 23,
        /// <summary>
        /// 24	计费号码无效，鉴权不成功时反馈的错误信息
        /// </summary>
        FeeNumberError = 24,
        /// <summary>
        /// 25	用户不能通信（如不在服务区、未开机等情况）
        /// </summary>
        UserCanNotReachable = 25,
        /// <summary>
        /// 26	手机内存不足
        /// </summary>
        HandsetFull = 26,
        /// <summary>
        /// 27	手机不支持短消息
        /// </summary>
        HandsetCanNotRecvSms = 27,
        /// <summary>
        /// 28	手机接收短消息出现错误
        /// </summary>
        HandsetReturnError = 28,
        /// <summary>
        /// 29	不知道的用户
        /// </summary>
        UnknownUser = 29,
        /// <summary>
        /// 30	不提供此功能
        /// </summary>
        NoDevice = 30,
        /// <summary>
        /// 31	非法设备
        /// </summary>
        InvalidateDevice = 31,
        /// <summary>
        /// 32	系统失败
        /// </summary>
        SystemError = 32,
        /// <summary>
        /// 33	短信中心队列满
        /// </summary>
        FullSequence = 33,
        /// <summary>
        /// 未知错误
        /// </summary>
        OtherError = 99,
    }
    /// <summary>
    /// Bind操作，登录类型。
    /// </summary>
    public enum LoginTypes : byte
    {
        /// <summary>
        /// 1：SP向SMG建立的连接，用于发送命令
        /// </summary>
        SpToSmg = 1,
        /// <summary>
        /// 2：SMG向SP建立的连接，用于发送命令
        /// </summary>
        SmgToSp = 2,
        /// <summary>
        /// 3：SMG之间建立的连接，用于转发命令
        /// </summary>
        SmgToSmg = 3,
        /// <summary>
        /// 4：SMG向GNS建立的连接，用于路由表的检索和维护
        /// </summary>
        SmgToGns = 4,
        /// <summary>
        /// 5：GNS向SMG建立的连接，用于路由表的更新
        /// </summary>
        GnsToSmg = 5,
        /// <summary>
        /// 6：主备GNS之间建立的连接，用于主备路由表的一致性
        /// </summary>
        GnsToGns = 6,
        /// <summary>
        /// 11：SP与SMG以及SMG之间建立的测试连接，用于跟踪测试
        /// </summary>
        Test = 11,
        /// <summary>
        /// 其它：保留
        /// </summary>
        Unknown = 0,
    }
    /// <summary>
    /// 短消息的编码格式。
    /// </summary>
    public enum MessageCodings : byte
    {
        /// <summary>
        /// 0：纯ASCII字符串
        /// </summary>
        Ascii = 0,
        /// <summary>
        /// 3：写卡操作
        /// </summary>
        WriteCard = 3,
        /// <summary>
        /// 4：二进制编码
        /// </summary>
        Binary = 4,
        /// <summary>
        /// 8：UCS2编码
        /// </summary>
        Ucs2 = 8,
        /// <summary>
        /// 15: GBK编码
        /// </summary>
        Gbk = 15,
        /// <summary>
        /// 其它参见GSM3.38第4节：SMS Data Coding Scheme
        /// </summary>
        Others = 99,
    }
    /// <summary>
    /// 引起MT消息的原因
    /// </summary>
    public enum SubmitMorelatetoMTFlags : byte
    {
        /// <summary>
        /// 0-MO点播引起的第一条MT消息；
        /// </summary>
        VoteFirst = 0,
        /// <summary>
        /// 1-MO点播引起的非第一条MT消息；
        /// </summary>
        VoteNonFirst = 1,
        /// <summary>
        /// 2-非MO点播引起的MT消息；
        /// </summary>
        NormalFirst = 2,
        /// <summary>
        /// 3-系统反馈引起的MT消息。
        /// </summary>
        NormalNonFirst = 3,
    }
    /// <summary>
    /// Report命令类型
    /// </summary>
    public enum ReportTypes : byte
    {
        /// <summary>
        /// 0：对先前一条Submit命令的状态报告
        /// </summary>
        Submit = 0,
        /// <summary>
        /// 1：对先前一条前转Deliver命令的状态报告
        /// </summary>
        Deliver = 1,
    }
    /// <summary>
    /// 该命令所涉及的短消息的当前执行状态
    /// </summary>
    public enum ReportStates : byte
    {
        /// <summary>
        /// 0：发送成功
        /// </summary>
        Success = 0,
        /// <summary>
        /// 1：等待发送
        /// </summary>
        Accepted = 1,
        /// <summary>
        /// 2：发送失败
        /// </summary>
        Error = 2,
    }
    /// <summary>
    /// 代收费标志，0：应收；1：实收
    /// </summary>
    public enum SubmitAgentFlag : byte
    {
        /// <summary>
        /// 0：应收
        /// </summary>
        SouldIncome = 0,
        /// <summary>
        /// 1：实收
        /// </summary>
        RealIncome = 1,
    }
    /// <summary>
    /// 状态报告标记
    /// </summary>
    public enum SubmitReportFlag : byte
    {
        /// <summary>
        /// 0-该条消息只有最后出错时要返回状态报告
        /// </summary>
        ErrorReport = 0,
        /// <summary>
        /// 1-该条消息无论最后是否成功都要返回状态报告
        /// </summary>
        Always = 1,
        /// <summary>
        /// 2-该条消息不需要返回状态报告
        /// </summary>
        NoReport = 2,
        /// <summary>
        /// 3-该条消息仅携带包月计费信息，不下发给用户，要返回状态报告
        /// </summary>
        MonthReport = 3,
    }
}
