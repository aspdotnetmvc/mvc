using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZHSMSPlatform.Root.Finance
{
    public partial class FinanceDetails : System.Web.UI.Page
    {
        private const string financeDetails = "Finance.Index.Details";
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            Model.SysAccount account = (Model.SysAccount)Session["Login"];
            if (account.UserCode != "admin")
            {
                bool ok = BLL.Permission.IsUsePermission(account.UserCode, financeDetails);
                if (!ok)
                {
                    Message.Alert(this, "无权限", "null");
                    btnFind.Enabled = false;
                    return;
                }
            }
            if (!IsPostBack)
            {
               //load();
            }
        }
        void load()
        {
            string accountName = txt_name.Text.Trim();
            long smsCount = 0;
            long sendCount = 0;
            long remainSmsCount = 0;
            decimal money = 0;
            string account = Request.QueryString["AccountID"];
            DataTable dt = CreateTable();
            SMSModel.RPCResult<List<Model.ChargeStatics>> r = ZHSMSProxy.GetZHSMSPlatService().GetChargeStatics(accountName);
            if (r.Success)
            {
                foreach (var v in r.Value)
                {
                    DataRow dr = dt.NewRow();
                    dr["accountCode"] = v.Enterprese;
                    dr["smsCount"] = v.SMSCount;
                    dr["moneny"] = v.TotalMoney;
                    dr["sendCount"] = v.SendCount;
                    dr["remainCount"] = v.RemainSMSNumber;
                    dt.Rows.Add(dr);
                    smsCount += v.SMSCount;
                    sendCount += v.SendCount;
                    remainSmsCount += v.RemainSMSNumber;
                    money += v.TotalMoney;
                }
            }
            else
            {
                Message.Alert(this, r.Message, "null");
            }
            lbl_money.Text = money.ToString();
            lbl_remainSMSCount.Text = remainSmsCount.ToString();
            lbl_sendSMSCount.Text = sendCount.ToString();
            lbl_smsCount.Text = smsCount.ToString();
            GridView1.DataSource = dt;
            GridView1.DataBind();

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
                DataTable dt = (DataTable)this.ViewState["staticsTable"];
                
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

        public DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("accountCode", Type.GetType("System.String"));
            table.Columns.Add("moneny", Type.GetType("System.String"));
            table.Columns.Add("smsCount", Type.GetType("System.String"));
            table.Columns.Add("sendCount", Type.GetType("System.String"));
            table.Columns.Add("remainCount", Type.GetType("System.String"));
            return table;
        }

        protected void btnFind_Click(object sender, EventArgs e)
        {
            load();
        }
    }
}