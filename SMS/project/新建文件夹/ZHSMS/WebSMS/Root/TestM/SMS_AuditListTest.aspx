﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SMS_AuditListTest.aspx.cs" Inherits="WebSMS.Root.TestM.SMS_AuditListTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>短信批量审核</title>
    <link type="text/css" rel="stylesheet" href="../../scripts/ui/skins/Aqua/css/ligerui-all.css" />
    <link type="text/css" rel="stylesheet" href="../images/style.css" />
    <link type="text/css" rel="stylesheet" href="../../css/pagination.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.3.2.min.js"></script>
    <script type="text/javascript" src="../../scripts/ui/js/ligerBuild.min.js"></script>
    <script type="text/javascript" src="../js/function.js"></script>
    <script type="text/javascript" src="../../JavaScript/Prompt/ymPrompt.js"></script>
    <link rel="stylesheet" type="text/css" href="../../JavaScript/Prompt/skin/qq/ymPrompt.css" />
</head>
<body class="mainbody">
    <form id="form1" runat="server">
        <div class="tools_box">
            <div class="tools_bar">
                <a href="../SMSM/SMS_List.aspx" class="tools_btn"><span><b class="all">返回短信审核</b></span></a>
            </div>
        </div>
        <div class="tab_con" style="display: block;">
            <table class="form_table">
                <tr>
                    <td>
                        <asp:Label ID="lbl_message" runat="server" Text="没有短信批量审核" Font-Bold="True" Font-Size="Medium"></asp:Label>
                        <asp:Label ID="Label1" runat="server" Text="" Font-Bold="True" Font-Size="Medium"></asp:Label>
                        <asp:GridView ID="GridView1" SkinID="GridViewSkin" runat="server" AutoGenerateColumns="False" DataKeyNames="SerialNumber"
                            Width="100%" AllowPaging="True" OnDataBound="GridView1_DataBound"
                            OnPageIndexChanging="GridView1_PageIndexChanging" PageSize="15" CellPadding="3" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px">
                            <FooterStyle BackColor="White" ForeColor="#000066" />
                            <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
                            <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
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
                                <asp:BoundField DataField="SerialNumber" HeaderText="业务流水号" />
                                <asp:BoundField DataField="AccountID" HeaderText="企业账号" />
                                <asp:BoundField DataField="SMSContent" HeaderText="短信内容" />
                                <asp:BoundField DataField="StatusReport" HeaderText="状态报告" />
                                <asp:BoundField DataField="Level" HeaderText="短信优先级" />
                                <asp:BoundField DataField="SendTime" HeaderText="发送时间" />
                                <asp:BoundField DataField="BussType" HeaderText="业务类型" />
                                <asp:BoundField DataField="Signature" HeaderText="签名" />
                                <asp:BoundField DataField="Number" HeaderText="电话号码" />
                            </Columns>
                            <RowStyle ForeColor="#000066" />
                            <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                            <SortedAscendingCellStyle BackColor="#F1F1F1" />
                            <SortedAscendingHeaderStyle BackColor="#007DBB" />
                            <SortedDescendingCellStyle BackColor="#CAC9C9" />
                            <SortedDescendingHeaderStyle BackColor="#00547E" />
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
            <asp:Button ID="bt1" runat="server" Text="批量审核成功" CssClass="btnSubmit" OnClick="bt1_Click" />
            &nbsp;    
            <asp:Button ID="bt2" runat="server" Text="批量审核失败" CssClass="btnSubmit" OnClick="bt2_Click" />
        </div>
    </form>
</body>
</html>
