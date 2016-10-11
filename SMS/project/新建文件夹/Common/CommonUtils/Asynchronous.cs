using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BXM.Utils
{
    public class Asynchronous
    {
        /// <summary>
        ///执行一个异步方法，是否在规定的时间内完成
        /// </summary>
        /// <param name="action">异步方法</param>
        /// <param name="wait">等待时间，毫秒</param>
        /// <returns>是否执行成功</returns>
        internal static bool RunTask(Action action, int wait)
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            bool err = false;
            var t = Task.Factory.StartNew(action, token);

            if (!t.Wait(wait, token))
            {
                tokenSource.Cancel();
                return false;
            }
            if (err) return false;
            return true;
        }
    }
}
