using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGIP
{
    //SGIP 消息定义 
    public enum SGIP_COMMAND : uint
    {
        SGIP_BIND = 0x1,
        SGIP_BIND_RESP = 0x80000001,
        SGIP_UNBIND = 0x2,
        SGIP_UNBIND_RESP = 0x80000002,
        SGIP_SUBMIT = 0x3,
        SGIP_SUBMIT_RESP = 0x80000003,
        SGIP_DELIVER = 0x4,
        SGIP_DELIVER_RESP = 0x80000004,
        SGIP_REPORT = 0x5,
        SGIP_REPORT_RESP = 0x80000005,
        //SGIP_ADDSP = 0x6,
        //SGIP_ADDSP_RESP = 0x80000006,
        //SGIP_MODIFYSP = 0x7,
        //SGIP_MODIFYSP_RESP = 0x80000007,
        //SGIP_DELETESP = 0x8,
        //SGIP_DELETESP_RESP = 0x80000008,
        //SGIP_QUERYROUTE = 0x9,
        //SGIP_QUERYROUTE_RESP = 0x80000009,
        //SGIP_ADDTELESEG = 0xa,
        //SGIP_ADDTELESEG_RESP = 0x8000000a,
        //SGIP_MODIFYTELESEG = 0xb,
        //SGIP_MODIFYTELESEG_RESP = 0x8000000b,
        //SGIP_DELETETELESEG = 0xc,
        //SGIP_DELETETELESEG_RESP = 0x8000000c,
        //SGIP_ADDSMG = 0xd,
        //SGIP_ADDSMG_RESP = 0x8000000d,
        //SGIP_MODIFYSMG = 0xe,
        //SGIP_MODIFYSMG_RESP = 0x0000000e,
        //SGIP_DELETESMG = 0xf,
        //SGIP_DELETESMG_RESP = 0x8000000f,
        //SGIP_CHECKUSER = 0x10,
        //SGIP_CHECKUSER_RESP = 0x80000010,
        //SGIP_USERRPT = 0x11,
        //SGIP_USERRPT_RESP = 0x80000011,
        //SGIP_TRACE = 0x1000,
        //SGIP_TRACE_RESP = 0x80001000,
    }
}
