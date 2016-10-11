<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RolePermission.aspx.cs" Inherits="ZHSMSPlatform.Root.Account.RolePermission" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>角色权限编辑</title>
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
            <a href="javascript:history.go(-1);" class="back">后退</a>首页 &gt; 帐号管理 &gt; 角色权限
        </div>
        <div id="contentTab">
            <ul class="tab_nav">
                <li class="selected"><a onclick="tabs('#contentTab',0);" href="javascript:;">角色权限编辑</a></li>
            </ul>
            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <tr>
                        <th>角色名称：</th>
                        <td>
                            <asp:HiddenField ID="hf_roleID" runat="server" />
                            <asp:Label ID="lbl_roleName" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <th>系统模块：</th>
                        <td>
                            <asp:DataList ID="DataListSysModule" runat="server" Width="688px" OnItemDataBound="DataListSysModule_ItemDataBound">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_SysModule" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"Code") %>'
                                        Visible="False"></asp:Label>
                                    <asp:Label ID="lbl_title" runat="server" ForeColor="#000" Font-Bold="true"
                                        Text='<%# DataBinder.Eval(Container.DataItem,"Title")%>'></asp:Label>
                                    <asp:DataList ID="DataListChild" runat="server" Width="688px" OnItemDataBound="DataListChild_ItemDataBound">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_menuCode" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"ChildCode")%>' Visible="false"></asp:Label>
                                            <asp:Label ID="lbl_menu" runat="server" ForeColor="#ffffff" BackColor="#669999" Text='<%# DataBinder.Eval(Container.DataItem,"ChildTitle")%>'></asp:Label>
                                            <asp:CheckBoxList ID="Application_ID" runat="server" Visible="true" RepeatColumns="5"
                                                Height="30px">
                                            </asp:CheckBoxList>
                                        </ItemTemplate>
                                    </asp:DataList>
                                    <%--<asp:CheckBoxList ID="Application_ID" runat="server" Visible="true" RepeatColumns="5"
                                        Height="30px">
                                    </asp:CheckBoxList>--%>
                                </ItemTemplate>
                            </asp:DataList>
                        </td>
                    </tr>
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
