using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SMSModel;

namespace ZHSMSPlatform.Root.SMSM
{
    public partial class SMS_List : System.Web.UI.Page
    {
        private const string del = "EnterpriseSms.List.list";
        private const string suc = "EnterpriseSms.List.level";
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                this.ViewState["Permissions"] = BLL.Permission.GetPermissionByAccount(account.UserCode);
            }
            if (!IsPostBack)
            {
                load();
            }

        }
        private void load()
        {
            DataTable dt = CreateTable();
            string account = Request.QueryString["AccountID"];
            RPCResult<int> i = ZHSMSProxy.GetZHSMSPlatService().GetSMSCountByAccount(account);
            if (i.Success)
            {
                lbl_message.Visible = false;
                Label1.Text = "已提交但未发送的短信有" + i.Value + "批次";
            }
            RPCResult<List<SMS>> r = ZHSMSProxy.GetZHSMSPlatService().GetSMSByAccount(account);
            if (r.Success)
            {
                var l = r.Value.OrderByDescending(s => s.SendTime);
                foreach (SMS s in l)
                {
                    DataRow dr = dt.NewRow();
                    dr["AccountID"] = s.Account;
                    dr["Audit"] = GetAudit(s.Audit);
                    dr["SMSContent"] = s.Content;
                    dr["ContentFilter"] = GetContentFilter(s.Filter);
                    dr["Level"] = s.Level;
                    dr["SendTime"] = s.SendTime;
                    dr["SerialNumber"] = s.SerialNumber;
                    dr["StatusReport"] = GetStatusReport(s.StatusReport);
                    dr["Signature"] = s.Signature;
                    List<string> num = s.Number;
                    string str = "";
                    foreach (string st in num)
                    {
                        str += st + ",";
                    }
                    dr["Number"] = str == "" ? "" : str.Substring(0, str.Length - 1);
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                Message.Alert(this, r.Message, "null");
            }
          //  dt.DefaultView.Sort = "SendTime desc";
            GridView1.DataSource = dt;
            GridView1.DataBind();
            Session["dt"] = dt;
        }
        string GetStatusReport(StatusReportType a)
        {
            string str = "";
            switch (a)
            {
                case StatusReportType.Enabled:
                    str = "发送";
                    break;
                case StatusReportType.Disable:
                    str = "不发送";
                    break;
                case StatusReportType.Push:
                    str = "推送";
                    break;
            }
            return str;
        }
        string GetContentFilter(FilterType a)
        {
            string str = "";
            switch (a)
            {
                case FilterType.NoOperation:
                    str = "无操作";
                    break;
                case FilterType.Failure:
                    str = "发送失败";
                    break;
                case FilterType.Replace:
                    str = "替换";
                    break;
            }
            return str;
        }
        string GetAudit(AuditType a)
        {
            string str = "企业鉴权";
            switch (a)
            {
                case AuditType.Auto:
                    str = "自动审核";
                    break;
                case AuditType.Manual:
                    str = "人工审核";
                    break;

            }
            return str;
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ((LinkButton)e.Row.Cells[10].Controls[0]).Attributes.Add("onclick", "return confirm('确认删除吗？');");
                Model.SysAccount account = (Model.SysAccount)Session["Login"];
                if (account.UserCode != "admin")
                {
                    List<string> permissions = (List<string>)this.ViewState["Permissions"];
                    if (!permissions.Contains(del))
                    {
                        e.Row.Cells[10].Text = "无权限";
                    }
                }
            }
        }

        public int CurrentPage
        {
            get
            {
                if (ViewState["CurrentPage"] == null)
                    return 0;
                else
                    return (int)ViewState["CurrentPage"];
            }
            set
            {
                ViewState["CurrentPage"] = value;
            }
        }
        protected void PageDropDownList_SelectedIndexChanged(Object sender, EventArgs e)
        {
            GridViewRow pagerRow = GridView1.BottomPagerRow;
            DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
            GridView1.PageIndex = pageList.SelectedIndex;
            this.CurrentPage = pageList.SelectedIndex;
            load();
        }
        protected void GridView1_DataBound(object sender, EventArgs e)
        {
            try
            {
                GridViewRow pagerRow = GridView1.BottomPagerRow;
                LinkButton linkBtnFirst = (LinkButton)pagerRow.Cells[0].FindControl("linkBtnFirst");
                LinkButton linkBtnPrev = (LinkButton)pagerRow.Cells[0].FindControl("linkBtnPrev");
                LinkButton linkBtnNext = (LinkButton)pagerRow.Cells[0].FindControl("linkBtnNext");
                LinkButton linkBtnLast = (LinkButton)pagerRow.Cells[0].FindControl("linkBtnLast");
                if (GridView1.PageIndex == 0)
                {
                    linkBtnFirst.Enabled = false;
                    linkBtnPrev.Enabled = false;
                }
                else if (GridView1.PageIndex == GridView1.PageCount - 1)
                {
                    linkBtnLast.Enabled = false;
                    linkBtnNext.Enabled = false;
                }
                else if (GridView1.PageCount <= 0)
                {
                    linkBtnFirst.Enabled = false;
                    linkBtnPrev.Enabled = false;
                    linkBtnNext.Enabled = false;
                    linkBtnLast.Enabled = false;
                }
                DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
                Label pageLabel = (Label)pagerRow.Cells[0].FindControl("CurrentPageLabel");
                if (pageList != null)
                {
                    for (int i = 0; i < GridView1.PageCount; i++)
                    {
                        int pageNumber = i + 1;
                        ListItem item = new ListItem(pageNumber.ToString() + "/" + GridView1.PageCount.ToString(), pageNumber.ToString());
                        if (i == GridView1.PageIndex)
                        {
                            item.Selected = true;
                        }
                        pageList.Items.Add(item);
                    }
                }
                if (pageLabel != null)
                {
                    int currentPage = GridView1.PageIndex + 1;
                    pageLabel.Text = "当前页： " + currentPage.ToString() +
                      " / " + GridView1.PageCount.ToString();
                }
            }
            catch
            {
            }
        }
        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            load();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            load();
        }

        public DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("AccountID", Type.GetType("System.String"));
            table.Columns.Add("SerialNumber", Type.GetType("System.String"));
            table.Columns.Add("SMSContent", Type.GetType("System.String"));
            table.Columns.Add("StatusReport", Type.GetType("System.String"));
            table.Columns.Add("Level", Type.GetType("System.String"));
            table.Columns.Add("SendTime", Type.GetType("System.String"));
            table.Columns.Add("Audit", Type.GetType("System.String"));
            table.Columns.Add("ContentFilter", Type.GetType("System.String"));
            table.Columns.Add("AuditTime", Type.GetType("System.String"));
            table.Columns.Add("Number", Type.GetType("System.String"));
            table.Columns.Add("BussType", Type.GetType("System.String"));
            table.Columns.Add("Signature", Type.GetType("System.String"));
            return table;
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                bool ok = BLL.Permission.IsUsePermission(account.UserCode, del);
                if (!ok)
                {
                    Message.Alert(this, "无权限", "null");
                    return;
                }
            }
            string serialNumber = this.GridView1.DataKeys[e.RowIndex].Value.ToString();
            RPCResult r = ZHSMSProxy.GetZHSMSPlatService().DelSMS(new List<Guid> { new Guid(serialNumber) });
            if (r.Success)
            {
                Message.Success(this, "操作成功", "null");
                load();
            }
            else
            {
                Message.Error(this, r.Message, "null");
            }
        }

        protected void bt_n_Click(object sender, EventArgs e)
        {
            CheckBoxAll.Checked = false;
            CheckBox1.Checked = false;
            for (int i = 0; i <= GridView1.Rows.Count - 1; i++)
            {
                CheckBox CheckBox = (CheckBox)GridView1.Rows[i].FindControl("CheckBox");
                CheckBox.Checked = false;
            }
        }

        protected void bt1_Click(object sender, EventArgs e)
        {
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                bool ok = BLL.Permission.IsUsePermission(account.UserCode, suc);
                if (!ok)
                {
                    bt1.Visible = false;
                    Message.Alert(this, "无权限", "null");
                    return;
                }
            }
            for (int i = 0; i <= GridView1.Rows.Count - 1; i++)
            {
                CheckBox CheckBox = (CheckBox)GridView1.Rows[i].FindControl("CheckBox");
                if (CheckBox.Checked == true)
                {
                    string serialNumber = GridView1.DataKeys[i].Value.ToString();
                    RPCResult r = ZHSMSProxy.GetZHSMSPlatService().SetSMSLevel(Guid.Parse(serialNumber), (SMSModel.LevelType)(int.Parse(dd_l.SelectedItem.Text)), account.UserCode);
                }
            }
            CheckBoxAll.Checked = false;
            CheckBox1.Checked = false;
            load();
            Message.Success(this, " 短信优先级调整成功", "null");

        }

        protected void CheckBoxAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i <= GridView1.Rows.Count - 1; i++)
            {
                CheckBox CheckBox = (CheckBox)GridView1.Rows[i].FindControl("CheckBox");
                if (CheckBoxAll.Checked == true)
                {
                    CheckBox.Checked = true;
                }
                else
                {
                    CheckBox.Checked = false;
                }
            }
            CheckBox1.Checked = false;
        }

        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i <= GridView1.Rows.Count - 1; i++)
            {
                CheckBox CheckBox = (CheckBox)GridView1.Rows[i].FindControl("CheckBox");
                if (CheckBox.Checked == false)
                {
                    CheckBox.Checked = true;
                }
                else
                {
                    CheckBox.Checked = false;
                }
            }
            CheckBoxAll.Checked = false;
        }
    }
}