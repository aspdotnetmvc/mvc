<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoleDetail.aspx.cs" Inherits="ZHCRM.Root.Account.RoleDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>角色详情</title>
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
            <a href="javascript:history.go(-1);" class="back">后退</a>首页 &gt; 帐号管理 &gt; 帐号详情
        </div>
        <div id="contentTab">
            <ul class="tab_nav">
                <li class="selected"><a onclick="tabs('#contentTab',0);" href="javascript:;">角色信息</a></li>
                <li><a onclick="tabs('#contentTab',1);" href="javascript:;">角色权限</a></li>
            </ul>

            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <th>角色代码：</th>
                            <td>
                                <asp:Label ID="lbl_roleCode" Text="" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th>角色名称：</th>
                            <td>
                                <asp:Label ID="lbl_name" Text="" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th>角色描述：</th>
                            <td>
                                <asp:Label ID="lbl_remark" Text="" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="tab_con" style="display: none;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <th>拥有权限：</th>
                            <td>
                                <asp:DataList ID="DataListSysModule" runat="server" Width="688px" OnItemDataBound="DataListSysModule_ItemDataBound">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_SysModule" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"Code") %>'
                                            Visible="False"></asp:Label>
                                        <asp:Label ID="lbl_title" runat="server" ForeColor="#000"
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
                                    </ItemTemplate>
                                </asp:DataList>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </form>
</body>
</html>
