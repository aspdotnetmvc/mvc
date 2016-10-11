using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    [Serializable]
    public class RPC_Result<T> : RPC_Result
    {
        public RPC_Result(bool success, T value, string message)
            : base(success, message)
        {
            Value = value;
        }
        public RPC_Result(bool success, T value)
            : this(success, value, "")
        {

        }
        public RPC_Result(bool success)
            : this(success, default(T), "")
        {

        }

        public RPC_Result(bool success, string message)
            : this(success, default(T), message)
        {

        }


        /// <summary>
        /// 返回值
        /// </summary>
        public T Value { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }

    [Serializable]
    public class RPC_Result
    {
        public RPC_Result(bool success = true, string message = "", int errorcode = 0)
        {
            Success = success;
            Message = message;
            ErrorCode = errorcode;
        }

        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrorCode { get; set; }
        /// <summary>
        ///消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        public override string ToString()
        {
            return Message;
        }
    }
}
