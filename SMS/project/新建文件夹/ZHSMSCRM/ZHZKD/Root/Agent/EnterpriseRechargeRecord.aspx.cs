using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SMSModel;
namespace ZKD.Root.Agent
{
    public partial class EnterpriseRechargeRecord : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();

            if (!IsPostBack)
            {
                txt_startTime.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd HH:mm:ss");
                txt_endTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                load();
            }
        }
        void load()
        {
            string accountID = Request.QueryString["AccountID"];
            RPCResult<List<Model.ChargeRecord>> r = ZHSMSProxy.GetZKD().GetEnterpriseChargeRecord(accountID, DateTime.Parse(txt_startTime.Text), DateTime.Parse(txt_endTime.Text));
            DataTable dt = CreateTable();
            if (r.Success)
            {
                List<Model.ChargeRecord> list = r.Value;
                foreach (var v in list)
                {
                    DataRow dr = dt.NewRow();
                    dr["operatorAccount"] = v.OperatorAccount;
                    dr["accountCode"] = v.PrepaidAccount;
                    dr["number"] = v.SMSCount;
                    dr["moneny"] = v.Money;
                    dr["dateTime"] = v.PrepaidTime;
                    dr["rate"] = v.ThenRate;
                    dr["type"] = v.PrepaidType == 0 ? "金额充值" : "短信条数充值";
                    dr["Remark"] = v.Remark;
                    dt.Rows.Add(dr);
                }
                dt.DefaultView.Sort = "dateTime desc";
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
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
            try
            {
                if (txt_startTime.Text == "" || txt_endTime.Text == "")
                {
                    Message.Alert(this, "时间不能为空", "null");
                    return;
                }
                if (DateTime.Compare(DateTime.Parse(txt_startTime.Text), DateTime.Parse(txt_endTime.Text)) > 0)
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

        public DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("operatorAccount", Type.GetType("System.String"));
            table.Columns.Add("accountCode", Type.GetType("System.String"));
            table.Columns.Add("number", Type.GetType("System.String"));
            table.Columns.Add("moneny", Type.GetType("System.String"));
            table.Columns.Add("dateTime", Type.GetType("System.String"));
            table.Columns.Add("rate", Type.GetType("System.String"));
            table.Columns.Add("type", Type.GetType("System.String"));
            table.Columns.Add("Remark", Type.GetType("System.String"));
            return table;
        }
    }
}