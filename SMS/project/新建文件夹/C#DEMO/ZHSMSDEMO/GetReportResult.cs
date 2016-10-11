using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZHSMSDEMO
{
    public class SMSReport
    {
        //短信发送标识
        public string MsgID { get; set; }
        //接收短信的号码
        public string Destination { get; set; }
        //发送结果
        public string Stat { get; set; }
    }
    public class GetReportResult
    {
        //调用接口返回的结果
        public string Result { get; set; }

        public List<SMSReport> Reports { get; set; }
    }
}
