<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuditFailure.aspx.cs" Inherits="ZKD.Root.SMSM.AuditFailure" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>审核失败短信</title>
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
<body>
    <form id="form1" runat="server">
        <div class="navigation">首页 &gt; 短信管理 &gt; 审核失败短信</div>
        <div class="tab_con" style="display: block;">
            <table class="form_table">
                <tr>
                    <td>
                        <asp:Label ID="lbl_message" runat="server" Text="没有审核失败短信" Font-Bold="True" Font-Size="Medium"></asp:Label>
                        <asp:Label ID="Label1" runat="server" Text="" Font-Bold="True" Font-Size="Medium"></asp:Label>
                        <asp:GridView ID="GridView1" CssClass="gridview" runat="server" AutoGenerateColumns="False" DataKeyNames="SerialNumber"
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
                                <asp:BoundField DataField="SerialNumber" HeaderText="业务流水号">
                                    <ItemStyle Width="15%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="SMSContent" HeaderText="短信内容">
                                    <ItemStyle Width="15%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="StatusReport" HeaderText="状态报告" />
                                <asp:BoundField DataField="Level" HeaderText="短信优先级" />
                                <asp:BoundField DataField="SendTime" HeaderText="发送时间" />
                                <%-- <asp:BoundField DataField="AuditTime" HeaderText="审核时间" />
                                <asp:BoundField DataField="BussType" HeaderText="业务类型" />--%>
                                <asp:BoundField DataField="Signature" HeaderText="签名" />
                                <asp:BoundField DataField="PhoneCount" HeaderText="电话号码" />
                                <asp:BoundField DataField="FailureCase" HeaderText="审核失败原因" />
                                <%-- <asp:TemplateField HeaderText="操作">
                                    <ItemTemplate>
                                        <asp:Button ID="btn_start" runat="server" CommandArgument='<%# Bind("SerialNumber") %>'
                                            CommandName="start" Text="审核成功" Width="100px" />
                                        <asp:Button ID="btn_stop" runat="server" CommandArgument='<%# Bind("SerialNumber") %>'
                                            CommandName="stop" Text="审核失败" Width="100px" />

                                    </ItemTemplate>
                                </asp:TemplateField>--%>
                                <asp:TemplateField HeaderText="重新编辑">
                                    <ItemTemplate>
                                        <a href="ReSend.aspx?SerialNumber=<%# Eval("SerialNumber")%>">重新编辑</a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:CommandField HeaderText="确认" ShowDeleteButton="True" DeleteText="确认">
                                    <ControlStyle ForeColor="Black" />
                                </asp:CommandField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
