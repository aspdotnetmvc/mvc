using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GatewayInterface
{
    [Serializable]
    public class SMSEventArgs : EventArgs
    {
        #region 字段
        private SMS_Event _type;
        private object _data;
        #endregion

        #region 属性
        /// <summary>
        /// 事件类型。
        /// </summary>
        public SMS_Event Type
        {
            get
            {
                return _type;
            }
        }
        /// <summary>
        /// 事件数据。
        /// </summary>
        public object Data
        {
            get
            {
                return _data;
            }
        }

        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化 <see cref="SMSEventArgs"/> 类新实例。
        /// </summary>
        public SMSEventArgs(SMS_Event type, object data)
        {
            _type = type;
            _data = data;
        }
        #endregion

    }
}
