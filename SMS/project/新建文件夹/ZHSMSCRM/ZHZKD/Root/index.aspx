<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="ZKD.Root.index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>终端用户管理平台</title>
    <link href="../scripts/ui/skins/Aqua/css/ligerui-all.css" rel="stylesheet" type="text/css" />
    <link href="images/style.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../scripts/jquery/jquery-1.8.3.min.js"></script>
    <script src="../scripts/ui/js/ligerBuild.min.js" type="text/javascript"></script>
    <script src="js/function.js" type="text/javascript"></script>
    <script src="../Script/formValidator/DateTimeMask.js" type="text/javascript"></script>
   <%-- <script src="../Script/formValidator/datepicker/calendar.js" type="text/javascript"></script>--%>
    <script src="../Script/formValidator/datepicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
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



            ////加载插件菜单
            //loadPluginsNav();
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
                    f_addTab('manager_pwd', '修改密码', '/Root/Enterprise/EnterprisePass.aspx');
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
                url: "",
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
                        <span><b><%=user.AccountCode%></b> 您好，欢迎光临</span>
                        <input type="hidden" id="hh1" value="<%=user.IsAgent%>" />
                        <br />
                        <a href="javascript:f_addTab('home','首页','center.aspx')">首页</a> | 
                    <a href="javascript:f_addTab('passwordchange','密码修改','Enterprise/EnterprisePass.aspx')">密码修改</a> | 
                    <asp:LinkButton ID="lbtnExit" runat="server" OnClick="lbtnExit_Click">安全退出</asp:LinkButton>
                    </div>
                    <a class="logo">logo</a>
                </div>
            </div>
            <!--左边-->
            <div position="left" title="管理菜单" id="global_left_nav">
                <div title="公告" iconcss="menu-icon-plugins">
                    <ul id="Announcement" class="nlist">
                        <li><a class="l-link" href="javascript:f_addTab('sys_config60','公告','Notice/RecNotice.aspx')">公告</a></li>

                    </ul>
                </div>
                <div title="通讯录" iconcss="menu-icon-plugins">
                    <ul id="Contact" class="nlist">
                        <li><a class="l-link" href="javascript:f_addTab('sys_config40','通讯录添加','Contacts/PhoneSingle.aspx')">通讯录添加</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config44','批量通讯录导入','Contacts/BulkUploadContacts.aspx')">批量通讯录导入</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config45','通讯录CSV导入','Contacts/CsvUpload.aspx')">通讯录CSV导入</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config46','通讯录组列表','Contacts/GroupList.aspx')">通讯录组列表</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config47','通讯录分组','Contacts/Assignment.aspx')">通讯录分组</a></li>
                    </ul>
                </div>
                <div title="短信管理" iconcss="menu-icon-plugins">
                    <ul id="SMSManagement" class="nlist">
                        <li><a class="l-link" href="javascript:f_addTab('sys_config1','按号码发送短信','SMSM/SMS_Send.aspx')">按号码发送短信</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config12','短信模板管理','SMSM/SMSTemplet.aspx')">短信模板管理</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config3','短信接收','SMSM/MoList.aspx')">短信接收</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config6','审核失败短信','SMSM/AuditFailure.aspx')">审核失败短信</a></li>
                    </ul>
                </div>
                <div title="短信报告" iconcss="menu-icon-plugins">
                    <ul id="SMSReport" class="nlist">
                        <li><a class="l-link" href="javascript:f_addTab('sys_config17','状态报告','Report/Rep_StatisticsList.aspx')">状态报告(3天以内)</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config18','历史状态报告','Report/Rep_HistoryList.aspx')">历史状态报告</a></li>

                    </ul>
                </div>
                <div title="企业管理" iconcss="menu-icon-plugins">
                    <ul id="EnManagement" class="nlist">
                        <li><a class="l-link" href="javascript:f_addTab('sys_config31','修改密码','Enterprise/EnterprisePass.aspx')">修改密码</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config32','余额查询','Enterprise/EnterpriseBalance.aspx')">余额查询</a></li>
                    </ul>
                </div>

                <div id="me" title="终端企业管理" iconcss="menu-icon-plugins" runat="server">
                    <ul id="CustomerManagement" class="nlist">
                        <li><a class="l-link" href="javascript:f_addTab('sys_config51','添加企业','Enterprise/EnterpriseAdd.aspx')">添加企业</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config52','终端企业列表','Agent/AgentLowerEnterpriseManage.aspx')">终端企业列表</a></li>
                        <li><a class="l-link" href="javascript:f_addTab('sys_config53','企业审核状态','Agent/FailAgentLowerEnterprise.aspx')">企业审核状态</a></li>
                    </ul>
                </div>

            </div>
            <div position="center" id="framecenter" toolsid="tab-tools-nav">
                <div tabid="home" title="首页" iconcss="tab-icon-home" style="height: 300px">
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
