<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="KeywordAdd.aspx.cs" Inherits="ZHSMSPlatform.Root.Keywords.KeywordAdd" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>敏感词添加</title>
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
        <div class="navigation">首页 &gt; 敏感词管理 &gt; 敏感词添加</div>
        <div class="tools_box">
            <div class="tools_bar">
                <a href="KeywordsMange.aspx" class="tools_btn"><span><b class="all">返回敏感词管理</b></span></a>
            </div>
        </div>
        <div id="contentTab">
            <ul class="tab_nav">
                <li class="selected"><a onclick="tabs('#contentTab',0);" href="javascript:;">敏感词添加</a></li>
            </ul>
            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <th>
                                <asp:Label ID="lbl_thUpName" runat="server" Text="敏感词组："></asp:Label>
                            </th>
                            <td>
                                <asp:DropDownList ID="dd_groups" runat="server"></asp:DropDownList>*&nbsp;&nbsp;&nbsp; <asp:Button ID="btn_newGroup" runat="server" Text="新建敏感词组"  CssClass="btnSubmit" CausesValidation="false" PostBackUrl="~/Root/Keywords/KeywordGroupAdd.aspx"/>
                            </td>
                        </tr>
                        <tr>
                            <th>敏感词：</th>
                            <td>
                                <asp:TextBox ID="txt_keywords" TextMode="MultiLine" Width="360" Height="180" runat="server" CssClass="txtInput" />*多个请用英文，隔开
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                    ControlToValidate="txt_keywords" Display="Dynamic"
                                    ErrorMessage="请填写敏感词" ForeColor="Red">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                敏感词类型：
                            </th>
                            <td>
                                <asp:RadioButtonList ID="rb_types" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="-1" Selected="True">无</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <th>是否启用：</th>
                            <td>
                                <asp:RadioButtonList ID="rb_status" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="1" Selected="True">启用</asp:ListItem>
                                    <asp:ListItem Value="0">不启用</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <th>敏感词替换成为：</th>
                            <td>
                                <asp:TextBox ID="txt_other" runat="server" TextMode="MultiLine" Width="360" Height="180" CssClass="txtInput" MaxLength="128" />
                            </td>
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
