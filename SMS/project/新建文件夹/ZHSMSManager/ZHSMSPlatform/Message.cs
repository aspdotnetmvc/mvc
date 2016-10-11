using System;
using System.Text;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Message 的摘要说明
/// </summary>
public class Message
{
    public Message()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    #region Ajax 操作提示信息
    /// <summary>
    /// 警告提示信息
    /// </summary>
    /// <param name="page">Asp.Net窗体对象</param>
    /// <param name="msg">提示信息字符串</param>
    /// <param name="handler">客户端回调函数</param>
    public static void Alert(Page page, string msg, string handler)
    {
        StringBuilder script = new StringBuilder("<script language='javascript'>");
        script.Append("ymPrompt.alert('" + msg + "',null,null,'友情提示'," + handler + ")");
        script.Append("</script>");
        ScriptManager.RegisterStartupScript(page, page.GetType(), "Alert", script.ToString(), false);
    }


    /// <summary>
    /// 成功提示信息
    /// </summary>
    /// <param name="page">Asp.Net窗体对象</param>
    /// <param name="msg">提示信息字符串</param>
    /// <param name="handler">客户端回调函数</param>
    public static void Success(Page page, string msg, string handler)
    {
        StringBuilder script = new StringBuilder("<script language='javascript'>");
        script.Append("ymPrompt.succeedInfo('" + msg + "',null,null,'友情提示'," + handler + ")");
        script.Append("</script>");
        ScriptManager.RegisterStartupScript(page, page.GetType(), "Alert", script.ToString(), false);
    }

    /// <summary>
    /// 错误提示信息
    /// </summary>
    /// <param name="page">Asp.Net窗体对象</param>
    /// <param name="msg">提示信息字符串</param>
    /// <param name="handler">客户端回调函数</param>
    public static void Error(Page page, string msg, string handler)
    {
        StringBuilder script = new StringBuilder("<script language='javascript'>");
        script.Append("ymPrompt.errorInfo('" + msg + "',null,null,'友情提示'," + handler + ")");
        script.Append("</script>");
        ScriptManager.RegisterStartupScript(page, page.GetType(), "Error", script.ToString(), false);
    }

    /// <summary>
    /// 询问提示信息
    /// </summary>
    /// <param name="page">Asp.Net窗体对象</param>
    /// <param name="msg">提示信息字符串</param>
    /// <param name="handler">客户端回调函数</param>
    public static void Confirm(Page page, string msg, string handler)
    {
        StringBuilder script = new StringBuilder("<script language='javascript'>");
        script.Append("ymPrompt.confirmInfo('" + msg + "',null,null,'友情提示'," + handler + ")");
        script.Append("</script>");
        ScriptManager.RegisterStartupScript(page, page.GetType(), "Confirm", script.ToString(), false);
    }

    /// <summary>
    /// 重定向页面
    /// </summary>
    /// <param name="page">转到首页</param>
    public static void Redirect(Page page)
    {
        StringBuilder script = new StringBuilder("<script language='javascript'>");
        script.Append("parent.location.href='/';");
        script.Append("</script>");
        ScriptManager.RegisterStartupScript(page, page.GetType(), "Check", script.ToString(), false);
    }

    //public static void Redirect(Page page, string strUrl)
    //{
    //    StringBuilder script = new StringBuilder("<script language='javascript'>");
    //    script.Append("window.location.href='" + strUrl + "';");
    //    script.Append("</script>");
    //    ScriptManager.RegisterStartupScript(page, page.GetType(), "Check", script.ToString(), false);
    //}


    #endregion

    #region 操作提示信息

    /// <summary>
    /// 警告提示信息
    /// </summary>
    /// <param name="page">Asp.Net窗体对象</param>
    /// <param name="msg">提示信息字符串</param>
    /// <param name="handler">客户端回调函数</param>
    public static void Alert(Page page, string msg)
    {
        StringBuilder script = new StringBuilder("<script language='javascript'>");
        script.Append("alert('" + msg + "')");
        script.Append("</script>");
        //ScriptManager.RegisterStartupScript(page, page.GetType(), "Alert", script.ToString(), false);

        ClientScriptManager csm = page.ClientScript;
        if (!csm.IsStartupScriptRegistered(page.GetType(), "Alert"))
        {
            csm.RegisterStartupScript(page.GetType(), "Alert", script.ToString());
        }
    }

    /// <summary>
    /// 重定向页面
    /// </summary>
    /// <param name="page">Asp.Net窗体对象</param>
    public static void Redirect(Page page,string strUrl)
    {

        StringBuilder script = new StringBuilder("<script language='javascript'>");
        script.Append("top.location.href='" + strUrl + "';");
        script.Append("</script>");
        //ScriptManager.RegisterStartupScript(page, page.GetType(), "Redirect", script.ToString(), false);

        ClientScriptManager csm = page.ClientScript;
        if (!csm.IsStartupScriptRegistered(page.GetType(), "Redirect"))
        {
            csm.RegisterStartupScript(page.GetType(), "Redirect", script.ToString());
        }
    }

    /// <summary>
    /// 重定向页面
    /// </summary>
    /// <param name="page">Asp.Net窗体对象</param>
    /// <param name="strUrl">重定向链接</param>
    /// <param name="target">链接目标 true：新开窗口 false：本窗口</param>
    public static void Redirect(Page page, string strUrl,bool target)
    {
        StringBuilder script = new StringBuilder("<script language='javascript'>");
        if (target)
        {
            script.Append("top.location.href='" + strUrl + "';");
        }
        else
        {
            script.Append("window.location.href='" + strUrl + "';");
        }
        script.Append("</script>");
        //ScriptManager.RegisterStartupScript(page, page.GetType(), "Redirect", script.ToString(), false);

        ClientScriptManager csm = page.ClientScript;
        if (!csm.IsStartupScriptRegistered(page.GetType(), "Redirect"))
        {
            csm.RegisterStartupScript(page.GetType(), "Redirect", script.ToString());
        }
    }
    #endregion
}
