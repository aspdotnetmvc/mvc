<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Accout_ModuleTest.aspx.cs" Inherits="WebSMS.Root.TestM.Accout_ModuleTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>性能测试</title>
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
        <div class="tools_box">
            <div class="tools_bar">
                <a href="../ACC/Account_List.aspx" class="tools_btn"><span><b class="all">返回账号列表</b></span></a>
            </div>
        </div>
        <div id="contentTab">
            
            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <th>数量级：</th>
                            <td colspan="2">
                                <asp:TextBox ID="txt_Magnitude" runat="server" CssClass="txtInput normal required" MaxLength="10" /><label>*</label>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server"
                                    ControlToValidate="txt_Magnitude" Display="Dynamic"
                                    ErrorMessage="请输入数量级">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="txt_Magnitude" ValidationExpression="^[0-9]*$"
                                    ID="RegularExpressionValidator1" runat="server" ErrorMessage="请输入数字！！"></asp:RegularExpressionValidator>

                            </td>

                        </tr>
                        <tr>
                            <th>创建账号：</th>
                            <td>
                                <asp:Label ID="Label1" runat="server" Text="Label">耗时</asp:Label>
                                <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
                            </td>
                            <td>
                                <asp:Button ID="bt1" runat="server" Text="创建" CssClass="btnSubmit" OnClick="bt1_Click" /></td>
                        </tr>

                        <tr>
                            <th>账号审核：</th>
                            <td>
                                <asp:Label ID="Label3" runat="server" Text="Label">耗时</asp:Label>
                                <asp:Label ID="Label4" runat="server" Text="Label"></asp:Label>
                            </td>
                            <td>
                                <asp:Button ID="bt2" runat="server" Text="审核" CssClass="btnSubmit" OnClick="bt2_Click" />
                            </td>
                        </tr>
                        <tr>
                            <th>账号优先级调整：</th>
                            <td>
                                <asp:Label ID="Label5" runat="server" Text="Label">耗时</asp:Label>
                                <asp:Label ID="Label6" runat="server" Text="Label"></asp:Label>
                            </td>
                            <td>
                                <asp:Button ID="bt3" runat="server" Text="调整" CssClass="btnSubmit" OnClick="bt3_Click" />
                            </td>
                        </tr>
                        <tr>
                            <th>修改密码：</th>
                            <td>
                                <asp:Label ID="Label7" runat="server" Text="Label">耗时</asp:Label>
                                <asp:Label ID="Label8" runat="server" Text="Label"></asp:Label></td>
                            <td>
                                <asp:Button ID="bt4" runat="server" Text="修改" CssClass="btnSubmit" OnClick="bt4_Click" />
                            </td>
                        </tr>
                      <%--  <tr>
                            <th>发送短信：</th>
                            <td>
                                <asp:Label ID="Label9" runat="server" Text="Label">耗时</asp:Label>
                                <asp:Label ID="Label10" runat="server" Text="Label"></asp:Label></td>
                            <td>
                                <asp:Button ID="bt5" runat="server" Text="发送" CssClass="btnSubmit" OnClick="bt5_Click"  />
                            </td>
                        </tr>--%>

                    </tbody>
                </table>
            </div>
        </div>
    </form>
</body>
</html>
