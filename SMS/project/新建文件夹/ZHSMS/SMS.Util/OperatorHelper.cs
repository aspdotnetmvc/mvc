using SMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Util
{
    public class OperatorHelper
    {
        /// <summary>
        /// 号码按照运营商分组
        /// </summary>
        /// <returns></returns>
        public static Dictionary<OperatorType, List<string>> GroupNumbersByOperator(List<string> Numbers, List<NumSect> NumSectList)
        {
            Dictionary<OperatorType, List<string>> dic = new Dictionary<OperatorType, List<string>>();
            foreach (var n in Numbers)
            {
                var sect = NumSectList.FirstOrDefault(nc => n.StartsWith(nc.NumberSect));
                if (sect != null)
                {
                    if (!dic.ContainsKey(sect.OperatorType))
                    {
                        List<string> list = new List<string>();
                        dic.Add(sect.OperatorType, list);
                    }
                    dic[sect.OperatorType].Add(n);
                }
                else
                {
                    if (!dic.ContainsKey(OperatorType.notdefined))
                    {
                        List<string> list = new List<string>();
                        dic.Add(OperatorType.notdefined, list);
                    }
                    dic[OperatorType.notdefined].Add(n);
                }
            }
            return dic;
        }
    }
}
