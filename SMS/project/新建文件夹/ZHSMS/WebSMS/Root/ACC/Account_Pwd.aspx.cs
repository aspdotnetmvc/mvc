using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Xml.Linq;
using System.Reflection;
using SMSModel;
namespace WebSMS.Root.ACC
{
    public partial class Account_Pwd : JudgeSession
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

            try
            {

                string newps = tools.DESEncrypt.Encrypt(txt_newpwd.Text.Trim().ToString());

                string old = tools.DESEncrypt.Encrypt(txt_oldpwd.Text.Trim().ToString());

                RPCResult rr = PretreatmentProxy.GetPretreatment().ChangeAccountPassword(txt_AccountID.Text.Trim().ToString(), old, newps);
                if (rr.Success)
                {
                    Message.Success(this, rr.Message, "null");
                }
                else
                {
                    Message.Error(this, rr.Message, "null");
                }
            }
            catch
            {
            }
        }
    }
}