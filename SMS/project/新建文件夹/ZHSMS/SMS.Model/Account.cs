using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Model
{
    [Serializable]
    public class Account
    {
        //帐号
        public string AccountID { get; set; }
        //密码

        //短信条数
        public int SMSNumber { get; set; }
    }
}
