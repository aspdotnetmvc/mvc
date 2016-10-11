<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SMS_Audit.aspx.cs" Inherits="ZHSMSPlatform.Root.SMSM.SMS_Audit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>短信审核</title>
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
                        <asp:TextBox ID="txt_S" runat="server" CssClass="txtInput normal" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})" Width="150"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                            ControlToValidate="txt_S" Display="Dynamic"
                            ErrorMessage="*请填写日期"></asp:RequiredFieldValidator>
                        结束时间
                        <asp:TextBox ID="txt_E" runat="server" CssClass="txtInput normal" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})" Width="150"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                            ControlToValidate="txt_E" Display="Dynamic"
                            ErrorMessage="*请填写日期"></asp:RequiredFieldValidator>
                        企业代码或名称
                            <asp:TextBox ID="txt_enterprise" runat="server" CssClass="txtInput" Height="25" Width="180" />
                        <asp:Button ID="btn_nn" runat="server" Text="查询" CssClass="btnSubmit" OnClick="btn_nn_Click" />
                  
                        <asp:Button ID="btn_timer" runat="server" Text="停止" OnClick="btn_timer_Click"/>
                   
                        <asp:Label ID="lbl_refresh" runat="server" Text="自动刷新(s)" ></asp:Label>
                        <asp:TextBox ID="txt_timespan" runat="server" Height="25" Width="55" Text="10" TextMode="Number"></asp:TextBox>
                        <asp:Timer ID="Timer1" runat="server"  Interval="10000" OnTick="Timer1_Tick"></asp:Timer>
                        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lbl_message" runat="server" Text="没有短信审核" Font-Bold="True" Font-Size="Medium"></asp:Label>
                        <asp:Label ID="Label1" runat="server" Text="" Font-Bold="True" Font-Size="Medium"></asp:Label>
                        <asp:GridView ID="GridView1" CssClass="gridview" runat="server" AutoGenerateColumns="False" DataKeyNames="SerialNumber"
                            Width="100%" AllowPaging="True" OnDataBound="GridView1_DataBound"
                            OnPageIndexChanging="GridView1_PageIndexChanging" PageSize="15">
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
                                <asp:TemplateField HeaderText="选择">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="CheckBox" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="SerialNumber" Visible="false" HeaderText="短信标识" />
                                <asp:BoundField DataField="EnterpriseName" HeaderText="企业名称" />
                                <asp:TemplateField HeaderText="短信内容">
                                    <ItemTemplate>
                                        <%--<asp:Label ID="Label1" runat="server" ToolTip='<%#Eval("SMSContent") %>' Text='<%# Eval("SMSContent").ToString().Length>20?Eval("SMSContent").ToString().Substring(0,20)+"...":Eval("SMSContent")%>'></asp:Label>--%>
                                        <asp:Label ID="Label1" runat="server" ToolTip='<%#Eval("SMSContent") %>' Text='<%# Eval("SMSContent")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="StatusReport" HeaderText="状态报告" />
                                <asp:BoundField DataField="Level" HeaderText="短信优先级" />
                                <asp:BoundField DataField="SendTime" HeaderText="短信提交时间" />
                                <asp:BoundField DataField="BussType" HeaderText="业务类型" Visible="false" />
                                <asp:BoundField DataField="Signature" HeaderText="签名" />
                                <asp:TemplateField HeaderText="电话号码">
                                    <ItemTemplate>
                                        <asp:Label ID="Label2" runat="server" ToolTip='<%#Eval("Number") %>' Text='<%# Eval("Number").ToString().Length>20?Eval("Number").ToString().Substring(0,20)+"...":Eval("Number")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>

                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </div>
        <div class="foot_btn_box">
            <asp:CheckBox ID="CheckBoxAll" runat="server" Text="全选" Width="80px" AutoPostBack="True" OnCheckedChanged="CheckBoxAll_CheckedChanged" />
            <asp:CheckBox ID="CheckBox1" runat="server" Text="反选" Width="80px" AutoPostBack="True" OnCheckedChanged="CheckBox1_CheckedChanged" />
            <asp:Button ID="bt_n" runat="server" Text="取消" CssClass="btnSubmit" OnClick="bt_n_Click" />
            &nbsp;    
            <asp:Button ID="bt1" runat="server" Text="审核成功" CssClass="btnSubmit" OnClick="bt1_Click" />
            &nbsp;    
            <asp:Button ID="bt2" runat="server" Text="审核失败" CssClass="btnSubmit" OnClick="bt2_Click" />

        </div>
    </form>
</body>
</html>
