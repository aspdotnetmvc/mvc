<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SMS_Level.aspx.cs" Inherits="ZHSMSPlatform.Root.SMSM.SMS_Level" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>短信优先级调整</title>
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
        <div class="tab_con" style="display: block;">
            <table class="form_table">
                <tr>
                    <td>
                        <asp:Label ID="lbl_message" runat="server" Text="没有短信优先级调整" Font-Bold="True" Font-Size="Medium"></asp:Label>
                        <asp:Label ID="Label1" runat="server" Text="" Font-Bold="True" Font-Size="Medium"></asp:Label>
                        <asp:GridView ID="GridView1" CssClass="gridview" runat="server" AutoGenerateColumns="False" DataKeyNames="SerialNumber"
                            Width="100%" OnRowDataBound="GridView1_RowDataBound" AllowPaging="True" OnDataBound="GridView1_DataBound"
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
                                <asp:BoundField DataField="SerialNumber" HeaderText="短信标识"/>
                                <asp:TemplateField HeaderText="短信内容">
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" ToolTip='<%#Eval("SMSContent") %>' Text='<%# Eval("SMSContent").ToString().Length>20?Eval("SMSContent").ToString().Substring(0,20)+"...":Eval("SMSContent")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Level" HeaderText="短信优先级" />
                                <asp:BoundField DataField="SendTime" HeaderText="短信提交时间" />
                                <asp:BoundField DataField="AuditTime" HeaderText="审核时间" />
                                <asp:BoundField DataField="BussType" HeaderText="短信通道" Visible="false" />
                                <asp:BoundField DataField="Signature" HeaderText="签名" />
                                <asp:TemplateField HeaderText="电话号码">
                                    <ItemTemplate>
                                        <asp:Label ID="Label2" runat="server" ToolTip='<%#Eval("PhoneCount") %>' Text='<%# Eval("PhoneCount").ToString().Length>20?Eval("PhoneCount").ToString().Substring(0,20)+"...":Eval("PhoneCount")%>'></asp:Label>
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
            <asp:Label ID="la1" Font-Bold="True" Font-Size="Medium" runat="server">请选择要调整的优先级</asp:Label>
            &nbsp;
            <asp:DropDownList ID="dd_l" runat="server">
                <asp:ListItem Value="Level0">0</asp:ListItem>
                <asp:ListItem Value="Level1">1</asp:ListItem>
                <asp:ListItem Value="Level2">2</asp:ListItem>
                <asp:ListItem Value="Level3">3</asp:ListItem>
                <asp:ListItem Value="Level4">4</asp:ListItem>
                <asp:ListItem Value="Level5">5</asp:ListItem>
                <asp:ListItem Value="Level6">6</asp:ListItem>
                <asp:ListItem Value="Level7">7</asp:ListItem>
                <asp:ListItem Value="Level8">8</asp:ListItem>
                <asp:ListItem Value="Level9">9</asp:ListItem>
                <asp:ListItem Value="Level10">10</asp:ListItem>
                <asp:ListItem Value="Level11">11</asp:ListItem>
                <asp:ListItem Value="Level12">12</asp:ListItem>
            </asp:DropDownList>
            <asp:Button ID="bt1" runat="server" Text="短信优先级调整" CssClass="btnSubmit" OnClick="bt1_Click" />
        </div>
    </form>
</body>
</html>
