<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FailAgentLowerEnterprise.aspx.cs" Inherits="ZKD.Root.Agent.FailAgentLowerEnterprise" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>企业审核状态 </title>
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
        <div class="navigation">
            <a href="javascript:history.go(-1);" class="back">后退</a>首页 &gt; 终端企业管理 &gt; 企业审核状态
        </div>
        <div>
            <asp:GridView ID="GridView1" CssClass="gridview" runat="server" AutoGenerateColumns="False" DataKeyNames="EnterpriseCode"
                Width="100%" OnRowDataBound="GridView1_RowDataBound"  PageSize="15" OnRowDeleting="GridView1_RowDeleting">
                <Columns>
                    <asp:BoundField DataField="ID" HeaderText="序号" />
                    <asp:BoundField DataField="EnterpriseCode" HeaderText="企业代码" />
                    <asp:BoundField DataField="EnterpriseName" HeaderText="企业名称" />
                    <asp:BoundField DataField="CreateTime" HeaderText="代理商终端用户开户时间" />
                    <asp:BoundField DataField="AuditTime" HeaderText="审核时间" />
                    <asp:BoundField DataField="AuditResult" HeaderText="是否审核" />
                    <asp:BoundField DataField="Auditor" HeaderText="审核人" />
                    <asp:BoundField DataField="EnterpriseResult" HeaderText="审核结果" />
                    <asp:BoundField DataField="AuditRemark" HeaderText="审核备注" />
                    <asp:TemplateField HeaderText="编辑">
                        <ItemTemplate>
                            <a href="EnterpriseEdit.aspx?accountID=<%# Eval("EnterpriseCode")%>">编辑</a>
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
        <div id="div_page" runat="server">
            <table class="form_table">
                <tr>
                    <td>
                        <asp:Label ID="MessageLabel" ForeColor="black" Text="页码:" runat="server" />&nbsp;
                        <asp:DropDownList ID="PageDropDownList" AutoPostBack="true" OnSelectedIndexChanged="PageDropDownList_SelectedIndexChanged" runat="server" />

                        <asp:LinkButton CommandName="Page" CommandArgument="First" ID="linkBtnFirst" runat="server" OnClick="linkBtnFirst_Click">首页</asp:LinkButton>&nbsp;
                        <asp:LinkButton CommandName="Page" CommandArgument="Prev" ID="linkBtnPrev" runat="server" OnClick="linkBtnPrev_Click">上一页</asp:LinkButton>&nbsp;
                        <asp:LinkButton CommandName="Page" CommandArgument="Next" ID="linkBtnNext" runat="server" OnClick="linkBtnNext_Click">下一页</asp:LinkButton>&nbsp;
                        <asp:LinkButton CommandName="Page" CommandArgument="Last" ID="linkBtnLast" runat="server" OnClick="linkBtnLast_Click">尾页</asp:LinkButton>&nbsp;
                    
                        <asp:Label ID="lbcurrentpage1" runat="server" Text="当前页:"></asp:Label>&nbsp;
                        <asp:Label ID="CurrentPageLabel" ForeColor="black" runat="server" />&nbsp;
                    
                        <asp:Label ID="recordscount" runat="server" Text="总条数:"></asp:Label>&nbsp;
                        <asp:Label ID="lbRecordCount" runat="server" Text=""></asp:Label>&nbsp;
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
