using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageTools
{
    public class MessageHelper
    {
        #region 单例
        private static object locker = new object();

        private static MessageHelper _instance = null;

        private MessageHelper()
        {
            try
            {
                string level = System.Configuration.ConfigurationManager.AppSettings["MessageLevel"];
                this.Level = (MessageLevel)Enum.Parse(typeof(MessageLevel), level);
            }
            catch (Exception)
            {
                Level = MessageLevel.ALL;
            }
        }
        public static MessageHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new MessageHelper();
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region
        public MessageLevel Level { get; set; }
        #endregion


        #region 消息容器
        private IMessageContainer container
        {
            get;
            set;
        }
        /// <summary>
        /// 设置消息容器
        /// </summary>
        /// <param name="container"></param>
        public void SetContainer(IMessageContainer container)
        {
            this.container = container;
        }
        #endregion

        #region 写消息
        /// <summary>
        /// 写消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        public void WriteMessage(string message, MessageLevel level = MessageLevel.Info, Exception exception = null)
        {
            if (this.Level > level) return;
            try
            {
                Message msg = new Message();
                msg.Level = level;
                msg.message = message;
                msg.Time = DateTime.Now;
                msg.Exception = exception;
                if (container == null)
                {
                    SetContainer(new ConsoleContainer());
                }
                container.WriteMessage(msg);
            }
            catch (Exception ex)
            {
                //发生异常该如何处理
                Console.WriteLine(ex.ToString());
            }
        }

        public void WirteTest(string message)
        {
            WriteMessage(message, MessageLevel.Test);
        }
        public void WirteInfo(string message)
        {
            WriteMessage(message, MessageLevel.Info);
        }
        public void WirteError(string message, Exception ex = null)
        {
            WriteMessage(message, MessageLevel.Error, ex);
        }

        public void WriteDisaster(string message, Exception ex)
        {
            WriteMessage(message, MessageLevel.Disaster, ex);
        }
        public void SendCMD(string cmd)
        {
            WriteMessage(cmd, MessageLevel.Command);
        }
        #endregion

    }
}
