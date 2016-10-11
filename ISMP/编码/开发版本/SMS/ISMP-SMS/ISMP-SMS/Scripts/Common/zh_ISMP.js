$zh = {
    //请求处理数据
    ajax: function (data, url, fnsucess, fnerror) {
        $.ajax({
            url: url,
            type: "POST",
            cache: false,
            data: data,
            success: function (rdata) {
                rdata = eval("(" + rdata + ")");
                fnsucess(rdata);
            }, error: fnerror
        });
    },
    ajaxGet: function (data, url, fnsucess, fnerror) {
        $.ajax({
            url: url,
            type: "Get",
            cache: false,
            data: data,
            success: function (rdata) {
                rdata = eval("(" + rdata + ")");
                fnsucess(rdata);
            }, error: fnerror
        });
    },
    GetRequestParam: function GetRequest() {
        var url = location.search; //获取url中"?"符后的字串
        var theRequest = new Object();
        if (url.indexOf("?") != -1) {
            var str = url.substr(1);
            strs = str.split("&");
            for (var i = 0; i < strs.length; i++) {
                theRequest[strs[i].split("=")[0]] = decodeURI(strs[i].split("=")[1]);
            }
        }
        return theRequest;
    },
    //验证控件是否有权限，返回验证结果，不做处理
    checkPermission: function (control, action, pageAction) {
        if (!pageAction)
            pageAction = "Index";
        var pageFlag = "/" + control + "/" + pageAction;
        var operateFlag = "/" + control + "/" + action;

        var permlist = window.top.ISMP_CurPagePermissionList;
        var plist = permlist[pageFlag];
        try {
            var curlist;
            if (plist) {
                var c = plist[operateFlag];
                if (pageFlag == operateFlag || c) {
                    return true;
                }
            }
        }
        catch (e) { }
        return false;
    },
    //验证按钮控件是否有权限，并做处理
    checkPermission_Button: function (control, actions, pageAction) {
        if (!pageAction)
            pageAction = "Index";
        var pageFlag = "/" + control + "/" + pageAction;

        var permlist = window.top.ISMP_CurPagePermissionList;
        var plist = permlist[pageFlag];
        var cs = new Array();
        cs = actions.split(",");
        for (i = 0; i < cs.length ; i++) {
            try {
                var operateFlag = "/" + control + "/" + cs[i];
                var curlist;
                if (plist) {
                    var c = plist[operateFlag];
                    if (pageFlag == operateFlag || c) {
                        $("[permission_id='" + cs[i] + "']").show();
                    }
                    else {
                        $("[permission_id='" + cs[i] + "']").remove();
                    }
                }
                else {
                    $("[permission_id='" + cs[i] + "']").remove();
                }
            }
            catch (e) { }
        }
    },
    //验证Grid隐藏列和操作按钮是否有权限，并做处理
    checkPermission_Grid: function (control, gridAction, buttonActions, pageAction) {
        if (!pageAction)
            pageAction = "Index";
        var pageFlag = "/" + control + "/" + pageAction;
        var permlist = window.top.ISMP_CurPagePermissionList;
        var plist = permlist[pageFlag];

        var gridPath = "/" + control + "/" + gridAction;
        try {
            var datagrid = $("table[permission_id='" + gridAction + "']");
            if (plist) {
                var c = plist[gridPath];
                if (c && c.HideColumns.length > 0) {
                    var columns = datagrid.datagrid('options').columns[0];
                    for (var j = 0; j < c.HideColumns.length; j++) {
                        for (var m = 0; m < columns.length; m++) {
                            if (columns[m].title == c.HideColumns[j]) {
                                datagrid.datagrid('removeColumn', columns[m].field);
                                break;
                            }
                        }

                    }
                }
                if (buttonActions != "") {
                    var gridpanel = datagrid.datagrid("getPanel");
                    var grid_btns = new Array();
                    grid_btns = buttonActions.split(",");
                    for (i = 0; i < grid_btns.length ; i++) {
                        try {
                            var btnpath = "/" + control + "/" + grid_btns[i];
                            if (plist) {
                                var c = plist[btnpath];
                                if (pageFlag == btnpath || c) {
                                    gridpanel.find("a[permission_id='" + grid_btns[i] + "']").show();
                                }
                                else {
                                    gridpanel.find("a[permission_id='" + grid_btns[i] + "']").remove();
                                }
                            }
                            else {
                                gridpanel.find("a[permission_id='" + grid_btns + "']").remove();
                            }
                        }
                        catch (e) { }
                    }
                }
            }
            else {
                if (buttonActions != "") {
                    var gridpanel = datagrid.datagrid("getPanel");
                    var grid_btns = new Array();
                    grid_btns = buttonActions.split(",");
                    for (i = 0; i < grid_btns.length ; i++) {
                        try {
                            gridpanel.find("a[permission_id='" + grid_btns[i] + "']").remove();
                        }
                        catch (e) { }
                    }
                }
            }
        }
        catch (e) { }
    },

    openTab: function (url, title, dataid) {
        var tabid = new Date().getTime();
        if (dataid) {
            tabid = dataid;
        }
        var $a = $("<a href='" + encodeURI(url) + "' data-id='" + tabid + "' class='ismp-tabopen' title='" + title + "'></a>");
        $("body").append($a);
        $($a).on("click", function (event) { window.top.ismpTab.iframeopentab(this, event) });
        $($a).click();
        setTimeout(function () { $($a).remove(); }, 1000);
    },
    //显示更多按钮 id：menu的Id,jq 要显示的菜单的位置参照物(this,或者jq对象)，index：grid 行号，onclick: 菜单响应函数需要menuitem 和index 作为参数
    showMoreMenu: function (id, jq, onclick) {
        $('#' + id).menu({
            onClick: function (item) {
                onclick(item);
            }
        });
        var offset = $(jq).offset();
        var top = offset.top + $(jq).height() + 8;
        var left = offset.left + $(jq).outerWidth() + 2;
        $('#' + id).menu('show', {
            top: top,
            left: offset.left
        });
        if ($(".menu").css("display") == "block") {
            $(this).children('i').attr("class", " fa-angle-up");
        }
        //$('#Menu_MoreAction_Grid').toggle();
    }
};
//定义一些多页面调用的公共方法
$zhop = {
    //发起呼叫
    MobileCall: function (mobileNumber) {
        toastr.options = {
            "closeButton": false,
            "debug": false,
            "positionClass": "toast-top-full-width",
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "3000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };
        $.messager.confirm('提示', '确定要发起呼叫吗?', function (r) {
            if (r) {
                var jstr = $.ajax({
                    url: "/Agent/SendCallFromLoginUser",
                    data: { Mobile: mobileNumber },
                    method: "post",
                    async: false
                }).responseText;

                var msg = eval("(" + jstr + ")");
                if (msg.success) {
                    toastr.success("发起呼叫，正在转接，请稍后...");
                }
                else {
                    $.messager.alert("提示", msg.message, msg.type);
                }
            }
        });


        //$.messager.confirm('提示', '确定要发起呼叫吗?', function (r) {
        //    if (r) {
        //        toastr.success("发起呼叫，正在转接，请稍后...");
        //    }
        //});
    }
    //SendEmail: function (email) {
    //    //$.messager.confirm('提示', '确定要发送邮件吗?', function (r) {
    //    //    if (r) {
    //            var jstr = $.ajax({
    //                url: "/Agent/SendEmailFromLoginUser",
    //                data: { Email: email },
    //                method: "post",
    //                async: false
    //            }).responseText;

    //            var msg = eval("(" + jstr + ")");
    //            if (msg.success) {
    //            }
    //            else {
    //                $.messager.alert("提示", msg.message, msg.type);
    //            }
    //    //    }
    //    //});
    //}
};

$(function () {
    $.extend($.fn.validatebox.defaults.rules, {
        DHYZ: {
            validator: function (value) {
                return /((\d{10})|(\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)/.test(value);
            },
            message: "请输入正确的电话号码！"
        },
        mobile: {
            validator: function (value) {
                return /^0?1\d{10}$/.test(value);
            },
            message: "请输入有效的手机号码！"
        },
        account: {
            validator: function (value) {
                return /^[a-zA-Z0-9_]+$/.test(value);
            },
            message: "用户名只允许字母数字及下划线！"
        }
    });
    if (typeof (toastr) != "undefined") {
        toastr.options = {
            "closeButton": false,
            "debug": false,
            "positionClass": "toast-top-full-width",
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "3000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };
    }
});



$formatter = {
    datetime: function (val, row, index) {
        return val;
    },
    date: function (val, row, index) {
        if (val) {
            return val.split(' ')[0];
        } else {
            return val;
        }
    },
    bool: function (val, row, index) {
        if (val > 0) {
            return "<i class='fa fa-check green_ziti'></i>";
        } else {
            return "<i class='fa fa-times red_ziti'></i>";
        }
    },
    smstype: function (val, row, index) {  //短信类型
        if (val == 0) {
            return "行业";
        }
        if (val == 1) {
            return "商业";
        }
        if (val == 2) {
            return "四大类";
        }
    }
};

