using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceManageWin
{
    public class ServiceManager
    {
        public IService.ServiceProp ServiceProp { get; set; }
        public IService.IService Service { get; set; }
        public ServiceLoader.ServiceLoader ServiceLoader { get; set; }
        /// <summary>
        /// 日志组件
        /// </summary>
        public ILog Logger { get; set; }
    }
}
