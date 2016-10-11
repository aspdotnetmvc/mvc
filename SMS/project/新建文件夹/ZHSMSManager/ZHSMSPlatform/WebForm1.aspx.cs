using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string TempletContent = "成都市蓝星维加科技有限公司是成都市电信400电话服务商，欢迎来电合作：18180651006（微信） QQ：2429408936 免费服务热线：4009939400";
            string content = "成都市蓝星维加科技有限公司是成都市电信400电话服务商，欢迎来电合作：18180651006（微信） QQ：2429408936 免费服务热线：4009939400";

           
                    if (TempletContent.Length == content.Length)
                    {
                        //模板匹配成功
                        bool ok = true;
                        int last = TempletContent.LastIndexOf('*');
                        char[] arr = TempletContent.ToCharArray();
                        for (int i = last - 1; i >= 0; i--)
                        {
                            if (!ok)
                            {
                                if (arr[i] == '*')
                                {
                                    ok = true;
                                    last = i;
                                }
                            }
                            else
                            {
                                if (arr[i] != '*')
                                {
                                    ok = false;
                                    content = content.Remove(i + 1, last - i);
                                }
                                else if (i == 0 && arr[i] == '*')
                                {
                                    content = content.Remove(0, last + 1);
                                }
                            }
                        }
                        TempletContent = TempletContent.Replace("*", "");
                        if (string.Compare(content, TempletContent) == 0)
                        {
                            string ccccccccccccc = "";
                            //return true;
                        }
                    }
                
            
        }
    }
}