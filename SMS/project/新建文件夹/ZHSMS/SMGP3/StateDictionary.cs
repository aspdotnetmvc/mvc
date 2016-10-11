using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMGP
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
        public static string stateRespDictionary(uint state)
        {
            string result = "结果：";
            switch (state)
            {
                case 0: result += "成功"; break;
                case 1: result += "系统忙"; break;
                case 2: result += "超过最大连接数"; break;
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9: result += "保留"; break;
                case 10: result += "消息结构错"; break;
                case 11: result += "命令字错"; break;
                case 12: result += "序列号重复"; break;
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19: result += "保留"; break;
                case 20: result += "IP 地址错"; break;
                case 21: result += "认证错"; break;
                case 22: result += "版本太高"; break;
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29: result += "保留"; break;
                case 30: result += "非法消息类型（MsgType）"; break;
                case 31: result += "非法优先级（Priority）"; break;
                case 32: result += "非法资费类型（FeeType）"; break;
                case 33: result += "非法资费代码（FeeCode）"; break;
                case 34: result += "非法短消息格式（MsgFormat）"; break;
                case 35: result += "非法时间格式"; break;
                case 36: result += "非法短消息长度（MsgLength）"; break;
                case 37: result += "有效期已过"; break;
                case 38: result += "非法查询类别（QueryType）"; break;
                case 39: result += "路由错误"; break;
                case 40: result += "非法包月费/封顶费（FixedFee）"; break;
                case 41: result += "非法更新类型（UpdateType"; break;
                case 42: result += "非法路由编号（RouteId）"; break;
                case 43: result += "非法服务代码（ServiceId）"; break;
                case 44: result += "非法有效期（ValidTime）"; break;
                case 45: result += "非法定时发送时间（AtTime）"; break;
                case 46: result += "非法发送用户号码（SrcTermId）"; break;
                case 47: result += "非法接收用户号码（DestTermId）"; break;
                case 48: result += "非法计费用户号码（ChargeTermId）"; break;
                case 49: result += "非法SP 服务代码（SPCode）"; break;
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55: result += " 其它用途"; break;
                case 56: result += "非法源网关代码（SrcGatewayID）"; break;
                case 57: result += "非法查询号码（QueryTermID）"; break;
                case 58: result += "没有匹配路由"; break;
                case 59: result += "非法SP 类型（SPType）"; break;
                case 60: result += "非法上一条路由编号（LastRouteID）"; break;
                case 61: result += "非法路由类型（RouteType）"; break;
                case 62: result += "非法目标网关代码（DestGatewayID）"; break;
                case 63: result += "非法目标网关IP（DestGatewayIP）"; break;
                case 64: result += "非法目标网关端口（DestGatewayPort）"; break;
                case 65: result += "非法路由号码段（TermRangeID）"; break;
                case 66: result += "非法终端所属省代码（ProvinceCode）"; break;
                case 67: result += "非法用户类型（UserType）"; break;
                case 68: result += "本节点不支持路由更新"; break;
                case 69: result += "非法SP 企业代码（SPID）"; break;
                case 70: result += "非法SP 接入类型（SPAccessType）"; break;
                case 71: result += "路由信息更新失败"; break;
                case 72: result += "非法时间戳（Time）"; break;
                case 73: result += "非法业务代码（MServiceID）"; break;
                case 74: result += "SP 禁止下发时段"; break;
                case 75: result += "SP 发送超过日流量"; break;
                case 76: result += "SP 帐号过有效期"; break;
                case 112:
                case 113:
                case 114:
                case 115:
                case 116: result += "其它用途"; break;
                default: result += "未知错误"; break;
            }

            return result;
        }
    }
}
