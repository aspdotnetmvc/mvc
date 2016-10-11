<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="center.aspx.cs" Inherits="ZKD.center" %>

<%@ Import Namespace="Common" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>管理首页</title>
    <link href="../scripts/ui/skins/Aqua/css/ligerui-all.css" rel="stylesheet" type="text/css" />
    <link href="images/style.css" rel="stylesheet" type="text/css" />
    <link href="../css/gonggao.css" rel="Stylesheet" type="text/css" />
    <link type="text/css" rel="stylesheet" href="../../css/msg.css" />
    <script type="text/javascript" src="../scripts/jquery/jquery-1.8.3.min.js"></script>
    <script src="../scripts/ui/js/ligerBuild.min.js" type="text/javascript"></script>
    <script src="../../Script/formValidator/DateTimeMask.js" type="text/javascript"></script>
<%--    <script src="../../Script/formValidator/datepicker/calendar.js" type="text/javascript"></script>--%>
    <script src="../../Script/formValidator/datepicker/WdatePicker.js" type="text/javascript"></script>
    <script src="js/function.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $(".gg_con_title").click(function () {
                var num = $(this).index();
                $(this).hide();
                $(".gonggao_con li").eq(num + 1).slideDown();
            });
            $(".gg_open_title").mouseover(function () {
                $(this).css({ "backgroundImage": "url(../images/login/gonggao_title_bj_2.jpg)", "cursor": "pointer" });
            });
            $(".gg_open_title").mouseout(function () {
                $(this).css({ "backgroundImage": "url(../images/login/gonggao_title_bj_1.jpg)", "cursor": "" });
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
<body class="mainbody">
    <form id="form1" runat="server">
        <%--<div class="navigation nav_icon">你好，<i><%=user.AccountCode %></i>，欢迎进入终端用户管理平台</div>--%>
        <div class="gonggao_box">
            <div class="gonggao_title">今日统计</div>
            <div class="msg_con line">
                <ul>
                    <li style="margin-left: 100px">
                        <table style="line-height: 30px" width="700px">
                            <tr>
                                <td style="width: 100px">剩余短信条数</td>
                                <td style="width: 200px; text-decoration: underline">
                                    <a href="javascript:parent.f_addTab('sys_config32','剩余短信条数','Enterprise/EnterpriseBalance.aspx')">
                                        <asp:Label ID="lbl_smsCount" runat="server"></asp:Label></a>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100px">已发送短信条数</td>
                                <td style="width: 200px; text-decoration: underline">
                                    <a href="javascript:parent.f_addTab('sys_config17','已发送短信条数','Report/Rep_StatisticsList.aspx')">

                                        <asp:Label ID="lbl_sendCount" runat="server"></asp:Label></a></td>
                            </tr>
                            <tr>
                                <td>发送成功条数</td>
                                <td style="width: 100px; text-decoration: underline">
                                    <a href="javascript:parent.f_addTab('sys_config17','发送成功条数','Report/Rep_StatisticsList.aspx')">
                                        <asp:Label ID="lbl_sucCount" runat="server"></asp:Label></a></td>
                            </tr>
                            <tr>
                                <td>发送失败条数</td>
                                <td style="width: 100px; text-decoration: underline">
                                    <a href="javascript:parent.f_addTab('sys_config17','发送失败条数','Report/Rep_StatisticsList.aspx')">
                                        <asp:Label ID="lbl_lossCount" runat="server"></asp:Label></a></td>
                            </tr>
                            <tr>
                                <td>接收短信条数</td>
                                <td style="width: 100px; text-decoration: underline">
                                    <a href="javascript:parent.f_addTab('sys_config3','接收短信条数','SMSM/MOList.aspx')">
                                        <asp:Label ID="lbl_recCount" runat="server"></asp:Label></a></td>
                            </tr>
                            <tr>
                                <td>未处理短信批次</td>
                                <td style="width:auto">
                                        <asp:Label ID="lbl_undealt" runat="server"></asp:Label></td>
                            </tr>
                             <tr>
                                 <td></td>
                                <td><p style="color:red">注意：具体发送的短信条数取决于接收号码的数量和短信内容的拆分条数</p></td>
                            </tr>
                            <tr>
                                <td>审核失败条数</td>
                                <td style="width: 100px; text-decoration: underline">
                                    <a href="javascript:parent.f_addTab('sys_config6','审核失败条数','SMSM/AuditFailure.aspx')">
                                        <asp:Label ID="lbl_audit" runat="server"></asp:Label></a></td>
                            </tr>


                        </table>
                    </li>



                </ul>


            </div>

            <div class="gonggao_box" id="tdInfo" runat="server">
            </div>
    </form>
</body>
</html>
