using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common;
using SMSModel;
using BLL;

using System.Text;
using System.Text.RegularExpressions;

namespace ZKD
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                num.Value = Utils.GetCookie("smsname");
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string userName = num.Value.Trim();
            string userPwd = psw.Value.Trim();
            string codes = code.Value.Trim();

            if (userName.Equals("") || userPwd.Equals(""))
            {
                lblTip.Visible = true;
                lblTip.Text = "请输入用户名或密码";
                return;
            }
            if (codes.Equals(""))
            {
                lblTip.Visible = true;
                lblTip.Text = "请输入验证码";
                return;
            }
            if (Session[DTKeys.SESSION_CODE] == null)
            {
                lblTip.Visible = true;
                lblTip.Text = "系统找不到验证码";
                return;
            }
            if (codes.ToLower() != Session[DTKeys.SESSION_CODE].ToString().ToLower())
            {
                lblTip.Visible = true;
                lblTip.Text = "验证码输入不正确";
                return;

            }
          
            string r = BLL.Login.Logon(userName, userPwd);
            if (r == "3")
            {
                lblTip.Visible = true;
                lblTip.Text = "用户被禁用";
                return;
            }
            else
            {
                if (r == "1")
                {
                    if (cbRememberId.Checked)
                    {
                        Utils.WriteCookie("smsname", userName, 14400);
                    }
                    else
                    {
                        Utils.WriteCookie("smsname", userName, -14400);
                    }
                    Response.Redirect("Root/index.aspx");
                    return;
                }
                else
                {
                    lblTip.Visible = true;
                    lblTip.Text = "用户名或密码错误";
                    return;
                }
            }
        }
    }
}