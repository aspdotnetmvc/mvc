using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    /// <summary>
    /// 系统日志
    /// </summary>
    [Serializable]
    public class Sys_Log
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

     

        /// <summary>
        /// 账号Id
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string LoginName { get; set; }

      
        /// <summary>
        /// 所属平台
        /// </summary>
        public String Platform { get; set; }

        /// <summary>
        /// 系统模块
        /// </summary>
        public String Module { get; set; }

        /// <summary>
        /// 日志消息
        /// </summary>
        public String LogMessage { get; set; }

        /// <summary>
        /// 日志类型（操作日志，错误日志）
        /// </summary>
        public LogType LogType { get; set; }

        /// <summary>
        /// 日志日期
        /// </summary>
        public DateTime? LogTime { get; set; }

        /// <summary>
        /// 记录异常等详细信息
        /// </summary>
        public String Log { get; set; }

        /// <summary>
        /// 操作类型 注销，新开，冻结 等等
        /// </summary>
        public String OperationType { get; set; }

        /// <summary>
        /// IP 地址
        /// </summary>
        public String IPAddress
        {
            get;
            set;
        }
    }
    public enum LogType
    {
        Exception = 0,//异常
        Operation = 1,//用户操作
        SysAdmin = 2  //系统管理
    }
}
