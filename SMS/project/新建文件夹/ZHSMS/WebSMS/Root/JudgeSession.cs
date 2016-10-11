using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSMS.Root
{
    public class JudgeSession : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            if (Session["AccountID"] == null)
            {
                Response.Write("session过期!");
                Response.Redirect("../Login.aspx");
            }
            else
            {
                //Response.Write("session没有过期，用户名：" + Session["AccountID"].ToString());
            }
        }
    }
}