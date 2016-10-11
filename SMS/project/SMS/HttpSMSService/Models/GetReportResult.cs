using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpSMSService.Models
{
    public class Report
    {
        // 短信标识
        public string MsgID { get; set; }
        // 接收短信的手机号码
        public string Mobile { get; set; }
        // 状态报告
        public string Status { get; set; }
    }

    public class GetReportResult
    {
        // 调用接口返回的结果
        public string Result { get; set; }
        // 状态报告
        public List<Report> Reports { get; set; }
    }
}
