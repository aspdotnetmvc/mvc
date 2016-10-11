//工具栏样式
$(document).ready(function() {
    
    $("#tools li").hover(function(){
        $(this).addClass("onHover");
    },
    function()
    {
        $(this).removeClass("onHover");
    }).mousedown(function(){
        $("#tools li").removeClass("onHover");
        $(this).addClass("onDown");
    }).mouseup(function(){
        $("#tools li").removeClass("onDown");
        $(this).addClass("onHover");
    }); 
    
//        $("#tools li").mousedown(function(){
//            $("#tools li").removeClass("onHover");
//            $(this).addClass("onDown");
//        }).mouseup(function(){
//            $("#tools li").removeClass("onDown");
//            $(this).addClass("onHover");
//        });
});


//提示框返回事件
function handler(tp)
{
	if(tp=='ok')
	{
		window.location.href = window.location.href;
	}
	if(tp=='cancel')
	{
		//cancelFn();
	}
	if(tp=='close')
	{
		//closeFn()
	}

}