using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Xml.Linq;
using SMSModel;

namespace WebSMS.Root.ACC
{
    public partial class Account_List : JudgeSession
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                load();
            }
        }
        private void load()
        {
            DataTable dt = CreateTable();
            RPCResult<Account> r = PretreatmentProxy.GetPretreatment().GetAccount(Session["AccountID"].ToString());
            if (r.Success)
            {
                if (r.Value!= null)
                {
                    Account a = r.Value;
                    DataRow dr = dt.NewRow();
                    dr["A"] = a.AccountID;
                    dr["Audit"] = GetAudit(a.Audit);
                    dr["Password"] = a.Password;
                    dr["Priority"] = a.Priority;
                    dr["RegisterDate"] = a.RegisterDate;
                    dr["SMSNumber"] = a.SMSNumber;
                    dr["Enabled"] = a.Enabled == true ? "是" : "否";
                    dr["SPNumber"] = a.SPNumber;
                    dt.Rows.Add(dr);
                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                    Session["dt"] = dt;
                    lbl_message.Visible = false;
                }
            }
        }
        string GetAudit(AccountAuditType Audit)
        {
            string str = "";
            switch (Audit)
            {
                case  SMSModel.AccountAuditType.Auto:
                    str = "自动审核";
                    break;
                case SMSModel.AccountAuditType.Manual:
                    str = "人工审核";
                    break;
            }
            return str;
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string Enabled = DataBinder.Eval(e.Row.DataItem, "Enabled").ToString();
                switch (Enabled)
                {
                    case "是":
                        (e.Row.Cells[8].FindControl("btn_start") as Button).Enabled = false;
                        (e.Row.Cells[8].FindControl("btn_stop") as Button).Enabled = true;
                        break;
                    case "否":
                        (e.Row.Cells[8].FindControl("btn_start") as Button).Enabled = true;
                        (e.Row.Cells[8].FindControl("btn_stop") as Button).Enabled = false;
                        break;
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
            table.Columns.Add("A", Type.GetType("System.String"));
            table.Columns.Add("Password", Type.GetType("System.String"));
            table.Columns.Add("Priority", Type.GetType("System.String"));
            table.Columns.Add("SMSNumber", Type.GetType("System.String"));
            table.Columns.Add("RegisterDate", Type.GetType("System.String"));
            table.Columns.Add("Audit", Type.GetType("System.String"));
            table.Columns.Add("Enabled", Type.GetType("System.String"));
            table.Columns.Add("SPNumber", Type.GetType("System.String"));
            return table;
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string AccountID = e.CommandArgument.ToString();
            if (e.CommandName == "start")
            {
                RPCResult r = PretreatmentProxy.GetPretreatment().SetAccountEnable(AccountID, true);
                if (r.Success)
                {
                    DataTable dt = (DataTable)Session["dt"];
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["A"].ToString() == AccountID)
                        {
                            dr["Enabled"] = "是";
                            break;
                        }
                    }
                    load();
                    Message.Success(this, r.Message, "null");
                }
                else
                {
                    load();
                    Message.Error(this, r.Message, "null");
                }
            }
            else if (e.CommandName == "stop")
            {
                RPCResult r = PretreatmentProxy.GetPretreatment().SetAccountEnable(AccountID, false);
                if (r.Success)
                {
                    DataTable dt = (DataTable)Session["dt"];
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["A"].ToString() == AccountID)
                        {
                            dr["Enabled"] = "否";
                            break;
                        }
                    }
                    load();
                    Message.Success(this, r.Message, "null");
                }
                else
                {
                    load();
                    Message.Success(this, r.Message, "null");
                }
            }
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string a = this.GridView1.DataKeys[e.RowIndex].Value.ToString();
            RPCResult r = PretreatmentProxy.GetPretreatment().DelAccount(a);
            if (r.Success)
            {
                Message.Success(this, r.Message, "null");
                //   load();
            }
            else
            {
                load();
                Message.Error(this, r.Message, "null");
            }
        }
    }
}