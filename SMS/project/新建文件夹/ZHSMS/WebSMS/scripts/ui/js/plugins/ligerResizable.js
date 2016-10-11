/**
* jQuery ligerUI 1.0.0
* 
* Author leoxie [ gd_star@163.com ] 
* 
*/
(function ($)
{
    $.ligerDefaults = $.ligerDefaults || {};
    $.ligerDefaults.Resizable = {
        handles: 'n, e, s, w, ne, se, sw, nw',
        maxWidth: 2000,
        maxHeight: 2000,
        minWidth: 20,
        minHeight: 20,
        onStartResize: function (e) { },
        onResize: function (e) { },
        onStopResize: function (e) { },
        onEndResize: null
    };

    $.fn.ligerResizable = function (p)
    {
        return this.each(function ()
        {
            p = $.extend({}, $.ligerDefaults.Resizable, p || {});
            var g = {
                init: function ()
                {
                    g.target.append('<div class="l-resizable"></div>');
                    //add handler dom elements 
                    var handles = p.handles.split(',');
                    for (var i = 0; i < handles.length; i++)
                    {
                        switch (handles[i].replace(/(^\s*)|(\s*$)/g, "")) //trim
                        {
                            case "nw":
                                g.target.append('<div class="l-resizable-h-l" direction="nw"></div>');
                                break;
                            case "ne":
                                g.target.append('<div class="l-resizable-h-r" direction="ne"></div>');
                                break;
                            case "n":
                                g.target.append('<div class="l-resizable-h-c" direction="n"></div>');
                                break;
                            case "w":
                                g.target.append('<div class="l-resizable-c-l" direction="w"></div>');
                                break;
                            case "e":
                                g.target.append('<div class="l-resizable-c-r" direction="e"></div>');
                                break;
                            case "sw":
                                g.target.append('<div class="l-resizable-f-l" direction="sw"></div>');
                                break;
                            case "se":
                                g.target.append('<div class="l-resizable-f-r" direction="se"></div>');
                                break;
                            case "s":
                                g.target.append('<div class="l-resizable-f-c" direction="s"></div>');
                                break;
                        }
                    }
                    $("> .l-resizable-h-c , > .l-resizable-f-c", g.target).width(g.target.width());
                    $("> .l-resizable-c-l , > .l-resizable-c-r", g.target).height(g.target.height());
                    g.target.resizable = $("> .l-resizable", g.target);
                },
                start: function (e, dir)
                {

                    if ($.browser.msie || $.browser.safari) 
                        $('body').bind('selectstart', function () { return false; }); // ����ѡ��
                    $(".l-window-mask-nobackground").remove();
                    $("<div class='l-window-mask-nobackground' style='display: block;'></div>").appendTo($("body"));
                    g.target.resizable.css({
                        width: g.target.width(),
                        height: g.target.height(),
                        left: 0,
                        top: 0
                    });
                    g.current = {
                        dir: dir,
                        left: g.target.offset().left,
                        top: g.target.offset().top,
                        width: g.target.width(),
                        height: g.target.height()
                    };
                    $(document).bind('mouseup', g.stop);
                    $(document).bind('mousemove', g.drag);
                    g.target.resizable.show();
                    if (p.onStartResize) p.onStartResize(g.current, e);
                },
                drag: function (e)
                {
                    var dir = g.current.dir;
                    var resizableObj = g.target.resizable[0];
                    var width = g.current.width;
                    var height = g.current.height;
                    var moveWidth = (e.pageX || e.screenX) - g.current.left;
                    var moveHeight = (e.pageY || e.clientY) - g.current.top;
                    if (dir.indexOf("e") >= 0) moveWidth -= width;
                    if (dir.indexOf("s") >= 0) moveHeight -= height;
                    if (dir != "n" && dir != "s")
                    {
                        width += (dir.indexOf("w") >= 0) ? -moveWidth : moveWidth;
                    }
                    if (width >= p.minWidth)
                    {
                        if (dir.indexOf("w") >= 0)
                        {
                            resizableObj.style.left = moveWidth + 'px';
                        }
                        if (dir != "n" && dir != "s")
                        {
                            resizableObj.style.width = width + 'px';
                        }
                    }
                    if (dir != "w" && dir != "e")
                    {
                        height += (dir.indexOf("n") >= 0) ? -moveHeight : moveHeight;
                    }
                    if (height >= p.minHeight)
                    {
                        if (dir.indexOf("n") >= 0)
                        {
                            resizableObj.style.top = moveHeight + 'px';
                        }
                        if (dir != "w" && dir != "e")
                        {
                            resizableObj.style.height = height + 'px';
                        }
                    }
                    g.current.newWidth = width;
                    g.current.newHeight = height;
                    g.current.diffTop = parseInt(resizableObj.style.top);
                    if (isNaN(g.current.diffTop)) g.current.diffTop = 0;
                    g.current.diffLeft = parseInt(resizableObj.style.left);
                    if (isNaN(g.current.diffLeft)) g.current.diffLeft = 0;
                    $("body").css("cursor", dir + '-resize');
                    if (p.onResize) p.onResize(g.current, e);
                },
                stop: function (e)
                {
                    if ($.browser.msie || $.browser.safari)
                        $('body').unbind('selectstart');
                    $(".l-window-mask-nobackground").remove();
                    if (!p.onStopResize) g.applyResize();
                    else if (p.onStopResize(g.current, e) != false) g.applyResize();
                    p.onEndResize && p.onEndResize(g.current, e);
                    $("body").css("cursor", "");
                    g.target.resizable.hide();
                    $(document).unbind('mousemove', g.drag);
                    $(document).unbind('mouseup', g.stop);
                },
                applyResize: function ()
                {
                    var top = 0;
                    var left = 0;
                    if (!isNaN(parseInt(g.target.css('top'))))
                        top = parseInt(g.target.css('top'));
                    if (!isNaN(parseInt(g.target.css('left'))))
                        left = parseInt(g.target.css('left'));
                    if (g.current.diffTop != undefined)
                    {
                        if (g.current.diffTop != 0)
                            g.target.css({ top: top + g.current.diffTop });
                        if (g.current.diffLeft != 0)
                            g.target.css({ left: left + g.current.diffLeft });
                        g.target.css({
                            width: g.current.newWidth,
                            height: g.current.newHeight
                        });
                    }
                }
            };
            g.target = $(this);
            g.init();
            $(">div", g.target).mousedown(function (e)
            {
                if (!$(this).attr("direction")) return;
                g.start(e, $(this).attr("direction"));
                return false;
            });
        });
    };
})(jQuery);