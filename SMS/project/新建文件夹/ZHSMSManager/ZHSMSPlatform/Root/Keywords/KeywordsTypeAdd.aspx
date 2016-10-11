<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="KeywordsTypeAdd.aspx.cs" Inherits="ZHSMSPlatform.Root.Keywords.KeywordsTypeAdd" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>添加敏感词类型</title>
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
        <div class="navigation">首页 &gt; 敏感词管理 &gt; 敏感词类别添加</div>
        <div class="tools_box">
            <div class="tools_bar">
                <a href="KeywordsMange.aspx" class="tools_btn"><span><b class="return">返回敏感词管理</b></span></a>
                <a href="KeywordAdd.aspx" class="tools_btn"><span><b class="add">敏感词添加</b></span></a>
                <a href="KeywordImport.aspx" class="tools_btn"><span><b class="export">敏感词批量导入</b></span></a>
            </div>
        </div>
        <div id="contentTab">
            <ul class="tab_nav">
                <li class="selected"><a onclick="tabs('#contentTab',0);" href="javascript:;">敏感词类别添加</a></li>
            </ul>

            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <th>敏感词类型：</th>
                            <td>
                                <asp:TextBox ID="txt_keyType" runat="server" CssClass="txtInput normal required" MaxLength="32" />*
                            </td>
                        </tr>
                        <tr>
                            <th>备注：</th>
                            <td>
                                <asp:TextBox ID="txt_remark" TextMode="MultiLine" Width="360" Height="180" runat="server" CssClass="txtInput" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="foot_btn_box">
                <asp:Button ID="btnSubmit" runat="server" Text="添加" CssClass="btnSubmit" OnClick="btnSubmit_Click" />
                &nbsp;<input name="重置" type="reset" class="btnSubmit" value="重 置" />
            </div>
            <div class="tab_con" style="display: block;">
                <asp:GridView ID="GridView1" runat="server" CssClass="gridview" AutoGenerateColumns="False" DataKeyNames="Type"
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
                        <asp:BoundField DataField="ID" HeaderText="序号" />
                        <asp:BoundField DataField="Type" HeaderText="敏感词类型" />
                        <asp:TemplateField HeaderText="备注">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" ToolTip='<%#Eval("Remark") %>' Text='<%# Eval("Remark").ToString().Length>40?Eval("Remark").ToString().Substring(0,40)+"...":Eval("Remark")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        无记录
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </form>
</body>
</html>

