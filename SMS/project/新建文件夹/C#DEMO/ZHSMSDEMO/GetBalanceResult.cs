using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZHSMSDEMO
{
    public class GetBalanceResult
    {
        //调用接口返回的结果
        public string Result { get; set; }
        //短信剩余条数
        public int SmsBalance { get; set; }
        //彩信剩余条数
        public int MmsBalance { get; set; }
    }
}
