using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using SMSModel;
namespace WebSMS.Root.ACC
{
    public partial class SMS_Audit : JudgeSession
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string AccountID = Request.QueryString["AccountID"];
                txt_AccountID.Text = AccountID;
                dd_Audit.SelectedIndex = 0;
            }
        }

        AccountAuditType GetAccountAudit(string a)
        {
            AccountAuditType d = AccountAuditType.Manual;
            switch (a)
            {
                case "1":
                    d = AccountAuditType.Auto;
                    break;
                case "0":
                    d = AccountAuditType.Manual;
                    break;

            }
            return d;
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }
            string d = dd_Audit.SelectedItem.Value;
            RPCResult r = PretreatmentProxy.GetPretreatment().ChangeAccountAudit(txt_AccountID.Text, Session["password"].ToString(), GetAccountAudit(d));
            if (r.Success)
            {
                Message.Success(this, r.Message, "null");

            }
            else
            {
                Message.Success(this, r.Message, "null");
            }
        }
    }
}