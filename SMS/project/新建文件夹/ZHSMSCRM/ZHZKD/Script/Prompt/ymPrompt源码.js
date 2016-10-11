/**
 * ymPrompt.js ��Ϣ��ʾ���
 * @author netman8410@163.com
 */

var ymPrompt={
    version:'2.01',
    pubDate:'2008-07-21',
	show:function(args){this.initCallCfg=args}
};

//ʵ�ֶ���̳�
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
	//ΪԪ�ذ��¼��ĺ���
    var addEvent=function(env,fn,obj){
    	obj=obj||d;	//Ĭ����document����
    	isIE?obj.attachEvent('on'+env,fn):obj.addEventListener(env,fn,false);
    };
	//����������ж�
	var browser=function(s){return navigator.userAgent.toLowerCase().indexOf(s)!=-1}; 
	var isOpera=browser('opera'),isIE=browser('msie')!=-1&&(d.all&&!isOpera);
	//�ȴ�ҳ��������
	if(!db||(typeof db!='object')){
		return addEvent('load',arguments.callee,window);
	}
	//����html Doctype��ȡhtml���ڵ㣬�Լ��ݷ�xhtml��ҳ��
	var rootEl=d.compatMode=='CSS1Compat'?d.documentElement:db;	//��Ԫ��

    //���ú�������,cΪԪ�ػ���
    var c={},$=function(id){
		var cc=c[id];
		return cc&&cc.parentNode?cc:c[id]=d.getElementById(id)
	};
    var $height=function(obj){return parseInt(obj.style.height)||obj.offsetHeight};    //��ȡԪ�ظ߶�
    //ΪԪ�����css��obj:Ҫ���css�Ķ���css:css�ı�,append:׷�ӻ��Ǹ��ǣ�Ĭ�ϸ���
    var addCSS=function(obj,css,append){
    	!append?(isOpera?obj.setAttribute('style',css):obj.style.cssText=css):(isOpera?obj.setAttribute('style',obj.getAttribute('style')+css):obj.style.cssText+=css);
    }
    
    //������Ϣ��ʱ���������¼�
    var btnIndex=0,listenKeydown=function(){
    	//�޵�����򵯳������������β���
    	if(!btnCache||!$('promptWinContainer')||$('promptWinContainer').style.display=='none') return true;
    	var ev=window.event||arguments[0],l=btnCache.length;
		if(l==1){	//һ����ť�Ͳ����鷳��
			$(btnCache[0].id).focus();
		}else{
			var forward=function(){return $(btnCache[++btnIndex<l?btnIndex:(btnIndex=0)].id)};//��һ����ť
			var backward=function(){return $(btnCache[--btnIndex<0?(btnIndex=l-1):btnIndex].id)}//ǰһ����ť
    		//tab��/���ҷ�����л�����
    		if(ev.keyCode==9||ev.keyCode==39){forward().focus()}
    		if(ev.keyCode==37){backward().focus()}
		}
    	if(ev.keyCode==13)return true;	//����س���
    	//�������м��̲�������ˢ�µ�
    	try{
    		ev.keyCode=0;
    		ev.cancelBubble=true;
    		ev.returnValue=false;
    	}catch(e){
    		try{	//2007-11-13 ����IE��event.keycode=0ִ�г����ת��˴�����������ʱ��try..catch�����
    			ev.stopPropagation();
    			ev.preventDefault();
    		}catch(e){}
    	}
    }
    
    //��ť��¼��id����
	var btnCache,seed=0;
	//���ɰ�ť�ĺ���
	var mkBtn=function(txt,sign,id){
		id=id||'ymPrompt_btn_'+seed++;
		return {
			id:id,
			html:"<input type='button' id='"+id+"' onclick='ymPrompt.doHandler(\""+sign+"\")' style='cursor:pointer' class='btnStyle' value='"+txt+"' />"
		};
	}
    var okBtn=mkBtn(' ȷ �� ','ok'), cancelBtn=mkBtn(' ȡ �� ','cancel');
	//���ɰ�ť��ϵ�html
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
    //ÿ��ͼ������Ҫ�İ�ť
    var btnMap={alert:okBtn,succeed:okBtn,error:okBtn,confirm:[okBtn,cancelBtn]};

    //��ʼ��
    var init=function(){
        showMask();	//��ʾ������
        createWin();
    } 
    //����
    var destory=function(){
        hiddenMask();	//���ر�����
	    $('promptWinContainer').style.display='none';	//��������
    }
    //���ֲ�
    var showMask=function(){
	    //��һ����Ҫ����һ���ɰ��
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
    	//�����ɰ�ĸ߿���Ϊҳ�����ݿ��ܱ仯������ÿ�ε�����Ӧ�ø��¿��
    	$('maskLevel').style.display='none';	//�����ʾ�������ر��ں������ҳ��ĸ߿�
    	//ʹ��scrollTop��scrollWidth�ж��Ƿ��й���������׼ȷ������Ҫ����onscroll����,һ��������scrollTop��scrollLeft��ʹ��scrollWidth/Height
    	$('maskLevel').style.width=(rootEl.scrollLeft==0?rootEl.clientWidth:rootEl.scrollWidth)+"px";
    	$('maskLevel').style.height=(rootEl.scrollTop==0?rootEl.clientHeight:rootEl.scrollHeight)+"px";
    
    	//2007-11-15 ���Iframe���֣�����IE�²Ż����Iframe����
    	var maskIframe=$('maskIframe');
    	if(maskIframe){
    		addCSS(maskIframe,$('maskLevel').style.cssText+';z-index:9999;filter:alpha(opacity=0);opacity:0');
    		maskIframe.style.display='';
    	}
    	//��ʾ�ɰ�
    	$('maskLevel').style.display='';		
    	//��ֹ��ҳ����κβ���
    	db.onselectstart = function(){return false};
    	db.oncontextmenu = function(){return false};
    };

    //�������ֲ�
    var hiddenMask=function(){
    	$('maskLevel').style.display='none';
    	if($('maskIframe')){$('maskIframe').style.display='none'};
    	db.onselectstart = function(){return true};
    	db.oncontextmenu = function(){return true};
    };
    //��ʼ�϶�
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
    //ȡ���϶�
    addEvent("mouseup",function(){setDrag.startDrag=false});
    
    //��������
    var createWin=function(){
    	//��һ����Ҫ����һ������
    	//����������ʽ
    	outerStyle='position:absolute;left:'+((rootEl.clientWidth-y.width)/2+rootEl.scrollLeft)+'px;width:'+y.width+'px;top:'+((rootEl.clientHeight-y.height)/2+rootEl.scrollTop)+'px;height:'+y.height+'px;z-index:10001';
    	if(!$('promptWinContainer')){
    		//����������
    		var title_div="<div style=\'cursor:move;width:100%;overflow:hidden\' id=\'titleContainer\'><div style=\'float:left\' id=\'titleText\'>&nbsp;</div><div style=\'float:right\' id=\'titleCtrl\'><div class='ymPrompt_close' onclick='ymPrompt.doHandler(\"close\")'>&nbsp;</div></div></div>";
    		//����������
    		var content_div="<table cellpadding=0 cellspacing=0 border=0 align=center width='100%' height=100% id='promptContentTable'><tr><td id='winMiddleLeft' width='3'>&nbsp;</td><td id='winMiddleCenter'>&nbsp;</td><td id='winMiddleRight' width='3'>&nbsp;</td></tr>";
    		//��ť��
            content_div+="<tr><td height='30' id='winBtnLineLeft'>&nbsp;</td><td align='center' id='winBtnLineCenter'>&nbsp;</td><td id='winBtnLineRight'>&nbsp;</td></tr>";
    		//���²�������
            content_div+="<tr><td id='winBottomLeft' width='3'></td><td id='winBottomCenter'></td><td id='winBottomRight' width='3'></td></tr></table>";
    		//����������
            var outContainer=d.createElement('div');
    		outContainer.id='promptWinContainer';
    		outContainer.innerHTML=title_div+content_div;
    		db.appendChild(outContainer);
			addEvent('mousedown',setDrag,$('titleContainer'));
			//����ԭʼ�߶�
			y.cacheH=[$height($('promptContentTable')),$height($('winMiddleCenter'))];
			//��Ӽ����¼�
			addEvent("keydown",listenKeydown);	//���̰����¼�
			function resizeMask(){if($("maskLevel")&&$("maskLevel").style.display!="none")showMask()}	//���¼������ִ�С
    		addEvent("resize",resizeMask,window);
    		addEvent("scroll",resizeMask,window);
    	}
		
    	//������������
    	$('titleText').innerHTML=y.title;	//����
    	$('winMiddleCenter').innerHTML=y.message;	//����
		$('winMiddleCenter').className="ymPrompt_"+y.winType;	//ͼ������
    	$('winBtnLineCenter').innerHTML=useBtn(btnMap[y.winType]);	//���°�ť����
    	
		$('promptContentTable').style.height=y.cacheH[0];
		$("winMiddleCenter").style.height=y.cacheH[1];
    	//��ʾ��Ϣ����
    	addCSS($('promptWinContainer'),outerStyle);	//���ж�λ��Ϣ��
		//�趨�������ĸ߶�
		$('promptContentTable').style.height=(y.height-$('titleContainer').offsetHeight)+'px';
		
		//�������ĸ߶�,����xhtmlҳ���������������
		var contentHeight=$height($("promptContentTable"))-$height($("winBottomLeft"));
		if($("winBtnLineLeft")){contentHeight-=$height($("winBtnLineLeft"))}
		$("winMiddleCenter").style.height=contentHeight+"px";
    	$('promptWinContainer').style.display='';	//��ʾ����

    	if(btnCache)$(btnCache[btnIndex=0].id).focus();	//ȷ����ť��ȡ����
    };
	//Ĭ������
	y.defaultCfg={
		maskAlphaColor:'#000',    //����͸��ɫ
        maskAlpha:0.1,    //����͸����
        title: '����', //��Ϣ�����
        message: '����', //��Ϣ��ť
        width: 300, //��
        height: 185, //��
		handler: function(){}    //�ص��¼�
	}
	var execFn=function(hd){
		return function(){
			destory();
			try{eval(this['handler']).call(window,hd)}catch(e){}
		}
	}
    y.apply(y, {
		winType: y.winType||'alert', //��Ϣ�����ͣ�����alert��succeed��error��confirm���֣�Ĭ��Ϊalert
        show:function(args){    //��ʾ��Ϣ��
			//֧�����ֲ������뷽ʽ:(1)JSON��ʽ (2)�����������
			var a=Array.prototype.slice.call(args,0),cfg=['message','width','height','title','handler','maskAlphaColor','maskAlpha'],obj={};
			if(typeof a[0]!='object'){
				for(var i=0,l=a.length;i<l;i++){
					if(a[i]){obj[cfg[i]]=a[i]}
				}
			}else{
				obj=a[0];
			}
			this.apply(this,obj,this.defaultCfg);	//�Ȼ�ԭĬ������
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

//����Ϣ�����ͬ����
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