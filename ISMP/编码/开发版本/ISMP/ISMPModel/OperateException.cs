using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    /// <summary>
    /// 自定义异常，操作失败时，返回异常信息
    /// </summary>
    [Serializable]
    public class OperateException : Exception
    {

        /// <summary>
        /// 异常信息
        /// </summary>
        /// <param name="message"></param>
        public OperateException(string message) : base(message) { }
        public OperateException(int errorcode, string message)
            : base(message)
        {

            this.ErrorCode = errorcode;
        }
        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrorCode { get; set; }
    }
}
