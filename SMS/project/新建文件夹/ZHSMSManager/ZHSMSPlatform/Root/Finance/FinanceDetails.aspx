<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FinanceDetails.aspx.cs" Inherits="ZHSMSPlatform.Root.Finance.FinanceDetails" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>财务明细与统计</title>
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
    <script type="text/javascript" language="javascript" src="../../JavaScript/Prompt/ymPrompt.js"></script>
    <link rel="stylesheet" type="text/css" href="../../JavaScript/Prompt/skin/qq/ymPrompt.css" />
    <style type="text/css">
        .auto-style1 {
            height: 35px;
        }
    </style>
</head>
<body class="mainbody">
    <form id="form1" runat="server">
        <div class="navigation">首页 &gt; 企业用户管理 &gt; 管理列表</div>
        <div class="tab_con">
            <table class="form_table">
                <col width="150px">
                <col>
                <tbody>
                    <tr>
                        <th>企业帐号或名称：</th>
                        <td>
                            <asp:TextBox ID="txt_name" runat="server" CssClass="txtInput" Height="25" Width="180" />&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnFind" runat="server" Text="查询" CssClass="btnSubmit" OnClick="btnFind_Click" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="tab_con" style="display: block;">
            <table class="form_table">
                <col width="150px">
                <col>
                <tbody>
                    <tr>
                        <th rowspan="2" style="text-align: center; font-size: 14px;">   &nbsp;   企业总营收合计：
                        </th>
                        <th class="auto-style1">企业总充值金额</th>

                        <th class="auto-style1">企业总充值短信数</th>

                        <th class="auto-style1">企业总发送短信数</th>

                        <th class="auto-style1">企业总剩余短信数</th>
                    </tr>
                    <tr>
                        <td style="width: 20%; text-align: center"><asp:Label Text="0" ID="lbl_money" ForeColor="#33CCFF" runat="server" Font-Bold="True" Font-Size="14px" /></td>
                        <td style="width: 20%; text-align: center"><asp:Label Text="0" ID="lbl_smsCount" runat="server" ForeColor="#33CCFF" Font-Bold="True" Font-Size="14px" /></td>
                        <td style="width: 20%; text-align: center"><asp:Label Text="0" ID="lbl_sendSMSCount" runat="server" ForeColor="#33CCFF" Font-Bold="True" Font-Size="14px" /></td>
                        <td style="width: 20%; text-align: center"><asp:Label Text="0" ID="lbl_remainSMSCount" runat="server" ForeColor="#33CCFF" Font-Bold="True" Font-Size="14px" /></td>
                    </tr>
                </tbody>
            </table>
        </div><br />
        <div>
            <asp:GridView ID="GridView1" CssClass="gridview" runat="server" AutoGenerateColumns="False"
                Width="100%" OnRowDataBound="GridView1_RowDataBound" AllowPaging="True" OnDataBound="GridView1_DataBound"
                OnPageIndexChanging="GridView1_PageIndexChanging" PageSize="15">
                <PagerTemplate>
                    <table>
                        <tr>
                            <td style="text-align: left">
                                <asp:Label ID="MessageLabel" ForeColor="black" Text="页码:" runat="server" />
                                <asp:DropDownList ID="PageDropDownList" AutoPostBack="true" OnSelectedIndexChanged="PageDropDownList_SelectedIndexChanged"
                                    runat="server" />
                                <asp:LinkButton CommandName="Page" CommandArgument="First" ID="linkBtnFirst" runat="server">首页</asp:LinkButton>
                                <asp:LinkButton CommandName="Page" CommandArgument="Prev" ID="linkBtnPrev" runat="server">上一页</asp:LinkButton>
                                <asp:LinkButton CommandName="Page" CommandArgument="Next" ID="linkBtnNext" runat="server">下一页</asp:LinkButton>
                                <asp:LinkButton CommandName="Page" CommandArgument="Last" ID="linkBtnLast" runat="server">尾页</asp:LinkButton>
                            </td>
                            <td style="width: 260px"></td>
                            <td>
                                <asp:Label ID="CurrentPageLabel" ForeColor="black" runat="server" />
                            </td>
                        </tr>
                    </table>
                </PagerTemplate>
                <Columns>
                    <asp:BoundField DataField="ID" HeaderText="序号" />
                    <asp:BoundField DataField="accountCode" HeaderText="企业帐号" />
                    <asp:BoundField DataField="moneny" HeaderText="充值金额总数" />
                    <asp:BoundField DataField="smsCount" HeaderText="充值短信总数" />
                    <asp:BoundField DataField="sendCount" HeaderText="发送短信总数" />
                    <asp:BoundField DataField="remainCount" HeaderText="剩余发送短信数" />
                </Columns>
                <EmptyDataTemplate>
                    无记录
                </EmptyDataTemplate>
            </asp:GridView>

        </div>
    </form>
</body>
</html>
