/**
* jQuery ligerUI 1.0.1
* 
* Author leoxie [ gd_star@163.com ] 
* 
*/
if(typeof (LigerUIManagers) == "undefined") LigerUIManagers = {};
(function($)
{
    ///	<param name="$" type="jQuery"></param>

    $.fn.ligerGetButtonManager = function()
    {
        return LigerUIManagers[this[0].id + "_Button"];
    };
    $.fn.ligerRemoveButtonManager = function()
    {
        return this.each(function()
        {
            LigerUIManagers[this.id + "_Button"] = null;
        });
    };
    $.fn.ligerButton = function(p)
    { 
        return this.each(function()
        { 
            if (this.usedButton) return;
            p = p || {};
            var g ={
                setText :function(itemid){
                },
                setEnable:function(itemid){
                },
                setDisable:function(itemid){
                }
            };
            g.button = $(this);
            if(!g.button.hasClass("l-btn")) g.button.addClass("l-btn");
            p.text && g.button.append("<span>"+p.text+"</span>");
            g.button.append('<div class="l-btn-l"></div><div class="l-btn-r"></div>');  
            p.click && g.button.click(function(){ p.click()});
            if (this.id == undefined) this.id = "LigerUI_" + new Date().getTime();
            LigerUIManagers[this.id + "_Button"] = g;
            this.usedButton = true;
        });
    };

})(jQuery);