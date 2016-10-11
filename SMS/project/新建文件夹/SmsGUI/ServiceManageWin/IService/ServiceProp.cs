using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IService
{
    /// <summary>
    /// 服务配置
    /// </summary>
    [Serializable]
    public class ServiceProp
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string ClassName { get; set; }
        public string DLLFile { get; set; }
        public string DLLFullPath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Directory, DLLFile);
            }
        }
        public string Directory { get; set; }
        /// <summary>
        /// 日志级别
        /// </summary>
        public string LogLevel { get; set; }
        /// <summary>
        /// 是否启动
        /// </summary>
        public bool Start { get; set; }

    }
}
