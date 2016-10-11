using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMS.Util
{
    /// <summary>
    /// 流量控制器
    /// </summary>
    public class TrafficController
    {
        /// <summary>
        /// 控制间隔
        /// </summary>
        public int IntervalMicroSeconds { get; set; }
        /// <summary>
        /// 控制间隔内的最大数量
        /// </summary>
        public int MaxCount { get; set; }

        /// <summary>
        /// 启动计时器
        /// </summary>
        public void Start()
        {
            CurrentCount = 0;
            watch.Start();
        }

        /// <summary>
        /// 添加计数，并检查流速
        /// </summary>
        public void AddCountAndCheckTraffic(int count)
        {
            lock (this)
            {
                CurrentCount += count;
                if (CurrentCount > MaxCount)
                {
                    var interval = watch.ElapsedMilliseconds;
                    if (interval < IntervalMicroSeconds)
                    {
                        //计算sleep 时间
                        int sleep = Convert.ToInt32(Math.Floor(((CurrentCount * 1.0 / MaxCount) * IntervalMicroSeconds - interval)));
                        if (sleep > 0)
                        {
                            MessageTools.MessageHelper.Instance.WirteInfo("流速超速，睡眠：" + sleep + " ms");
                            Thread.Sleep(sleep);
                        }
                    }
                    else
                    {
                        MessageTools.MessageHelper.Instance.WirteInfo("没有超速，时间" + interval + " ms，计数：" + CurrentCount);
                    }
                    //重置
                    CurrentCount = 0;
                    watch.Restart();
                }
            }
        }

        #region

        /// <summary>
        /// 计时器
        /// </summary>
        private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        /// <summary>
        /// 当前计数
        /// </summary>
        private int CurrentCount { get; set; }


        #endregion
    }
}
