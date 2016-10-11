<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Rep_StatisticsList.aspx.cs" Inherits="ZKD.Root.Report.Rep_StatisticsList" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>状态报告</title>
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
    <link type="text/css" rel="stylesheet" href="../../css/tab.css" />
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
        <div class="tab_con" style="display: block;">
            <div class="navigation">首页 &gt; 短信报告 &gt; 状态报告</div>
            <table class="form_table">
                <tr>
                    <td style="width: 95%">
                        <asp:Label ID="lbl_message" runat="server" Text="当前没有返回状态报告" Font-Bold="True" Font-Size="Medium"></asp:Label>
                        <div id="div_query" runat="server">
                            电话号码：
                            <asp:TextBox ID="txt_Num" runat="server" CssClass="txtInput normal"></asp:TextBox>
                            <asp:Button ID="btn_n" runat="server" Text="刷新" CssClass="btnSubmit" OnClick="btn_n_Click" />
                        </div>
                        <asp:Label ID="Label1" runat="server" Text="" Font-Bold="True" Font-Size="Medium"></asp:Label>
                        <p style="color:red">说明：按行业标准，系统以72小时为一个周期准确显示发送结果。请点击【历史状态报告】查看。</p>
                    </td>
                    <td>
                      
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:GridView ID="GridView1" CssClass="gridview" runat="server" AutoGenerateColumns="False" DataKeyNames="SerialNumber"
                            Width="100%" OnRowDataBound="GridView1_RowDataBound" AllowPaging="True" OnDataBound="GridView1_DataBound"
                            OnPageIndexChanging="GridView1_PageIndexChanging" PageSize="15" AllowSorting="True" OnSorting="GridView1_Sorting">
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
                                <asp:BoundField DataField="SerialNumber" Visible="false" HeaderText="业务流水号">
                                    <ItemStyle Width="19%" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="电话号码">
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" ToolTip='<%#Eval("PhoneCount") %>' Text='<%# Eval("PhoneCount").ToString().Length>20?Eval("PhoneCount").ToString().Substring(0,20)+"...":Eval("PhoneCount")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="短信内容">
                                    <ItemTemplate>
                                       <%-- <asp:Label ID="Label2" runat="server" ToolTip='<%#Eval("SMSContent") %>' Text='<%# Eval("SMSContent").ToString().Length>20?Eval("SMSContent").ToString().Substring(0,20)+"...":Eval("SMSContent")%>'></asp:Label>--%>
                                         <asp:Label ID="Label2" runat="server" ToolTip='<%#Eval("SMSContent") %>' Text='<%# Eval("SMSContent")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="BeginSendTime" HeaderText="开始发送时间"   />
                                <asp:BoundField DataField="LastResponseTime" HeaderText="状态报告返回时间"/>
                                <asp:BoundField DataField="Numbers" HeaderText="号码数" />
                                <asp:BoundField DataField="SendCount" HeaderText="发送条数" />
                                <asp:BoundField DataField="SplitNumber" HeaderText="拆分条数" />
                                <asp:BoundField DataField="Succeed" HeaderText="成功条数" />
                                <asp:BoundField DataField="FailureCount" HeaderText="失败条数" />
                                <asp:BoundField DataField="Account" HeaderText="" Visible="false" />
                                <asp:TemplateField HeaderText="操作">
                                    <ItemTemplate>
                                        <a href="Rep_Statistics.aspx?SendTime=<%# Eval("SendTime")%>&&SerialNumber=<%# Eval("SerialNumber")%>">查看短信明细</a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>

            </table>
        </div>
    </form>
</body>
</html>
