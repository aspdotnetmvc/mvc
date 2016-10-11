<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Assignment.aspx.cs" Inherits="ZKD.Root.Contacts.Assignment" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>通讯录分组</title>
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
        <div class="navigation">首页 &gt; 通讯录 &gt; 通讯录分组</div>
        <div style="width: 12%; height: 100%; background-color: #F0F8FF; float: left;">
            <table>
                <tr>
                    <td>
                        <asp:TreeView ID="TreeView1" runat="server" ImageSet="Inbox" OnSelectedNodeChanged="TreeView1_SelectedNodeChanged" Style="top: 0px" Height="800px" Width="140px" ShowLines="True">
                            <HoverNodeStyle Font-Underline="True" />
                            <NodeStyle Font-Names="Verdana" Font-Size="8pt" ForeColor="Black" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
                            <ParentNodeStyle Font-Bold="False" />
                            <SelectedNodeStyle Font-Underline="True" HorizontalPadding="0px" VerticalPadding="0px" />
                        </asp:TreeView>
                    </td>
                </tr>
            </table>
        </div>
        <div style="width: 88%; height: 95%; float: left;">
            <table class="form_table">
                <tr>
                    <td>
                        <asp:Label ID="lbl_message" runat="server" Text="没有号码可供分组" Font-Bold="True" Font-Size="Medium"></asp:Label>
                        <asp:GridView ID="GridView1" CssClass="gridview" runat="server" AutoGenerateColumns="False" DataKeyNames="PID,TelPhoneNum"
                            Width="100%" Height="100%" AllowPaging="True" OnDataBound="GridView1_DataBound"
                            OnPageIndexChanging="GridView1_PageIndexChanging" PageSize="15" OnRowDataBound="GridView1_RowDataBound" OnRowDeleting="GridView1_RowDeleting">
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
                                <asp:BoundField DataField="PID" HeaderText="编号" Visible="false" />
                                <asp:BoundField DataField="AccountCode" HeaderText="企业帐号" Visible="false" />
                                <asp:BoundField DataField="UserName" HeaderText="姓名" />
                                <asp:BoundField DataField="UserSex" HeaderText="性别" />
                                <asp:BoundField DataField="UserBrithday" HeaderText="生日" />
                                <asp:BoundField DataField="TelPhoneNum" HeaderText="电话号码" />
                                <asp:BoundField DataField="CompanyEmail" HeaderText="Email" />
                                <asp:BoundField DataField="QQ" HeaderText="QQ" />
                                <asp:BoundField DataField="WebChat" HeaderText="微信" />
                                <asp:BoundField DataField="CompanyName" HeaderText="公司名称" />
                                <asp:BoundField DataField="ComPostion" HeaderText="职位" />
                                <asp:BoundField DataField="CompanyWeb" HeaderText="公司网站" />
                                <asp:BoundField DataField="AddTime" HeaderText="创建时间" />
                                <asp:BoundField DataField="GroupID" HeaderText="值" Visible="false" />
                                <asp:BoundField DataField="GID" HeaderText="通讯录组编号" Visible="false" />
                                <asp:BoundField DataField="TelPhoneGroupName" HeaderText="通讯录组" />
                                <asp:BoundField DataField="ReMark" HeaderText="通讯录组描述" Visible="false" />
                                <asp:CommandField HeaderText="删除通讯录" ShowDeleteButton="True" DeleteText="删除">
                                    <ControlStyle ForeColor="Black" />
                                </asp:CommandField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </div>
        <div style="width: 50%; margin: 0 auto; height: 30px;">
            <asp:CheckBox ID="CheckBoxAll" runat="server" Text="全选" Width="80px" AutoPostBack="True" OnCheckedChanged="CheckBoxAll_CheckedChanged" />
            <asp:CheckBox ID="CheckBox1" runat="server" Text="反选" Width="80px" AutoPostBack="True" OnCheckedChanged="CheckBox1_CheckedChanged" />
            <asp:Button ID="btn_cancel" runat="server" Text="取消" CssClass="btnSubmit" OnClick="btn_cancel_Click" />
            &nbsp;  
                        <asp:Label ID="la1" Font-Bold="True" Font-Size="Medium" runat="server">请选择通讯录组</asp:Label>
            &nbsp;
            <asp:DropDownList ID="dd_l" runat="server">
            </asp:DropDownList>
            <asp:Button ID="btn_group" runat="server" Text="分组" CssClass="btnSubmit" OnClick="btn_group_Click" />
            &nbsp;    
            <asp:Button ID="btn_cancelGroup" runat="server" Text="取消分组" CssClass="btnSubmit" OnClick="btn_cancelGroup_Click" />
            &nbsp;  
           
             <asp:Button ID="btn_import" runat="server" Text="导出通讯录" CssClass="btnSubmit" OnClick="btn_import_Click" />
            &nbsp;  
            <asp:Button ID="btn_sendSMS" runat="server" Text="短信发送" CssClass="btnSubmit" OnClick="btn_sendSMS_Click" />
        </div>
    </form>
</body>
</html>
