/**
* jQuery ligerUI 1.0.0
* 
* Author leoxie [ gd_star@163.com ] 
* 
*/

(function ($)
{
    ///	<param name="$" type="jQuery"></param>
    $.fn.ligerRadio = function ()
    {
        return this.each(function ()
        {
            if (this.usedRadio) return;
            if ($(this).hasClass('l-hidden')) { return; }
            var g = {};
            g.input = $(this);
            g.link = $('<a href="javascript:void(0)" class="l-radio"></a>');
            g.wrapper = g.input.addClass('l-hidden').wrap('<div class="l-radio-wrapper"></div>').parent();
           

            g.wrapper.prepend(g.link);
            g.input.change(function ()
            {
                if (this.checked)
                {
                    g.link.addClass('l-radio-checked');
                }
                else
                {
                    g.link.removeClass('l-radio-checked');
                }
                return true;
            });
            g.link.click(function ()
            {
                if (g.input.attr('disabled')) { return false; }
                g.input.trigger('click').trigger('change');
                var formEle;
                if (g.input[0].form) formEle = g.input[0].form;
                else formEle = document;
                $("input:radio[name=" + g.input[0].name + "]", formEle).not(g.input).trigger("change");
                return false;
            });
            g.wrapper.hover(function ()
            {
                $(this).addClass("l-over");
            }, function ()
            {
                $(this).removeClass("l-over");
            });
            this.checked && g.link.addClass('l-radio-checked');

            this.usedRadio = true;
        });
    };

})(jQuery);