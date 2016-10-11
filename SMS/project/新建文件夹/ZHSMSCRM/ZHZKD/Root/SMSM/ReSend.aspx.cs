using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using SMSModel;
using System.IO;
using System.Data;


namespace ZKD.Root.SMSM
{
    public partial class ReSend : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            if (!IsPostBack)
            {
                load();
            }
        }
        private void load()
        {
            string SerialNumber = Request.QueryString["SerialNumber"].ToString();
            Guid c = Guid.Parse(SerialNumber);
            DataTable dt = this.CreateTable();
            Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];
            RPCResult<List<FailureSMS>> r = ZHSMSProxy.GetZKD().GetSMSByAuditFailure(user.AccountCode);
            if (r.Success)
            {
                List<FailureSMS> smss = r.Value;
                if (smss.Count > 0)
                {
                    foreach (SMS s in smss)
                    {
                        if (s.SerialNumber == c)
                        {
                            txt_Content.InnerText = s.Content;

                            txt_wapURL.Value = s.WapURL;
                            List<string> num = s.Number;
                            foreach (string a in num)
                            {
                                DataRow dr = dt.NewRow();
                                dr["phone"] = a;
                                dt.Rows.Add(dr);
                            }
                            if (dt.Rows.Count > 0)
                            {
                                CheckBoxList1.DataSource = dt;
                                CheckBoxList1.DataBind();
                            }
                            for (int x = 0; x < this.CheckBoxList1.Items.Count; x++)
                            {
                                CheckBoxList1.Items[x].Selected = true;
                            }
                        }
                    }
                }
            }
            else
            {
                Message.Alert(this, r.Message, "null");
                return;
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string SerialNumber = Request.QueryString["SerialNumber"].ToString();
            Guid c = Guid.Parse(SerialNumber);
            Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];
            if (!Page.IsValid)
            {
                return;
            }

            if (txt_Content.Value.ToString().Trim() == "")
            {
                Message.Alert(this, "请输入短信内容", "null");
                return;
            }

            SMSModel.SMS sms = new SMS();
            List<string> num = new List<string>();
            foreach (ListItem li in CheckBoxList1.Items)
            {
                if (li.Selected == true)
                {
                    num.Add(li.Text);
                }
            }
            if (num.Count == 0)
            {
                Message.Alert(this, "请选择电话号码", "null");
                return;
            }
            bool tt = false;
            if (txt_SendTime.Value != "")
            {
                sms.SMSTimer = Convert.ToDateTime(txt_SendTime.Value);
                tt = true;
            }
            RPCResult<Guid> r = ZHSMSProxy.GetZKD().SendSMS(user.AccountCode, user.Password, txt_Content.Value.ToString().Trim(), txt_wapURL.Value.ToString(), num, tt, sms.SMSTimer);
            if (r.Success)
            {
                sms.SerialNumber = r.Value;
                sms.Account = user.AccountCode;
                sms.Content = txt_Content.Value;
                sms.Number = num;
                sms.SendTime = DateTime.Now;
                sms.WapURL = txt_wapURL.Value;
                BLL.SMSdo.SMSAdd(sms);
                RPCResult rr = ZHSMSProxy.GetZKD().AffirmAuditFailureSMS(c);
                if (rr.Success)
                {
                    Message.Success(this, "发送成功", "null");
                }
                else
                {
                    Message.Error(this, rr.Message, "null");
                }
                Response.Redirect("AuditFailure.aspx");
            }
            else
            {
                Message.Error(this, r.Message, "null");
            }
        }



        private bool IsNumeric(string str)
        {

            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^0?(13[0-9]|15[012356789]|18[0-9]|14[57])[0-9]{8}$");
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


        private DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("phone", Type.GetType("System.String"));
            return table;

        }

    }
}