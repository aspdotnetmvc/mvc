using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IService
{
    /// <summary>
    /// 关联主Form 的一个东西，用于服务向主界面输出信息，调用命令等
    /// </summary>
    public class Host : MarshalByRefObject, MessageTools.IMessageContainer
    {
        public override Object InitializeLifetimeService()
        {
            return null; // 永不过期
        }
        public Host(IForm ui, ServiceProp sp)
        {
            UI = ui;
            serviceProp = sp;
        }
        public IForm UI { get; set; }
        public ServiceProp serviceProp { get; set; }
         

        public void ExecuteCmd(string cmd)
        {
            throw new NotImplementedException();
        }

        public void WriteMessage(MessageTools.Message message)
        {
            try
            {
                UI.WriteLog(serviceProp.Code, message.message, message.Level.ToString(), message.Time);
            }
            catch (Exception ex)
            {
                try
                {
                    UI.WriteLog(serviceProp.Code, "写消息时发生错误", MessageTools.MessageLevel.Error.ToString(), DateTime.Now);
                    UI.WriteLog(serviceProp.Code, ex.ToString(), MessageTools.MessageLevel.Error.ToString(), DateTime.Now);
                    UI.WriteLog(serviceProp.Code, message.Level.ToString() + ":" + message, MessageTools.MessageLevel.Error.ToString(), DateTime.Now);
                }
                catch (Exception ex2)
                {
                    string str = "写消息时发生错误\n"
                                          + ex.ToString()
                                           + "message:\n" + message.Level.ToString() + ":" + message.message;
                    try
                    {
                        System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "err/error.log", str, Encoding.UTF8);
                    }
                    catch { }
                }
            }
        }
    }
}
