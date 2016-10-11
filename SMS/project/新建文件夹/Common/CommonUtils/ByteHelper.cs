using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BXM.Utils
{
    public class ByteHelper
    {
        /// <summary>
        /// 查找一个byte数组在另一个byte数组第一次出现位置
        /// </summary>
        /// <param name="array1">被查找的数组</param>
        /// <param name="array2">要查找的数组</param>
        /// <returns>找到返回索引，找不到返回-1</returns>
        public static int Find(byte[] array1, byte[] array2)
        {
            int i, j;

            for (i = 0; i < array1.Length; i++)
            {
                if (i + array2.Length <= array1.Length)
                {
                    for (j = 0; j < array2.Length; j++)
                    {
                        if (array1[i + j] != array2[j]) break;
                    }

                    if (j == array2.Length) return i;
                }
                else
                    break;
            }

            return -1;
        }
    }
}
