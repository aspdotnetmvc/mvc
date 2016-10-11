using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZKD.Root.Agent
{
    public partial class EnterpriseRecharge : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            if (!IsPostBack)
            {
                lbl_account.Text = Request.QueryString["AccountID"];
                Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];
                lbl_fromAccount.Text = user.AccountCode;
                SMSModel.RPCResult<Model.UserBalance> r = ZHSMSProxy.GetZKD().GetBalance(user.AccountCode, user.Password);
                if (r.Success)
                {
                    lbl_remain.Text = r.Value.SmsBalance.ToString();
                }
                else
                {
                    lbl_remain.Text = "0";
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (txt_sms.Text.Trim() == "")
            {
                Message.Alert(this, "请输入充值短信数", "null");
                return;
            }
            if (txt_SMSRate.Text.Trim() == "")
            {
                Message.Alert(this, "请输入每条短信的费率", "null");
                return;
            }
            //if (txt_pass.Text.Trim() == "")
            //{
            //    Message.Alert(this, "请输入支付密码", "null");
            //    return;
            //}
            int count = 0;
            decimal rate = 0;
            try
            {
                count = int.Parse(txt_sms.Text.Trim());
                rate = decimal.Parse(txt_SMSRate.Text.Trim());
            }
            catch
            {
                Message.Alert(this, "请输入数字", "null");
                return;
            }
            SMSModel.RPCResult r = ZHSMSProxy.GetZKD().LowerAccountPrepaidByAgent(((Model.EnterpriseUser)Session["Login"]).AccountCode, ((Model.EnterpriseUser)Session["Login"]).Password, lbl_account.Text, count, rate);
            if (r.Success)
            {
                Message.Alert(this, "充值成功", "null");
            }
            else
            {
                Message.Alert(this, r.Message, "null");
            }
        }
    }
}