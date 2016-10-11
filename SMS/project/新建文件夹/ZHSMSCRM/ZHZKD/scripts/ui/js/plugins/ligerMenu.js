/**
* jQuery ligerUI 1.0.2
* 
* Author leoxie [ gd_star@163.com ] 
* 
*/ 
if (typeof (LigerUIMenu) == "undefined") LigerUIMenu = {}; 
(function ($)
{
    $.ligerDefaults = $.ligerDefaults || {};
    $.ligerDefaults.Menu = {
        width: 120,
        top: 0,
        left: 0,
        items:null,
        shadow: true
    };
    ///	<param name="$" type="jQuery"></param> 
    $.ligerMenu = function (p)
    {
        p = $.extend({ }, $.ligerDefaults.Menu, p || {});
        var g = {
            show: function (options,menu)
            {
                if(menu==undefined) menu = g.menu;
                if (options && options.left != undefined)
                {
                    menu.css({ left: options.left });
                }
                if (options && options.top != undefined)
                {
                    menu.css({ top: options.top });
                }
                menu.show();
                g.updateShadow(menu);
            },
            updateShadow: function (menu)
            {
                if (!p.shadow) return;
                menu.shadow.css({
                    left: menu.css('left'),
                    top: menu.css('top'),
                    width: menu.outerWidth(),
                    height: menu.outerHeight()
                });
                if (menu.is(":visible"))
                    menu.shadow.show();
                else
                    menu.shadow.hide();
            },
            hide: function (menu)
            {
                if(menu==undefined) menu = g.menu;
                g.hideAllSubMenu(menu);
                menu.hide();
                g.updateShadow(menu);
            },
            toggle: function ()
            {
                g.menu.toggle();
                g.updateShadow(g.menu);
            },
            removeItem: function (itemid)
            {
                $("> .l-menu-item[menuitemid=" + itemid + "]", g.menu.items).remove();
                g.itemCount--;
            },
            setEnable: function (itemid)
            {
                $("> .l-menu-item[menuitemid=" + itemid + "]", g.menu.items).removeClass("l-menu-item-disable");
            },
            setDisable: function (itemid)
            {
                $("> .l-menu-item[menuitemid=" + itemid + "]", g.menu.items).addClass("l-menu-item-disable");
            },
            isEnable: function (itemid)
            {
                return !$("> .l-menu-item[menuitemid=" + itemid + "]", g.menu.items).hasClass("l-menu-item-disable");
            },
            getItemCount: function ()
            {
                return $("> .l-menu-item", g.menu.items).length;
            },
            addItem: function (item, menu)
            {
                if(!item) return ;
                if(menu== undefined) menu = g.menu;
                
                if (item.line)
                {
                    menu.items.append('<div class="l-menu-item-line"></div>');
                    return;
                }
                var ditem = $('<div class="l-menu-item"><div class="l-menu-item-text"></div> </div>');
                var itemcount = $("> .l-menu-item", menu.items).length;
                menu.items.append(ditem); 
                item.id && ditem.attr("menuitemid", item.id);
                item.text && $(">.l-menu-item-text:first", ditem).html(item.text);
                item.icon && ditem.prepend('<div class="l-menu-item-icon l-icon-' + item.icon + '"></div>');
                item.disable && ditem.addClass("l-menu-item-disable");
                if (item.children)
                {
                    if (ditem.attr("menuitemid") == undefined) ditem.attr("menuitemid", new Date().getTime());
                    ditem.append('<div class="l-menu-item-arrow"></div>');
                    var newmenu = g.createMenu(ditem.attr("menuitemid"));
                    LigerUIMenu[ditem.attr("menuitemid")] = newmenu;
                    newmenu.width(p.width);
                    newmenu.hover(null,function(){
                        if(!newmenu.showedSubMenu)
                            g.hide(newmenu);
                    });
                    $(item.children).each(function ()
                    {
                        g.addItem(this, newmenu);
                    });
                }
                item.click && ditem.click(function ()
                {
                    if ($(this).hasClass("l-menu-item-disable")) return;
                    item.click(item, itemcount);
                });
                item.dblclick && ditem.dblclick(function ()
                {
                    if ($(this).hasClass("l-menu-item-disable")) return;
                    item.dblclick(item, itemcount);
                });

                var menuover = $("> .l-menu-over:first", menu);
                ditem.hover(function ()
                { 
                    if ($(this).hasClass("l-menu-item-disable")) return; 
                    var itemtop = $(this).offset().top;
                    var top = itemtop - menu.offset().top;
                    menuover.css({ top: top });
                    g.hideAllSubMenu(menu);
                    if (item.children)
                    {
                        var meniitemid = $(this).attr("menuitemid");
                        if (!meniitemid) return; 
                        if(LigerUIMenu[meniitemid])
                        {
                            g.show({top:itemtop,left:$(this).offset().left+$(this).width()-5},LigerUIMenu[meniitemid]);
                            menu.showedSubMenu = true;
                        } 
                    } 
                }, function ()
                {
                    if ($(this).hasClass("l-menu-item-disable")) return; 
                    var meniitemid = $(this).attr("menuitemid");
                    if (item.children)
                    {
                        var meniitemid = $(this).attr("menuitemid");
                        if (!meniitemid) return;
                    };
                });
            },
            hideAllSubMenu:function(menu)
            {
                if(menu==undefined) menu = g.menu;
                $("> .l-menu-item",menu.items).each(function(){
                    if($("> .l-menu-item-arrow",this).length>0)
                    {
                        var meniitemid = $(this).attr("menuitemid");
                        if (!meniitemid) return;
                        LigerUIMenu[meniitemid] && g.hide(LigerUIMenu[meniitemid]);
                    }
                });
                menu.showedSubMenu = false;
            },
            createMenu: function (parentMenuItemID)
            {
                var menu = $('<div class="l-menu" style="display:none"><div class="l-menu-yline"></div><div class="l-menu-over"><div class="l-menu-over-l"></div> <div class="l-menu-over-r"></div></div><div class="l-menu-inner"></div></div>');
                parentMenuItemID && menu.attr("parentmenuitemid", parentMenuItemID);
                menu.items = $("> .l-menu-inner:first", menu);
                menu.appendTo('body');
                if (p.shadow)
                {
                    menu.shadow = $('<div class="l-menu-shadow"></div>').insertAfter(menu);
                    g.updateShadow(menu);
                }
                menu.hover(null,function(){
                    if(!menu.showedSubMenu)
                        $("> .l-menu-over:first", menu).css({ top: -24 });
                });
                return menu;
            }
        };
        g.menu = g.createMenu();
        g.menu.css({ top: p.top, left: p.left, width: p.width });  
        p.items && $(p.items).each(function (i, item)
        { 
            g.addItem(item);
        });
        return g;
    };
    $(document).click(function ()
    {
        $(".l-menu,.l-menu-shadow").hide();
    });
})(jQuery);