/**
* jQuery ligerUI 1.0.0
* 
* Author leoxie [ gd_star@163.com ] 
* 
*/

(function ($)
{
    $.ligerDefaults = $.ligerDefaults || {};
    $.ligerDefaults.TextBox = {
        onBeforeInput: null,
        onInputed: null,
        onChangeValue: null,
        width: null
    };
    ///	<param name="$" type="jQuery"></param>
    $.fn.ligerTextBox = function (options)
    {
        return this.each(function ()
        {
            if (this.usedTextBox) return;
            var p = $.extend({}, options || {});
            if ($(this).attr("ligerui"))
            {
                try
                {
                    var attroptions = $(this).attr("ligerui");
                    if (attroptions.indexOf('{') < 0) attroptions = "{" + attroptions + "}";
                    eval("attroptions = " + attroptions + ";");
                    if (attroptions) p = $.extend({}, attroptions, p || {});
                }
                catch (e) { }
            }
            p = $.extend({}, $.ligerDefaults.TextBox, p || {}); 
            var g = {};
            g.inputText = $(this);
            //外层
            g.wrapper = g.inputText.wrap('<div class="l-text"></div>').parent();
            g.wrapper.append('<div class="l-text-l"></div><div class="l-text-r"></div>');
            if (!g.inputText.hasClass("l-text-field"))
                g.inputText.addClass("l-text-field");
            if (!p.width)
            {
                p.width = 120;
            }
            g.wrapper.css({ width: p.width });
            g.inputText.css({ width: p.width - 4 });
            if (p.height)
            {
                g.wrapper.height(p.height);
                g.inputText.height(p.height - 2);
            }
            g.inputText
            .bind('blur.ligerTextBox', function ()
            {
                g.wrapper.removeClass("l-text-focus");
            }).bind('focus.ligerTextBox', function ()
            {
                g.wrapper.addClass("l-text-focus");
            })
            .change(function ()
            {
                if (p.onChangeValue)
                {
                    p.onChangeValue(this.value);
                }
            });
            g.wrapper.hover(function ()
            {
                g.wrapper.addClass("l-text-over");
            }, function ()
            {
                g.wrapper.removeClass("l-text-over");
            });

            this.usedTextBox = true;
        });
    };

})(jQuery);