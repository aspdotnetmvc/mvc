/**
 * By www.865171.cn
 * q  q : 215288671
 * Date : 2010.4.22
 */
var Effect = CC.create();

Effect.LEFT = 0x01;
Effect.RIGHT = 0x02;
Effect.UP = 0x04;
Effect.DOWN = 0x08;

CC.extend(Effect,{
	copyPosition : function(des, src) {
		
		CC.setStyle(des,'width',Posi);
	}
});


CC.extend(Effect.prototype,{
	//延迟x秒后开始.
	delay : 0.0,
	
	//持续时间
	duration : 1.0, //秒
	
	//帧速/秒
	fps : 100,
	
	//是否同步渲染动作
	sync : true,
	
	//
	actions : null,
	
	//作用元素
	element : null,
	
	timerId : null,
	
	startOn : -1.0,
	
	state : 'idle',
	
	finishOn : -1.0,
	
	currentFrame : 0,
	
	syncLoop : true,
	
	_currActionIdx : 0,
	
	//结束后回调
	onstop : null,
	
	initialize : function(opts){
		CC.extend(this,opts);
		this.element = $(this.element);
		this.actions = this.actions || [];
	},
	
	start : function(opts){
		if(opts){
			CC.extend(this,opts);
		}

    this.startOn   = this.delay*1000;
    this.finishOn  = this.startOn+(this.duration*1000);
    this.totalTime    = this.finishOn-this.startOn;
    this.totalFrames  = Math.ceil(this.fps*this.duration);
    
    //初始化
		
		for(var i=0,len=this.actions.length;i<len;i++){
				this.actions[i].initAction(this);
		}
    
    //start on delay.
    if(this.startOn > 0.0){
    	this.state = 'delay';
    	this.timerId = setTimeout(this._delayStart.bind(this),startOn);
    	return;
    }
		
    this._startFPS();
	},
	
	render : function() {
		if(this.busy){
			return;
		}
		
		this.busy = true;
		
		this.currentFrame++;
		
		if(this.currentFrame > this.totalFrames) {
			this.abort();
			this.busy = false;
			return;
		}
		
		//同步,一帧内同时渲染所有动作.
		if(this.sync){
			for(var i=0,len=this.actions.length;i<len;i++){
				this.actions[i].render(this);
			}
		}else {
			if(this._currActionIdx>=this.actions.length){
				if(this.syncLoop){
					this._currActionIdx=0;
				}else {
					this.abort();
					return;
				}
			}
			this.actions[this._currActionIdx++].render(this);
		}
		this.busy = false;
	},
	
	_startFPS : function() {
		this.timerId = setInterval(this.render.bind(this),Math.ceil(1.0/this.fps*1000));
		state = 'running';
	},
	
	_delayStart : function() {
		if(this.timerId){
			clearTimeout(this.timerId);
		}
		this._startFPS();
	},
	
	abort : function() {
		
		if(this.state=='wait') {
			clearTimeout(this.timerId);
			this.state = 'abort';
			if(this.onstop){this.onstop(this);}
			return;
		}
		
		//结束
		
		for(var i=0,len=this.actions.length;i<len;i++){
				this.actions[i].endAction(this);
		}
		
		this.state='stop';
		
		if(this.timerId){
			clearInterval(this.timerId);
			this.timerId = null;
		}
		if(this.onstop){this.onstop(this);}
	},
	
	//先前设置值不会被重置.
	reset : function(){
		//reset
		this.startOn = -1.0;
		this.finishOn = -1.0;
		this.currentFrame = 0;
		this._currActionIdx = 0;
	},
	
	recoverEffect : function() {
		for(var i=0,len=this.actions.length;i<len;i++) {
				this.actions[i].recoverEffect(this);
		}
	},
	
	add : function(action) {
		this.actions.push(action);
		return this;
	},
	
	remove : function(action) {
		this.actions.remove(action);
		return this;
	},
	
	setOpacity: function(value) {
    CC.style(this.element,'opacity',value);
  },
  
  getOpacity : function() {
  	return CC.getOpacity(this.element);
  },
  
  saveClientView : function() {
  	this.viewData = Position.cumulativeOffset(this.element);
  	this.viewData.push(this.element.offsetWidth);
  	this.viewData.push(this.element.offsetHeight);
  },
  
  recoveClientrView : function(){
  	this.element.style.left = this.viewData[0];
  	this.element.style.top = this.viewData[1];
  	this.element.style.width = this.viewData[2];
  	this.element.style.height = this.viewData[3];
  }
  
});

