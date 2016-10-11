using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendQueueService
{
    public class BlacklistManager
    {
        private volatile static BlacklistManager mng = null;
        private static object lockHelper = new object();

        List<string> numbers;

        private BlacklistManager()
        {
            numbers = new List<string>();
            LoadNumber();
        }

        void LoadNumber()
        {
            List<string> phones = BlacklistDB.GetNumbers();
            foreach (var phone in phones)
            {
                if (!ContainsNumber(phone))
                {
                    numbers.Add(phone);
                }
            }
        }

        public static BlacklistManager Instance
        {
            get
            {
                if (mng == null)
                {
                    lock (lockHelper)
                    {
                        if (mng == null)
                        {
                            mng = new BlacklistManager();
                            return mng;
                        }
                    }
                }
                return mng;
            }
        }

        public bool ContainsNumber(string number)
        {
            return numbers.Contains(number);
        }
        internal List<string> Get()
        {
            return numbers;
        }

        internal void Add(List<string> number)
        {
            foreach (var n in number)
            {
                if (!ContainsNumber(n))
                {
                    numbers.Add(n);
                }
            }
        }
        internal void Del(List<string> number)
        {
            foreach (var n in number)
            {
                numbers.Remove(n);
            }
        }
    }
}