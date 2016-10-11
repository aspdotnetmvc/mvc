/**
* jQuery ligerUI 1.0.1
* 
* Author leoxie [ gd_star@163.com ] 
* 
*/

(function($) {
    $.ligerDefaults = $.ligerDefaults || {};
    $.ligerDefaults.Drag = {
        onStartDrag: false,
        onDrag: false,
        onStopDrag: false
    };

    ///	<param name="$" type="jQuery"></param>  
    $.fn.ligerDrag = function(p) {
        p = $.extend({}, $.ligerDefaults.Drag, p || {});
        return this.each(function() {
            if (this.useDrag) return;
            var g = {
                start: function(e) {
                    $('body').css('cursor', 'move');
                    g.current = {
                        target: g.target,
                        left: g.target.offset().left,
                        top: g.target.offset().top,
                        startX: e.pageX || e.screenX,
                        startY: e.pageY || e.clientY
                    };
                    $(document).bind('mouseup.drag', g.stop);
                    $(document).bind('mousemove.drag', g.drag);
                    if (p.onStartDrag) p.onStartDrag(g.current, e);
                },
                drag: function(e) {
                    if (!g.current) return;
                    var pageX = e.pageX || e.screenX;
                    var pageY = e.pageY || e.screenY;
                    g.current.diffX = pageX - g.current.startX;
                    g.current.diffY = pageY - g.current.startY;
                    if (p.onDrag) {
                        if (p.onDrag(g.current, e) != false) {
                            g.applyDrag();
                        }
                    }
                    else {
                        g.applyDrag();
                    }

                    //                    //每30毫秒触发一次
                    //                    $(document).unbind('mousemove.drag');
                    //                    setTimeout(function ()
                    //                    {
                    //                        $(document).bind('mousemove.drag', g.drag);
                    //                    }, 30);
                },
                stop: function(e) {
                    $(document).unbind('mousemove.drag');
                    $(document).unbind('mouseup.drag');
                    $("body").css("cursor", "");
                    if (p.onStopDrag) p.onStopDrag(g.current, e);
                    g.current = null;
                },
                //更新当前坐标
                applyDrag: function() {
                    if (g.current.diffX) {
                        g.target.css("left", (g.current.left + g.current.diffX));
                    }
                    if (g.current.diffY) {
                        g.target.css("top", (g.current.top + g.current.diffY));
                    }
                }
            };
            g.target = $(this);
            if (p.handler == undefined || p.handler == null)
                g.handler = $(this);
            else
                g.handler = (typeof p.handler == 'string' ? $(p.handler, this) : p.handle);
            g.handler.hover(function() {
                $('body').css('cursor', 'move');
            }, function() {
                $("body").css("cursor", "default");
            }).mousedown(function(e) { 
                g.start(e);
                return false;
            });
            this.useDrag = true;
        });
    };
})(jQuery);