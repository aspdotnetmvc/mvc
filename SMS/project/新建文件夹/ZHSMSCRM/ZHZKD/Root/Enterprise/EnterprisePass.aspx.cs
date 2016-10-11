using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SMSModel;

namespace ZKD.Root.Enterprise
{
    public partial class EnterprisePass : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.EnterpriseUser account = (Model.EnterpriseUser)Session["Login"];
            if (!IsPostBack)
            {
                lbl_account.Text = account.AccountCode;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Model.EnterpriseUser account = (Model.EnterpriseUser)Session["Login"];
            if (!IsPassword(txt_new.Text))
            {
                Message.Alert(this, "密码必须是以字母开头，长度在6~18之间，只能包含字母、数字和下划线", "null");
                return;
            }
            if (!txt_new.Text.Equals(txt_Pass2.Text))
            {
                Message.Alert(this, "确认密码与新密码不一致，请重新输入", "null");
                return;
            }
          
            RPCResult rr = ZHSMSProxy.GetZKD().ChangeEnterprisePass(account.AccountCode, txt_Pass.Text.ToString(), txt_new.Text.ToString());
            if (rr.Success)
            {
                System.Web.HttpContext.Current.Response.Redirect("../../login.aspx", true);
            }
            else
            {
                Message.Alert(this, rr.Message, "null");
            }
        }

        private bool IsPassword(string str)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z]\w{5,17}$");
            if (reg1.IsMatch(str))
            {
                //数字
                return true;
            }
            else
            {
                //非数字
                return false;
            }
        }
    }
}