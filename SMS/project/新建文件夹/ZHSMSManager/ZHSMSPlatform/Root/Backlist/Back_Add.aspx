<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Back_Add.aspx.cs" Inherits="ZHSMSPlatform.Root.Backlist.Back_Add" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
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
<body class="mainbody">
    <form id="form1" runat="server">
        <div class="navigation">首页 &gt; 黑名单添加 </div>
        <div id="contentTab">
            <div class="tools_box">
                <div class="tools_bar">
                    <a href="Back_List.aspx" class="tools_btn"><span><b class="all">返回黑名单列表</b></span></a>
                </div>
            </div>

            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <th>黑名单号码：</th>
                            <td>
                                <asp:TextBox ID="txt_phone" runat="server" CssClass="txtInput normal required" MaxLength="11" /><label></label>
                            </td>
                        </tr>
                        <tr>
                            <th>批量上传：</th>
                            <td>
                                <asp:FileUpload ID="FileUpload1" runat="server" />
                                <asp:Button ID="btn_import" runat="server" Text="增加" OnClick="btn_import_Click" />
                                <asp:Label ID="Label1" runat="server" Text="备注：选择文件为txt格式，每个号码之间用，隔开" ForeColor="#CC00FF"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <asp:GridView ID="GridView1" SkinID="GridViewSkin" runat="server" AutoGenerateColumns="False"
                                    Width="80%" AllowPaging="True" OnDataBound="GridView1_DataBound" CssClass="gridview"
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
                                        <asp:BoundField DataField="phone" HeaderText="号码" />
                                    </Columns>
                                </asp:GridView>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="foot_btn_box">
                <asp:Button ID="btnSubmit" runat="server" Text="添加" CssClass="btnSubmit" OnClick="btnSubmit_Click" />
                &nbsp;<input name="重置" type="reset" class="btnSubmit" value="重 置" />
            </div>
        </div>
    </form>
</body>
</html>
