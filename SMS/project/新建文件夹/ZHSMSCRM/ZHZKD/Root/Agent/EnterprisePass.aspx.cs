using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SMSModel;

namespace ZKD.Root.Agent
{
    public partial class EnterprisePass : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            if (!IsPostBack)
            {
                lbl_account.Text = Request.QueryString["AccountID"];
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string account = Request.QueryString["AccountID"];

            RPCResult r = ZHSMSProxy.GetZKD().ResetEnterprisePass(account, "666666");
            if (r.Success)
            {
                Message.Success(this, "操作成功", "null");
            }
            else
            {
                Message.Alert(this, r.Message, "null");
            }
        }
    }
}