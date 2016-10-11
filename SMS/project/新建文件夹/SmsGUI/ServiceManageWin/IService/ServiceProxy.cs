using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IService
{
    /// <summary>
    /// 作为Service 在各个项目内的代理，一般用于通过代理向Service 控制台输出信息
    /// </summary>
    public class ServiceProxy
    {
        private ServiceProxy()
        {

        }
        private static ServiceProxy proxy;
        public static ServiceProxy Instance
        {
            get
            {
                if (proxy == null)
                {
                    proxy = new ServiceProxy();
                }
                return proxy;
            }
            set
            {

            }
        }
        public IService Service { get; set; }
        public void WriteToService(string message, string type)
        {
            if (Service != null)
            {
                Service.WriteToHost(message, type);
            }
            else
            {
                Console.WriteLine(DateTime.Now.ToString() + " " + type + ": " + message);
            }
        }
        public void WriteTest(string message)
        {
            WriteToService(message, "test");
        }
        public void WriteError(string message)
        {
            WriteToService(message, "error");
        }
        public void WriteInfo(string message)
        {
            WriteToService(message, "info");
        }
        /// <summary>
        /// 暂未执行
        /// </summary>
        /// <param name="cmd"></param>
        public void SendCmd(string cmd)
        {
            WriteToService(cmd, "cmd");
        }
    }
}
