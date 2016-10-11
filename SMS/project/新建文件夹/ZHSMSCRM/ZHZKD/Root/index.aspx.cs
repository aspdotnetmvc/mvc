using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common;


namespace ZKD.Root
{
    public partial class index : System.Web.UI.Page
    {
        protected Model.EnterpriseUser user;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BLL.Login.IsLogin();
                user = (Model.EnterpriseUser)Session["Login"];
                if (user == null)
                {
                    return;
                }
                else
                {
                    if (user.IsAgent)
                    {
                        me.Visible = true;
                    }
                    else
                    {
                        me.Visible = false;
                    }
                }
            }
        }

        //安全退出
        protected void lbtnExit_Click(object sender, EventArgs e)
        {
            Session["Login"] = null;
            Response.Redirect("../login.aspx");
        }
    }
}