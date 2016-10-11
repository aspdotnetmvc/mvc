using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IService
{
    /// <summary>
    /// 服务接口，要使用管理窗口，必须实现此接口
    /// </summary>
    public interface IService
    {
        Host Host { get; set; }
        /// <summary>
        /// 启动服务
        /// </summary>
        void StartService(Host host);
        //停止服务
        void StopService();
        //void WriteToHost(string message, string type);
    }
}
