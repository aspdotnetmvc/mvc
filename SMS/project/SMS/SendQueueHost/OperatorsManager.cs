using SMS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendQueueHost
{
    [Serializable]
    public class Operators
    {
        public string Name;
        public List<string> DNSEG;
    }

    internal class OperatorsManager
    {
        private volatile static OperatorsManager mng = null;
        private static object lockHelper = new object();

        Dictionary<string, string> _number3;
        Dictionary<string, string> _number4;
        private OperatorsManager()
        {
            _number3 = new Dictionary<string, string>();
            _number4 = new Dictionary<string, string>();
            LoadNumber();
        }
        
        private void LoadNumber()
        {
            List<Operators> operators = XmlSerialize.DeSerialize<List<Operators>>("Operators.Config");

            foreach (Operators oper in operators)
            {
                foreach (string num in oper.DNSEG)
                {
                    if (num.Length == 3)
                    {
                        if (!_number3.ContainsKey(num))
                        {
                            _number3.Add(num, oper.Name);
                        }
                    }
                    if(num.Length ==4)
                    {
                        if (!_number4.ContainsKey(num))
                        {
                            _number4.Add(num, oper.Name);
                        }
                    }

                }
            }
        }

        internal static OperatorsManager Instance
        {
            get
            {
                if (mng == null)
                {
                    lock (lockHelper)
                    {
                        if (mng == null)
                        {
                            mng = new OperatorsManager();
                            return mng;
                        }
                    }
                }
                return mng;
            }
        }

        internal string GetOperators(string number)
        {
            string s ="";
            if (number.Length > 4)
            {
                _number4.TryGetValue(number.Substring(0, 4), out s);
                if (!string.IsNullOrEmpty(s)) return s;
                _number3.TryGetValue(number.Substring(0, 3), out s);
            }
            return s;
        }
    }
}
