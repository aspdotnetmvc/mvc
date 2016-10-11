<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PhoneSingle.aspx.cs" Inherits="ZKD.Root.Contacts.PhoneSingle" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>通讯录添加</title>
    <link type="text/css" rel="stylesheet" href="../../scripts/ui/skins/Aqua/css/ligerui-all.css" />
    <link type="text/css" rel="stylesheet" href="../images/style.css" />
    <link type="text/css" rel="stylesheet" href="../../css/pagination.css" />
    <link type="text/css" rel="stylesheet" href="../../JavaScript/Prompt/gridview.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.8.3.min.js"></script>
    <script type="text/javascript" src="../../scripts/ui/js/ligerBuild.min.js"></script>
    <script type="text/javascript" src="../js/function.js"></script>
    <script src="../../Script/formValidator/DateTimeMask.js" type="text/javascript"></script>
    <%--<script src="../../Script/formValidator/datepicker/calendar.js" type="text/javascript"></script>--%>
    <script src="../../Script/formValidator/datepicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../../JavaScript/Prompt/ymPrompt.js"></script>
    <link rel="stylesheet" type="text/css" href="../../JavaScript/Prompt/skin/qq/ymPrompt.css" />
    <script type="text/javascript">
        $(function () {
            var color;
            $("#GridView1>tbody>tr:first~tr:even").css("backgroundColor", "#ebf5fc");
            $("#GridView1>tbody>tr:first~tr:odd").css("backgroundColor", "#d6e7fc");
            $("#GridView1>tbody>tr").mouseover(function () {
                color = $(this).css("backgroundColor");
                $(this).css("backgroundColor", "#fdfde2");
            });
            $("#GridView1>tbody>tr").mouseout(function () {
                $(this).css("backgroundColor", color);
            });
        });
    </script>
</head>
<body class="mainbody">
    <form id="form1" runat="server">
        <div class="navigation">首页 &gt; 通讯录 &gt; 通讯录添加</div>
        <div id="contentTab">
            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <col width="150px">
                    <tbody>
                        <tr>
                            <th>电话号码：</th>
                            <td style="width: 660px">
                                <asp:TextBox ID="txt_phone" runat="server" CssClass="txtInput normal required" MaxLength="16" />*<label></label>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                    ControlToValidate="txt_phone" Display="Dynamic"
                                    ErrorMessage="请填写电话号码">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <th>姓名：</th>
                            <td style="width: 660px">
                                <asp:TextBox ID="txt_name" runat="server" CssClass="txtInput normal required" MaxLength="16" /><label></label>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <th>生日：</th>
                            <td style="width: 660px">
                                <asp:TextBox ID="txt_brithday" runat="server" CssClass="txtInput normal required" MaxLength="64" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})" /><label></label>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <th>性别：</th>
                            <td>
                                <asp:RadioButtonList ID="r_sex" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="1" Selected="True">男</asp:ListItem>
                                    <asp:ListItem Value="0">女</asp:ListItem>
                                </asp:RadioButtonList></td>
                        </tr>
                        <tr>
                            <th>公司：</th>
                            <td style="width: 660px">
                                <asp:TextBox ID="txt_Company" runat="server" CssClass="txtInput normal required" MaxLength="40" /><label></label>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <th>职位：</th>
                            <td style="width: 660px">
                                <asp:TextBox ID="txt_Postion" runat="server" CssClass="txtInput normal required" MaxLength="20" /><label></label>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <th>Email：</th>
                            <td style="width: 660px">
                                <asp:TextBox ID="txt_Email" runat="server" CssClass="txtInput normal required" MaxLength="20" /><label></label>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <th>QQ：</th>
                            <td style="width: 660px">
                                <asp:TextBox ID="txt_QQ" runat="server" CssClass="txtInput normal required" MaxLength="16" /><label></label>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <th>WebChat：</th>
                            <td style="width: 660px">
                                <asp:TextBox ID="txt_webchat" runat="server" CssClass="txtInput normal required" MaxLength="16" /><label></label>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <th>网站：</th>
                            <td style="width: 660px">
                                <asp:TextBox ID="txt_Comweb" runat="server" CssClass="txtInput normal required" MaxLength="30" /><label></label>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <th>通讯录组：</th>
                            <td style="width: 660px">
                                <asp:DropDownList ID="dd_l" runat="server" Style="width: 40%">
                                </asp:DropDownList>
                            </td>
                            <td></td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="foot_btn_box">
                <asp:Button ID="btnSubmit" runat="server" Text="添加" CssClass="btnSubmit" OnClick="btnSubmit_Click" />
                &nbsp;<input name="重置" type="reset" class="btnSubmit" value="重 置" />
            </div>
        </div>
    </form>
</body>
</html>
