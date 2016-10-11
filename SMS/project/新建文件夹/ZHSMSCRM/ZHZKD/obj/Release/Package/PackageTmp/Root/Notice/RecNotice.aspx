<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecNotice.aspx.cs" Inherits="ZKD.Root.Notice.RecNotice" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>公告列表</title>
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
    <link href="../../css/gonggao.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(function () {
            $(".gg_con_title").click(function () {
                var num = $(this).index();
                $(this).hide();
                $(".gonggao_con li").eq(num + 1).slideDown();
            });
            $(".gg_open_title").mouseover(function () {
                $(this).css({ "backgroundImage": "url(../../images/login/gonggao_title_bj_2.jpg)", "cursor": "pointer" });
            });
            $(".gg_open_title").mouseout(function () {
                $(this).css({ "backgroundImage": "url(../../images/login/gonggao_title_bj_1.jpg)", "cursor": "" });
            });
            $(".gg_open_title").click(function () {
                var num = $(this).parent().index();
                $(this).parent().slideUp(500, function () {
                    $(".gonggao_con li").eq(num - 1).show();
                });
            });
        });
    </script>
</head>

<body>
    <form id="form1" runat="server">
        <div style="display: block;">
            <div class="navigation">首页 &gt; 公告 &gt; 公告列表</div>
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
                    </td>
                </tr>
            </table>

            <div class="gonggao_box" id="tdInfo" runat="server" style="float:left;" >
            </div>
        </div>
    </form>
</body>
</html>
