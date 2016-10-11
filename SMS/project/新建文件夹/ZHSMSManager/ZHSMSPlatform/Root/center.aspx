<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="center.aspx.cs" Inherits="ZHCRM.center" %>

<%@ Import Namespace="Common" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>管理首页</title>
    <link href="images/style.css" rel="stylesheet" type="text/css" />
    <link href="../scripts/ui/skins/Aqua/css/ligerui-all.css" rel="stylesheet" type="text/css" />
    <link href="images/style.css" rel="stylesheet" type="text/css" />
    <script src="../scripts/jquery/jquery-1.8.3.min.js" type="text/javascript"></script>
    <script src="../scripts/ui/js/ligerBuild.min.js" type="text/javascript"></script>
    <script src="js/function.js" type="text/javascript"></script>
    <link href="../css/gonggao.css" rel="stylesheet" type="text/css" />
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
        <div class="gonggao_box">

            <div class="gonggao_box" id="tdInfo" runat="server">
            </div>
        </div>
        <div class="clear" style="height: 20px;"></div>

    </form>
</body>
</html>
