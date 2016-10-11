using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZHSMSServiceHost
{
    internal class ResultList<T>
    {
        static internal List<T> GetList(List<T> list, int pSize, int pIndex)
        {
            int startIndex = pIndex * pSize;
            if (startIndex != 0)
            {
                list = list.Skip(startIndex).ToList();
            }
            list = list.Take(pSize).ToList();
            return list;

        }
    }
}
