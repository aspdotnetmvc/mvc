/**
* jQuery ligerUI 1.0.2
* 
* Author leoxie [ gd_star@163.com ] 
* 
*/
(function($) {
    $.ligerDefaults = $.ligerDefaults || {};
    $.ligerDefaults.Spinner = {
        type: 'float',     //类型 float:浮点数 int:整数 time:时间
        isNegative: true, //是否负数
        decimalplace: 2,   //小数位 type=float时起作用
        step: 0.1,         //每次增加的值
        interval: 50,      //间隔，毫秒
        onChangeValue: false    //改变值事件
    };
    ///	<param name="$" type="jQuery"></param>;
    $.fn.ligerSpinner = function(p) { 
        return this.each(function() {
            if (this.useSpinner) return;
            if ($(this).attr("ligerui"))
            {
                try
                {
                    var attroptions = $(this).attr("ligerui");
                    if (attroptions.indexOf('{') < 0)  attroptions = "{" + attroptions + "}";
                    eval("attroptions = " + attroptions + ";");
                    if (attroptions) p = $.extend({}, attroptions, p || {});
                }
                catch (e) { }
            } 
            p = $.extend({}, $.ligerDefaults.Spinner, p || {});
            if (p.type == 'float')
            {
                p.step = 0.1;
                p.interval = 50;
            } else if (p.type == 'int')
            {
                p.step = 1;
                p.interval = 100;
            } else if (p.type == 'time')
            {
                p.step = 1;
                p.interval = 100;
            }
            var g = {
                round: function(v, e) {
                    var t = 1;
                    for (; e > 0; t *= 10, e--);
                    for (; e < 0; t /= 10, e++);
                    return Math.round(v * t) / t;
                },
                isInt: function(str) {
                    var strP = p.isNegative ? /^-?\d+$/ : /^\d+$/;
                    if (!strP.test(str)) return false;
                    if (parseFloat(str) != str) return false;
                    return true;
                },
                isFloat: function(str) {
                    var strP = p.isNegative ? /^-?\d+(\.\d+)?$/ : /^\d+(\.\d+)?$/;
                    if (!strP.test(str)) return false;
                    if (parseFloat(str) != str) return false;
                    return true;
                },
                isTime: function(str) {
                    var a = str.match(/^(\d{1,2}):(\d{1,2})$/);
                    if (a == null) return false;
                    if (a[1] > 24 || a[2] > 60) return false;
                    return true;

                },
                isVerify: function(str) {
                    if (p.type == 'float') {
                        return g.isFloat(str);
                    } else if (p.type == 'int') {
                        return g.isInt(str);
                    } else if (p.type == 'time') {
                        return g.isTime(str);
                    }
                    return false;
                },
                getVerifyValue: function(value) {
                    var newvalue = null;
                    if (p.type == 'float') {
                        newvalue = g.round(value, p.decimalplace);
                    } else if (p.type == 'int') {
                        newvalue = parseInt(value);
                    } else if (p.type == 'time') {
                        newvalue = value;
                    }
                    if (!g.isVerify(newvalue)) {
                        return g.value;
                    } else {
                        return newvalue;
                    }
                },
                getDefaultValue: function() {
                    if (p.type == 'float' || p.type == 'int') { return 0; }
                    else if (p.type == 'time') { return "00:00"; }
                },
                addValue: function(num) {
                    var value = g.inputText.val();
                    value = parseFloat(value) + num;
                    g.inputText.val(value);
                    g.inputText.trigger("change");
                },
                addTime: function(minute) {
                    var value = g.inputText.val();
                    var a = value.match(/^(\d{1,2}):(\d{1,2})$/);
                    newminute = parseInt(a[2]) + minute;
                    if (newminute < 10) newminute = "0" + newminute;
                    value = a[1] + ":" + newminute;
                    g.inputText.val(value);
                    g.inputText.trigger("change");
                },
                uping: function() {
                    if (p.type == 'float' || p.type == 'int') {
                        g.addValue(p.step);
                    } else if (p.type == 'time') {
                        g.addTime(p.step);
                    }
                },
                downing: function() {
                    if (p.type == 'float' || p.type == 'int') {
                        g.addValue(-1 * p.step);
                    } else if (p.type == 'time') {
                        g.addTime(-1 * p.step);
                    }
                },
                isDateTime: function(dateStr) {
                    var r = dateStr.match(/^(\d{1,4})(-|\/)(\d{1,2})\2(\d{1,2})$/);
                    if (r == null) return false;
                    var d = new Date(r[1], r[3] - 1, r[4]);
                    if (d == "NaN") return false;
                    return (d.getFullYear() == r[1] && (d.getMonth() + 1) == r[3] && d.getDate() == r[4]);
                },
                isLongDateTime: function(dateStr) {
                    var reg = /^(\d{1,4})(-|\/)(\d{1,2})\2(\d{1,2}) (\d{1,2}):(\d{1,2})$/;
                    var r = dateStr.match(reg);
                    if (r == null) return false;
                    var d = new Date(r[1], r[3] - 1, r[4], r[5], r[6]);
                    if (d == "NaN") return false;
                    return (d.getFullYear() == r[1] && (d.getMonth() + 1) == r[3] && d.getDate() == r[4] && d.getHours() == r[5] && d.getMinutes() == r[6]);
                },
                interval: null,
                inputText: null,
                value: null,
                textFieldID: ""
            };

            if (this.tagName.toLowerCase() == "input" && this.type && this.type == "text") {
                g.inputText = $(this);
                if (this.id)
                    g.textFieldID = this.id;
            }
            else {
                g.inputText = $('<input type="text"/>');
                g.inputText.appendTo($(this));
            }
            if (g.textFieldID == "" && p.textFieldID)
                g.textFieldID = p.textFieldID;

            g.link = $('<div class="l-trigger"><div class="l-spinner-up"><div class="l-spinner-icon"></div></div><div class="l-spinner-split"></div><div class="l-spinner-down"><div class="l-spinner-icon"></div></div></div>');
            g.wrapper = g.inputText.wrap('<div class="l-text"></div>').parent();
            g.wrapper.append('<div class="l-text-l"></div><div class="l-text-r"></div>');
            g.wrapper.append(g.link).after(g.selectBox).after(g.valueField);
            g.link.up = $(".l-spinner-up", g.link);
            g.link.down = $(".l-spinner-down", g.link);
            if (!g.inputText.hasClass("l-text-field")) g.inputText.addClass("l-text-field");

            //数据初始化
            if (p.width) {
                g.wrapper.css({ width: p.width });
                g.inputText.css({ width: p.width - 20 });
            }
            if (p.height) {
                g.wrapper.height(p.height);
                g.inputText.height(p.height - 2);
                g.link.height(p.height - 4);
            }
            //初始化
            if (!g.isVerify(g.inputText.val())) {
                g.value = g.getDefaultValue();
                g.inputText.val(g.value);
            }
            //事件
            g.link.up.hover(function() {
                $(this).addClass("l-spinner-up-over");
            }, function() {
                clearInterval(g.interval);
                $.fn.ligerNoSelect && $('body').ligerNoSelect(false);
                $(this).removeClass("l-spinner-up-over");
            }).mousedown(function() {
                g.uping();
                g.interval = setInterval(g.uping, p.interval);
                $.fn.ligerNoSelect && $('body').ligerNoSelect();
            }).mouseup(function() {
                clearInterval(g.interval);
                g.inputText.trigger("change").focus();
                $.fn.ligerNoSelect && $('body').ligerNoSelect(false);
            });
            g.link.down.hover(function() {
                $(this).addClass("l-spinner-down-over");
            }, function() {
                clearInterval(g.interval);
                $.fn.ligerNoSelect && $('body').ligerNoSelect(false);
                $(this).removeClass("l-spinner-down-over");
            }).mousedown(function() {
                g.interval = setInterval(g.downing, p.interval);
                $.fn.ligerNoSelect && $('body').ligerNoSelect();
            }).mouseup(function() {
                clearInterval(g.interval);
                g.inputText.trigger("change").focus();
                $.fn.ligerNoSelect && $('body').ligerNoSelect(false);
            });

            g.inputText.change(function() {
                var value = g.inputText.val();
                g.value = g.getVerifyValue(value);
                if (p.onChangeValue) {
                    p.onChangeValue(g.value);
                }
                g.inputText.val(g.value);
            }).blur(function() {
                g.wrapper.removeClass("l-text-focus");
            }).focus(function() {
                g.wrapper.addClass("l-text-focus");
            });
            g.wrapper.hover(function() {
                g.wrapper.addClass("l-text-over");
            }, function() {
                g.wrapper.removeClass("l-text-over");
            });
            this.useSpinner = true;
        });
    };
})(jQuery);