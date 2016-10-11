/**
 * ymPrompt.js 消息提示组件
 * @author netman8410@163.com
 */

var ymPrompt={
    version:'2.01',
    pubDate:'2008-07-21',
	show:function(args){this.initCallCfg=args}
};

//实现对象继承
ymPrompt.apply = function(o, c, defaults){
	if(defaults){
        ymPrompt.apply(o, defaults);
    }
    if (o && c && typeof c == 'object') {
        for (var p in c) {
            o[p] = c[p]
        }
    }
    return o;
};

(function(){
	var d=document,db=d.body,y=ymPrompt;
	//为元素绑定事件的函数
    var addEvent=function(env,fn,obj){
    	obj=obj||d;	//默认是document对象
    	isIE?obj.attachEvent('on'+env,fn):obj.addEventListener(env,fn,false);
    };
	//浏览器类型判断
	var browser=function(s){return navigator.userAgent.toLowerCase().indexOf(s)!=-1}; 
	var isOpera=browser('opera'),isIE=browser('msie')!=-1&&(d.all&&!isOpera);
	//等待页面加载完成
	if(!db||(typeof db!='object')){
		return addEvent('load',arguments.callee,window);
	}
	//根据html Doctype获取html根节点，以兼容非xhtml的页面
	var rootEl=d.compatMode=='CSS1Compat'?d.documentElement:db;	//根元素

    //公用函数区域,c为元素缓存
    var c={},$=function(id){
		var cc=c[id];
		return cc&&cc.parentNode?cc:c[id]=d.getElementById(id)
	};
    var $height=function(obj){return parseInt(obj.style.height)||obj.offsetHeight};    //获取元素高度
    //为元素添加css。obj:要添加css的对象，css:css文本,append:追加还是覆盖，默认覆盖
    var addCSS=function(obj,css,append){
    	!append?(isOpera?obj.setAttribute('style',css):obj.style.cssText=css):(isOpera?obj.setAttribute('style',obj.getAttribute('style')+css):obj.style.cssText+=css);
    }
    
    //弹出消息框时监听键盘事件
    var btnIndex=0,listenKeydown=function(){
    	//无弹出框或弹出框隐藏则不屏蔽操作
    	if(!btnCache||!$('promptWinContainer')||$('promptWinContainer').style.display=='none') return true;
    	var ev=window.event||arguments[0],l=btnCache.length;
		if(l==1){	//一个按钮就不用麻烦了
			$(btnCache[0].id).focus();
		}else{
			var forward=function(){return $(btnCache[++btnIndex<l?btnIndex:(btnIndex=0)].id)};//后一个按钮
			var backward=function(){return $(btnCache[--btnIndex<0?(btnIndex=l-1):btnIndex].id)}//前一个按钮
    		//tab键/左右方向键切换焦点
    		if(ev.keyCode==9||ev.keyCode==39){forward().focus()}
    		if(ev.keyCode==37){backward().focus()}
		}
    	if(ev.keyCode==13)return true;	//允许回车键
    	//屏蔽所有键盘操作包括刷新等
    	try{
    		ev.keyCode=0;
    		ev.cancelBubble=true;
    		ev.returnValue=false;
    	}catch(e){
    		try{	//2007-11-13 避免IE下event.keycode=0执行出错后转向此处而报错，先暂时用try..catch解决吧
    			ev.stopPropagation();
    			ev.preventDefault();
    		}catch(e){}
    	}
    }
    
    //按钮记录，id种子
	var btnCache,seed=0;
	//生成按钮的函数
	var mkBtn=function(txt,sign,id){
		id=id||'ymPrompt_btn_'+seed++;
		return {
			id:id,
			html:"<input type='button' id='"+id+"' onclick='ymPrompt.doHandler(\""+sign+"\")' style='cursor:pointer' class='btnStyle' value='"+txt+"' />"
		};
	}
    var okBtn=mkBtn(' 确 定 ','ok'), cancelBtn=mkBtn(' 取 消 ','cancel');
	//生成按钮组合的html
	var useBtn=function(btn){
		if(!btn) return btnCache=null;
		if(!(btn instanceof Array))btn=[btn];
		btnCache=btn;
		var html=[];
		for(var i=0;i<btn.length;i++){
			html.push(btn[i].html);
		}
		return html.join('&nbsp;&nbsp;');
	}
    //每种图标所需要的按钮
    var btnMap={alert:okBtn,succeed:okBtn,error:okBtn,confirm:[okBtn,cancelBtn]};

    //初始化
    var init=function(){
        showMask();	//显示背景层
        createWin();
    } 
    //销毁
    var destory=function(){
        hiddenMask();	//隐藏背景层
	    $('promptWinContainer').style.display='none';	//隐藏容器
    }
    //遮罩层
    var showMask=function(){
	    //第一次需要创建一个蒙板层
	    if(!$('maskLevel')){
    		var shieldStyle='position:absolute;top:0px;left:0px;width:0;height:0;background:'+y.maskAlphaColor+';text-align:center;z-index:10000;filter:alpha(opacity='+(y.maskAlpha*100)+');opacity:'+y.maskAlpha;
    		try{	//IE
    			db.appendChild(d.createElement("<div id='maskLevel' style=\'"+shieldStyle+"\'></div>"));
    			db.appendChild(d.createElement("<iframe id='maskIframe'></iframe>"));
    		}catch(e){
    			var promptShield=d.createElement('div');
    			promptShield.id='maskLevel';
    			promptShield.setAttribute('style',shieldStyle);
    			db.appendChild(promptShield);
    			promptShield=null;
    		}
    	}
    	//计算蒙板的高宽，因为页面内容可能变化，所以每次弹出都应该更新宽高
    	$('maskLevel').style.display='none';	//如果显示则先隐藏便于后面计算页面的高宽
    	//使用scrollTop和scrollWidth判断是否有滚动条更加准确，但需要加上onscroll监听,一旦发现有scrollTop或scrollLeft则使用scrollWidth/Height
    	$('maskLevel').style.width=(rootEl.scrollLeft==0?rootEl.clientWidth:rootEl.scrollWidth)+"px";
    	$('maskLevel').style.height=(rootEl.scrollTop==0?rootEl.clientHeight:rootEl.scrollHeight)+"px";
    
    	//2007-11-15 添加Iframe遮罩，仅在IE下才会存在Iframe遮罩
    	var maskIframe=$('maskIframe');
    	if(maskIframe){
    		addCSS(maskIframe,$('maskLevel').style.cssText+';z-index:9999;filter:alpha(opacity=0);opacity:0');
    		maskIframe.style.display='';
    	}
    	//显示蒙板
    	$('maskLevel').style.display='';		
    	//禁止对页面的任何操作
    	db.onselectstart = function(){return false};
    	db.oncontextmenu = function(){return false};
    };

    //隐藏遮罩层
    var hiddenMask=function(){
    	$('maskLevel').style.display='none';
    	if($('maskIframe')){$('maskIframe').style.display='none'};
    	db.onselectstart = function(){return true};
    	db.oncontextmenu = function(){return true};
    };
    //开始拖动
    var setDrag=function(){
        var event=window.event||arguments[0];
        setDrag.startDrag=true;
    	setDrag.startX=event.x||event.pageX;
    	setDrag.startY=event.y||event.pageY;
    	setDrag.containX=$("promptWinContainer").offsetLeft;
    	setDrag.containY=$("promptWinContainer").offsetTop;
    };
    addEvent("mousemove",function(){
		if(setDrag.startDrag){
			var event=window.event||arguments[0];
			try{
				$("promptWinContainer").style.left=(setDrag.containX+(event.x||event.pageX)-setDrag.startX)+"px";
				$("promptWinContainer").style.top=(setDrag.containY+(event.y||event.pageY)-setDrag.startY)+"px";
			}catch(e){}
		}
	});
    //取消拖动
    addEvent("mouseup",function(){setDrag.startDrag=false});
    
    //弹出窗体
    var createWin=function(){
    	//第一次需要创建一个容器
    	//总容器的样式
    	outerStyle='position:absolute;left:'+((rootEl.clientWidth-y.width)/2+rootEl.scrollLeft)+'px;width:'+y.width+'px;top:'+((rootEl.clientHeight-y.height)/2+rootEl.scrollTop)+'px;height:'+y.height+'px;z-index:10001';
    	if(!$('promptWinContainer')){
    		//标题容器层
    		var title_div="<div style=\'cursor:move;width:100%;overflow:hidden\' id=\'titleContainer\'><div style=\'float:left\' id=\'titleText\'>&nbsp;</div><div style=\'float:right\' id=\'titleCtrl\'><div class='ymPrompt_close' onclick='ymPrompt.doHandler(\"close\")'>&nbsp;</div></div></div>";
    		//内容容器层
    		var content_div="<table cellpadding=0 cellspacing=0 border=0 align=center width='100%' height=100% id='promptContentTable'><tr><td id='winMiddleLeft' width='3'>&nbsp;</td><td id='winMiddleCenter'>&nbsp;</td><td id='winMiddleRight' width='3'>&nbsp;</td></tr>";
    		//按钮区
            content_div+="<tr><td height='30' id='winBtnLineLeft'>&nbsp;</td><td align='center' id='winBtnLineCenter'>&nbsp;</td><td id='winBtnLineRight'>&nbsp;</td></tr>";
    		//最下部的区域
            content_div+="<tr><td id='winBottomLeft' width='3'></td><td id='winBottomCenter'></td><td id='winBottomRight' width='3'></td></tr></table>";
    		//创建总容器
            var outContainer=d.createElement('div');
    		outContainer.id='promptWinContainer';
    		outContainer.innerHTML=title_div+content_div;
    		db.appendChild(outContainer);
			addEvent('mousedown',setDrag,$('titleContainer'));
			//缓存原始高度
			y.cacheH=[$height($('promptContentTable')),$height($('winMiddleCenter'))];
			//添加监听事件
			addEvent("keydown",listenKeydown);	//键盘按下事件
			function resizeMask(){if($("maskLevel")&&$("maskLevel").style.display!="none")showMask()}	//重新计算遮罩大小
    		addEvent("resize",resizeMask,window);
    		addEvent("scroll",resizeMask,window);
    	}
		
    	//传入标题和内容
    	$('titleText').innerHTML=y.title;	//标题
    	$('winMiddleCenter').innerHTML=y.message;	//内容
		$('winMiddleCenter').className="ymPrompt_"+y.winType;	//图标类型
    	$('winBtnLineCenter').innerHTML=useBtn(btnMap[y.winType]);	//更新按钮类型
    	
		$('promptContentTable').style.height=y.cacheH[0];
		$("winMiddleCenter").style.height=y.cacheH[1];
    	//显示消息容器
    	addCSS($('promptWinContainer'),outerStyle);	//居中定位消息框
		//设定内容区的高度
		$('promptContentTable').style.height=(y.height-$('titleContainer').offsetHeight)+'px';
		
		//内容区的高度,对于xhtml页面必须有下面三行
		var contentHeight=$height($("promptContentTable"))-$height($("winBottomLeft"));
		if($("winBtnLineLeft")){contentHeight-=$height($("winBtnLineLeft"))}
		$("winMiddleCenter").style.height=contentHeight+"px";
    	$('promptWinContainer').style.display='';	//显示容器

    	if(btnCache)$(btnCache[btnIndex=0].id).focus();	//确定按钮获取焦点
    };
	//默认配置
	y.defaultCfg={
		maskAlphaColor:'#000',    //遮罩透明色
        maskAlpha:0.1,    //遮罩透明度
        title: '标题', //消息框标题
        message: '内容', //消息框按钮
        width: 300, //宽
        height: 185, //高
		handler: function(){}    //回调事件
	}
	var execFn=function(hd){
		return function(){
			destory();
			try{eval(this['handler']).call(window,hd)}catch(e){}
		}
	}
    y.apply(y, {
		winType: y.winType||'alert', //消息框类型，包括alert、succeed、error、confirm四种，默认为alert
        show:function(args){    //显示消息框
			//支持两种参数传入方式:(1)JSON方式 (2)多个参数传入
			var a=Array.prototype.slice.call(args,0),cfg=['message','width','height','title','handler','maskAlphaColor','maskAlpha'],obj={};
			if(typeof a[0]!='object'){
				for(var i=0,l=a.length;i<l;i++){
					if(a[i]){obj[cfg[i]]=a[i]}
				}
			}else{
				obj=a[0];
			}
			this.apply(this,obj,this.defaultCfg);	//先还原默认配置
            init();
        },
        doHandler:function(sign){
			destory();
			try{eval(this['handler']).call(window,sign)}catch(e){}
		}
    },y.defaultCfg);
	
	if(y.initCallCfg){
		y.show(y.initCallCfg);
	}
})();

//各消息框的相同操作
ymPrompt.apply(ymPrompt,{
    alert:function(){
        ymPrompt.winType='alert';
        ymPrompt.show(arguments);
    },
    succeedInfo:function(){
        ymPrompt.winType='succeed';
        ymPrompt.show(arguments);
    },
    errorInfo:function() {
        ymPrompt.winType='error';
        ymPrompt.show(arguments);
    },
    confirmInfo:function() {
        ymPrompt.winType='confirm';
        ymPrompt.show(arguments); 
    }
});