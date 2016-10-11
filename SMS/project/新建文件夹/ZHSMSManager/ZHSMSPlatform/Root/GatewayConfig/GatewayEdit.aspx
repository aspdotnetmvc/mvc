<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GatewayEdit.aspx.cs" Inherits="ZHSMSPlatform.Root.GatewayConfig.GatewayEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>网关配置修改</title>
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
            <a href="javascript:history.go(-1);" class="back">后退</a>首页 &gt; 网关管理 &gt; 网关配置修改
        </div>
        <div class="tools_box">
            <div class="tools_bar">
                <a href="GatewayConfigManage.aspx" class="tools_btn"><span><b class="all">网关管理</b></span></a>
            </div>
        </div>
        <div id="contentTab">
            <ul class="tab_nav">
                <li class="selected"><a onclick="tabs('#contentTab',0);" href="javascript:;">网关配置</a></li>
            </ul>
            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <th>网关编号：</th>
                            <td>
                                <asp:Label ID="lbl_gateway" Text="" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th>网关吞吐能力：</th>
                            <td>
                                <asp:TextBox ID="txt_handlity" Text="100" runat="server" CssClass="txtInput" MaxLength="6"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server"
                                    ControlToValidate="txt_handlity" Display="Dynamic"
                                    ErrorMessage="*请设置网关吞吐能力">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="txt_handlity" ValidationExpression="^[1-9]\d*$"
                                    ID="RegularExpressionValidator6" runat="server" ErrorMessage="请输入数字"></asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <th>运营商：</th>
                            <td>
                                <asp:CheckBoxList ID="cb_operators" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="mobile">移动</asp:ListItem>
                                    <asp:ListItem Value="telecom">电信</asp:ListItem>
                                    <asp:ListItem Value="unicom">联通</asp:ListItem>
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <div class="foot_btn_box">
                    <asp:Button ID="btnSubmit" runat="server" Text="提交保存" CssClass="btnSubmit" OnClick="btnSubmit_Click" />
                    &nbsp;<input name="重置" type="reset" class="btnSubmit" value="重 置" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
