<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="ZHCRM.Root.index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>短信管理平台</title>
    <link href="../scripts/ui/skins/Aqua/css/ligerui-all.css" rel="stylesheet" type="text/css" />
    <link href="images/style.css" rel="stylesheet" type="text/css" />
    <script src="../scripts/jquery/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="../scripts/ui/js/ligerBuild.min.js" type="text/javascript"></script>
    <script src="js/function.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            //LoadData();
        });
        var tab = null;
        var accordion = null;
        var tree = null;
        $(function () {
            //页面布局
            $("#global_layout").ligerLayout({ leftWidth: 180, height: '100%', topHeight: 65, bottomHeight: 24, allowTopResize: false, allowBottomResize: false, allowLeftCollapse: true, onHeightChanged: f_heightChanged });

            var height = $(".l-layout-center").height();

            //Tab
            $("#framecenter").ligerTab({ height: height });

            //左边导航面板
            $("#global_left_nav").ligerAccordion({ height: height - 25, speed: null });

            $(".l-link").hover(function () {
                $(this).addClass("l-link-over");
            }, function () {
                $(this).removeClass("l-link-over");
            });

            //设置频道菜单
            $("#global_channel_tree").ligerTree({
                url: '../tools/admin_ajax.ashx?action=sys_channel_load',
                checkbox: false,
                nodeWidth: 112,
                //attribute: ['nodename', 'url'],
                onSelect: function (node) {
                    if (!node.data.url) return;
                    var tabid = $(node.target).attr("tabid");
                    if (!tabid) {
                        tabid = new Date().getTime();
                        $(node.target).attr("tabid", tabid)
                    }
                    f_addTab(tabid, node.data.text, node.data.url);
                }
            });

            //加载插件菜单
            loadPluginsNav();
            //快捷菜单
            var menu = $.ligerMenu({
                width: 120, items:
                [
                    { text: '管理首页', click: itemclick },
                    { text: '修改密码', click: itemclick },
                    { line: true },
                    { text: '关闭菜单', click: itemclick }
                ]
            });
            $("#tab-tools-nav").bind("click", function () {
                var offset = $(this).offset(); //取得事件对象的位置
                menu.show({ top: offset.top + 27, left: offset.left - 120 });
                return false;
            });

            tab = $("#framecenter").ligerGetTabManager();
            accordion = $("#global_left_nav").ligerGetAccordionManager();
            tree = $("#global_channel_tree").ligerGetTreeManager();
            //tree.expandAll(); //默认展开所有节点
            $("#pageloading_bg,#pageloading").hide();
        });

        //频道菜单异步加载函数，结合ligerMenu.js使用
        function loadChannelTree() {
            if (tree != null) {
                tree.clear();
                tree.loadData(null, "../tools/admin_ajax.ashx?action=sys_channel_load");
            }
        }

        //加载插件管理菜单
        function loadPluginsNav() {
            $.ajax({
                type: "POST",
                url: "../tools/admin_ajax.ashx?action=plugins_nav_load&time=" + Math.random(),
                timeout: 20000,
                beforeSend: function (XMLHttpRequest) {
                    $("#global_plugins").html("<div style=\"line-height:30px; text-align:center;\">正在加载，请稍候...</div>");
                },
                success: function (data, textStatus) {
                    $("#global_plugins").html(data);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    $("#global_plugins").html("<div style=\"line-height:30px; text-align:center;\">加载插件菜单出错！</div>");
                }
            });
        }

        //快捷菜单回调函数
        function itemclick(item) {
            switch (item.text) {
                case "首页":
                    f_addTab('home', '首页', 'center.aspx');
                    break;
                case "快捷导航":
                    //调用函数
                    break;
                case "修改密码":
                    f_addTab('passwordchange', '修改密码', 'Account/Password_Change.aspx');
                    break;
                default:
                    //关闭窗口
                    break;
            }
        }
        function f_heightChanged(options) {
            if (tab)
                tab.addHeight(options.diff);
            if (accordion && options.middleHeight - 24 > 0)
                accordion.setHeight(options.middleHeight - 24);
        }
        //添加Tab，可传3个参数
        function f_addTab(tabid, text, url, iconcss) {
            if (arguments.length == 4) {
                tab.addTabItem({ tabid: tabid, text: text, url: url, iconcss: iconcss });
            } else {
                tab.addTabItem({ tabid: tabid, text: text, url: url });
            }
        }
        //提示Dialog并关闭Tab
        function f_errorTab(tit, msg) {
            $.ligerDialog.open({
                isDrag: false,
                allowClose: false,
                type: 'error',
                title: tit,
                content: msg,
                buttons: [{
                    text: '确定',
                    onclick: function (item, dialog, index) {
                        //查找当前iframe名称
                        var itemiframe = "#framecenter .l-tab-content .l-tab-content-item";
                        var curriframe = "";
                        $(itemiframe).each(function () {
                            if ($(this).css("display") != "none") {
                                curriframe = $(this).attr("tabid");
                                return false;
                            }
                        });
                        if (curriframe != "") {
                            tab.removeTabItem(curriframe);
                            dialog.close();
                        }
                    }
                }]
            });
        }

        function LoadData() {
            $.ajax({
                type: "POST",
                url: "GetLoadMenu.ashx",
                data: {},
                dataType: "json",
                contentType: "application/json",
                success: function (obj) {
                    var sysModule = obj.SysModules;
                    $.each(sysModule, function (i, val) {
                        var str = "<div title='" + val.SysModuleTitle + "' iconcss='menu-icon-plugins' class='l-scroll'><ul id='global_plugins' class='nlist'>";
                        var list = val.Menus;
                        $.each(list, function (j, va) {
                            str = str + "<li><a  class='l-link' href='javascript:f_addTab('sys_config1','" + va.MenuTitle + "','" + va.MenuUrl + "')'>" + va.MenuTitle + "</a></li>";
                        });
                        str = str + "</ul></div>";
                        $("#global_left_nav").append(str);
                        alert(str);
                    });
                },
                error: function (x, e) {
                    alert("数据获取异常……。");
                    //alert(x.responseText);
                }
            });
        }
    </script>
