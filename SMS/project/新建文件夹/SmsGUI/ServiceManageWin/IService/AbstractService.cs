using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading;
namespace IService
{
    public abstract class AbstractService : MarshalByRefObject,IService
    {
        /// <summary>
        /// 设置远程对象永不过期
        /// </summary>
        /// <returns></returns>
        public override Object InitializeLifetimeService()
        {
            return null; // 永不过期
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="host"></param>
        public virtual void StartService(Host host)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            this.Host = host;
        }
        /// <summary>
        /// 关闭服务
        /// </summary>
        public virtual void StopService()
        {
            MessageTools.MessageHelper.Instance.WirteInfo("服务已停止");
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageTools.MessageHelper.Instance.WirteError("发生了未处理的异常", (Exception)e.ExceptionObject);
        }

        private Host _host;
        public Host Host
        {
            get
            {
                return _host;
            }
            set
            {
                _host = value;
                MessageTools.MessageHelper.Instance.SetContainer(value);
            }
        }
 
    }
}
