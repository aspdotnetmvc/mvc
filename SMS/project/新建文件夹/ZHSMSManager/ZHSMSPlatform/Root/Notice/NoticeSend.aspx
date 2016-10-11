<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NoticeSend.aspx.cs" Inherits="ZHSMSPlatform.Root.Notice.NoticeSend" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>公告发送</title>
 <link type="text/css" rel="stylesheet" href="../../scripts/ui/skins/Aqua/css/ligerui-all.css" />
    <link type="text/css" rel="stylesheet" href="../images/style.css" />
    <link type="text/css" rel="stylesheet" href="../../css/pagination.css" />
    <link type="text/css" rel="stylesheet" href="../../JavaScript/Prompt/gridview.css" />
    <script type="text/javascript" src="../../JavaScript/Prompt/jquery-1.8.3.min.js"></script>
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.3.2.min.js"></script>
    <script type="text/javascript" src="../../scripts/ui/js/ligerBuild.min.js"></script>
    <script type="text/javascript" src="../js/function.js"></script>
    <script src="../../Script/formValidator/DateTimeMask.js" type="text/javascript"></script>
    <script src="../../Script/formValidator/datepicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../../JavaScript/Prompt/ymPrompt.js"></script>
    <link rel="stylesheet" type="text/css" href="../../JavaScript/Prompt/skin/qq/ymPrompt.css" />
    <link type="text/css" rel="stylesheet" href="../../css/tab.css" />
    <link href="../../css/fabugonggao.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../../JavaScript/Prompt/jquery-1.8.3.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $(".fabugonggao_box").height($(window).height()-1);
            var box_height = $(".fabugonggao_box").height();
            var box_width = $(".fabugonggao_box").width();
            $(".lianxiren").height(box_height);
            $(".bianji_box").height(box_height);
            $(".user_list").height(box_height - 120);
            $(".bianji_box").width(box_width - 300);
            $(".bianji_box").height(box_height);
            $(".con textarea").height(box_height - 200);

            $(window).resize(function () {
                $(".fabugonggao_box").height($(window).height()-1);
                var box_height = $(".fabugonggao_box").height();
                var box_width = $(".fabugonggao_box").width();
                $(".lianxiren").height(box_height);
                $(".bianji_box").height(box_height);
                $(".user_list").height(box_height - 120);
                $(".bianji_box").width(box_width - 300);
                $(".bianji_box").height(box_height);
                $(".con textarea").height(box_height - 200);
            });
        });
</script>


    <script type="text/javascript">
        // 点击TreeView复选框时触发事件
        function postBackByObject() {
            var o = window.event.srcElement;
            if (o.tagName == "INPUT" && o.type == "checkbox") {
                __doPostBack("", "");
            }
        }
    </script>
</head>

<body>
    <form id="form1" runat="server">
        <div class="fabugonggao_box">
            <div class="bianji_box ">
                <table class="bianji">
                    <tr>
                        <td valign="top" width="3%" class="title">标题</td>
                        <td class="con" width="95%">
                            <input type="text" class="title_con" runat="server" id="txt_title" /></td>
                    </tr>
                    <tr>
                        <td height="10px"></td>
                    </tr>
                    <tr>
                        <td height="10px"></td>
                    </tr>
                    <tr>
                        <td valign="top" class="title">内容</td>
                        <td class="con">
                            <textarea class="title_con" runat="server" id="txt_content"></textarea></td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button ID="btn_Sendnotice" runat="server" CssClass="fasong_btn" Text="发送" OnClick="btn_Sendnotice_Click" />
                        </td>
                    </tr>
                </table>

            </div>
            <div class="lianxiren">
                <div class="gonggao_search">
                    <input type="text" class="search_text" runat="server" id="txt_search" />
                    <asp:Button ID="btn_search" CssClass="search_button" runat="server" Text="" OnClick="btn_search_Click" />
                </div>
                <div class="user_list">
                    <table>
                        <tr>
                            <td>
                                <asp:TreeView ID="TreeView1" runat="server" ShowCheckBoxes="All" OnTreeNodeCheckChanged="TreeView1_TreeNodeCheckChanged"></asp:TreeView>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
