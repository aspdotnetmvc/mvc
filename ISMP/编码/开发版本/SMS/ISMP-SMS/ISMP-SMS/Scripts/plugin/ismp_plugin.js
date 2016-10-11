(function($){
	/*
		* 更多按钮插件.
		* string offsettop 数值加单位的形式传参，距离顶部的偏移量。
		* string offsetleft 数值加单位的形式传参，距离左侧的偏移量。
		* string offsetright 数值加单位的形式传参，距离右侧侧的偏移量。
		* class string ico1 类名传参，未点击状态的图标。
		* class string ico2 类名传参，点击后的图标。
		* string place 更多菜单要挂载到的层上
		* string siblings 获取兄弟节点得UL内容，而不是下一级的UL内容
	*/
    $.fn.more = function (offsettop, offsetleft, offsetright, ico1, ico2, place, siblings) {
		var show=false;
		var $that=$(this);
		var more_caozuo;
		function removeMenus() {
			$('#more').remove();
			$('#more_click').remove();
			show=false;
			$that.children("i").attr("class",ico1);
		};
		$(this).bind('click', function(e) {
			if($(this).hasClass($(e.target).attr("class"))||$(e.target).hasClass("fa")){
				e.stopPropagation();
			}
			more_caozuo="<ul id='more' class='more_con' style='top:"+offsettop+";left:"+offsetleft+";right:"+offsetright+"'>";
			if(siblings=="siblings"){
				more_caozuo+=$(this).siblings('ul').html();
			}else{
				more_caozuo+=$(this).children('ul').html();
			}
			more_caozuo+="</ul>"
			if(place){
				if(!show){
					$(place).append(more_caozuo);
					$(place).children("i").attr("class",ico2);
					show=true;
				}else{
					removeMenus();
				}
			}else{
				if(!show){
					$(this).append(more_caozuo);
					$(this).children("i").attr("class",ico2);
					show=true;
				}else{
					removeMenus();
				}
			}
			$("body").append("<div id='more_click' style='position:absolute;top:0px;bottom:0px;left: 0px;right: 0px;z-index: 2'></div>");
		});
		$(document).unbind('click', removeMenus).bind('click', removeMenus);
		$("#more_click").unbind('click', removeMenus).bind('click', removeMenus);
	}
	/*
		* 选项卡切换.
		* string tab 选项卡选项。
		* array,string div 切换内容。
		* number time 循环切换的速度（以毫秒为单位,可以设置为null就不会自动切换了）。
		* string active 鼠标移上li后的样式。
	*/
	$.Tab=function(tab,div,time,active){
		if(tab===undefined||div===undefined){
			alert("Tab方法属性不能为空！");
			return flase;
		}
		var tab_child=$(tab).children();
		var num=0;
		var timeout;
		var con;
		function show(xuhao){
			for(var i=0;i<tab_child.length;i++){
				tab_child.eq(i).removeClass(active);
			}
			tab_child.eq(xuhao).addClass(active);
			for(var a=0;a<div.length;a++){
				$(div[a]).css("display","none");
			}
			$(div[xuhao]).css("display","block");
		}
		if(time!==null){
			time_play(0);
			function time_play(n){
				if(n==tab_child.length){
					n=0;
				}
				show(n);
				timeout=setTimeout(function(){
					time_play(n);
				},time);
				n++;
			}
			tab_child.mouseover(function(){
				clearTimeout(timeout);
				num=$(this).index();
				show(num);
			});
			tab_child.mouseout(function(){
				time_play(num);
			});
			con=div.join(",");
			$(con).mouseover(function(){
				clearTimeout(timeout);
				num=$(this).index();
			})
			$(con).mouseout(function(){
				time_play(num);
			});
		}else{
			show(0);
			tab_child.click(function(e){
				e.preventDefault()
				num=$(this).index();
				show(num);
			});
		}
	};
})($);
/*
	* 关闭对应的模板和遮罩层.
	* JQuery obj 参数地址。
	* array closeArray 需要添加关闭时间的类或者ID，用于多个关闭按钮的情况下。
*/
function ismpCloseZhezhao(obj,closeArray){
	for(var i=0;i<closeArray.length;i++){
		$(document).on("click",closeArray[i],function(){
			obj.remove();
			$("#zhezhao").css("display","none");
		});
	}
}
/*
	* 调用JS模板接口.
	* string tempURL 模板的URL地址。
	* JQuery Locate 模板加载到的地址，用于存放模板显示的位置。
	* json datajson 模板中的变量值。
*/
function ajaxTemp(tempURL,Locate,datajson){
	$("#zhezhao").css("display","block");
	$("#zhezhao table").css("display","table");
	$.ajax({
		url: tempURL,
		dataType: 'text',
		success:function(data){
			var temp= _.template(data);
			Locate.append(temp(datajson));
			$("#zhezhao table").css("display","none");
		}
	});
}
/*
	* 打开新的连接时挂载到Tab上.
*/
function openNewTab(){
	var that=this;
	this.openTab=[];
	this.openDiv=["#ismp-right"];
	this.title;
	this.tabUrl;
	this.dataid;
	this.n=0;
	/*
		* 打开新的选项卡.
	*/
	$(document).on("click",".ismp-tabopen",function(e){
		e.preventDefault();
		var repeat=false;
		var num;
		that.title=$(this).attr('title');
		that.tabUrl=$(this).attr('href');
		that.dataid=$(this).attr('data-id');
		for(var i=0;i<that.openTab.length;i++){
			if(that.dataid===that.openTab[i]){
				repeat=true;
				num=i;
				break;
			}
		}
		if(!repeat){
			that.Addtab(that.title,that.tabUrl,that.dataid);
		}else{
			$("#ismp_tab_box a").removeClass('action');
			$("#ismp_tab_box a").eq(num+1).addClass('action');
			for(var i=0;i<that.openDiv.length;i++){
				$(that.openDiv[i]).css("display","none");
			}
			$(that.openDiv[num+1]).css("display","block");

			var $List=$("#ismp_tab_box a");
			var tab=[];
			var Element=0;
			for(var j=0;j<$List.length;j++){
				tab[j]=$List.eq(j).innerWidth()+Element+1;
				Element=tab[j];
			}

			$("#ismp_tab_box").animate({marginLeft:-tab[num]+"px"},300);
		}
	});
	/*
		* 关闭一个选项卡.
	*/
	$(document).on("click",".closetab",function(e){
		e.preventDefault();
		e.stopPropagation();
		var num=$(this).parent().index();
		$("#ismp_tab_box").children('a').eq(num).remove();
		that.openTab.splice((num-1),1);
		$(that.openDiv[num]).remove();
		that.openDiv.splice(num,1);
		var flgTab=true;
		var flgDiv=true;
		for(var i=0;i<that.openTab.length;i++){
			if($("#ismp_tab_box a").eq(i+1).attr("class")=="action"){
				flgTab=false;
			}
		};
		if(flgTab){
			$("#ismp_tab_box a").last().addClass('action');
		}
		for(var j=0;j<that.openDiv.length;j++){
			if($(that.openDiv[j+1]).css("display")=="block"){
				flgDiv=false;
			}
		};
		if(flgDiv){
			$(that.openDiv[that.openDiv.length-1]).css("display","block");
		}
		tabSlide($('.bss_right').width()-183);
		var marginleft=$("#ismp_tab_box").css("margin-left");
		marginleft=Math.abs(parseInt(marginleft));
		var sum=0;
		var boxlist=$("#ismp_tab_box a");
			for(var k=0;k<boxlist.length;k++){
			sum+=boxlist.eq(k).innerWidth()+1;
		}
		if(sum<=marginleft){
			$("#ismp_tab_box").animate({marginLeft:"0px"},300);
		}
	});
	
	/*
		* 选项卡切换.
	*/
	$(document).on("click","#ismp_tab_box a",function(e){
		e.preventDefault();
		e.stopPropagation();
		var num=$(this).index();
		if($(this).attr("class")!="action"){
			$("#ismp_tab_box a").removeClass('action');
			$(this).addClass('action');
			for(var i=0;i<that.openDiv.length;i++){
				$(that.openDiv[i]).css("display","none");
			}
			$(that.openDiv[num]).css("display","block");
		}
	});
	/*
		* 刷新当前选项卡.
	*/
	$(document).on("click",".ismp_refresh",function(){
		for(var i=1;i<$("#ismp_tab_box a").length;i++){
			if($("#ismp_tab_box a").eq(i).attr('class')=="action"){
				// $.ajax({
				// 	url: $("#ismp_tab_box a").eq(i).attr('href'),
				// 	dataType: 'html',
				// 	success:function(data){
				// 		$(that.openDiv[i]).html(data);
				// 	},
				// 	error:function(){
				// 		alert("抱歉未找到该页面");
				// 	}
				// });
				$("#zhezhao").css("display","block");
				$(that.openDiv[i]).html('<iframe  onload="closezhezhao();" src="'+$("#ismp_tab_box a").eq(i).attr('href')+'" marginheight="0" marginwidth="0" frameborder="0" width="100%" height="100%"><script>window.location.reload();</script></iframe>');
				return
			}
		}
	});
	/*
		* 关闭全部选项卡.
	*/
	$(document).on("click",".ismp_closeall",function(){
		$("#ismp_tab_box a:not(:eq(0))").remove();
		$("#ismp_tab_box a").addClass('action');
		for(var j=0;j<that.openDiv.length;j++){
			$(that.openDiv[j+1]).remove();
		}
		$("#ismp-right").css("display","block");
		that.openTab=[];
		that.openDiv=["#ismp-right"];
		$("#ismp_tab_box").css("margin-left","0px");
	});
	/*
		* 关闭其他选项卡.
	*/
	$(document).on("click",".ismp_closeother",function(){
		var num;
		for(var i=0;i<$("#ismp_tab_box a").length;i++){
			if($("#ismp_tab_box a").eq(i).attr('class')=="action"){
				num=i-1;
				break;
			}
		}
		$("#ismp_tab_box a:not(:eq(0)):not([class='action'])").remove();
		var lastTab=that.openTab[num];
		that.openTab.length=0;
		that.openTab[0]=lastTab;
		for(var j=1;j<that.openDiv.length;j++){
			if(j==(num+1)){
				continue;
			}
			$(that.openDiv[j]).remove();
		}
		var lastDiv=that.openDiv[num+1];
		that.openDiv.length=0;
		that.openDiv[0]="#ismp-right";
		if(lastDiv!="#ismp-right"){
			that.openDiv[1]=lastDiv;
		}
		$("#ismp_tab_box").css("margin-left","0px");
	});
}
openNewTab.prototype.Addtab = function(title,url,dataid){
	var that=this;
	if(dataid==undefined){
		return false;
	}
	$("#ismp_tab_box a").removeAttr("class");
	$(".bss_main").css("display","none");
	$("#ismp_tab_box").append('<a class="action" href="'+url+'">'+title+' <i class="fa fa-times-circle closetab"></i></a>');
	$("#zhezhao").css("display","block");
	$('.bss_right').append('<div class="bss_main" id="ismp-right'+that.n+'"><iframe onload="closezhezhao();" src="'+url+'" marginheight="0" marginwidth="0" frameborder="0" width="100%" height="100%"></iframe></div>');
	that.openDiv.push('#ismp-right'+that.n);
	that.openTab.push(dataid);
	that.n++;
	tabSlide($('.bss_right').width()-183);
	// $.ajax({
	// 	url: url,
	// 	dataType: 'html',
	// 	success:function(data){
	// 		$("#ismp_tab_box a").removeAttr("class");
	// 		$(".bss_main").css("display","none");
	// 		$("#ismp_tab_box").append('<a class="action" href="'+url+'">'+title+' <i class="fa fa-times-circle closetab"></i></a>');
	// 		$('.bss_right').append('<div class="bss_main" id="ismp-right'+that.n+'">'+data+'</div>');
	// 		that.openDiv.push('#ismp-right'+that.n);
	// 		that.openTab.push(dataid);
	// 		that.n++;
	// 		tabSlide($('.bss_right').width()-183);
	// 	},
	// 	error:function(){
	// 		alert("抱歉未找到该页面");
	// 	}
	// });
};
/*
	* 在iframe中调用打开一个新标签页面
*/
openNewTab.prototype.iframeopentab = function (obj, event) {
	var that=this;
	event.preventDefault();
	var repeat=false;
	var num;
	that.title=$(obj).attr('title');
	that.tabUrl=$(obj).attr('href');
	that.dataid=$(obj).attr('data-id');
	for(var i=0;i<that.openTab.length;i++){
		if(that.dataid===that.openTab[i]){
			repeat=true;
			num=i;
			break;
		}
	}
	if(!repeat){
		that.Addtab(that.title,that.tabUrl,that.dataid);
	}else{
		$("#ismp_tab_box a").removeClass('action');
		$("#ismp_tab_box a").eq(num+1).addClass('action');
		for(var i=0;i<that.openDiv.length;i++){
			$(that.openDiv[i]).css("display","none");
		}
		$(that.openDiv[num+1]).css("display","block");

		var $List=$("#ismp_tab_box a");
		var tab=[];
		var Element=0;
		for(var j=0;j<$List.length;j++){
			tab[j]=$List.eq(j).innerWidth()+Element+1;
			Element=tab[j];
		}
		$("#ismp_tab_box").animate({marginLeft:-tab[num]+"px"},300);
	}	
}
/*
	* 在iframe中调用关闭当前标签页面
*/
openNewTab.prototype.closeTab=function(event){
	var that=this;
	if(event){
		event.preventDefault();
		event.stopPropagation();
	}
	var num=$("#ismp_tab_box").find('a.action').index();
	console.log($("#ismp_tab_box").find('a.action').index());
	$("#ismp_tab_box").children('a').eq(num).remove();
	that.openTab.splice((num-1),1);
	$(that.openDiv[num]).remove();
	that.openDiv.splice(num,1);
	var flgTab=true;
	var flgDiv=true;
	for(var i=0;i<that.openTab.length;i++){
		if($("#ismp_tab_box a").eq(i+1).attr("class")=="action"){
			flgTab=false;
		}
	};
	if(flgTab){
		$("#ismp_tab_box a").last().addClass('action');
	}
	for(var j=0;j<that.openDiv.length;j++){
		if($(that.openDiv[j+1]).css("display")=="block"){
			flgDiv=false;
		}
	};
	if(flgDiv){
		$(that.openDiv[that.openDiv.length-1]).css("display","block");
	}
	tabSlide($('.bss_right').width()-183);
	var marginleft=$("#ismp_tab_box").css("margin-left");
	marginleft=Math.abs(parseInt(marginleft));
	var sum=0;
	var boxlist=$("#ismp_tab_box a");
		for(var k=0;k<boxlist.length;k++){
		sum+=boxlist.eq(k).innerWidth()+1;
	}
	if(sum<=marginleft){
		$("#ismp_tab_box").animate({marginLeft:"0px"},300);
	}
}
/*
	* 在iframe中刷新指定页面
*/
openNewTab.prototype.refresh=function(title){
	var that=this;
	for(var i=1;i<$("#ismp_tab_box a").length;i++){
		if($("#ismp_tab_box a").eq(i).text()==title+' '){
			$(that.openDiv[i]).html('<iframe  onload="closezhezhao();" src="'+$("#ismp_tab_box a").eq(i).attr('href')+'" marginheight="0" marginwidth="0" frameborder="0" width="100%" height="100%"><script>window.location.reload();</script></iframe>');
			return
		}
	}
}
var ismpTab=new openNewTab();
/*
	* 用来关闭遮罩层（主要用于openNewTab对象中）
*/
function closezhezhao(){
	$("#zhezhao").css("display","none");
}

