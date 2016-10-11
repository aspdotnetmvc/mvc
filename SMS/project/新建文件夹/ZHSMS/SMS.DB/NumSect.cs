using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMS.Model;

namespace SMS.DB
{
    public partial class SMSDAL
    {
        /// <summary>
        /// 获取号码划分表
        /// </summary>
        /// <returns></returns>
        public static List<NumSect> GetNumSectList()
        {
            string sql = "select ID,Operators,NumberSec from Numsect";
            var list = DBHelper.Instance.Query(sql);
            return (from numsect in list select new NumSect() { ID = Convert.ToString(numsect.ID), OperatorType = Enum.Parse(typeof(OperatorType), numsect.Operators), NumberSect = Convert.ToString(numsect.NumberSec) }).ToList();
        }
    }
}
