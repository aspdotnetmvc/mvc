<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Rep_StatisticsList.aspx.cs" Inherits="ZHSMSPlatform.Root.Report.Rep_StatisticsList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>统计状态报告</title>
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
    <script src="../../Script/formValidator/DateTimeMask.js" type="text/javascript"></script>
    <script src="../../Script/formValidator/datepicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../../JavaScript/Prompt/ymPrompt.js"></script>
    <link rel="stylesheet" type="text/css" href="../../JavaScript/Prompt/skin/qq/ymPrompt.css" />
    <link type="text/css" rel="stylesheet" href="../../css/tab.css" />
    <script type="text/javascript">
        $(document).ready(function () {
            LoadData();
        });
        function LoadData() {
            $.ajax({
                type: "POST",
                url: "../Enterprise/GetSMSMenu.ashx",
                data: {},
                dataType: "json",
                contentType: "application/json",
                success: function (obj) {
                    var sysModule = obj.SysModules;
                    $.each(sysModule, function (i, val) {
                        var str = "<ul>";
                        var list = val.Menus;
                        $.each(list, function (j, va) {
                            str = str + "<li><a href='" + va.MenuUrl + "?AccountID=<%=Request.QueryString["AccountID"]%>'><span>" + va.MenuTitle + "</span></a></li>";
                        });
                        str = str + "</ul>";
                        $("#tabsJ").append(str);
                        //  alert(str);
                    });
                },
                error: function (x, e) {
                    alert("数据获取异常……。");
                    //alert(x.responseText);
                }
            });
            }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="tabsJ">
        </div>
        <div style="clear:both;"></div>
        <div class="tab_con" style="display: block;">
            <table class="form_table">
                <tr>
                    <td>开始时间
                        <asp:TextBox ID="txt_Start" runat="server" CssClass="txtInput normal" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})"
                            ></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                            ControlToValidate="txt_Start" Display="Dynamic"
                            ErrorMessage="*请填写日期"></asp:RequiredFieldValidator>
                        结束时间
                        <asp:TextBox ID="txt_End" runat="server" CssClass="txtInput normal" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})"
                           ></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                            ControlToValidate="txt_End" Display="Dynamic"
                            ErrorMessage="*请填写日期"></asp:RequiredFieldValidator>
                        手机号码：<asp:TextBox ID="txt_num" runat ="server" CssClass="txtInput normal" ></asp:TextBox>
                        <asp:Button ID="btn_Query" runat="server" Text="查询" CssClass="btnSubmit" OnClick="btnQuery_Click" />
                    </td>
                </tr>
                <tr>
                    <td colspan="10">
                        <asp:Label ID="lbl_message" runat="server" Text="状态报告统计" Font-Bold="True" Font-Size="Medium"></asp:Label>
                        <asp:GridView ID="GridView1" CssClass="gridview" runat="server" AutoGenerateColumns="False" DataKeyNames="SerialNumber"
                            Width="100%" OnRowDataBound="GridView1_RowDataBound" AllowPaging="True" OnDataBound="GridView1_DataBound"
                            OnPageIndexChanging="GridView1_PageIndexChanging" PageSize="15">
                            <Columns>
                                <asp:BoundField DataField="SerialNumber" Visible="false" HeaderText="业务流水号">
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="电话号码">
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" ToolTip='<%#Eval("PhoneCount") %>' Text='<%# Eval("PhoneCount").ToString().Length>20?Eval("PhoneCount").ToString().Substring(0,20)+"...":Eval("PhoneCount")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="短信内容">
                                    <ItemTemplate>
                                        <%--<asp:Label ID="Label2" runat="server" ToolTip='<%#Eval("SMSContent") %>' Text='<%# Eval("SMSContent").ToString().Length>20?Eval("SMSContent").ToString().Substring(0,20)+"...":Eval("SMSContent")%>'></asp:Label>--%>
                                        <asp:Label ID="Label2" runat="server" ToolTip='<%#Eval("SMSContent") %>' Text='<%# Eval("SMSContent")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="BeginSendTime" HeaderText="开始发送时间" />
                                <asp:BoundField DataField="LastResponseTime" HeaderText="报告最后返回时间" />
                                <asp:BoundField DataField="Numbers" HeaderText="号码数" />
                                <asp:BoundField DataField="SendCount" HeaderText="发送短信总数" />
                                <asp:BoundField DataField="SplitNumber" HeaderText="拆分条数" />
                                <asp:BoundField DataField="Succeed" HeaderText="成功条数" />
                                <asp:BoundField DataField="FailureCount" HeaderText="短信失败数" />
                                <asp:BoundField DataField="Account" HeaderText="" Visible="false"/>
                                <asp:TemplateField HeaderText="操作">
                                    <ItemTemplate>
                                        <a href="Rep_Status.aspx?AccountID=<%#Eval("Account") %>&&SendTime=<%# Eval("SendTime")%>&&SerialNumber=<%# Eval("SerialNumber")%>">查看短信明细</a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                             <EmptyDataTemplate>
                                无记录
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
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
