using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.Report
{
    public partial class EnterpriseSMSStatistics : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                string permissionValue = Request.QueryString["type"] + "Enterprise.Manage.enterpriseSMSStatistics";
                bool ok = BLL.Permission.IsUsePermission(account.UserCode, permissionValue);
                if (!ok)
                {
                    Message.Alert(this, "无权限", "null");
                    btn_n.Visible = false;
                    return;
                }
            }
            if (!IsPostBack)
            {
                txt_Start.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd HH:mm:ss");
                txt_End.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                load();
            }
        }
        void load()
        {
            string code = Request.QueryString["AccountID"];
            DataTable dt = CreateTable();
            SMSModel.RPCResult<string[,]> r = ZHSMSProxy.GetZHSMSPlatService().GetAccountSMSStatistics(code, DateTime.Parse(txt_Start.Text), DateTime.Parse(txt_End.Text));
            if (r.Success)
            {
                if (r.Value != null && r.Value.Length > 0)
                {
                    string[,] str = r.Value;
                    long sendcount = str[0, 0] == "" ? 0 : long.Parse(str[0, 0]);
                    long failurecount = str[0, 1] == "" ? 0 : long.Parse(str[0, 1]);

                    DataRow dr = dt.NewRow();
                    dr["SendCount"] = sendcount;
                    dr["SuccessCount"] = (sendcount - failurecount).ToString();
                    dr["FailureCount"] = failurecount;
                    if (sendcount == 0)
                    {
                        dr["SuccessRate"] = "没有短信发送";
                    }
                    else
                    {
                        dr["SuccessRate"] = Decimal.Round(((decimal)(sendcount - failurecount) / sendcount) * 100, 2) + "%";
                    }
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                Message.Alert(this, r.Message, "null");
            }
            GridView1.DataSource = dt;
            GridView1.DataBind();

        }
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
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
            table.Columns.Add("SendCount", Type.GetType("System.String"));
            table.Columns.Add("SuccessCount", Type.GetType("System.String"));
            table.Columns.Add("FailureCount", Type.GetType("System.String"));
            table.Columns.Add("SuccessRate", Type.GetType("System.String"));
            return table;
        }

        protected void btn_n_Click(object sender, EventArgs e)
        {
            try
            {
                if (txt_Start.Text == "" || txt_End.Text == "")
                {
                    Message.Alert(this, "时间不能为空", "null");
                    return;
                }
                if (DateTime.Compare(DateTime.Parse(txt_Start.Text), DateTime.Parse(txt_End.Text)) > 0)
                {
                    Message.Alert(this, "开始时间应小于结束时间", "null");
                    return;
                }
            }
            catch
            {
                Message.Alert(this, "输入的不是时间", "null");
                return;
            }
            load();
        }
    }
}