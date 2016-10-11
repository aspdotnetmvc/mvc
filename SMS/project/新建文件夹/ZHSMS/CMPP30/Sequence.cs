using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CMPP
{
    internal class Sequence
    {
        private volatile static Sequence _instance = null;
        private static object lockHelper = new object();

        ushort _iSeqID;

        private Sequence()
        {
        }

        internal static Sequence Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockHelper)
                    {
                        if (_instance == null)
                        {
                            _instance = new Sequence();
                            return _instance;
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 创建消息流水号。
        /// </summary>
        /// <remarks>
        /// 消息流水号，顺序累加，步长为 1，循环使用（每对请求和应答消息的流水号必须相同）。
        /// </remarks>
        internal uint CreateID()
        {
            DateTime dt = DateTime.Now;

            StringBuilder id = new StringBuilder();
            id.Append(dt.Day.ToString());

            if (id.Length > 1)
            {
                id.Remove(0, 1);
            }
            id.Append(dt.Hour.ToString());
            id.Append(dt.Minute.ToString());
            id.Append(dt.Second.ToString());
            if (_iSeqID > 999) _iSeqID = 0;
            id.Append(_iSeqID++);
            
            return System.Convert.ToUInt32(id.ToString());
        }
    }
}
