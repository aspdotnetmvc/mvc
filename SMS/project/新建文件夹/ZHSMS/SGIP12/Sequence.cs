using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SGIP
{
    internal class Sequence
    {
        private volatile static Sequence _instance = null;
        private static object lockHelper = new object();

        int _iSeqID = 0;

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
            Interlocked.Increment(ref _iSeqID);
            if (_iSeqID > 2147483640)
            {
                _iSeqID = 0;
            }
            return (uint)_iSeqID;
        }
    }
}
