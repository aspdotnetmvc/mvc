using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMGP
{
    /// <summary>
    /// SMGP3 消息定义
    /// </summary>
    public enum SMGP3_COMMAND : uint
    {
        Login = 0x00000001          //客户端登录                ----sp use
            ,
        Login_Resp = 0x80000001     //客户端登录应答            ----sp use
            ,
        Submit = 0x00000002         //提交短消息                ----sp use
            ,
        Submit_Resp = 0x80000002    //提交短消息应答            ----sp use
            ,
        Deliver = 0x00000003        //下发短消息                ----sp use
            ,
        Deliver_Resp = 0x80000003   //下发短消息应答            ----sp use
            ,
        Active_Test = 0x00000004    //链路检测                  ----sp use
            ,
        Active_Test_Resp = 0x80000004   //链路检测应答          ----sp use
            ,
        Forward = 0x00000005            //短消息前转 
            ,
        Forward_Resp = 0x80000005       //短消息前转应答 
            ,
        Exit = 0x00000006               //退出请求              ----sp use
            ,
        ExitResp = 0x80000006               //退出应答          ----sp use
            ,
        Query = 0x00000007              //SP 统计查询 
            ,
        Query_Resp = 0x80000007         //SP 统计查询应答 
            ,
        Query_TE_Route = 0x00000008     //查询TE 路由 
            ,
        Query_TE_Route_Resp = 0x80000008    //查询TE 路由应答 
            ,
        Query_SP_Route = 0x00000009         //查询SP 路由
            ,
        Query_SP_Route_Resp = 0x80000009    //查询SP 路由应答 
            ,
        Payment_Request = 0x0000000A          //扣款请求(用于预付费系统，参见增值业务计费方案)
            ,
        Payment_Request_Resp = 0x8000000A     //扣款请求响应(用于预付费系统，参见增值业务计费方案，下同)
            ,
        Payment_Affirm = 0x0000000B          //扣款确认(用于预付费系统，参见增值业务计费方案)
            ,
        Payment_Affirm_Resp = 0x8000000B     //扣款确认响应(用于预付费系统，参见增值业务计费方案)
            ,
        Query_UserState = 0x0000000C         //查询用户状态(用于预付费系统，参见增值业务计费方案)
            ,
        Query_UserState_Resp = 0x8000000C      //查询用户状态响应(用于预付费系统，参见增值业务计费方案)
            ,
        Get_All_TE_Route = 0x0000000D           //获取所有终端路由
            ,
        Get_All_TE_Route_Resp = 0x8000000D      //获取所有终端路由应答
            ,
        Get_All_SP_Route = 0x0000000E           //获取所有SP 路由
            ,
        Get_All_SP_Route_Resp = 0x8000000E      //获取所有SP 路由应答
            ,
        Update_TE_Route = 0x0000000F            //SMGW 向GNS 更新终端路由
            ,
        Update_TE_Route_Resp = 0x8000000F       //SMGW 向GNS 更新终端路由应答
            ,
        Update_SP_Route = 0x00000010            //SMGW 向GNS 更新SP 路由
            ,
        Update_SP_Route_Resp = 0x80000010       // SMGW 向GNS 更新SP 路由应答
            ,
        Push_Update_TE_Route = 0x00000011       //GNS 向SMGW 更新终端路由
            ,
        Push_Update_TE_Route_Resp = 0x80000011  //GNS 向SMGW 更新终端路由应答 
            ,
        Push_Update_SP_Route = 0x00000012       //GNS 向SMGW 更新SP 路由
            ,
        Push_Update_SP_Route_Resp = 0x80000012  // GNS 向SMGW 更新SP 路由应答
    }
}
