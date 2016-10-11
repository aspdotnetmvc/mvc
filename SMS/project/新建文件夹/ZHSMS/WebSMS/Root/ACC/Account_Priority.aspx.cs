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
    public partial class Account_Priority : JudgeSession
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string AccountID = Request.QueryString["AccountID"];
                txt_AccountID.Text = AccountID;
                dd_Level.SelectedIndex = 2;
            }
        }
        AccountPriorityType GetAccountPriorityType(string a)
        {
            AccountPriorityType d = AccountPriorityType.Level2;
            switch (a)
            {
                case "0":
                    d = AccountPriorityType.Level0;
                    break;
                case "1":
                    d = AccountPriorityType.Level1;
                    break;
                case "2":
                    d = AccountPriorityType.Level2;
                    break;
                case "3":
                    d = AccountPriorityType.Level3;
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

            string d = dd_Level.SelectedValue;
            RPCResult r = PretreatmentProxy.GetPretreatment().ChangeAccountPriority(txt_AccountID.Text, Session["password"].ToString(), GetAccountPriorityType(d));
            if (r.Success)
            {
                Message.Success(this, r.Message, "null");

            }
            else
            {
                Message.Error(this, r.Message, "null");
            }
        }
    }
}