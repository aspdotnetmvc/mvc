using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZHSMSServiceHost
{
    public delegate void OutputEventHandler(string message);
    public class HostOutput
    {
        #region 单例
        private volatile static HostOutput _instance = null;
        private static readonly object lockHelper = new object();
        private HostOutput()
        {
        }

        public static HostOutput Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockHelper)
                    {
                        if (_instance == null)
                            _instance = new HostOutput();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region 事件
        public event OutputEventHandler Output;
        #endregion


        #region 公有方法
        /// <summary>
        /// 输出文本消息
        /// </summary>
        /// <param name="text"></param>
        public void OutputText(string text)
        {
            if (Output != null)
            {
                Output(text);
            }
        }
        #endregion
    }
}