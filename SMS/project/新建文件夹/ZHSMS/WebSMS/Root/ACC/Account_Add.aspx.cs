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
    public partial class Account_Add : JudgeSession
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dd_Audit.SelectedIndex = 0;
                dd_Priority.SelectedIndex = 2;
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
            SMSModel.Account acc = new SMSModel.Account();
            acc.AccountID = txt_UserID.Text.ToString();
            acc.Audit = GetAccountAudit(dd_Audit.SelectedValue);
            string password = txt_Password.Text.ToString();
            acc.Password = tools.DESEncrypt.Encrypt(password);
            acc.Priority = GetAccountPriorityType(dd_Priority.SelectedValue);
            acc.RegisterDate = DateTime.Now;
            acc.SMSNumber = Convert.ToInt32(txt_SMSNumber.Text.ToString());
            acc.SPNumber = txt_SPNumber.Text.ToString();
            RPCResult r = PretreatmentProxy.GetPretreatment().CreateAccount(acc);
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