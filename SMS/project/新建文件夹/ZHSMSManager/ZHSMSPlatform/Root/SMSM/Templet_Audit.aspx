<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Templet_Audit.aspx.cs" Inherits="ZHSMSPlatform.Root.SMSM.Templet_Audit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>短信模板审核</title>
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
</head>
<body>
    <form id="form1" runat="server">
        <div class="navigation">首页 &gt; 短信审核 </div>

        <div class="tab_con" style="display: block;">
            <table class="form_table">
                <tr>
                    <td>开始时间
                        <asp:TextBox ID="txt_S" runat="server" CssClass="txtInput normal" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                            ControlToValidate="txt_S" Display="Dynamic"
                            ErrorMessage="*请填写日期"></asp:RequiredFieldValidator>
                        结束时间
                        <asp:TextBox ID="txt_E" runat="server" CssClass="txtInput normal" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                            ControlToValidate="txt_E" Display="Dynamic"
                            ErrorMessage="*请填写日期"></asp:RequiredFieldValidator>
                        <asp:Button ID="btn_nn" runat="server" Text="查询" CssClass="btnSubmit" OnClick="btn_nn_Click" />

                        <asp:Button ID="btn_timer" runat="server" Text="停止" OnClick="btn_timer_Click"/>
                   
                        <asp:Label ID="lbl_refresh" runat="server" Text="自动刷新(s)" ></asp:Label>
                        <asp:TextBox ID="txt_timespan" runat="server" Height="25" Width="55" Text="10" TextMode="Number"></asp:TextBox>
                        <asp:Timer ID="Timer1" runat="server"  Interval="10000" OnTick="Timer1_Tick"></asp:Timer>
                        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

                    </td>
                </tr>
            </table>
        </div>
        <div class="tab_con" style="display: block;">
            <table class="form_table">
                <tr>
                    <td>
                        <asp:GridView ID="GridView1" runat="server" CssClass="gridview" AutoGenerateColumns="False" DataKeyNames="TempletID"
                            Width="100%" OnRowDataBound="GridView1_RowDataBound" PageSize="15" OnRowCommand="GridView1_RowCommand" OnRowDeleting="GridView1_RowDeleting">
                            <Columns>
                                <%--<asp:BoundField DataField="ID" HeaderText="序号" />--%>
                                <asp:BoundField DataField="TempletID" HeaderText="模板ID" Visible="false" />
                                <asp:BoundField DataField="AccountCode" HeaderText="企业代码" />
                                <asp:BoundField DataField="AccountName" HeaderText="企业名称" />
                                <asp:TemplateField ItemStyle-Width="45%" HeaderText="模板内容">
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" ToolTip='<%#Eval("TempletContent") %>' Text='<%# Eval("TempletContent")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Signature" HeaderText="短信签名" />
                                <asp:BoundField DataField="SubmitTime" HeaderText="提交时间" />
                                <asp:TemplateField HeaderText="审核通过">
                                    <ItemTemplate>
                                        <asp:Button ID="btn_start" runat="server" CommandArgument='<%# Eval("TempletID")%>'
                                            CommandName="start" Text="通过" Width="85px" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="审核不通过">
                                    <ItemTemplate>
                                        <asp:Button ID="btn_Failure" runat="server" CommandArgument='<%# Eval("TempletID")%>' CommandName="failure" Text="不通过"/>
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