</head>
<body style="padding: 0px;">
    <form id="form1" runat="server">
        <div class="pageloading_bg" id="pageloading_bg"></div>
        <div id="pageloading">数据加载中，请稍等...</div>
        <div id="global_layout" class="layout" style="width: 100%">
            <!--头部-->
            <div position="top" class="header">
                <div class="header_box">
                    <div class="header_right">
                        <span><b><%=account.UserCode %></b> 您好，欢迎光临</span>
                        <br />
                        <a href="javascript:f_addTab('home','首页','center.aspx')">首页</a> | 
                    <a href="javascript:f_addTab('passwordchange','密码修改','Account/Password_Change.aspx')">密码修改</a> | 
                    <asp:LinkButton ID="lbtnExit" runat="server" OnClick="lbtnExit_Click">安全退出</asp:LinkButton>
                    </div>
                    <a class="logo">logo</a>
                </div>
            </div>
            <!--左边-->
            <div position="left" title="管理菜单" id="global_left_nav">
                <div title="内容管理" iconcss="menu-icon-plugins" class="l-scroll">
                    <ul id="global_channel_tree" style="margin-top: 3px;">
                    </ul>
                </div>

            </div>
            <div position="center" id="framecenter" toolsid="tab-tools-nav">
                <div tabid="home" title="主页" iconcss="tab-icon-home" style="height: 300px">
                    <iframe frameborder="0" name="sysMain" src="center.aspx"></iframe>
                </div>
            </div>
            <div position="bottom" class="footer">
                <div class="copyright">Copyright &copy; 2010 - 2015. 技术支持：山东中呼信息科技有限公司. All Rights Reserved.</div>
            </div>
        </div>
    </form>
</body>
</html>
