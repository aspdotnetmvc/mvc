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
    public partial class MO_List : System.Web.UI.Page
    {
        private const string loads = "EnterpriseSms.MOList.molist";
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
                txt_S.Text = DateTime.Now.AddDays(-1).ToString();
                txt_E.Text = DateTime.Now.ToString();
            }
        }
        private void load()
        {
            DataTable dt = CreateTable();
            DateTime starttime = Convert.ToDateTime(txt_S.Text);
            DateTime endtime = Convert.ToDateTime(txt_E.Text);
            string account = Request.QueryString["AccountID"];
            RPCResult<List<MOSMS>> rr = ZHSMSProxy.GetZHSMSPlatService().GetMOSMS(account, starttime, endtime);
            if (rr.Success)
            {
                lbl_message.Visible = false;
                Label1.Text = "当前已接收的短信有" + rr.Value.Count + "批次";
                var l = rr.Value.OrderByDescending(m => m.ReceiveTime);
                foreach (MOSMS s in l)
                {
                    DataRow dr = dt.NewRow();
                    dr["Gateway"] = s.Gateway;
                    dr["Message"] = s.Message;
                    dr["ReceiveTime"] = s.ReceiveTime;
                    dr["Serial"] = s.Serial;
                    dr["SPNumber"] = s.SPNumber;
                    dr["UserNumber"] = s.UserNumber;
                    dr["Service"] = s.Service;
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                Message.Alert(this, rr.Message, "null");
            }
         //   dt.DefaultView.Sort = "ReceiveTime desc";
            this.ViewState["MOSMS"] = dt;
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView1.Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
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
            DataTable dt = (DataTable)this.ViewState["MOSMS"];
            GridView1.DataSource = dt;
            GridView1.DataBind();
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
            DataTable dt = (DataTable)this.ViewState["MOSMS"];
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                bool ok = BLL.Permission.IsUsePermission(account.UserCode, loads);
                if (!ok)
                {
                    Message.Alert(this, "无权限", "null");
                    return;
                }
            }
            load();
        }

        public DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("Gateway", Type.GetType("System.String"));
            table.Columns.Add("Message", Type.GetType("System.String"));
            table.Columns.Add("ReceiveTime", Type.GetType("System.String"));
            table.Columns.Add("Serial", Type.GetType("System.String"));
            table.Columns.Add("SPNumber", Type.GetType("System.String"));
            table.Columns.Add("UserNumber", Type.GetType("System.String"));
            table.Columns.Add("Service", Type.GetType("System.String"));
            return table;
        }
        protected void btn_nn_Click(object sender, EventArgs e)
        {
            load();
        }
    }
}