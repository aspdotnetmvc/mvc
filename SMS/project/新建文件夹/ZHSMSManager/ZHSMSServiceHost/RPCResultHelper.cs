using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZHSMSServiceHost
{
    internal class RPCResultHelper
    {
        static internal RPCResult BuildFailureResult(string message)
        {
            RPCResult rpc = new RPCResult();
            rpc.Data = "";
            rpc.Success = false;
            rpc.Message = message;
            return rpc;
        }
        static internal RPCResult BuildSucessResult(string data)
        {
            RPCResult rpc = new RPCResult();
            rpc.Data = data;
            rpc.Success = true;
            rpc.Message = "";
            return rpc;
        }
        static internal RPCResult BuildSucessResult()
        {
            RPCResult rpc = new RPCResult();
            rpc.Data = "";
            rpc.Success = true;
            rpc.Message = "";
            return rpc;
        }
    }
}
