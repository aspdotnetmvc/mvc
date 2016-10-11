<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AgentLowerEnterpriseManage.aspx.cs" Inherits="ZKD.Root.Agent.AgentLowerEnterpriseManage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>终端企业管理</title>
    <link type="text/css" rel="stylesheet" href="../../scripts/ui/skins/Aqua/css/ligerui-all.css" />
    <link type="text/css" rel="stylesheet" href="../images/style.css" />
    <link type="text/css" rel="stylesheet" href="../../css/pagination.css" />
    <link type="text/css" rel="stylesheet" href="../../JavaScript/Prompt/gridview.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.8.3.min.js"></script>
    <script type="text/javascript" src="../../scripts/ui/js/ligerBuild.min.js"></script>
    <script type="text/javascript" src="../js/function.js"></script>
    <script src="../../Script/formValidator/DateTimeMask.js" type="text/javascript"></script>
<%--    <script src="../../Script/formValidator/datepicker/calendar.js" type="text/javascript"></script>--%>
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
        <div class="navigation">
            <a href="javascript:history.go(-1);" class="back">后退</a>首页 &gt; 终端企业管理 &gt; 终端企业列表
        </div>
        <div>
            <asp:GridView ID="GridView1" CssClass="gridview" runat="server" AutoGenerateColumns="False" DataKeyNames="code,accountID"
                Width="100%" OnRowDataBound="GridView1_RowDataBound" AllowPaging="True" OnDataBound="GridView1_DataBound"
                OnPageIndexChanging="GridView1_PageIndexChanging" PageSize="15" OnRowDeleting="GridView1_RowDeleting" OnRowCommand="GridView1_RowCommand">
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
                    <asp:BoundField DataField="ID" HeaderText="序号" />
                    <asp:BoundField DataField="code" HeaderText="企业帐号" />
                    <asp:BoundField DataField="name" HeaderText="企业名称" />
                    <asp:BoundField DataField="contact" HeaderText="联系人" />
                    <asp:BoundField DataField="phone" HeaderText="联系电话" />
                    <asp:BoundField DataField="address" HeaderText="地址" />
                    <asp:TemplateField HeaderText="企业设置编辑">
                        <ItemTemplate>
                            <a href="EnterpriseEdit.aspx?type=AgentLower&&accountID=<%# Eval("code")%>">企业设置编辑</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="密码重置">
                        <ItemTemplate>
                            <a href="EnterprisePass.aspx?type=AgentLower&&accountID=<%# Eval("code")%>">密码重置</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="企业充值">
                        <ItemTemplate>
                            <a href="EnterpriseRecharge.aspx?type=AgentLower&&accountID=<%# Eval("code")%>">企业充值</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="余额查询">
                        <ItemTemplate>
                            <a href="EnterpriseBalance.aspx?type=AgentLower&&accountID=<%# Eval("code")%>">余额查询</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="充值记录">
                        <ItemTemplate>
                            <a href="EnterpriseRechargeRecord.aspx?type=AgentLower&&accountID=<%# Eval("code")%>">充值记录</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="详情">
                        <ItemTemplate>
                            <a href="EnterpriseDetails.aspx?type=AgentLower&&accountID=<%# Eval("code")%>">详情</a>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="企业短信统计">
                        <ItemTemplate>
                            <a href="EnterpriseSMSStatistics.aspx?type=AgentLower&&accountID=<%# Eval("code")%>">企业短信统计</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    无记录
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
