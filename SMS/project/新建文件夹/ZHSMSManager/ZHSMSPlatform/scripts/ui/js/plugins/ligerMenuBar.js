/**
* jQuery ligerUI 1.0.1
* 
* Author leoxie [ gd_star@163.com ] 
* 
* Depend on:
* 1,LigerMenu
*/
if(typeof (LigerUIManagers) == "undefined") LigerUIManagers = {};
(function($)
{
    ///	<param name="$" type="jQuery"></param>

    $.fn.ligerGetMenuBarManager = function()
    {
        return LigerUIManagers[this[0].id + "_MenuBar"];
    }; 
    $.fn.ligerMenuBar = function(p)
    { 
        return this.each(function()
        {
            if (this.usedMenuBar) return;
            var g ={
                addItem :function(item){
                    var ditem = $('<div class="l-menubar-item l-panel-btn"><span></span><div class="l-panel-btn-l"></div><div class="l-panel-btn-r"></div><div class="l-menubar-item-down"></div></div>');
                    g.menubar.append(ditem);
                    item.id && ditem.attr("menubarid",item.id);
                    item.text && $("span:first",ditem).html(item.text);
                    item.disable && ditem.addClass("l-menubar-item-disable");
                    item.click && ditem.click(function(){ item.click(item);});
                    if(item.menu)
                    {
                        var menu = $.ligerMenu(item.menu);
                        ditem.hover(function ()
                        {
                            g.actionMenu && g.actionMenu.hide();
                            var left = $(this).offset().left;
                            var top = $(this).offset().top + $(this).height();
                            menu.show({ top: top, left: left }); 
                            g.actionMenu = menu;
                            $(this).addClass("l-panel-btn-over l-panel-btn-selected").siblings(".l-menubar-item").removeClass("l-panel-btn-selected");
                        }, function ()
                        { 
                            $(this).removeClass("l-panel-btn-over");
                        }); 
                    }
                    else
                    {
                        ditem.hover(function ()
                        {
                            $(this).addClass("l-panel-btn-over");
                        }, function ()
                        {
                            $(this).removeClass("l-panel-btn-over");
                        });
                        $(".l-menubar-item-down",ditem).remove();
                    }
                    
                }
            };
            g.menubar = $(this);
            if(!g.menubar.hasClass("l-menubar")) g.menubar.addClass("l-menubar");
            if(p && p.items)
            {
                $(p.items).each(function(i,item){
                    g.addItem(item); 
                });
            }
            $(document).click(function ()
            {
                $(".l-panel-btn-selected",g.menubar).removeClass("l-panel-btn-selected");
            });
            if (this.id == undefined) this.id = "LigerUI_" + new Date().getTime();
            LigerUIManagers[this.id + "_MenuBar"] = g;
            this.usedMenuBar = true;
        });
    };

})(jQuery);