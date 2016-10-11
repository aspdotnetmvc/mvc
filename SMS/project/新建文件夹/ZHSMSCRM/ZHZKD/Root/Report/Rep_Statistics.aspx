<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Rep_Statistics.aspx.cs" Inherits="ZKD.Root.Report.Rep_Statistics" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>短信状态报告</title>
    <link type="text/css" rel="stylesheet" href="../../scripts/ui/skins/Aqua/css/ligerui-all.css" />
    <link type="text/css" rel="stylesheet" href="../images/style.css" />
    <link type="text/css" rel="stylesheet" href="../../css/pagination.css" />
    <link type="text/css" rel="stylesheet" href="../../JavaScript/Prompt/gridview.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.8.3.min.js"></script>
    <script type="text/javascript" src="../../scripts/ui/js/ligerBuild.min.js"></script>
    <script type="text/javascript" src="../js/function.js"></script>
    <script src="../../Script/formValidator/DateTimeMask.js" type="text/javascript"></script>
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
            <a href="javascript:history.go(-1);" class="back">后退</a>首页 &gt;短信报告 &gt;状态报告&gt; 状态报告明细
        </div>

        <div class="tab_con" style="display: block;">
            <table class="form_table">
                <tr>
                    <td style="width: 95%">
                        <asp:Label ID="lbl_message" runat="server" Text="没有返回状态报告" Font-Bold="True" Font-Size="Medium"></asp:Label>
                        <asp:Label ID="Label1" runat="server" Text="" Font-Bold="True" Font-Size="Medium"></asp:Label>
                    </td>
                    <td>
                        <asp:Button ID="btn_n" runat="server" Text="刷新" CssClass="btnSubmit" OnClick="btn_n_Click" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:GridView ID="GridView1" CssClass="gridview" runat="server" AutoGenerateColumns="False" DataKeyNames="Serial"
                            Width="100%" OnRowDataBound="GridView1_RowDataBound" AllowPaging="True" OnDataBound="GridView1_DataBound"
                            OnPageIndexChanging="GridView1_PageIndexChanging" PageSize="15" OnRowCommand="GridView1_RowCommand" OnRowDeleting="GridView1_RowDeleting">
                            <PagerTemplate>
                                <table>
                                    <tr>
                                        <td align="left">
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
                                <asp:BoundField DataField="Serial" HeaderText="网关返回序列号">
                                    <ItemStyle Width="13%" />
                                </asp:BoundField>
                                <%--                                <asp:BoundField DataField="Gateway" HeaderText="网关ID" />--%>
                                <asp:BoundField DataField="PhoneCount" HeaderText="电话号码">
                                    <ItemStyle Width="8%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="SMSContent" HeaderText="短信内容">
                                    <ItemStyle Width="25%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="SendTime" HeaderText="发送时间" />
                                <asp:BoundField DataField="ResponseTime" HeaderText="状态报告返回时间" />
                                <asp:BoundField DataField="SplitNumber" HeaderText="当前拆分" />
                                <asp:BoundField DataField="SplitTotal" HeaderText="拆分总条数" />
                                <asp:BoundField DataField="AuditTime" HeaderText="审核时间" Visible="false" />
                                <asp:BoundField DataField="BussType" HeaderText="业务类型" Visible="false" />
                                <asp:BoundField DataField="Level" HeaderText="短信优先级" Visible="false" />
                                <asp:BoundField DataField="StatusReport" HeaderText="状态报告" />
                                <asp:BoundField DataField="ReportStatus" HeaderText="状态码" />
                                <asp:BoundField DataField="Describe" HeaderText="状态描述" />
                                <asp:BoundField DataField="Succeed" HeaderText="发送成功" />
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
