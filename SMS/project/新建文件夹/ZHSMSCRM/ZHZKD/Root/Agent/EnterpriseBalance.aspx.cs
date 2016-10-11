using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SMSModel;

namespace ZKD.Root.Agent
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
            string code = Request.QueryString["AccountID"];
            RPCResult<Model.EnterpriseUser> r = ZHSMSProxy.GetZKD().GetEnterprise(code);
            if (r.Success)
            {
                Model.EnterpriseUser user = r.Value;
                if (user != null)
                {
                    lbl_account.Text = user.AccountCode;
                    SMSModel.RPCResult<Model.UserBalance> rr = ZHSMSProxy.GetZKD().GetBalance(user.AccountCode, user.Password);
                    if (r.Success)
                    {
                        lbl_smsCount.Text = rr.Value.SmsBalance.ToString();
                    }
                }
            }
        }
    }
}