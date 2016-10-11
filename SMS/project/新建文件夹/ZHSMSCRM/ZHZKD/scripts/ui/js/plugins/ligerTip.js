/**
* jQuery ligerUI 1.0.2
* 
* Author leoxie [ gd_star@163.com ] 
* 
*/

(function($) {

    $.ligerDefaults = $.ligerDefaults || {};
    $.ligerDefaults.Tip = {
        content: null,
        callback: null,
        width: 150,
        height: null,
        distanceX: 7,
        distanceY: 0,
        appendIdTo: null       //保存ID到那一个对象(jQuery)
    };

    ///	<param name="$" type="jQuery"></param>
    $.fn.ligerTip = function(p) {
        p = $.extend({}, $.ligerDefaults.Tip, p || {});
        this.each(function() { 
            var tip = null;
            var tipid = $(this).attr("ligerTipId");
            if (tipid) {
                tip = $("#" + tipid);
                if (p.content == "") tip.remove();
                else $(".l-verify-tip-content", tip).html(p.content);
            }
            else if (p.content) {
                tip = $('<div class="l-verify-tip"><div class="l-verify-tip-corner"></div><div class="l-verify-tip-content">' + p.content + '</div></div>');
                tip.attr("id", "ligerUI" + new Date().getTime());
				tip.fadeTo(0, 0.85); //对象的透明度
                //tip.appendTo('body');
				$(this).after(tip);
            }
            if (!tip) return;
            //方法一：tip.css({ left: $(this).offset().left + $(this).width() + p.distanceX, top: $(this).offset().top + p.distanceY }).show();
			tip.css({ left: $(this).position().left + $(this).width() + p.distanceX, top: $(this).position().top + p.distanceY }).show();
			//方法三：$(this).parent().css('position','relative');
			//方法三：tip.css({ left: $(this).outerWidth(true) + p.distanceX, top: p.distanceY }).show();
			
            $(this).attr("ligerTipId", tip.attr("id"));
            p.width && $("> .l-verify-tip-content", tip).width(p.width - 8);
            p.height && $("> .l-verify-tip-content", tip).width(p.height);
            p.appendIdTo && p.appendIdTo.attr("ligerTipId", tip.attr("id"));
            p.callback && p.callback(tip);
        });
        if (this.length == 0) return null;
        if (this.length == 1) return this[0].ligerTip;
        var tips = [];
        this.each(function() {
            tips.push(this.ligerTip);
        });
        return tips;
    };
    $.fn.ligerHideTip = function(p) {
        return this.each(function() {
            var tipid = $(this).attr("ligerTipId");
            if (tipid) {
                $("#" + tipid).remove();
                $("[ligerTipId=" + tipid + "]").removeAttr("ligerTipId");
            }
        });
    };
})(jQuery);

