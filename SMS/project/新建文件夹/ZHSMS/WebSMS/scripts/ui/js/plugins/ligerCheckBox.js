/**
* jQuery ligerUI 1.0.2
* 
* Author leoxie [ gd_star@163.com ] 
* 
*/
(function ($)
{
    ///	<param name="$" type="jQuery"></param>
    $.fn.ligerCheckBox = function (p)
    {
        p = p || {};
        return this.each(function ()
        {
            if (this.usedCheckBox) return;
            if ($(this).hasClass('l-hidden')) { return; }
            var g = {};
            g.input = $(this);
            g.link = $('<a class="l-checkbox"></a>');
            g.wrapper = g.input.addClass('l-hidden').wrap('<div class="l-checkbox-wrapper"></div>').parent();
            g.wrapper.prepend(g.link);
            if (p.css) g.wrapper.css(p.css);
            g.link.click(function ()
            {
                if (g.input.attr('disabled')) { return false; }
                if (p.onBeforeClick)
                {
                    if (!p.onBeforeClick(g.input[0]))
                        return false;
                }
                if ($(this).hasClass("l-checkbox-checked"))
                {
                    g.input[0].checked = false;
                    g.link.removeClass('l-checkbox-checked');
                }
                else
                {
                    g.input[0].checked = true;
                    g.link.addClass('l-checkbox-checked');
                }
                g.input.trigger("change");
            });
            g.wrapper.hover(function ()
            {
                $(this).addClass("l-over");
            }, function ()
            {
                $(this).removeClass("l-over");
            });
            this.checked && g.link.addClass('l-checkbox-checked');
            this.usedCheckBox = true;
        });
    };

})(jQuery);