using SMSModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebSMS.Root.Keywords
{
    public partial class Keywords_List : JudgeSession
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                RPCResult<List<string>> r = PretreatmentProxy.GetPretreatment().GetKeyGroups();
                bool b = false;
                foreach (var v in r.Value)
                {
                    dd_KeyGroup.Items.Add(new ListItem(v, v));
                    if (!b)
                    {
                        serach(v); b = true;
                    }
                }
                dd_KeyGroup.Items.Insert(0, new ListItem("--请选择--", "-1"));
            }
        }
        private void serach(string group)
        {
            DataTable dt = CreateTable();
            RPCResult<List<string>> r = PretreatmentProxy.GetPretreatment().GetKeywords(group);
            if (r.Success)
            {
                lbl_message.Visible = false;
                List<string> ss = r.Value;
                if (ss.Count > 0)
                {
                    foreach (string s in ss)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Group"] = dd_KeyGroup.SelectedValue;
                        dr["Keywords"] = s;
                        dt.Rows.Add(dr);
                    }
                }
            }
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ((LinkButton)e.Row.Cells[3].Controls[0]).Attributes.Add("onclick", "return confirm('确认删除吗？');");
                string keyword = DataBinder.Eval(e.Row.DataItem, "Keywords").ToString();
                if (keyword == "")
                {
                    e.Row.Cells[3].Text = "";
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
            serach(dd_KeyGroup.SelectedValue);
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
            serach(dd_KeyGroup.SelectedValue);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (dd_KeyGroup.SelectedIndex == 0)
            {
                return;
            }
            serach(dd_KeyGroup.SelectedValue);
        }

        public DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("Group", Type.GetType("System.String"));
            table.Columns.Add("Keywords", Type.GetType("System.String"));
            return table;
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string group = this.GridView1.DataKeys[e.RowIndex][0].ToString();
            string keywords = this.GridView1.DataKeys[e.RowIndex][1].ToString();

            RPCResult r = PretreatmentProxy.GetPretreatment().DelKeywords(group, new List<string> { keywords });
            if (r.Success)
            {
                Message.Success(this, "删除成功", "null");
                serach(dd_KeyGroup.SelectedValue);
            }
            else
            {
                serach(dd_KeyGroup.SelectedValue);
                Message.Error(this, r.Message, "null");
            }
        }
    }
}