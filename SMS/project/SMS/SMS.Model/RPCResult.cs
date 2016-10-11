using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    [Serializable]
    public class RPCResult<T> : RPCResult
    {
        public RPCResult(bool success, T value, string message)
            : base(success, message)
        {
            Value = value;
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
    public class RPCResult
    {
        public RPCResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

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
