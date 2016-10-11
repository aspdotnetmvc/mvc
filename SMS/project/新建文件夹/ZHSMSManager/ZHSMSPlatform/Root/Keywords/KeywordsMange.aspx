<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="KeywordsMange.aspx.cs" Inherits="ZHSMSPlatform.Root.Keywords.KeywordsMange" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>敏感词管理</title>
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
        <div class="navigation">首页 &gt; 敏感词管理 &gt; 敏感词列表</div>
        <div class="tools_box">
            <div class="tools_bar">
                <a href="KeywordAdd.aspx" class="tools_btn"><span><b class="add">敏感词添加</b></span></a>
                <a href="KeywordImport.aspx" class="tools_btn"><span><b class="export">敏感词批量导入</b></span></a>
                <a href="KeywordGroupAdd.aspx" class="tools_btn"><span><b class="badoo">新增敏感词组</b></span></a>
                <a href="KeywordsTypeAdd.aspx" class="tools_btn"><span><b class="box">新增敏感词类别</b></span></a>
            </div>
        </div>
        <ul class="tab_nav">
            <li class="selected"><a onclick="tabs('#contentTab',0);" href="javascript:;">敏感词管理</a></li>
        </ul>
        <div class="tab_con">
            <table class="form_table">
                <col width="150px">
                <col>
                <tbody>
                    <tr>
                        <th>敏感词组：</th>
                        <td>
                            <asp:DropDownList ID="dd_groups" runat="server"></asp:DropDownList>&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnFindByGroup" runat="server" Text="查询" CssClass="btnSubmit" OnClick="btnFindByGroup_Click" />
                        </td>
                    </tr>
                    <tr>
                        <th>敏感词类型：
                        </th>
                        <td>
                            <asp:DropDownList ID="dd_types" runat="server"></asp:DropDownList>&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnFindType" runat="server" Text="查询" CssClass="btnSubmit" OnClick="btnFindType_Click" />

                        </td>
                    </tr>
                    <tr>
                        <th>敏感词：</th>
                        <td>
                            <asp:TextBox ID="txt_keywords" runat="server" CssClass="txtInput" />&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnFindByKeyword" runat="server" Text="查询" CssClass="btnSubmit" OnClick="btnFindByKeyword_Click" />

                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="tab_con" style="display: block;">
            <table class="form_table">
                <tr>
                    <td>
                        <asp:GridView ID="GridView1" runat="server" CssClass="gridview" AutoGenerateColumns="False" DataKeyNames="Group,Keywords"
                            Width="100%" OnRowDataBound="GridView1_RowDataBound" PageSize="15" OnRowCommand="GridView1_RowCommand" OnRowDeleting="GridView1_RowDeleting">
                            <Columns>
                                <asp:BoundField DataField="ID" HeaderText="序号" />
                                <asp:BoundField DataField="Group" HeaderText="敏感词组" />
                                <asp:BoundField DataField="Keywords" HeaderText="敏感词" />
                                <asp:BoundField DataField="KeywordType" HeaderText="敏感词类型" />
                                <asp:BoundField DataField="Enable" HeaderText="是否启用" />
                                <asp:BoundField DataField="Replace" HeaderText="替换为其他词" />
                                <asp:TemplateField HeaderText="操作">
                                    <ItemTemplate>
                                        <asp:Button ID="btn_start" runat="server" CommandArgument='<%# Eval("Keywords")+","+Eval("Group") %>'
                                            CommandName="start" Text="启用" Width="85px" />
                                        <asp:Button ID="btn_stop" runat="server" CommandArgument='<%# Eval("Keywords")+","+Eval("Group") %>'
                                            CommandName="stop" Text="禁用" Width="85px" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:CommandField HeaderText="删除" ShowDeleteButton="True" DeleteText="删除"></asp:CommandField>
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
