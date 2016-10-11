using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZKD.Root.Enterprise
{
    public partial class EnterpriseBalance : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            if (!IsPostBack)
            {
                load();
            }
        }
        void load()
        {
            Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];
            if (user.AccountID != null)
            {
                lbl_account.Text = user.AccountCode;
                SMSModel.RPCResult<Model.UserBalance> r = ZHSMSProxy.GetZKD().GetBalance(user.AccountCode, user.Password);
                if (r.Success)
                {
                    lbl_smsCount.Text = r.Value.SmsBalance.ToString();
                }
                else
                {
                    Message.Alert(this, r.Message, "null");
                    return;

                }
            }
        }
    }
}