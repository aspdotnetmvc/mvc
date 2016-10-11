<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChannelGateWayBind.aspx.cs" Inherits="ZHSMSPlatform.Root.GatewayConfig.ChannelGateWayBind" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>通道网关绑定</title>
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
    <script type="text/javascript" language="javascript" src="../../JavaScript/Prompt/ymPrompt.js"></script>
    <link rel="stylesheet" type="text/css" href="../../JavaScript/Prompt/skin/qq/ymPrompt.css" />
</head>
<body class="mainbody">
    <form id="form1" runat="server">
        <div class="navigation">首页 &gt; 通道网关配置 &gt; 通道网关绑定</div>
        <div>
            <asp:GridView ID="GridView1" CssClass="gridview" SkinID="GridViewSkin" runat="server" AutoGenerateColumns="False" DataKeyNames="gateway"
                Width="100%" OnRowDataBound="GridView1_RowDataBound" AllowPaging="True" OnDataBound="GridView1_DataBound"
                OnPageIndexChanging="GridView1_PageIndexChanging" PageSize="15" OnRowDeleting="GridView1_RowDeleting">
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
                    <asp:TemplateField HeaderText="选择">
                        <ItemTemplate>
                            <asp:CheckBox ID="CheckBox" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="ID" HeaderText="序号" />
                    <asp:BoundField DataField="gateway" HeaderText="网关名字(编码)" />
                    <asp:BoundField DataField="gatewayType" HeaderText="网关类型" Visible="false" />
                    <asp:BoundField DataField="operators" HeaderText="运营商" />
                    <asp:BoundField DataField="handlingAbility" HeaderText="网关吞吐能力" />
                    <asp:BoundField DataField="othersend" HeaderText="是否发送非本运营商号码" HeaderStyle-Width="180px" />
                </Columns>
            </asp:GridView>
        </div>
        <div class="foot_btn_box">
            <asp:CheckBox ID="CheckBoxAll" runat="server" Text="全选" Width="80px" AutoPostBack="True" OnCheckedChanged="CheckBoxAll_CheckedChanged" />
            <asp:CheckBox ID="CheckBox1" runat="server" Text="反选" Width="80px" AutoPostBack="True" OnCheckedChanged="CheckBox1_CheckedChanged" />
            <asp:Button ID="bt_n" runat="server" Text="取消" CssClass="btnSubmit" OnClick="bt_n_Click" />
            &nbsp;    
            <asp:Label ID="la1" Font-Bold="True" Font-Size="Medium" runat="server">请选择通道</asp:Label>
            &nbsp;
            <asp:DropDownList ID="dd_l" runat="server">
            </asp:DropDownList>
            <asp:Button ID="bt1" runat="server" Text="绑定通道网关" CssClass="btnSubmit" OnClick="bt1_Click" />
        </div>
    </form>
</body>
</html>