/**
 * 动作类,用于实现效果细节.
 **/
var ActionBase = CC.create();

CC.extend(ActionBase, {
	
	//接口函数
	
	//动作启动时初始化,常用于保留现场.
	initAction : fGo,
	
	//每帧时渲染.
	render : fGo,
	
	//动作结束后调用.
	endAction : fGo,
	
	//常用于恢复现场.
	recoverEffect : fGo,
	
	from : false,
	
	to : false,
	
	renderDelta : false,
	
	currentValue : false,
	
	fromToDelta : false,
	
	initialize : function(opts){
		CC.extend(this,opts);
		//如果from,to已设置,计算fromToDelta.
		if(this.from!==false && this.to !== false){
			this.fromToDelta  = this.to-this.from;
		}
	}
});

/**
 * 元素透明效果.
 * Alpha : 0.0--1.0 完全透明到完全不透明.
 * 示例 : new Effect({element:'layer',duration:3.0}).add(new Opacity({from:0.0,to:1.0})).start();
 */
var Opacity = CC.create();
CC.extend(Opacity.prototype, ActionBase);
CC.extend(Opacity.prototype,{
	
	from : 0.0,
	
	to : 1.0,
	
	initAction : function(e) {
		this.e = e;
		this.initOpacity = e.getOpacity();
		e.element.style.display='none';
		e.setOpacity(this.from);
		e.element.style.display='block';
		this.currentValue = this.from;
		this.renderDelta = this.fromToDelta/e.totalFrames;
	},
	
	render : function(e){
		this.currentValue += this.renderDelta; 
		e.setOpacity(this.currentValue);
	},
	
	recoverEffect : function(){
		this.e.setOpacity(this.initOpacity);
	}
});

/**
 * 缩放特效:how = 'expand'或'fold'.
 * 方向,direction : Effect.LEFT | Effect.RIGHT | Effect.UP | Effect.DOWN.
 */
var Fold = CC.create();

Fold.Swich = [ 0,-1,0,1,  0,0,0,1,  -1,0,1,0,  0,0,1,0, 0,0,0,-1, 0,1,0,-1, 0,0,-1,0, 1,0,-1,0];