/*
	* Tab选项卡滚动
*/
function tabSlide(showWidth){
	var tabWidth=[];
	var leftWidth=[];
		leftWidth[0]=0;
	var node=1;
	var clicknode=0;
	var backElement=0;
	var nowElement;
	var offset=0;
	var $tabList=$("#ismp_tab_box a");
	for(var i=0;i<$tabList.length;i++){
		tabWidth[i]=$tabList.eq(i).innerWidth()+backElement+1;
		backElement=tabWidth[i];
	}
	for(var j=0;j<tabWidth.length;j++){
		if(tabWidth[j]-offset>showWidth){
			leftWidth[node]=tabWidth[j-1];
			offset=tabWidth[j-1];
			node++;
		}
	}
	function tabLeftSlide(){
		clicknode--;
		if(clicknode<=0){
			clicknode=0;
		}
		$("#ismp_tab_box").animate({marginLeft: -leftWidth[clicknode]+'px'}, 50);
	}
	function tabRightSlide(){
		clicknode++;
		if(clicknode>node){
			clicknode=node;
		}
		$("#ismp_tab_box").animate({marginLeft: -leftWidth[clicknode]+'px'}, 50);
	}
	$('.ismp_tab_left').off('click').on('click',tabLeftSlide);
	$('.ismp_tab_right').off('click').on('click',tabRightSlide);
}

$(function(){
	/*
		* 遮罩层的兼容性（兼容IE）
	*/
	if((navigator.userAgent.indexOf('MSIE') >= 0) && (navigator.userAgent.indexOf('Opera') < 0)){
		$("#zhezhao_con").html("<div id='loading'>Loading......</div>");
		$("#loading").css({
			overflow: "hidden",
			fontSize:"26px",
			width: "140px",
			position: "absolute",
			top:"50%",
			left: "50%",
			marginTop: "-13px",
			marginLeft:"-70px"
		});
		var n=103;
		setInterval(function(){
			if(n>=135){
				n=103;
			}else{
				n++;
			}
			$('#loading').css("width",n+"px");
		},25);
	}
	$("#zhezhao").css("opacity","0.5");
});