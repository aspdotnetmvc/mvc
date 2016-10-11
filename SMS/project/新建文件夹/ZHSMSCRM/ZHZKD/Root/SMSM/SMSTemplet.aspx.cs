using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SMSModel;

namespace ZKD.Root.SMSM
{
    public partial class SMSTemplet : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BLL.Login.IsLogin();
            if (!IsPostBack)
            {
                txt_TempletContent.Value = "";
                LoadSMSTemplet();
            }
        }

        private void LoadSMSTemplet()
        {
            Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];
            RPCResult<List<Model.SMSTemplet>> smsTemplet = ZHSMSProxy.GetZKD().GetZKDSMSTempletStauts(user.AccountCode);
            if (smsTemplet.Success)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("TempletID", typeof(String));
                dt.Columns.Add("TempletContent", typeof(String));
                dt.Columns.Add("SubmitTime",typeof(String));
                dt.Columns.Add("AuditTime",typeof(String));
                dt.Columns.Add("AuditState",typeof(String));
                dt.Columns.Add("Remark",typeof(String));

                foreach(Model.SMSTemplet smsT in smsTemplet.Value)
                {
                    DataRow dr;
                    dr = dt.NewRow();
                    dr[0] = smsT.TempletID;
                    dr[1] = smsT.TempletContent;
                    dr[2] = smsT.SubmitTime.ToString("yyyy-MM-dd HH:mm:ss");
                    dr[3] = smsT.AuditTime.ToString();
                    switch (smsT.AuditState)
                    {
                        case Model.TempletAuditType.NoAudit:
                            dr[4] = "未审核";
                            break;
                        case Model.TempletAuditType.Failure:
                            dr[4] = "审核失败";
                            break;
                        case Model.TempletAuditType.Success:
                            dr[4] = "审核成功";
                            break;
                    }
                    dr[5] = smsT.Remark;
                    dt.Rows.Add(dr);
                }

                gvSMSTemplet.DataSource = dt;
                gvSMSTemplet.DataBind();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (txt_TempletContent.Value.ToString().Trim() == "")
            {
                Message.Alert(this, "请输入短信内容", "null");
                return;
            }

            Model.EnterpriseUser user = (Model.EnterpriseUser)Session["Login"];
            RPCResult<Guid> SubmitResult = ZHSMSProxy.GetZKD().AddSMSTemplet(user.AccountCode, txt_TempletContent.Value);
            if(SubmitResult.Success)
            {
                txt_TempletContent.Value = "";
                LoadSMSTemplet();
            }
            else
            {
                Message.Error(this, SubmitResult.Message, "null");
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txt_TempletContent.Value = "";
        }

        protected void gvSMSTemplet_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSMSTemplet.PageIndex = e.NewPageIndex;
            LoadSMSTemplet();
        }

        protected void gvSMSTemplet_RowCommand(object sender, GridViewCommandEventArgs e)
        {
           
        }

        protected void gvSMSTemplet_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string templetID = this.gvSMSTemplet.DataKeys[e.RowIndex].Value.ToString();
            RPCResult resultDel = ZHSMSProxy.GetZKD().ZKDDelSMSTemplet(templetID);
            if (resultDel.Success)
            {
                LoadSMSTemplet();
                Message.Success(this, "删除成功", "null");
            }
            else
            {
                Message.Error(this, resultDel.Message, "null");
            }
        }

        protected int CurrentPage
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
            GridViewRow pagerRow = gvSMSTemplet.BottomPagerRow;
            DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
            gvSMSTemplet.PageIndex = pageList.SelectedIndex;
            this.CurrentPage = pageList.SelectedIndex;
            LoadSMSTemplet();
        }

        protected void gvSMSTemplet_DataBound(object sender, EventArgs e)
        {
            try
            {
                GridViewRow pagerRow = gvSMSTemplet.BottomPagerRow;
                LinkButton linkBtnFirst = (LinkButton)pagerRow.Cells[0].FindControl("linkBtnFirst");
                LinkButton linkBtnPrev = (LinkButton)pagerRow.Cells[0].FindControl("linkBtnPrev");
                LinkButton linkBtnNext = (LinkButton)pagerRow.Cells[0].FindControl("linkBtnNext");
                LinkButton linkBtnLast = (LinkButton)pagerRow.Cells[0].FindControl("linkBtnLast");

                linkBtnFirst.Enabled = true;
                linkBtnPrev.Enabled = true;
                linkBtnNext.Enabled = true;
                linkBtnLast.Enabled = true;
                
                if (gvSMSTemplet.PageIndex == 0)
                {
                    linkBtnFirst.Enabled = false;
                    linkBtnPrev.Enabled = false;
                }
                else if (gvSMSTemplet.PageIndex == gvSMSTemplet.PageCount - 1)
                {
                    linkBtnLast.Enabled = false;
                    linkBtnNext.Enabled = false;
                }
                else if (gvSMSTemplet.PageCount <= 0)
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
                    for (int i = 0; i < gvSMSTemplet.PageCount; i++)
                    {
                        int pageNumber = i + 1;
                        ListItem item = new ListItem(pageNumber.ToString() + "/" + gvSMSTemplet.PageCount.ToString(), pageNumber.ToString());
                        if (i == gvSMSTemplet.PageIndex)
                        {
                            item.Selected = true;
                        }
                        pageList.Items.Add(item);
                    }
                }
                if (pageLabel != null)
                {
                    int currentPage = gvSMSTemplet.PageIndex + 1;
                    pageLabel.Text = "当前页： " + currentPage.ToString() +
                      " / " + gvSMSTemplet.PageCount.ToString();
                }
            }
            catch
            {
            }
        }

        protected void gvSMSTemplet_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ((LinkButton)e.Row.Cells[6].Controls[0]).Attributes.Add("onclick", "return confirm('确认删除吗？');");
            }
        }
    }
}