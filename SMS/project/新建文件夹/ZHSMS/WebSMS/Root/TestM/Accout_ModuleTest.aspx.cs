using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

using System.IO;
using System.Data;
using SMSModel;

namespace WebSMS.Root.TestM
{
    public partial class Accout_ModuleTest : System.Web.UI.Page
    {
        int c = 100;
        DataTable dt = new DataTable();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {


            }
        }
        public DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("AccountID", Type.GetType("System.String"));
            return table;
        }

        /// <summary>
        /// 创建账号
        /// </summary>
        public string CreateAccount()
        {
            SMSModel.Account acc = new Account();
            acc.AccountID = Guid.NewGuid().ToString();
            acc.Audit = AccountAuditType.Auto;
            acc.Password = "123";
            acc.Priority = AccountPriorityType.Level2;
            acc.RegisterDate = DateTime.Now;
            acc.SMSNumber = 10000;
            RPCResult r = PretreatmentProxy.GetPretreatment().CreateAccount(acc);
            return acc.AccountID;
        }
        public void ChangeAccountAudit()
        {
            //PretreatmentProxy.GetPretreatment().GetAccounts();
            //ushort tt = 4;
            //RPCResult r = PretreatmentProxy.GetPretreatment().ChangeAccountAudit(txt_AccountID.Text, 123, tt);

        }
        private int pd()
        {
            if (c == 0 && c < 0)
            {
                Message.Alert(this, "不为空");
            }
            string d = txt_Magnitude.Text;
            c = Int32.Parse(d);
            return c;
        }

        protected void bt1_Click(object sender, EventArgs e)
        {
            dt = CreateTable();
            long bt = DateTime.Now.Ticks;
            c = pd();
            for (int i = 0; i < c; i++)
            {
                DataRow dr = dt.NewRow();
                dr["AccountID"] = CreateAccount();
                dt.Rows.Add(dr);
                Session["dt"] = dt;

            }
            Message.Success(this, " 创建账号,耗时" + (decimal)((DateTime.Now.Ticks - bt) / 10000000) + "秒", "null");


        }

        protected void bt2_Click(object sender, EventArgs e)
        {

            long bt = DateTime.Now.Ticks;
            c = pd();
            for (int i = 0; i < c; i++)
            {
                CreateAccount();
            }
            Message.Success(this, " 创建账号,耗时" + (decimal)((DateTime.Now.Ticks - bt) / 10000000) + "秒", "null");

        }

        protected void bt3_Click(object sender, EventArgs e)
        {

            long bt = DateTime.Now.Ticks;
            c = pd();
            for (int i = 0; i < c; i++)
            {
                CreateAccount();
            }
            Message.Success(this, " 创建账号,耗时" + (decimal)((DateTime.Now.Ticks - bt) / 10000000) + "秒", "null");

        }

        protected void bt4_Click(object sender, EventArgs e)
        {
            long bt = DateTime.Now.Ticks;
            c = pd();
            for (int i = 0; i < c; i++)
            {
                CreateAccount();
            }
            Message.Success(this, " 创建账号,耗时" + (decimal)((DateTime.Now.Ticks - bt) / 10000000) + "秒", "null");


        }
        private bool IsNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[-]?\d+[.]?\d*$");

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

        protected void bt5_Click(object sender, EventArgs e)
        {
            long bt = DateTime.Now.Ticks;
            c = pd();
            for (int i = 0; i < c; i++)
            {
                CreateAccount();
            }
            Message.Success(this, " 创建账号,耗时" + (decimal)((DateTime.Now.Ticks - bt) / 10000000) + "秒", "null");
        }
    }
}