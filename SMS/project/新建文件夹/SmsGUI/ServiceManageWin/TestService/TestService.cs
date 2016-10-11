using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TestService
{
    /// <summary>
    /// 测试服务，也是服务的一个范例
    /// </summary>
    public class TestService : IService.AbstractService
    {
        public override void StartService(IService.Host host)
        {
            base.StartService(host);

            MessageTools.MessageHelper.Instance.WirteTest("正在启动服务" + System.Configuration.ConfigurationManager.AppSettings["Title"]);
            //此处实现服务内容，如启动Remoting 服务，启动WebService ，WCF 等

            int i = 0;
            Task t = new Task(delegate()
             {
                 while (true)
                 {
                     MessageTools.MessageHelper.Instance.WirteTest("每隔5s 执行一次,已执行：" + (i++) + "次");
                     System.Threading.Thread.Sleep(5 * 1000);
                     MessageTools.MessageHelper.Instance.WirteTest("新线程的AppDomain:" + AppDomain.CurrentDomain.FriendlyName);
                     //  throw new NotSupportedException("1未处理的异常！！！！");
                 }
             });
            t.Start();
            MessageTools.MessageHelper.Instance.WirteTest("服务已启动" + System.Configuration.ConfigurationManager.AppSettings["Title"]);
            //throw new NotSupportedException("2未处理的异常！！！！");
        }

        public override void StopService()
        {
            base.StopService();
            MessageTools.MessageHelper.Instance.WirteTest("服务已停止" + System.Configuration.ConfigurationManager.AppSettings["Title"]);
        }
    }
}
