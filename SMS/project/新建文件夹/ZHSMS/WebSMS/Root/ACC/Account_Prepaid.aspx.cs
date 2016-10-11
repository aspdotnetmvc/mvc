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
    public partial class Account_Prepaid : JudgeSession
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string AccountID = Request.QueryString["AccountID"];
                txt_AccountID.Text = AccountID;

            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }
            if (txt_cc.Text != "")
            {
                if (!IsNumeric(txt_cc.Text))
                {
                    Message.Alert(this, "请输入数字", "null");
                    return;
                }
            }
            uint d = uint.Parse(txt_cc.Text.ToString());
            RPCResult r = PretreatmentProxy.GetPretreatment().AccountPrepaid(txt_AccountID.Text,d, txt_AccountID.Text);
            if (r.Success)
            {
                Message.Success(this, r.Message, "null");

            }
            else
            {
                Message.Success(this, r.Message, "null");
            }
        }

        private bool IsNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[-]?\d+[.]?\d*$");

            if (reg1.IsMatch(str))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}