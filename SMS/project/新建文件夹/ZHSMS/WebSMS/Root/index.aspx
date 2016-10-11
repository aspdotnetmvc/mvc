<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="WebSMS.Root.index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>SMS短信管理</title>
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



        //加载插件管理菜单
        function loadPluginsNav() {
            $.ajax({
                type: "POST",
                url: "" + Math.random(),
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
                    f_addTab('home', '首页', 'CC.aspx');
                    break;
                case "快捷导航":
                    //调用函数
                    break;
                case "修改密码":
                    f_addTab('manager_pwd', '修改密码', 'ACC/Account_Pwd.aspx');
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


    </script>
</head>
<body style="padding: 0px;">
    <form id="form1" runat="server">
        <div class="pageloading_bg" id="pageloading_bg"></div>
        <%-- <div id="pageloading">数据加载中，请稍等...</div>--%>
        <div id="global_layout" class="layout" style="width: 100%">
            <!--头部-->
            <div position="top" class="header">
                <div class="header_box">
                    <div class="header_right">
                        <asp:LinkButton ID="lbtnExit" runat="server" OnClick="lbtnExit_Click">安全退出</asp:LinkButton>
                    </div>
                </div>
            </div>
            <!--左边-->
            <div position="left" title="管理菜单" id="global_left_nav">
                <div title="账号" iconcss="menu-icon-plugins">
                    <ul id="ttttttt" class="nlist">
                        <li><a class="l-link" href="javascript:f_addTab('sys_config1','账号列表','ACC/Account_List.aspx')">账号列表</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config2','账号添加','ACC/Account_Add.aspx')">账号添加</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config3','充值记录','ACC/PrepaidRecord_List.aspx')">充值记录</a></li>

                    </ul>
                </div>
                <div title="短信" iconcss="menu-icon-plugins">
                    <ul id="tttt" class="nlist">
                        <li><a class="l-link" href="javascript:f_addTab('sys_config12','短信接收','SMSM/MoList.aspx')">短信接收</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config13','短信发送','SMSM/SMS_Send.aspx')">短信发送</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config14','短信列表','SMSM/SMS_List.aspx')">短信列表</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config15','短信审核','SMSM/SMS_Audit.aspx')">短信审核</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config16','短信优先级调整','SMSM/SMS_Level.aspx')">短信优先级</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config17','审核失败短信','SMSM/AuditFailure.aspx')">审核失败短信</a></li>
                    </ul>
                </div>
                <div title="报表" iconcss="menu-icon-plugins">
                    <ul id="t" class="nlist">
                        <li><a class="l-link" href="javascript:f_addTab('sys_config18','状态报告','Report/Rep_StatusList.aspx')">状态报告</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config19','统计状态报告','Report/Rep_StatisticsList.aspx')">统计状态报告</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config20','审核记录','Report/Rep_AuditRecordsList.aspx')">审核记录</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config21','优先级调整记录','Report/Rep_LevelModifyRecordsList.aspx')">优先级调整记录</a></li>
                    </ul>
                </div>
                <div title="黑名单" iconcss="menu-icon-plugins">
                    <ul id="t2" class="nlist">
                        <li><a class="l-link" href="javascript:f_addTab('sys_config31','添加黑名单','Back/Back_List.aspx')">黑名单列表</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config32','添加黑名单','Back/Back_Add.aspx')">添加黑名单</a></li>

                    </ul>
                </div>
                 <div title="关键词" iconcss="menu-icon-plugins">
                    <ul id="keyword" class="nlist">
                        <li><a class="l-link" href="javascript:f_addTab('sys_config33','关键词列表','Keywords/Keywords_List.aspx')">关键词列表</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config34','关键词添加','Keywords/Keywords_Add.aspx')">关键词添加</a></li>

                    </ul>
                </div>
                <div title="测试" iconcss="menu-icon-setting">
                    <ul id="pbxInfo" class="nlist">
                        <li><a class="l-link" href="javascript:f_addTab('sys_config41','账号测试','TestM/Accout_ModuleTest.aspx')">账号测试</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config42','短信发送测试','TestM/SMS_SendTest.aspx')">短信发送测试</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config43','批量审核','TestM/SMS_AuditListTest.aspx')">批量审核</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config45','批量调整优先级','TestM/SMS_LevelTest.aspx')">批量调整优先级</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config46','批量确认审核失败短信','TestM/AuditFailure_Test.aspx')">批量确认审核失败短信</a></li>

                    </ul>
                </div>
                <div title="内容管理" iconcss="menu-icon-model" class="l-scroll">
                    <ul id="global_channel_tree" style="margin-top: 3px;">

                        <%--<li isexpand="false"><span>资讯动态</span>
                        <ul> 
                            <li url="http://bbs.it134.cn"><span>内容管理</span></li>
                            <li url="demos/base/drag.htm"><span>栏目类别</span></li>
                        </ul>
                    </li>--%>
                    </ul>
                </div>
            </div>
            <div position="center" id="framecenter" toolsid="tab-tools-nav">
                <div tabid="home" title="首页" iconcss="tab-icon-home" style="height: 300px">
                    <iframe frameborder="0" name="sysMain" src="CC.aspx"></iframe>
                </div>
            </div>
            <div position="bottom" class="footer">
                <div class="copyright">Copyright &copy; 2009 - 2012. dtcms.net. All Rights Reserved.</div>
            </div>
        </div>
    </form>
</body>
</html>
