<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountEnterprise.aspx.cs" Inherits="ZHSMSPlatform.Root.Account.AccountEnterprise" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>管辖企业</title>
    <link type="text/css" rel="stylesheet" href="../../scripts/ui/skins/Aqua/css/ligerui-all.css" />
    <link type="text/css" rel="stylesheet" href="../images/style.css" />
    <link type="text/css" rel="stylesheet" href="../../css/pagination.css" />
    <link type="text/css" rel="stylesheet" href="../../JavaScript/Prompt/gridview.css" />
    <script type="text/javascript" src="../../JavaScript/Prompt/jquery-1.8.3.min.js"></script>
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
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.3.2.min.js"></script>
    <script type="text/javascript" src="../../scripts/ui/js/ligerBuild.min.js"></script>
    <script type="text/javascript" src="../js/function.js"></script>
    <script type="text/javascript" lang="javascript" src="../../JavaScript/Prompt/ymPrompt.js"></script>
    <link rel="stylesheet" type="text/css" href="../../JavaScript/Prompt/skin/qq/ymPrompt.css" />
</head>
<body class="mainbody">
    <form id="form1" runat="server">
        <div class="navigation">
            <a href="javascript:history.go(-1);" class="back">后退</a>首页 &gt; 帐号管理 &gt; 管辖企业分配&gt;分配
        </div>

        <div class="tools_box">
            <div class="tools_bar">
                <a href="AccountManage.aspx" class="tools_btn"><span><b class="all">返回账号管理</b></span></a>
            </div>
        </div>
        <div>
            <asp:GridView ID="GridView1" CssClass="gridview" runat="server" AutoGenerateColumns="False" DataKeyNames="AccountCode"
                Width="100%" OnRowDataBound="GridView1_RowDataBound" AllowPaging="True" OnDataBound="GridView1_DataBound"
                OnPageIndexChanging="GridView1_PageIndexChanging" PageSize="15">
                <PagerTemplate>
                    <table>
                        <tr>
                            <td style="text-align: left">
                                <asp:Label ID="MessageLabel" ForeColor="black" Text="页码:" runat="server" />
                                <asp:DropDownList ID="PageDropDownList" AutoPostBack="true" OnSelectedIndexChanged="PageDropDownList_SelectedIndexChanged"
                                    runat="server" />
                                <asp:LinkButton CommandName="Page" CommandArgument="First" ID="linkBtnFirst" runat="server">首页</asp:LinkButton>
                                <asp:LinkButton CommandName="Page" CommandArgument="Prev" ID="linkBtnPrev" runat="server">上一页</asp:LinkButton>
                                <asp:LinkButton CommandName="Page" CommandArgument="Next" ID="linkBtnNext" runat="server">下一页</asp:LinkButton>
                                <asp:LinkButton CommandName="Page" CommandArgument="Last" ID="linkBtnLast" runat="server">尾页</asp:LinkButton>
                            </td>
                            <td style="width: 460px"></td>
                            <td>
                                <asp:Label ID="CurrentPageLabel" ForeColor="black" runat="server" />
                            </td>
                        </tr>
                    </table>
                </PagerTemplate>
                <Columns>
                   
                    <asp:BoundField DataField="AccountCode" HeaderText="企业名称" Visible="false" />
                    <asp:BoundField DataField="Name" HeaderText="企业名称" />
                    <asp:BoundField DataField="Contact" HeaderText="联系人" />
                    <asp:BoundField DataField="Telephone" HeaderText="联系电话" />
                    <asp:BoundField DataField="Address" HeaderText="联系地址" />
                    <asp:BoundField DataField="ChannelManager" HeaderText="渠道经理" />
                    <asp:BoundField DataField="CSManager" HeaderText="客服" />
                </Columns>
            </asp:GridView>
        </div>
        <%--<div class="foot_btn_box">
            <asp:CheckBox ID="CheckBoxAll" runat="server" Text="全选" Width="80px" AutoPostBack="True" OnCheckedChanged="CheckBoxAll_CheckedChanged" />
            <asp:CheckBox ID="CheckBox1" runat="server" Text="反选" Width="80px" AutoPostBack="True" OnCheckedChanged="CheckBox1_CheckedChanged" />
            <asp:Button ID="bt1" runat="server" Text="分配" CssClass="btnSubmit" OnClick="bt1_Click" />
            &nbsp;    
             <asp:Button ID="bt2" runat="server" Text="取消分配" CssClass="btnSubmit" OnClick="bt2_Click" />
            &nbsp;
            <asp:Button ID="bt_n" runat="server" Text="取消" CssClass="btnSubmit" OnClick="bt_n_Click" />
            &nbsp;    
             <asp:Button ID="btnAll" runat="server" Text="全部分配" CssClass="btnSubmit" OnClick="btnAll_Click" />
        </div>--%>
    </form>
</body>
</html>
