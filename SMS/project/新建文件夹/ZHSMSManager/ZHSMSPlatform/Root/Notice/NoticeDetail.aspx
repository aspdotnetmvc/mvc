<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NoticeDetail.aspx.cs" Inherits="ZHSMSPlatform.Root.Notice.NoticeDetail" %>

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
    <script type="text/javascript" src="../../scripts/ui/js/ligerBuild.min.js"></script>
    <script type="text/javascript" src="../js/function.js"></script>
    <script src="../../Script/formValidator/DateTimeMask.js" type="text/javascript"></script>
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
        <div class="navigation">
            <a href="javascript:history.go(-1);" class="back">后退</a>首页 &gt; 公告 &gt; 公告详情
        </div>
        <div class="tab_con" style="display: block;">
            <div class="gonggao_box" id="tdInfo" runat="server">
            </div>
        </div>
    </form>
</body>
</html>
