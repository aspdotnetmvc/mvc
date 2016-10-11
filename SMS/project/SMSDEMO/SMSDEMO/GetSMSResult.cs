using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZHSMSDEMO
{
    public class MOContent
    {
        //发送短信的号码
        public string Mobile { get; set; }
        //短信内容
        public string Content { get; set; }
        //发送短信时间
        public string MOTime { get; set; }
    }

    public class GetMOResult
    {
        //调用接口返回的结果
        public string Result { get; set; }
        //上行短信剩余的总条数
        public string MONum { get; set; }
        //前获取的短信条数，每次最大10条
        public string GetNum { get; set; }
        public List<MOContent> Msgs { get; set; }
    }
}
