/**
* jQuery ItemSelector
* @requires
* Version: 1.4
* Author:jiangchuan
*/
(function ($) {
    function strhtml(vtype, options) {
        var htmls = ["<td>"];
        var style = "";
        if (vtype == "center") {
            htmls.push("<div>");
            htmls.push('<div class="ItemSelector-mselect-ico ItemSelector-mselect-ico-top" style="margin-top: 4px;margin-right: 2px;margin-bottom: 4px;margin-left: 2px;"/>');
            //htmls.push('<div class="ItemSelector-mselect-ico ItemSelector-mselect-ico-up" style="margin-top: 4px;margin-right: 2px;margin-bottom: 4px;margin-left: 2px;"/>');
            htmls.push('<div class="ItemSelector-mselect-ico ItemSelector-mselect-ico-left" style="margin-top: 4px;margin-right: 2px;margin-bottom: 4px;margin-left: 2px;"/>');
            htmls.push('<div class="ItemSelector-mselect-ico ItemSelector-mselect-ico-right" style="margin-top: 4px;margin-right: 2px;margin-bottom: 4px;margin-left: 2px;"/>');
            //htmls.push('<div class="ItemSelector-mselect-ico ItemSelector-mselect-ico-down" style="margin-top: 4px;margin-right: 2px;margin-bottom: 4px;margin-left: 2px;"/>');
            htmls.push('<div class="ItemSelector-mselect-ico ItemSelector-mselect-ico-bottom" style="margin-top: 4px;margin-right: 2px;margin-bottom: 4px;margin-left: 2px;"/>');
            htmls.push("</div>");
        } else {
            style = "style='width:" + options.msWidth + "px;height:" + options.msHeight + "px;'";
            htmls.push("<div " + style + ">");
            style = "style='width:" + (options.msWidth - 2) + "px;'";
            var text = "";
            var fieldset_name = "";
            if (vtype == "right") {
                text = options.toLegend;
                fieldset_name = "resetright";
            }
            else if (vtype == "left") {
                text = options.fromLegend;
                fieldset_name = "resetleft";
            }
            htmls.push('<fieldset  fieldset_name="' + fieldset_name + '" class="ItemSelector-fieldset" ' + style + '><legend class="ItemSelector-legend">' + text + '</legend>');
            style = "style='height:" + (options.msHeight - 24) + "px;'";
            htmls.push('<div class="ItemSelector-mselect" ' + style + '></div>');
            htmls.push("</fieldset>");
            htmls.push("</div>");
        }
        htmls.push("</td>");
        return htmls.join("");
    }
    function setValue(jq) {
        var options = $.data(jq, "ItemSelector").options;
        var strValue = "";
        $(jq).find("input[name='" + options.name + "']").attr("value", strValue);
        var children = $(jq).find("fieldset[fieldset_name='resetright']").find(".ItemSelector-mselect").children();
        children.each(function (i) {
            strValue += strValue == "" ? "" : ",";
            strValue += $(this).attr("value");
        });
        $(jq).find("input[name='" + options.name + "']").attr("value", strValue);
    }
    function reset(jq, vtype, data) {
        if (typeof data != "object") return;
        var options = $.data(jq, "ItemSelector").options;
        if (vtype == "resetright")
            options.toData = data;
        else
            options.fromData = data;
        if (options.valuePrototype == "" || options.showPrototype == "" || data.length <= 0) return;
        var item = [];
        for (var i = 0; i < data.length; i++) {
            if (!data[i]) continue;
            item.push("<div class=\"ItemSelector-mselect-item\" value=\"" + data[i][options.valuePrototype] + "\">" + data[i][options.showPrototype] + "</div>"); //eval('data[i].' + options.valuePrototype)
        }
        var mselect = $(jq).find("fieldset[fieldset_name='" + vtype + "']").find(".ItemSelector-mselect");
        mselect.html(item.join(""));
        $(mselect).find(".ItemSelector-mselect-item").click(function () { $(".ItemSelector-mselect-item").removeClass("ItemSelector-mselect-item-select"); $(this).addClass("ItemSelector-mselect-item-select"); });
        $(mselect).find(".ItemSelector-mselect-item").dblclick(function () { onRowDblClick(jq, this); setValue(jq); });
    }
    function top(jq, item) {
        $(jq).find("fieldset[fieldset_name='resetright']").find(".ItemSelector-mselect").prepend(item);
    }
    function up(jq, item) {
        var parent = $(item).parent();
        var itemup;
        parent.children().each(function (i) {
            if (this == item[0]) {
                if (itemup) {
                    item.after(itemup);
                    return false;
                }
            }
            itemup = this;
        });
    }
    function toleft(jq, item) {
        $(jq).find("fieldset[fieldset_name='resetleft']").find(".ItemSelector-mselect").append(item);
    }
    function toright(jq, item) {
        $(jq).find("fieldset[fieldset_name='resetright']").find(".ItemSelector-mselect").append(item);
    }

    function toLeftAll(jq) {
        var items = $(jq).find("fieldset[fieldset_name='resetright']").find(".ItemSelector-mselect").find(".ItemSelector-mselect-item");
        for(var i =0; i<items.length;i++)
        {
            $(jq).find("fieldset[fieldset_name='resetleft']").find(".ItemSelector-mselect").append(items[i]);
        }
    }

    function toRightAll(jq) {
        var items = $(jq).find("fieldset[fieldset_name='resetleft']").find(".ItemSelector-mselect").find(".ItemSelector-mselect-item");
        for (var i = 0; i < items.length; i++) {
            $(jq).find("fieldset[fieldset_name='resetright']").find(".ItemSelector-mselect").append(items[i]);
        }
    }

    function down(jq, item) {
        var parent = $(item).parent();
        var itemup;
        parent.children().each(function (i) {
            if (itemup) {
                item.before(this);
                return false;
            }
            if (this == item[0]) {
                itemup = item;
                return true;
            } else itemup = undefined;
        });
    }
    function bottom(jq, item) {
        $(jq).find("fieldset[fieldset_name='resetright']").find(".ItemSelector-mselect").append(item);
    }
    function onRowDblClick(jq, item) {
        var vtype = $(item).parent().parent().attr('fieldset_name');
        if (!vtype) return;
        if (vtype == "resetright") {
            toleft(jq, item);
        } else if (vtype == "resetleft") {
            toright(jq, item);
        }
    }
    function getValue(jq) {
        var options = $.data(jq, "ItemSelector").options;
        return $(jq).find("input[name='" + options.name + "']").val();
    }
    //    function resetright(jq){
    //
    //    }
    //查找方法
    function find(jq, data) {
        var options = $.data(jq, "ItemSelector").options;

        var fromdata = options.fromData;
        var todata = options.toData;


        for (var i = 0; i < fromdata.length; i++) {
            if (fromdata[i].WorkerID == data) {
                var ls = fromdata[i];

                fromdata.splice(i, 1);
                fromdata.unshift(ls);

                break;
            }
        }
        for (var i = 0; i < todata.length; i++) {
            if (todata[i].WorkerID == data) {
                var ls = todata[i];
                todata.splice(i, 1);
                todata.unshift(ls);
                break;
            }
        }



        reset(jq, "resetleft", fromdata);


        var lefttop = $(jq).find("fieldset[fieldset_name='resetleft']").find(".ItemSelector-mselect");

        lefttop[0].scrollTop = 0;

        reset(jq, "resetright", todata);

        var righttop = $(jq).find("fieldset[fieldset_name='resetright']").find(".ItemSelector-mselect");
        righttop[0].scrollTop = 0;



    }






    function onEvents(jq) {
        $(".ItemSelector-mselect-ico").bind({
            click: function () {
                var classname = this.className;
                var select = $(jq).find("fieldset[fieldset_name='resetright']").find(".ItemSelector-mselect").find(".ItemSelector-mselect-item-select");
                if (!select) return;
                if (classname.indexOf("ItemSelector-mselect-ico-top") != -1) {
                    //top(jq, select);
                    toRightAll(jq);
                }
                else if (classname.indexOf("ItemSelector-mselect-ico-up") != -1) {
                    up(jq, select);
                }
                else if (classname.indexOf("ItemSelector-mselect-ico-left") != -1) {
                    toleft(jq, select);
                }
                else if (classname.indexOf("ItemSelector-mselect-ico-right") != -1) {
                    select = $(jq).find("fieldset[fieldset_name='resetleft']").find(".ItemSelector-mselect").find(".ItemSelector-mselect-item-select");
                    if (!select) return;
                    toright(jq, select);
                }
                else if (classname.indexOf("ItemSelector-mselect-ico-down") != -1) {
                    down(jq, select);
                }
                else if (classname.indexOf("ItemSelector-mselect-ico-bottom") != -1) {
                    toLeftAll(jq, select);
                }
                setValue(jq);
            },
            mouseover: function () { $(this).css("filter", "none"); },
            mouseout: function () { $(this).css("filter", "alpha(opacity=80)"); }
        });
    }

    function onRender(jq) {
        //throw ss;
        var options = $.data(jq, "ItemSelector").options;
        var style = "";
        var table = ['<table class="ItemSelector-table">'];
        table.push("<tr>");
        table.push(strhtml("left", options));
        table.push(strhtml("center", options));
        table.push(strhtml("right", options));
        table.push("</tr>");
        table.push("</table>"); //hidden
        table.push("<input type='hidden'value=\"\" name='" + options.name + "'/>");
        $(jq).html(table.join(""));
        if (options.isdistinct) {
            if (options.fromData == options.toData) {
                var todata = new Array();
                for (var t = 0; t < options.toData.length; t++) {
                    todata.push(options.toData[t]);
                }
                options.toData = todata;
            }
            var count = 0;
            var distinctField = options.distinctField == "" ? options.valuePrototype : options.distinctField;
            for (var i = 0; i < options.fromData.length; i++) {
                for (var k = 0; k < options.toData.length; k++) {
                    if (options.fromData[i][distinctField] == options.toData[k][distinctField]) {
                        delete options.fromData[i];
                        count++;
                        break;
                    }
                }
            }
            if (count > 0) {
                var arry = [];
                for (var j = 0; j < options.fromData.length; j++) {
                    if (!options.fromData[j]) continue;
                    arry.push(options.fromData[j]);
                }
                options.fromData = arry;
            }
        }
        if (options.fromData.length > 0) {
            reset(jq, "resetleft", options.fromData);
        }
        if (options.toData.length > 0) {
            reset(jq, "resetright", options.toData);
        }
        setValue(jq);
        onEvents(jq);
    }

    $.fn.ItemSelector = function (options, data) {
        if (typeof options == "string") {
            var method = $.fn.ItemSelector.methods[options];
            if (method)
                return method(this, data);
        }
        return this.each(function () {
            var target = $.data(this, "ItemSelector");
            if (target) {
                $.extend(target.options, options);
            } else {
                target = $.data(this, "ItemSelector", { options: $.extend({}, $.fn.ItemSelector.defaults, options) });
            }
            onRender(this);
        });
    };
    $.fn.ItemSelector.methods = {
        options: function (jq) {
            return $.data(jq[0], "ItemSelector").options;
        },
        resetright: function (jq, data) {
            return jq.each(function () { reset(this, "resetright", data); });
        },
        resetleft: function (jq, data) {
            return jq.each(function () { reset(this, "resetleft", data); });
        },
        getValue: function (jq) {
            return getValue(jq[0]);
        },
        find: function (jq, data) {
            return jq.each(function () { find(this, data); });
        }
    };
    $.fn.ItemSelector.defaults = {
        msWidth: 180,            //宽度
        msHeight: 222,           //高度
        valuePrototype: "",      //value的对象属性
        showPrototype: "",       //显示的对象属性
        fromLegend: "",          //左侧标题显示的文本
        fromData: new Array(),   //左侧的数据  类型 [{valuePrototype,showPrototype},{valuePrototype,showPrototype}]
        toLegend: "",            //右侧标题显示的文本
        toData: new Array(),     //右侧数据位  类型  [{valuePrototype,showPrototype},{valuePrototype,showPrototype}]
        name: "",                //post提交的名称
        isdistinct: true,        //在左侧是否去掉与右侧相同的数据  默认去掉
        distinctField: ""        //根具对象属性去重 如果isdistinct=true 默认 valuePrototype
    };
})(jQuery);