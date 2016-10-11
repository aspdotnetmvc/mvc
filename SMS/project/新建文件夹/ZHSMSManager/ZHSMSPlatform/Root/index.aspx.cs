using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common;


namespace ZHCRM.Root
{
    public partial class index : System.Web.UI.Page
    {
        protected Model.SysAccount account;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BLL.Login.IsLogin();
                account = (Model.SysAccount)Session["Login"];
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