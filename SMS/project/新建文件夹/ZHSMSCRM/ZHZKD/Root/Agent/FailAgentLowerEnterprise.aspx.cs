using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZKD.Root.Agent
{
    public partial class FailAgentLowerEnterprise : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            
            if (!IsPostBack)
            {
                //pageBottom(0, 0);
                search(0);
            }
        }


        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ((LinkButton)e.Row.Cells[10].Controls[0]).Attributes.Add("onclick", "return confirm('确认删除吗？');");
            }
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string account = this.GridView1.DataKeys[e.RowIndex].Value.ToString();
            
            SMSModel.RPCResult r = ZHSMSProxy.GetZKD().DelEnterprise(account);
            if (r.Success)
            {
                search(this.CurrentPage);
                Message.Success(this, "删除成功", "null");
            }
            else
            {
                Message.Error(this, r.Message, "null");
            }
        }


        protected void btn_nn_Click(object sender, EventArgs e)
        {
            search(0);
        }

        private void search(int pageIndex)
        {
            Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];
            DataTable dt = CreateTable();
            SMSModel.RPCResult<List<Model.AuditEnterprise>> r = ZHSMSProxy.GetZKD().GetFailueOrUnAuditEnterprises(user.AccountCode);
            if (r.Success)
            {
                List<Model.AuditEnterprise> list = r.Value;
                pageBottom(list.Count, pageIndex);
                int startIndex = pageIndex * GridView1.PageSize;
                if (startIndex != 0)
                {
                    list = list.Skip(startIndex).ToList();
                }
                list = list.Take(GridView1.PageSize).ToList();
                foreach (var s in list)
                {
                    DataRow dr = dt.NewRow();
                    dr["EnterpriseCode"] = s.EnterpriseCode;
                    dr["EnterpriseName"] = s.EnterpriseName;
                    dr["CreateTime"] = s.CreateTime;
                    dr["AuditTime"] = s.AuditTime;
                    dr["AuditResult"] = s.AuditResult==true?"已审":"未审";
                    dr["Auditor"] = s.Auditor;
                    dr["EnterpriseResult"] = s.EnterpriseResult==true?"通过":"未通过";
                    dr["AuditRemark"] = s.AuditRemark;
                    dt.Rows.Add(dr);
                }

            }
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        public DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", Type.GetType("System.Int32"));
            table.Columns[0].AutoIncrement = true;
            table.Columns[0].AutoIncrementSeed = 1;
            table.Columns[0].AutoIncrementStep = 1;
            table.Columns.Add("EnterpriseCode", Type.GetType("System.String"));
            table.Columns.Add("EnterpriseName", Type.GetType("System.String"));
            table.Columns.Add("CreateTime", Type.GetType("System.String"));
            table.Columns.Add("AuditTime", Type.GetType("System.String"));
            table.Columns.Add("AuditResult", Type.GetType("System.String"));
            table.Columns.Add("Auditor", Type.GetType("System.String"));
            table.Columns.Add("EnterpriseResult", Type.GetType("System.String"));
            table.Columns.Add("AuditRemark", Type.GetType("System.String"));
            return table;
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
            this.CurrentPage = PageDropDownList.SelectedIndex;
            search(this.CurrentPage);
        }

        protected void linkBtnFirst_Click(object sender, EventArgs e)
        {
            //首页
            this.CurrentPage = 0;
            search(this.CurrentPage);
        }

        protected void linkBtnPrev_Click(object sender, EventArgs e)
        {
            //上一页
            this.CurrentPage--;
            search(this.CurrentPage);
        }

        protected void linkBtnNext_Click(object sender, EventArgs e)
        {
            //下一页
            this.CurrentPage++;
            search(this.CurrentPage);
        }

        protected void linkBtnLast_Click(object sender, EventArgs e)
        {
            //尾页
            this.CurrentPage = PageDropDownList.Items.Count - 1;
            search(this.CurrentPage);
        }

        private void pageBottom(int count, int pageIndex)
        {
            div_page.Visible = true;
            if (count == 0 && pageIndex == 0) div_page.Visible = false;
            try
            {
                linkBtnFirst.Enabled = true;
                linkBtnPrev.Enabled = true;
                linkBtnNext.Enabled = true;
                linkBtnLast.Enabled = true;
                int pageCount = count % GridView1.PageSize == 0 ? count / GridView1.PageSize : count / GridView1.PageSize + 1;
                lbRecordCount.Text = count.ToString();
                if (this.CurrentPage == 0)
                {
                    linkBtnFirst.Enabled = false;
                    linkBtnPrev.Enabled = false;
                }
                if (this.CurrentPage >= pageCount - 1)
                {
                    linkBtnLast.Enabled = false;
                    linkBtnNext.Enabled = false;
                }
                if (pageCount <= 0)
                {
                    linkBtnFirst.Enabled = false;
                    linkBtnPrev.Enabled = false;
                    linkBtnNext.Enabled = false;
                    linkBtnLast.Enabled = false;
                }
                PageDropDownList.Items.Clear();

                for (int i = 0; i < pageCount; i++)
                {
                    int pageNumber = i + 1;
                    ListItem item = new ListItem(pageNumber + "/" + pageCount, pageNumber.ToString());
                    if (i == this.CurrentPage)
                    {
                        item.Selected = true;
                    }
                    PageDropDownList.Items.Add(item);
                }

                int currentPage = this.CurrentPage + 1;
                CurrentPageLabel.Text = currentPage.ToString() +
                  " / " + pageCount;

            }
            catch
            {
            }
        }
    }
}