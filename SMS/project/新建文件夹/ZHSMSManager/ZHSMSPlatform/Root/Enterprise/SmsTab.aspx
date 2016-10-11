<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SmsTab.aspx.cs" Inherits="ZHSMSPlatform.Root.Enterprise.SmsTab" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link type="text/css" rel="stylesheet" href="../../scripts/ui/skins/Aqua/css/ligerui-all.css" />
    <link type="text/css" rel="stylesheet" href="../images/style.css" />
    <link type="text/css" rel="stylesheet" href="../../css/pagination.css" />
    <link type="text/css" rel="stylesheet" href="../../JavaScript/Prompt/gridview.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.3.2.min.js"></script>
    <script type="text/javascript" src="../../scripts/ui/js/ligerBuild.min.js"></script>
    <script type="text/javascript" src="../js/function.js"></script>
    <script type="text/javascript" src="../../JavaScript/Prompt/ymPrompt.js"></script>
    <link rel="stylesheet" type="text/css" href="../../JavaScript/Prompt/skin/qq/ymPrompt.css" />
    <link type="text/css" rel="stylesheet" href="../../css/tab.css" />
    <script type="text/javascript">
        $(document).ready(function () {
            LoadData();
        });
        function LoadData() {
            var url = "";
            $.ajax({
                type: "POST",
                url: "GetSMSMenu.ashx",
                data: {},
                dataType: "json",
                contentType: "application/json",
                success: function (obj) {
                    var sysModule = obj.SysModules;
                    $.each(sysModule, function (i, val) {
                        var str = "<ul>";
                        var list = val.Menus;
                        $.each(list, function (j, va) {
                            str = str + "<li><a href='" + va.MenuUrl + "?AccountID=<%=Request.QueryString["accountID"]%>'><span>" + va.MenuTitle + "</span></a></li>";
                            <%--url = va.MenuUrl + "?AccountID=<%=Request.QueryString["accountID"]%>";--%>
                        });
                        str = str + "</ul>";
                        $("#tabsJ").append(str);
                    });
                },
                error: function (x, e) {
                    alert("数据获取异常……。");
                }
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="tabsJ">
        </div>
    <asp:Label runat="server" ID="aa"></asp:Label>
    </form>

</body>
</html>
