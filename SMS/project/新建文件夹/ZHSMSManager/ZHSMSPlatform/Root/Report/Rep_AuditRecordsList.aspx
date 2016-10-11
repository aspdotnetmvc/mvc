<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Rep_AuditRecordsList.aspx.cs" Inherits="ZHSMSPlatform.Root.Report.Rep_AuditRecordsList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>审核记录</title>
    <link type="text/css" rel="stylesheet" href="../../scripts/ui/skins/Aqua/css/ligerui-all.css" />
    <link type="text/css" rel="stylesheet" href="../images/style.css" />
    <link type="text/css" rel="stylesheet" href="../../css/pagination.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.3.2.min.js"></script>
    <script type="text/javascript" src="../../scripts/ui/js/ligerBuild.min.js"></script>
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
    <script type="text/javascript" src="../js/function.js"></script>
    <script src="../../Script/formValidator/DateTimeMask.js" type="text/javascript"></script>
    <script src="../../Script/formValidator/datepicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../../JavaScript/Prompt/ymPrompt.js"></script>
    <link rel="stylesheet" type="text/css" href="../../JavaScript/Prompt/skin/qq/ymPrompt.css" />
</head>
<body class="mainbody">
    <form id="form1" runat="server">
        <div class="navigation">首页 &gt;记录&gt;  审核记录</div>
        <div class="tab_con" style="display: block;">
            <table class="form_table">
                <tr>
                    <td>开始时间
                        <asp:TextBox ID="txt_Start" runat="server" CssClass="txtInput normal" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                            ControlToValidate="txt_Start" Display="Dynamic"
                            ErrorMessage="*请填写日期"></asp:RequiredFieldValidator>
                        结束时间
                        <asp:TextBox ID="txt_End" runat="server" CssClass="txtInput normal" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                            ControlToValidate="txt_End" Display="Dynamic"
                            ErrorMessage="*请填写日期"></asp:RequiredFieldValidator>
                        审核结果
                        <asp:RadioButton ID="rbtnAll" GroupName="AuditOption" Text="全部" AutoPostBack="true" runat="server" />&nbsp;&nbsp;<asp:RadioButton ID="rbtnSuccess" GroupName="AuditOption" Text="审核通过" AutoPostBack="true" runat="server" />&nbsp;&nbsp;<asp:RadioButton ID="rbtnFailure" AutoPostBack="true" GroupName="AuditOption" Text="审核失败" Checked="true" runat="server" />
                        <asp:Button ID="btnQuery" runat="server" Text="查询" CssClass="btnSubmit" OnClick="btnQuery_Click" />
                    </td>
                   
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lbl_message" runat="server" Text="没有审核记录" Font-Bold="True" Font-Size="Medium"></asp:Label>
                        <asp:Label ID="Label1" runat="server" Text="" Font-Bold="True" Font-Size="Medium"></asp:Label>
                        <asp:GridView ID="GridView1" SkinID="GridViewSkin" CssClass="gridview" runat="server" AutoGenerateColumns="False" DataKeyNames="SerialNumber"
                            Width="100%" OnRowDataBound="GridView1_RowDataBound" AllowPaging="True" OnDataBound="GridView1_DataBound" OnPageIndexChanging="GridView1_PageIndexChanging" PageSize="15">
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
                                <asp:BoundField DataField="SerialNumber" Visible="false" HeaderText="短信标识" />
                                <asp:BoundField DataField="AccountID" HeaderText="审核者" />
                                <asp:TemplateField HeaderText="短信内容">
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" ToolTip='<%#Eval("Content") %>' Text='<%# Eval("Content").ToString().Length>20?Eval("Content").ToString().Substring(0,20)+"...":Eval("Content")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="SendTime" HeaderText="短信提交时间" />
                                <asp:BoundField DataField="AuditTime" HeaderText="审核时间" />
                                <asp:BoundField DataField="Result" HeaderText="审核结果" />
                                <asp:BoundField DataField="FailureCase" HeaderText="原因" />
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
