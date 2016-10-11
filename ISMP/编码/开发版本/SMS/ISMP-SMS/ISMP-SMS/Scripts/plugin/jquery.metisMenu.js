/*
 * metismenu - v1.1.3
 * Easy menu jQuery plugin for Twitter Bootstrap 3
 * https://github.com/onokumus/metisMenu
 *
 * Made by Osman Nuri Okumus
 * Under MIT License
 */
;(function($, window, document, undefined) {
    var pluginName = "metisMenu";

    function Plugin(element, options) {
        this.element = $(element);
        this.init();
    }

    Plugin.prototype = {
        init: function() {
            var $this = this.element;
            $this.find("li.active").has("ul").children("ul").addClass("collapse in");
            $this.find("li").not(".active").has("ul").children("ul").addClass("collapse");
            $this.find("li").has("ul").children("a").on("click", function(e) {
                e.preventDefault();
                if(!$('body').hasClass('mini-navbar')){
                    $(this).parent("li").siblings().children("ul").slideUp();
                    $(this).parent("li").siblings().removeClass("active").children("ul.in").css("display","none");
                    $(this).parent("li").toggleClass("active").children("ul").slideToggle(300);
                }
            });
            $this.children('li').children('a.ismp-tabopen').on('click',function(){
            	if(!$('body').hasClass('mini-navbar')){
                    $(this).parent("li").siblings().children("ul").slideUp();
                    $(this).parent("li").siblings().removeClass("active").children("ul.in").css("display","none");
                    $(this).parent("li").toggleClass("active").children("ul").slideToggle(300);
                }
            });
        }
    };

    $.fn[pluginName] = function() {
        new Plugin(this);
        return this;
    };

})(jQuery, window, document);