CC.extend(Fold.prototype, ActionBase);
CC.extend(Fold.prototype,{
	//默认向上.
	direction : Effect.UP,
	
	//缩放类型,expand,fold
	how : 'expand',
	
	//缩放大小数组,放置[上,下,左,右]缩放.
	delta : false,
	
	initAction : function(e) {
		this._how = this.how == 'expand' ? 0 : 1;
		var element = e.element;
		
		//保存原来大小位置信息.
		if(!e.viewData) {
			e.saveClientView();
		}
		var del = this.delta;
		if(!del){
			del = this.delta = [];
		}
		
		var dl = (this.direction & Effect.LEFT) != 0x0,
				dr = (this.direction & Effect.RIGHT) != 0x0,
				du = (this.direction & Effect.UP) != 0x0,
				dd = (this.direction & Effect.DOWN) != 0x0;
		
		//如果缩放度未设置，缩放大小设置为其高或宽.
		
		if(CC.isUndefined(del[0])) {
			del[0] = 0.0;
			if(du || dd) {
				del[0] = e.viewData[3];
			}
		}
		
		if(CC.isUndefined(del[1])) {
			del[1] = 0.0;
			if(du || dd) {
				del[1] = e.viewData[3];
			}
		}
		
		if(CC.isUndefined(del[2])) {
			del[2] = 0.0;
			if(dl || dr) {
				del[2] = e.viewData[2];
					if(false){//TODO:自动调整向中心靠近.
						var t = e.viewData[3]*3/2;
						del[0] = t;
						del[1] = t*2;
					}
			}
		}
		
		if(CC.isUndefined(del[3])) {
			del[3] = 0.0;
			if(dl || dr) {
				del[3] = e.viewData[2];
			}
		}
		
		this.renderDelta = [];
		this.currentScale = [];
		for(var i=0;i<4;i++){
			this.renderDelta[i] = del[i]/e.totalFrames;
			this.currentScale[i] = 0.0;
		}
	},
	
	render : function(e){
		//已变化大小.
		var a = this.currentScale,b=this.renderDelta;
		
		a[0] += b[0];
		a[1] += b[1];
		a[2] += b[2];
		a[3] += b[3];
		if((this.direction & Effect.UP) != 0x0) {
			this._calSwitch(0,e);
		}
		
		if((this.direction & Effect.DOWN) != 0x0) {
			this._calSwitch(1,e);
		}
		
		if((this.direction & Effect.LEFT) != 0x0) {
			this._calSwitch(2,e);
		}
		
		if((this.direction & Effect.RIGHT) != 0x0) {
			this._calSwitch(3,e);
		}
	},
	
	endAction : function(e){
		//e.element.style.display = 'none';
	},
	
	_calSwitch : function(r,e) {
		p = e.viewData;
		e = e.element;
		var s = Fold.Swich;
		var idx = (this._how*4+r)*4;
		var cs = this.currentScale;
		var a = s[idx];
		if(a != 0) {a = ((a=p[0]+a*cs[2]) < 0)? 0 : a; e.style.left = a;}
		a = s[idx+1];
		if(a != 0) {a = ((a=p[1]+a*cs[1]) < 0)? 0 : a; e.style.top = a;}
		a = s[idx+2];
		if(a != 0) {a = ((a=p[2]+a*cs[3]) < 0)? 0 : a; e.style.width = a;}
		a = s[idx+3];
		if(a != 0) {a = ((a=p[3]+a*cs[0]) < 0)? 0 : a; e.style.height = a;}
	},
	
	recoverEffect : function(e) {
		e.recoveClientrView();
	}
});


 
/**
 * 添加组件效果.
 */
 
/**
 * 淡放浅出移动式关闭.
 */
Effect.slowShow = function(el,b,fnc) {
	if(b){
		new Effect({element:el,fps : 100, duration:0.2,onstop:function(e){if(fnc){fnc(el,b);}}}).add(new Opacity({from:0.0,to:1.0})).start();
	}else {
		new Effect({element:el,fps : 100, duration:0.2,onstop:function(e){CC.setStyle(el,'display','none');e.recoverEffect(); if(fnc){fnc(el,b);}}}).add(new Opacity({from:1.0,to:0.0})).add(new Fold({direction:Effect.DOWN|Effect.UP})).start();
	}
}

if(true) {
	
	function setEffectVisible(b){
		Effect.slowShow(this.view,b);
	}
	
	CTabItem.prototype.setVisible = 
	CItem.prototype.setVisible = 
	CWin.prototype.setVisible = 
	CMenuItem.prototype.setVisible = setEffectVisible;
	
	CFloatTip.prototype.initialize = function(opt){
	  this._super(opt); this.view.innerHTML = CTemplate[this.type]; 
    this.setViewAttr('className', 'important_tip clearfix'); 
    this.setTitle(opt.title);
    this.setMsg(opt.msg);
    CC.display(this.view,false);
    document.body.appendChild(this.view);
    //@override
    this.override('setVisible', function(it) {
      
      if (!this.proxy || !it) {
        return ;
      }
      
      var offs = Position.cumulativeOffset(this.proxy); 
      this.setLeft(offs[0] - 2); 
      this.setTop(offs[1] - this.view.offsetHeight - 2);
      //其实只是改变了以下这些.
      Effect.slowShow(this.view,true);
			var oThis = this;
      if (this.timeout) {
        setTimeout(function(){Effect.slowShow(oThis.view,false,oThis.clear.bind(oThis))}, this.timeout);
      }
    });
  }
}