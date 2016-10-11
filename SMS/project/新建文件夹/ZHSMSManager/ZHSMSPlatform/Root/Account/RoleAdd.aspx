<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoleAdd.aspx.cs" Inherits="ZHCRM.Root.Account.RoleAdd" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>新增角色</title>
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
            <a href="javascript:history.go(-1);" class="back">后退</a>首页 &gt; 企业用户管理 &gt; 角色新增
        </div>
        <div id="contentTab">
            <ul class="tab_nav">
                <li class="selected"><a onclick="tabs('#contentTab',0);" href="javascript:;">角色信息</a></li>
            </ul>

            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <th>角色代码：</th>
                            <td>
                                <asp:TextBox ID="txt_roleCode" Text="" runat="server" CssClass="txtInput" Width="260" MaxLength="24"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server"
                                    ControlToValidate="txt_roleCode" Display="Dynamic"
                                    ErrorMessage="*请设置角色代码">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <th>角色名称：</th>
                            <td>
                                <asp:TextBox ID="txt_roleName" Text="" runat="server" CssClass="txtInput" Width="260" MaxLength="32"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                    ControlToValidate="txt_roleName" Display="Dynamic"
                                    ErrorMessage="*请设置角色名称">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <th>角色描述：</th>
                            <td>
                                <asp:TextBox ID="txt_remar" TextMode="MultiLine" Width="260" Height="180" runat="server" CssClass="txtInput" MaxLength="128"></asp:TextBox>
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

