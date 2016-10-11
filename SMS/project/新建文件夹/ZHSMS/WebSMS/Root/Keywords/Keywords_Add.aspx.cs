using SMSModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebSMS.Root.Keywords
{
    public partial class Keywords_Add :JudgeSession
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.ViewState["data"] = this.CreateTable();
                RPCResult<List<string>> r = PretreatmentProxy.GetPretreatment().GetKeyGroups();
                foreach (var v in r.Value)
                {
                    dd_keyGroup.Items.Add(new ListItem(v, v));
                }
                dd_keyGroup.Items.Insert(0, new ListItem("--请选择--", "-1"));
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            DataTable dataTabe = (DataTable)this.ViewState["data"];

            if (!Page.IsValid)
            {
                return;
            }
            List<string> num = new List<string>();
            if (dataTabe.Rows.Count > 0)
            {
                foreach (DataRow dr in dataTabe.Rows)
                {
                    num.Add(dr["Keywords"].ToString());
                }
            }
            else
            {
                if (txt_keywords.Text == "")
                {
                    Message.Alert(this, "请添加关键词", "null");
                    return;
                }
                num.Add(txt_keywords.Text);
            }
            GridView1.DataSource = dataTabe;
            GridView1.DataBind();
            string group = txt_keyGroup.Text;
            if (group == "" && dd_keyGroup.SelectedIndex == 0)
            {
                Message.Alert(this, "请选择或填写一个关键词组名字", "null");
                return;
            }
            if (group == "")
            {
                group = dd_keyGroup.SelectedValue;
            }
            RPCResult r = PretreatmentProxy.GetPretreatment().AddKeywords(group, num);
            if (r.Success)
            {
                Message.Success(this, "添加成功", "null");

            }
            else
            {
                Message.Error(this, r.Message, "null");
            }
            this.ViewState["data"] = this.CreateTable();
        }


        protected void btn_import_Click(object sender, EventArgs e)
        {
            try
            {
                string path = this.FileUpload1.PostedFile.FileName;
                if (path == "" || path == null)
                {
                    Message.Alert(this, "请选择一个文件，文件格式为txt", "null");
                    return;
                }
                if (System.IO.Path.GetExtension(FileUpload1.FileName).ToLower() != ".txt")
                {
                    Message.Alert(this, "文件格式为txt", "null");
                    return;
                }
                path = Server.MapPath("~/Temp/") + FileUpload1.FileName;
                FileUpload1.PostedFile.SaveAs(path);
                string strdata = "";
                using (StreamReader sr = new StreamReader(path))
                {
                    string str;
                    while ((str = sr.ReadLine()) != null)
                    {
                        strdata += str + ",";
                    }
                }
                File.Delete(path);
                if (strdata == "")
                {
                    Message.Alert(this, "文件是空的", "null");
                    return;
                }
                DataTable t = CreateTable();
                string[] arr = strdata.Split(',');
                if (arr.Length > 0)
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        DataRow row = t.NewRow();
                        if (arr[i] != "")
                        {
                            row["Group"] = "default";
                            row["Keywords"] = arr[i];
                            t.Rows.Add(row);
                        }
                    }
                }
                this.ViewState["data"] = t;
                GridView1.DataSource = t;
                GridView1.DataBind();
            }
            catch
            {
            }
        }

        private DataTable CreateTable()
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
            DataTable dt = (DataTable)this.ViewState["data"];
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
            DataTable dt = (DataTable)this.ViewState["data"];
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
    }
}