using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BXM.Utils
{
    /// <summary>
    /// 集合帮助类
    /// </summary>
    public class CollectionHelper
    {
        /// <summary>
        /// 随机排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public IList<T> RandomSort<T>(IList<T> array)
        {
            int len = array.Count;
            List<int> list = new List<int>();
            List<T> ret = new List<T>();
            Random rand = new Random((int)DateTime.Now.Ticks);
            int i = 0;
            while (list.Count < len)
            {
                int iter = rand.Next(0, len);
                if (!list.Contains(iter))
                {
                    list.Add(iter);
                    ret.Add(array[iter]);
                    i++;
                }
            }
            return ret;
        }
        /// <summary>
        /// 随机排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public T[] RandomSort<T>(T[] array)
        {
            int len = array.Length;
            List<int> list = new List<int>();
            T[] ret = new T[len];
            Random rand = new Random();
            int i = 0;
            while (list.Count < len)
            {
                int iter = rand.Next(0, len);
                if (!list.Contains(iter))
                {
                    list.Add(iter);
                    ret[i] = array[iter];
                    i++;
                }
            }
            return ret;
        }
    }

}
