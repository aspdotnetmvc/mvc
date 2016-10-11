<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountEdit.aspx.cs" Inherits="ZHCRM.Root.Account.AccountEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>帐号编辑</title>
    <link type="text/css" rel="stylesheet" href="../../scripts/ui/skins/Aqua/css/ligerui-all.css" />
    <link type="text/css" rel="stylesheet" href="../images/style.css" />
    <link type="text/css" rel="stylesheet" href="../../css/pagination.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.3.2.min.js"></script>
    <script type="text/javascript" src="../../scripts/ui/js/ligerBuild.min.js"></script>
    <script type="text/javascript" src="../js/function.js"></script>
    <script src="../../Script/formValidator/DateTimeMask.js" type="text/javascript"></script>
    <script src="../../Script/formValidator/datepicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../../JavaScript/Prompt/ymPrompt.js"></script>
    <link rel="stylesheet" type="text/css" href="../../JavaScript/Prompt/skin/qq/ymPrompt.css" />
</head>
<body class="mainbody">
    <form id="form1" runat="server">
        <div class="navigation">
            <a href="javascript:history.go(-1);" class="back">后退</a>首页 &gt; 帐号管理 &gt; 帐号编辑
        </div>
        <div class="tools_box">
            <div class="tools_bar">
                <a href="AccountManage.aspx?accountID=<%=Request.QueryString["accountID"] %>" class="tools_btn"><span><b class="all">帐号管理</b></span></a>
            </div>
        </div>
        <div id="contentTab">
            <ul class="tab_nav">
                <li class="selected"><a onclick="tabs('#contentTab',0);" href="javascript:;">帐号信息</a></li>
            </ul>

            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <th>帐号：</th>
                            <td>
                                <asp:Label ID="lbl_accountID" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th>姓名：</th>
                            <td>
                                <asp:TextBox ID="txt_name" Text="" runat="server" CssClass="txtInput" MaxLength="32"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server"
                                    ControlToValidate="txt_name" Display="Dynamic"
                                    ErrorMessage="*请填写姓名">
                                </asp:RequiredFieldValidator></td>
                        </tr>
                        <tr>
                            <th>默认启用：</th>
                            <td>
                                <asp:RadioButtonList ID="rb_defalut" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="1" Selected="True">是</asp:ListItem>
                                    <asp:ListItem Value="0">否</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <th>分配角色：</th>
                            <td>
                                <asp:RadioButtonList ID="cb_roles" runat="server" RepeatDirection="Horizontal">
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <div class="foot_btn_box">
                    <asp:Button ID="btn_Submit" runat="server" Text="提交保存" CssClass="btnSubmit" OnClick="btnSubmit_Click" />
                    &nbsp;<input name="重置" type="reset" class="btnSubmit" value="重 置" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
