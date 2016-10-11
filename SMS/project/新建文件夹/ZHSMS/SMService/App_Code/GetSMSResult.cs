using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MO
{
    // 发送短信的源号码
    public string Src { get; set; }
    // 发送短信的内容
    public string Content { get; set; }
    // 发送短信的时间
    public string MOTime { get; set; }
}

public class GetSMSResult
{
    // 调用接口返回的结果
    public string Result { get; set; }
    // 目前剩余的上行短信总条数
    public string MONum { get; set; }
    // 当前获取到的短信条数
    public string GetNum { get; set; }
    // 上行短信
    public List<MO> Msgs { get; set; }
}
