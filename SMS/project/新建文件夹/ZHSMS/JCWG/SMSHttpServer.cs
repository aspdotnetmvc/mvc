using BXM.HttpServer;
using BXM.Utils;
using GatewayInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace JCWG
{
    public class SMSHttpServer : HttpServer
    {
        public event EventHandler<DeliverEventArgs> DeliverEvent;
        public event EventHandler<ReportEventArgs> ReportEvent;

        string moUrl, reportUrl;
        public SMSHttpServer(int port)
            : base(port)
        {
            XmlFileSerialize xfs = new XmlFileSerialize("JCWG.Config");
            Config _config = xfs.DeSerialize<Config>();

            reportUrl = _config.ReportURL;
            moUrl = _config.MOURL;
        }
        public override void handleGETRequest(HttpProcessor p)
        {
            p.writeSuccess();

            string[] ts = p.http_url.Split('?');


            string url = ts[0];


            if (url == "/MO")
            {
                //http://pushMoUrl?receiver=admin&pswd=12345&moTime=1208212205&mobile=13800210021&destcode=1065751600001&msg=hello&destcode=10657109012345
                ts = ts[1].Split('&');
                string receiver = "";
                string pswd = "";
                DateTime moTime = DateTime.Now;
                string mobile = "";
                string destcode = "";
                string msg = "";
                string emshead;
                string isems;
                string[] t;
                string time;
                foreach (string s in ts)
                {
                    t = s.Split('=');
                    switch (t[0])
                    {
                        case "receiver":
                            receiver = t[1];
                            break;
                        case "pswd":
                            pswd = t[1];
                            break;
                        case "moTime":
                            //格式YYMMDDhhmm，其中YY=年份的最后两位（00-99），MM=月份（01-12），DD=日（01-31），hh=小时（00-23），mm=分钟（00-59）
                            time = DateTime.Now.Year + "-" + t[1].Substring(2, 2) + "-" + t[1].Substring(4, 2) + " " + t[1].Substring(6, 2) + ":" + t[1].Substring(8, 2);
                            if (!DateTime.TryParse(time, out moTime))
                            {
                                moTime = DateTime.Now;
                            }
                            break;
                        case "mobile":
                            mobile = t[1];
                            break;
                        case "destcode":
                           // destcode = t[1];
                            break;
                        case "msg":
                            msg =System.Web.HttpUtility.UrlDecode( t[1]);
                            break;
                        case "isems":
                            isems = t[1];
                            break;
                        case "emshead":
                            emshead = t[1];
                            break;
                    }
                }

                try
                {
                    DeliverEventArgs re = new DeliverEventArgs(Guid.NewGuid().ToString(), moTime, msg, mobile, destcode, "");
                    DeliverEvent(this, re);
                    string tr = JsonSerialize.Instance.Serialize<DeliverEventArgs>(re);
                    LogClient.LogHelper.LogInfo("JCWG", "MO ->", tr);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    LogClient.LogHelper.LogInfo("JCWG", "MO Error ->", ex.Message);
                }

            }

            if (url == "/Report")
            {
                //"http://pushUrl?receiver=admin&pswd=12345&msgid=12345&reportTime=1012241002&mobile=13900210021&status=DELIVRD"
                ts = ts[1].Split('&');
                string receiver = "";
                string pswd = "";
                string msgid = "";
                DateTime reportTime = DateTime.Now;
                string mobile = "";
                string status = "";

                string[] t;
                string time;
                foreach (string s in ts)
                {
                    t = s.Split('=');
                    switch (t[0])
                    {
                        case "receiver":
                            receiver = t[1];
                            break;
                        case "pswd":
                            pswd = t[1];
                            break;
                        case "reportTime":
                            //格式YYMMDDhhmm，其中YY=年份的最后两位（00-99），MM=月份（01-12），DD=日（01-31），hh=小时（00-23），mm=分钟（00-59）
                            time = DateTime.Now.Year + "-" + t[1].Substring(2, 2) + "-" + t[1].Substring(4, 2) + " " + t[1].Substring(6, 2) + ":" + t[1].Substring(8, 2);
                            if (!DateTime.TryParse(time, out reportTime))
                            {
                                reportTime = DateTime.Now;
                            }
                            break;
                        case "mobile":
                            mobile = t[1];
                            break;
                        case "status":
                            status = t[1];
                            break;
                        case "msgid":
                            msgid = t[1];
                            break;
                    }
                }

                try
                {
                    bool ok = false;
                    ushort statecode;
                    string statetext = "短消息转发成功";
                    switch (status)
                    {
                        case "DELIVRD":
                            statetext = "短消息转发成功";
                            statecode = 2100;
                            ok = true;
                            break;
                        case "EXPIRED":
                            statetext = "短消息超过有效期";
                            statecode = 2101;
                            break;
                        case "UNDELIV":
                            statetext = "短消息是不可达的";
                            statecode = 2102;
                            break;
                        case "UNKNOWN":
                            statetext = "未知短消息状态";
                            statecode = 2103;
                            break;
                        case "REJECTD":
                            statetext = "短消息被短信中心拒绝";
                            statecode = 2104;
                            break;
                        case "DTBLACK":
                            statetext = "目的号码是黑名单号码";
                            statecode = 2105;
                            break;
                        case "ERR: 104":
                            statetext = "系统忙";
                            statecode = 2106;
                            break;
                        case "REJECT":
                            statetext = "审核驳回";
                            statecode = 2107;
                            break;
                        default:
                            statetext = "网关内部状态";
                            statecode = 2108;
                            break;
                    }

                    if (!ok)
                    {
                        statetext = "发送失败。";
                    }

                    ReportEventArgs re = new ReportEventArgs(msgid, ok, statecode, statetext, reportTime);
                    ReportEvent(this, re);
                    string tr = JsonSerialize.Instance.Serialize<ReportEventArgs>(re);
                    Console.WriteLine("返回状态报告：" + tr);
                    LogClient.LogHelper.LogInfo("JCWG", "GetReport Process ->", tr);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    LogClient.LogHelper.LogInfo("JCWG", "GetReport Error ->", ex.Message);
                }

            }
        }

        public override void handlePOSTRequest(HttpProcessor p, System.IO.StreamReader inputData)
        {

        }

        void ShowPopUpMessage(object message)
        {

        }

    }
}
