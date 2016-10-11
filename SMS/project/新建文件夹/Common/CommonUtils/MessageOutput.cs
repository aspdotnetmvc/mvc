using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BXM.Utils
{
    public delegate void OutputTextEventHandler(string text);
    public delegate void OutputMessageEventHandler(string type, object obj);
    public class MessageOutput
    {
        #region 单例
        private volatile static MessageOutput _instance = null;
        private static readonly object lockHelper = new object();
        private MessageOutput()
        {
        }

        public static MessageOutput Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockHelper)
                    {
                        if (_instance == null)
                            _instance = new MessageOutput();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region 事件
        public event OutputTextEventHandler OutputText;
        public event OutputMessageEventHandler OutputMessage;
        #endregion


        #region 公有方法
        /// <summary>
        /// 输出文本消息
        /// </summary>
        /// <param name="text"></param>
        public void Output(string text)
        {
            if (OutputText != null)
            {
                OutputText(text);
            }
        }

        /// <summary>
        /// 输出消息
        /// </summary>
        /// <param name="text"></param>
        public void Output(string type, object obj)
        {
            if (OutputMessage != null)
            {
                OutputMessage(type, obj);
            }
        }
        #endregion
    }
}
