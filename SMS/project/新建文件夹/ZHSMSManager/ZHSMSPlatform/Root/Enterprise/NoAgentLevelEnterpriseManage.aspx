<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NoAgentLevelEnterpriseManage.aspx.cs" Inherits="ZHSMSPlatform.Root.Enterprise.NoAgentLevelEnterpriseManage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>终端用户管理</title>
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
    <script type="text/javascript" src="../../JavaScript/Prompt/ymPrompt.js"></script>
    <link rel="stylesheet" type="text/css" href="../../JavaScript/Prompt/skin/qq/ymPrompt.css" />
</head>
<body class="mainbody">
    <form id="form1" runat="server">
        <div class="navigation">首页 &gt; 企业用户管理 &gt; 管理列表</div>
        <div class="tab_con">
            <table class="form_table">
                <col width="150px">
                <col>
                <tbody>
                    <tr>
                        <th>企业代码或名称：</th>
                        <td>
                            <asp:TextBox ID="txt_name" runat="server" CssClass="txtInput" Height="25" Width="180" />&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnFind" runat="server" Text="查询" CssClass="btnSubmit" OnClick="btnFind_Click" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <br />
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
                    <asp:TemplateField HeaderText="资料编辑" Visible="false">
                        <ItemTemplate>
                            <a href="EnterpriseInfoEdit.aspx?type=NoAgentLower&&accountID=<%# Eval("code")%>">资料编辑</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="企业设置编辑">
                        <ItemTemplate>
                            <a href="EnterpriseEdit.aspx?type=NoAgentLower&&accountID=<%# Eval("code")%>">企业设置编辑</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="短信设置编辑" Visible="false">
                        <ItemTemplate>
                            <a href="EnterpriseSMSEdit.aspx?type=NoAgentLower&&accountID=<%# Eval("code")%>">短信设置编辑</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="密码重置">
                        <ItemTemplate>
                            <a href="EnterprisePass.aspx?type=NoAgentLower&&accountID=<%# Eval("code")%>">密码重置</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="密钥修改">
                        <ItemTemplate>
                            <a href="EnterpriseSecretKeyChange.aspx?type=NoAgentLower&&accountID=<%# Eval("code")%>">密钥修改</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="企业充值">
                        <ItemTemplate>
                            <a href="EnterpriseRecharge.aspx?type=NoAgentLower&&accountID=<%# Eval("code")%>">企业充值</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="余额查询">
                        <ItemTemplate>
                            <a href="EnterpriseBalance.aspx?type=NoAgentLower&&accountID=<%# Eval("code")%>">余额查询</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="充值记录">
                        <ItemTemplate>
                            <a href="EnterpriseRechargeRecord.aspx?type=NoAgentLower&&accountID=<%# Eval("code")%>">充值记录</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="详情">
                        <ItemTemplate>
                            <a href="EnterpriseDetails.aspx?type=NoAgentLower&&accountID=<%# Eval("code")%>">详情</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="短信管理">
                        <ItemTemplate>
                            <a href="SmsTab.aspx?type=NoAgentLower&&accountID=<%# Eval("code")%>">短信管理</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="企业短信统计">
                        <ItemTemplate>
                            <a href="../Report/EnterpriseSMSStatistics.aspx?type=NoAgentLower&&accountID=<%# Eval("code")%>">企业短信统计</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="指定代理商">
                        <ItemTemplate>
                            <a href="AssignAgent.aspx?type=NoAgentLower&&accountID=<%# Eval("code")%>">指定代理商</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="升级为代理商">
                        <ItemTemplate>
                            <asp:Button ID="btn" runat="server" CommandArgument='<%# Bind("code") %>'
                                CommandName="upgrade" Text="升级为代理商" Width="85px" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:CommandField HeaderText="删除" ShowDeleteButton="True" DeleteText="删除">
                        <ControlStyle ForeColor="Black" />
                    </asp:CommandField>
                </Columns>
                <EmptyDataTemplate>
                    无记录
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </form>
</body>
</html>

