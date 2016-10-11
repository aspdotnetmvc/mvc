<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Account_Audit.aspx.cs" Inherits="WebSMS.Root.ACC.SMS_Audit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>修改审核状态</title>
    <link type="text/css" rel="stylesheet" href="../../scripts/ui/skins/Aqua/css/ligerui-all.css" />
    <link type="text/css" rel="stylesheet" href="../images/style.css" />
    <link type="text/css" rel="stylesheet" href="../../css/pagination.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.3.2.min.js"></script>
    <script type="text/javascript" src="../../scripts/ui/js/ligerBuild.min.js"></script>
    <script type="text/javascript" src="../js/function.js"></script>
    <script type="text/javascript" src="../../JavaScript/Prompt/ymPrompt.js"></script>
    <link rel="stylesheet" type="text/css" href="../../JavaScript/Prompt/skin/qq/ymPrompt.css" />

</head>
<body class="mainbody">
    <form id="form1" runat="server">
        <div class="navigation">
            <a href="javascript:history.go(-1);" class="back">后退</a>首页  &gt; 修改审核状态
        </div>
        <div class="tools_box">
            <div class="tools_bar">
                <a href="Account_List.aspx" class="tools_btn"><span><b class="all">返回账号列表</b></span></a>
            </div>
        </div>
        <div id="contentTab">
            <ul class="tab_nav">
                <li class="selected"><a onclick="tabs('#contentTab',0);" href="javascript:;">修改审核状态</a></li>
            </ul>

            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <th>账户编号：</th>
                            <td>
                                <asp:TextBox ID="txt_AccountID" runat="server" Enabled="false" /><label></label>
                            </td>
                        </tr>
                        <tr>
                            <th>审核状态：</th>
                            <td>
                                <asp:DropDownList ID="dd_Audit" runat="server" Width="262px">
                                    <asp:ListItem Value="1">自动审核</asp:ListItem>
                                    <asp:ListItem Value="0">人工审核</asp:ListItem>
                                    <%--  <asp:ListItem Value="2">企业鉴权</asp:ListItem>--%>
                                </asp:DropDownList>
                            </td>
                        </tr>

                    </tbody>
                </table>
            </div>
            <div class="foot_btn_box">
                <asp:Button ID="btnSubmit" runat="server" Text="提交审核" CssClass="btnSubmit" OnClick="btnSubmit_Click" />
                &nbsp;<input name="重置" type="reset" class="btnSubmit" value="重 置" />
            </div>
        </div>
    </form>
</body>
</html>
