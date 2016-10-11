using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMSModel;
namespace BLL
{
    public class MO
    {
        public static bool Add(MOSMS mo)
        {
            return DAL.MO.Add(mo);
        }

        public static List<MOSMS> Gets(string spNumber, DateTime beginTime, DateTime endTime)
        {
            return DAL.MO.Gets(spNumber, beginTime, endTime);
        }
    }
}
