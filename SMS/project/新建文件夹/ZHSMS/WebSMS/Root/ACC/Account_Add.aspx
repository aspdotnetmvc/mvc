<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Account_Add.aspx.cs" Inherits="WebSMS.Root.ACC.Account_Add" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>创建账号</title>
    <link type="text/css" rel="stylesheet" href="../../scripts/ui/skins/Aqua/css/ligerui-all.css" />
    <link type="text/css" rel="stylesheet" href="../images/style.css" />
    <link type="text/css" rel="stylesheet" href="../../css/pagination.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.5.2.min.js"></script>
    <script type="text/javascript" src="../../scripts/ui/js/ligerBuild.min.js"></script>
    <script type="text/javascript" src="../js/function.js"></script>
    <script type="text/javascript" src="../../JavaScript/Prompt/ymPrompt.js"></script>
    <link rel="stylesheet" type="text/css" href="../../JavaScript/Prompt/skin/qq/ymPrompt.css" />

</head>
<body class="mainbody">
    <form id="form1" runat="server">

        <div class="tools_box">
            <div class="tools_bar">
                <a href='Account_List.aspx' class="tools_btn"><span><b class="all">返回账号列表</b></span></a>
            </div>
        </div>
        <div id="contentTab">
            <ul class="tab_nav">
                <li class="selected"><a onclick="tabs('#contentTab',0);" href="javascript:;">账号添加</a></li>
            </ul>

            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <th>账号编号：</th>
                            <td>
                                <asp:TextBox ID="txt_UserID" runat="server" CssClass="txtInput normal required" MaxLength="10" /><label>*</label>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server"
                                    ControlToValidate="txt_UserID" Display="Dynamic"
                                    ErrorMessage="请输入账号编号">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <th>密码：</th>
                            <td>
                                <asp:TextBox ID="txt_Password" runat="server" CssClass="txtInput normal required" TextMode="Password" MaxLength="10" /><label>*</label>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server"
                                    ControlToValidate="txt_Password" Display="Dynamic"
                                    ErrorMessage="请输入密码">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>

                        <tr>
                            <th>优先级：</th>
                            <td>
                                <asp:DropDownList ID="dd_Priority" runat="server" Width="262px">
                                    <asp:ListItem Value="0">0</asp:ListItem>
                                    <asp:ListItem Value="1">1</asp:ListItem>
                                    <asp:ListItem Value="2">2</asp:ListItem>
                                    <asp:ListItem Value="3">3</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <th>短信条数：</th>
                            <td>
                                <asp:TextBox ID="txt_SMSNumber" runat="server" CssClass="txtInput normal required" MaxLength="7" /><label>*</label>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                    ControlToValidate="txt_SMSNumber" Display="Dynamic"
                                    ErrorMessage="请输入短信条数">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="txt_SMSNumber" ValidationExpression="^[0-9]*$"
                                    ID="RegularExpressionValidator1" runat="server" ErrorMessage="请输入数字！！"></asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <th>审核方式：</th>
                            <td>
                                <asp:RadioButtonList ID="dd_Audit" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" Width="262px">
                                    <asp:ListItem Value="1">自动审核</asp:ListItem>
                                    <asp:ListItem Value="0">人工审核</asp:ListItem>
                                    <%--     <asp:ListItem Value="2">企业鉴权</asp:ListItem>--%>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <th>SP源号码：</th>
                            <td>
                                <asp:TextBox ID="txt_SPNumber" runat="server" CssClass="txtInput normal required" MaxLength="10" /><label>*</label>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                    ControlToValidate="txt_SPNumber" Display="Dynamic"
                                    ErrorMessage="请输入SP源号码">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="foot_btn_box">
                <asp:Button ID="btnSubmit" runat="server" Text="创建" CssClass="btnSubmit" OnClick="btnSubmit_Click" />
                &nbsp;<input name="重置" type="reset" class="btnSubmit" value="重 置" />
            </div>
        </div>
    </form>
</body>
</html>
