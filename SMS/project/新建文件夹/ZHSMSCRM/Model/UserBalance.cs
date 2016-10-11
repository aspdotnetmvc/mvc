using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class UserBalance
    {
        public int SmsBalance { get; set; }
        public int MmsBalance { get; set; }
    }
}
