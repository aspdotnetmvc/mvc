/**
* jQuery ligerUI 1.0.2
* 
* Author leoxie [ gd_star@163.com ] 
* 
*/
if(typeof (LigerUIManagers) == "undefined") LigerUIManagers = {};
(function($)
{
    ///	<param name="$" type="jQuery"></param>

    $.fn.ligerGetTabManager = function()
    {
        return LigerUIManagers[this[0].id + "_Tab"];
    }; 

    $.ligerDefaults = $.ligerDefaults || {};
    $.ligerDefaults.Tab = {
            height: null,
            heightDiff: 0, // 高度补差 
			marginleft: 3,
            changeHeightOnResize: false,
            contextmenu : true,
            closeMessage : "关闭当前页",
            closeOtherMessage : "关闭其他",
            closeAllMessage : "关闭所有",
            reloadMessage : "刷新",
            onBeforeOverrideTabItem:null,
            onAfterOverrideTabItem:null,
            onBeforeRemoveTabItem:null,
            onAfterRemoveTabItem:null,
            onBeforeAddTabItem :null,
            onAfterAddTabItem:null,
            onBeforeSelectTabItem :null,
            onAfterSelectTabItem:null
    };

    $.fn.ligerTab = function(p)
    { 
        p = $.extend({},$.ligerDefaults.Tab, p || {});
        this.each(function()
        {
            if (this.usedTab) return;
            if ($(this).hasClass('l-hidden')) { return; }
            var g = {
                //设置tab按钮(左和右),显示返回true,隐藏返回false
                setTabButton: function()
                {
                    var sumwidth = 0;
                    $("li", g.tab.links.ul).each(function()
                    {
                        sumwidth += ($(this).width() + p.marginleft);
                    });
                    var mainwidth = g.tab.width();
                    if (sumwidth > mainwidth)
                    {
						$(".l-tab-links-left,.l-tab-links-right", g.tab.links).remove();
                        g.tab.links.append('<div class="l-tab-links-left"></div><div class="l-tab-links-right"></div>');
						if($(g.tab).attr("toolsid"))
					    {
							$(".l-tab-links-right", g.tab.links).css("right","17px");
					    }
                        g.setTabButtonEven();
                        return true;
                    } else
                    {
                        g.tab.links.ul.animate({ left: 0 });
                        $(".l-tab-links-left,.l-tab-links-right", g.tab.links).remove();
                        return false;
                    }
                },
                //设置左右按钮的事件 标签超出最大宽度时，可左右拖动
                setTabButtonEven: function()
                {
                    $(".l-tab-links-left", g.tab.links).hover(function()
                    {
                        $(this).addClass("l-tab-links-left-over");
                    }, function()
                    {
                        $(this).removeClass("l-tab-links-left-over");
                    }).click(function()
                    {
                        g.moveToPrevTabItem();
                    });
                    $(".l-tab-links-right", g.tab.links).hover(function()
                    {
                        $(this).addClass("l-tab-links-right-over");
                    }, function()
                    {
                        $(this).removeClass("l-tab-links-right-over");
                    }).click(function()
                    {
                        g.moveToNextTabItem();
                    });
                },
                //切换到上一个tab
                moveToPrevTabItem: function()
                {
                    var btnWitdth = $(".l-tab-links-left", g.tab.links).width();
                    var leftList = new Array(); //记录每个tab的left,由左到右
                    $("li", g.tab.links.ul).each(function(i, item)
                    {
                        var currentItemLeft = -1 * btnWitdth;
                        if (i > 0)
                        {
                            currentItemLeft = parseInt(leftList[i - 1]) + $(this).prev().width() + p.marginleft;
                        }
                        leftList.push(currentItemLeft);
                    });
                    var currentLeft = -1 * parseInt(g.tab.links.ul.css("left"));
                    for (var i = 0; i < leftList.length - 1; i++)
                    {
                        if (leftList[i] < currentLeft && leftList[i + 1] >= currentLeft)
                        {
                            g.tab.links.ul.animate({ left: -1 * parseInt(leftList[i]) });
                            return;
                        }
                    }
                },
                //切换到下一个tab
                moveToNextTabItem: function()
                {
                    var btnWitdth = $(".l-tab-links-right", g.tab).width() + $(".l-tab-links-tools", g.tab.links).width();
                    var sumwidth = 0;
                    var tabItems = $("li", g.tab.links.ul);
                    tabItems.each(function()
                    {
                        sumwidth += $(this).width() + p.marginleft;
                    });
                    var mainwidth = g.tab.width();
                    var leftList = new Array(); //记录每个tab的left,由右到左 
                    for (var i = tabItems.length - 1; i >= 0; i--)
                    {
                        var currentItemLeft = sumwidth - mainwidth + btnWitdth + p.marginleft;
                        if (i != tabItems.length - 1)
                        {
                            currentItemLeft = parseInt(leftList[tabItems.length - 2 - i]) - ($(tabItems[i + 1]).width() + p.marginleft);
                        }
                        leftList.push(currentItemLeft);
                    }
                    var currentLeft = -1 * parseInt(g.tab.links.ul.css("left"));
                    for (var i = 1; i < leftList.length; i++)
                    {
                        if (leftList[i] <= currentLeft && leftList[i - 1] > currentLeft)
                        {
                            g.tab.links.ul.animate({ left: -1 * parseInt(leftList[i - 1]) });
                            return;
                        }
                    }
                },
                getTabItemCount : function()
                {
                    return $("li", g.tab.links.ul).length;
                },
                getSelectedTabItemID:function()
                {
                    return $("li.l-selected", g.tab.links.ul).attr("tabid");
                }, 
                removeSelectedTabItem:function()
                {
                    g.removeTabItem(g.getSelectedTabItemID());
                },
                //覆盖选择的tabitem
                overrideSelectedTabItem : function(options){ 
                    g.overrideTabItem(g.getSelectedTabItemID(),options); 
                },
                //覆盖
                overrideTabItem : function(targettabid,options){
                    if(p.onBeforeOverrideTabItem && p.onBeforeOverrideTabItem(targettabid)==false) return false;

                    var tabid = options.tabid;
                    if (tabid == undefined) tabid = g.getNewTabid();
                    var url = options.url;
                    var content = options.content;
                    var target = options.target;
                    var text = options.text;
                    var showClose = options.showClose;
                    var height = options.height;
                    //如果已经存在
                    if (g.isTabItemExist(tabid))
                    { 
                        return;
                    }
                    var tabitem = $("li[tabid="+targettabid+"]", g.tab.links.ul); 
                    var contentitem = $(".l-tab-content-item[tabid="+targettabid+"]",g.tab.content);
                    if(!tabitem || !contentitem) return ;  
                    tabitem.attr("tabid", tabid);
                    contentitem.attr("tabid", tabid);
                    if($("iframe", contentitem).length==0 && url)
                    {
                        contentitem.html("<iframe frameborder='0'></iframe>");
                    }
                    else if(content)
                    {
                        contentitem.html(content);
                    }
                    $("iframe", contentitem).attr("name", tabid);
                    if (showClose == undefined)  showClose = true; 
                    if (showClose == false) $(".l-tab-links-item-close", tabitem).remove();
                    else{
                        if($(".l-tab-links-item-close", tabitem).length==0)
                           tabitem.append("<div class='l-tab-links-item-close'></div>");
                    }
                    if (text == undefined) text = tabid;
                    if (height) contentitem.height(height);
                    $("a", tabitem).text(text); 
                    $("iframe", contentitem).attr("src", url);


                    p.onAfterOverrideTabItem && p.onAfterOverrideTabItem(targettabid);
                },
                //选中tab项
                selectTabItem: function(tabid)
                {
                    if(p.onBeforeSelectTabItem && p.onBeforeSelectTabItem(tabid)==false) return false;
                    g.selectedTabId = tabid;
                    $("> .l-tab-content-item[tabid=" + tabid + "]", g.tab.content).show().siblings().hide();
                    $("li[tabid=" + tabid + "]", g.tab.links.ul).addClass("l-selected").siblings().removeClass("l-selected");
                    p.onAfterSelectTabItem && p.onAfterSelectTabItem(tabid);
                },
				//移动tab到当前合适的位置
				moveToCurrTabItem: function(tabid)
				{
					var btnWitdth = $(".l-tab-links-left", g.tab.links).width();
					var mainwidth = g.tab.width();
					var selecttableftwidth = 0;
					var selecttabrightwidth = 0;
                    $("li", g.tab.links.ul).each(function()
                    {
						selecttabrightwidth += $(this).width() + p.marginleft;
						if($(this).attr("tabid") == tabid)
						{
							return false;
						}
                        selecttableftwidth += $(this).width() + p.marginleft;
                    });
					var tableftnum = parseInt($(g.tab.links.ul).css("left"));
					if(selecttableftwidth < (0 - tableftnum))
					{
						g.tab.links.ul.animate({ left: -1 * (selecttableftwidth - btnWitdth) });
					}
					else if((selecttabrightwidth + tableftnum) > mainwidth)
					{
						btnWitdth += $(".l-tab-links-tools", g.tab.links).width();
						g.tab.links.ul.animate({ left: (-1 * (selecttabrightwidth - mainwidth + btnWitdth + p.marginleft)) });
					}
				},
                //移动到最后一个tab
                moveToLastTabItem: function()
                {
                    var sumwidth = 0;
                    $("li", g.tab.links.ul).each(function()
                    {
                        sumwidth += $(this).width() + p.marginleft;
                    });
                    var mainwidth = g.tab.width();
                    if (sumwidth > mainwidth)
                    {
                        var btnWitdth = $(".l-tab-links-right", g.tab.links).width() + $(".l-tab-links-tools", g.tab.links).width();
                        g.tab.links.ul.animate({ left: -1 * (sumwidth - mainwidth + btnWitdth + p.marginleft) });
                    }
                },
                //判断tab是否存在
                isTabItemExist: function(tabid)
                {
                    return $("li[tabid=" + tabid + "]", g.tab.links.ul).length > 0;
                },
                //增加一个tab
                addTabItem: function(options)
                {
                    if(p.onBeforeAddTabItem && p.onBeforeAddTabItem(tabid)==false) return false; 
                    var tabid = options.tabid;
                    if (tabid == undefined) tabid = g.getNewTabid();
                    var url = options.url;
					var iconcss = options.iconcss;
                    var content = options.content;
                    var text = options.text;
                    var showClose = options.showClose;
                    var height = options.height;
                    //如果已经存在
                    if (g.isTabItemExist(tabid))
                    {
                        g.selectTabItem(tabid);
						g.moveToCurrTabItem(tabid);
						g.reload(tabid);  //刷新页面
                        return;
                    }
                    var tabitem = $("<li><a></a><div class='l-tab-links-item-left'></div><div class='l-tab-links-item-right'></div><div class='l-tab-links-item-close'></div></li>");
                    var contentitem = $("<div class='l-tab-content-item'><iframe frameborder='0'></iframe></div>");
                    if (g.makeFullHeight)
                    {
                        var newheight = g.tab.height() - g.tab.links.height();
                        contentitem.height(newheight);
                    }
                    tabitem.attr("tabid", tabid);
                    contentitem.attr("tabid", tabid);
                    $("iframe", contentitem).attr("name", tabid);
                    if (showClose == undefined) showClose = true;
                    if (showClose == false) $(".l-tab-links-item-close", tabitem).remove();
                    if (text == undefined) text = tabid;
                    if (height) contentitem.height(height);
                    $("a", tabitem).text(text);
					if (iconcss) $("a", tabitem).addClass(iconcss);
                    $("iframe", contentitem).attr("src", url);
                    g.tab.links.ul.append(tabitem);
                    g.tab.content.append(contentitem);
                    g.selectTabItem(tabid);
                    if (g.setTabButton())
                    {
                        g.moveToLastTabItem();
                    }
                    //增加事件
                    g.addTabItemEvent(tabitem);
                    p.onAfterAddTabItem && p.onAfterAddTabItem(tabid);
                },
                addTabItemEvent: function(tabitem)
                {
                    tabitem.click(function()
                    {
                        var tabid = $(this).attr("tabid");
                        g.selectTabItem(tabid);
                    });
                    //右键事件支持
                    g.tab.menu && po.addTabItemContextMenuEven(tabitem);
                    $(".l-tab-links-item-close", tabitem).hover(function()
                    {
                        $(this).addClass("l-tab-links-item-close-over");
                    }, function()
                    {
                        $(this).removeClass("l-tab-links-item-close-over");
                    }).click(function()
                    {
                        var tabid = $(this).parent().attr("tabid");
                        g.removeTabItem(tabid);
                    });

                },
                //移除tab项
                removeTabItem: function(tabid)
                {
                    if(p.onBeforeRemoveTabItem && p.onBeforeRemoveTabItem(tabid)==false) return false; 
                    var currentIsSelected = $("li[tabid=" + tabid + "]", g.tab.links.ul).hasClass("l-selected");
                    if (currentIsSelected)
                    {
                        $(".l-tab-content-item[tabid=" + tabid + "]", g.tab.content).prev().show();
                        $("li[tabid=" + tabid + "]", g.tab.links.ul).prev().addClass("l-selected").siblings().removeClass("l-selected");
                    }
                    $(".l-tab-content-item[tabid=" + tabid + "]", g.tab.content).remove();
                    $("li[tabid=" + tabid + "]", g.tab.links.ul).remove();
                    g.setTabButton();
                    p.onAfterRemoveTabItem && p.onAfterRemoveTabItem(tabid);
                },
                addHeight: function(heightDiff)
                {
                    var newHeight = g.tab.height() + heightDiff;
                    g.setHeight(newHeight);
                },
                setHeight: function(height)
                {
                    g.tab.height(height);
                    g.setContentHeight();
                },
                setContentHeight: function()
                {
                    var newheight = g.tab.height() - g.tab.links.height();
                    g.tab.content.height(newheight);
                    $("> .l-tab-content-item", g.tab.content).height(newheight);
                },
                getNewTabid: function()
                {
                    var now = new Date();
                    return now.getTime();
                },
                //notabid 过滤掉tabid的
                //noclose 过滤掉没有关闭按钮的
                getTabidList : function(notabid,noclose)
                {
                    var tabidlist = [];
                    $("> li", g.tab.links.ul).each(function(){
                        if($(this).attr("tabid") 
                        && $(this).attr("tabid") != notabid
                        && (!noclose || $(".l-tab-links-item-close",this).length > 0)) 
                        { 
                             tabidlist.push($(this).attr("tabid")); 
                        }
                    });
                    return tabidlist;
                },
                removeOther :function(tabid,compel)
                {
                    var tabidlist = g.getTabidList(tabid,true); 
                    $(tabidlist).each(function(){
                        g.removeTabItem(this);
                    });
                },
                reload :function(tabid)
                {
                      $(".l-tab-content-item[tabid=" + tabid + "] iframe", g.tab.content).each(function(i,iframe){
                            $(iframe).attr("src",$(iframe).attr("src")); 
                      });
                },
                removeAll : function(compel)
                {
                    var tabidlist = g.getTabidList(null,true);
                    $(tabidlist).each(function(){
                        g.removeTabItem(this);
                    }); 
                },
                onResize: function()
                {
                    if (!p.height || typeof (p.height) != 'string' || p.height.indexOf('%') == -1) return false;
                    //set tab height
                    if (g.tab.parent()[0].tagName.toLowerCase() == "body")
                    {
                        var windowHeight = $(window).height();
                        windowHeight -= parseInt(g.tab.parent().css('paddingTop'));
                        windowHeight -= parseInt(g.tab.parent().css('paddingBottom'));
                        g.height = p.heightDiff + windowHeight * parseFloat(g.height) * 0.01;
                    }
                    else
                    {
                        g.height = p.heightDiff + (g.tab.parent().height() * parseFloat(p.height) * 0.01);
                    }
                    g.tab.height(g.height);
                    g.setContentHeight();
                }
            };
            var po = {
                menuItemClick:function(item)
                { 
                    if(!item.id || !g.actionTabid) return; 
                    switch(item.id)
                    {
                         case "close":
                            g.removeTabItem(g.actionTabid);
                            g.actionTabid = null;
                            break;
                        case "closeother":
                            g.removeOther(g.actionTabid);
                            break;
                        case "closeall":
                            g.removeAll();
                            g.actionTabid = null;
                            break;
                        case "reload":
                            g.selectTabItem(g.actionTabid);
                            g.reload(g.actionTabid); 
                            break;
                    }
                },
                addTabItemContextMenuEven:function(tabitem)
                {
                    tabitem.bind("contextmenu",function(e){
                        if(!g.tab.menu) return;
                        g.actionTabid = tabitem.attr("tabid");
                        g.tab.menu.show({ top: e.pageY, left: e.pageX }); 
                        if($(".l-tab-links-item-close",this).length == 0)
                        {
                            g.tab.menu.setDisable('close');
                        }
                        else
                        {
                            g.tab.menu.setEnable('close');
                        }
                        return false;
                    });
                }
            };
            if (p.height) g.makeFullHeight = true;
            g.tab = $(this);
            if (!g.tab.hasClass("l-tab")) g.tab.addClass("l-tab");

            if(p.contextmenu && $.ligerMenu)
            {
                g.tab.menu = $.ligerMenu({width:100,items:[
                    {text:p.closeMessage,id:'close',click:po.menuItemClick},
                    {text:p.closeOtherMessage,id:'closeother',click:po.menuItemClick},
                    {text:p.closeAllMessage,id:'closeall',click:po.menuItemClick},
                    {text:p.reloadMessage,id:'reload',click:po.menuItemClick}
                ]});
            }

            g.tab.content = $('<div class="l-tab-content"></div>');
            $("> div", g.tab).appendTo(g.tab.content);
            g.tab.content.appendTo(g.tab);
            g.tab.links = $('<div class="l-tab-links"><ul style="left: 0px; "></ul></div>');
            g.tab.links.prependTo(g.tab);
			//添加工具栏
			if($(g.tab).attr("toolsid"))
			{
				g.tab.links.append('<div id="' + $(g.tab).attr("toolsid") + '" class="l-tab-links-tools"></div>');
			}
            g.tab.links.ul = $("ul", g.tab.links);
            var haslselected = $("> div[lselected=true]", g.tab.content).length > 0;
            g.selectedTabId = $("> div[lselected=true]", g.tab.content).attr("tabid");
            $("> div", g.tab.content).each(function(i, box)
            {
                var li = $('<li class=""><a></a><div class="l-tab-links-item-left"></div><div class="l-tab-links-item-right"></div></li>'); 
                if ($(box).attr("title"))
                {
                    $("> a", li).html($(box).attr("title"));
                }
				if ($(box).attr("iconcss"))
				{
					$("> a", li).addClass($(box).attr("iconcss"));
				}
                var tabid = $(box).attr("tabid");
                if (tabid == undefined)
                {
                    tabid = g.getNewTabid();
                    $(box).attr("tabid", tabid);
                    if ($(box).attr("lselected"))
                    {
                        g.selectedTabId = tabid;
                    }
                }
                li.attr("tabid", tabid);
                if (!haslselected && i == 0) g.selectedTabId = tabid;
                var showClose = $(box).attr("showClose");
                if (showClose)
                {
                    li.append("<div class='l-tab-links-item-close'></div>");
                }
                $("> ul", g.tab.links).append(li);
                if (!$(box).hasClass("l-tab-content-item")) $(box).addClass("l-tab-content-item");
            });
            //init 
            g.selectTabItem(g.selectedTabId);

            //set content height
            if (p.height)
            {
                if (typeof (p.height) == 'string' && p.height.indexOf('%') > 0)
                {
                    g.onResize();
                    if (p.changeHeightOnResize)
                    {
                        $(window).resize(function()
                        {
                            g.onResize();
                        });
                    }
                } else
                {
                    g.setHeight(p.height);
                }
            }
            if (g.makeFullHeight)
                g.setContentHeight();


            //add even 
            $("li", g.tab.links).each(function()
            {
                g.addTabItemEvent($(this));
            });
            if (this.id == undefined) this.id = "LigerUI_" + new Date().getTime();
            LigerUIManagers[this.id + "_Tab"] = g;
            this.usedTab = true;
        });
        if (this.length == 0) return null;
        if (this.length == 1) return LigerUIManagers[this[0].id + "_Tab"];
        var managers = [];
        this.each(function() {
            managers.push(LigerUIManagers[this.id + "_Tab"]);
        });
        return managers;
    };

})(jQuery);