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
	//�ӳ�x���ʼ.
	delay : 0.0,
	
	//����ʱ��
	duration : 1.0, //��
	
	//֡��/��
	fps : 100,
	
	//�Ƿ�ͬ����Ⱦ����
	sync : true,
	
	//
	actions : null,
	
	//����Ԫ��
	element : null,
	
	timerId : null,
	
	startOn : -1.0,
	
	state : 'idle',
	
	finishOn : -1.0,
	
	currentFrame : 0,
	
	syncLoop : true,
	
	_currActionIdx : 0,
	
	//������ص�
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
    
    //��ʼ��
		
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
		
		//ͬ��,һ֡��ͬʱ��Ⱦ���ж���.
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
		
		//����
		
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
	
	//��ǰ����ֵ���ᱻ����.
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
 * ������,����ʵ��Ч��ϸ��.
 **/
var ActionBase = CC.create();

CC.extend(ActionBase, {
	
	//�ӿں���
	
	//��������ʱ��ʼ��,�����ڱ����ֳ�.
	initAction : fGo,
	
	//ÿ֡ʱ��Ⱦ.
	render : fGo,
	
	//�������������.
	endAction : fGo,
	
	//�����ڻָ��ֳ�.
	recoverEffect : fGo,
	
	from : false,
	
	to : false,
	
	renderDelta : false,
	
	currentValue : false,
	
	fromToDelta : false,
	
	initialize : function(opts){
		CC.extend(this,opts);
		//���from,to������,����fromToDelta.
		if(this.from!==false && this.to !== false){
			this.fromToDelta  = this.to-this.from;
		}
	}
});

/**
 * Ԫ��͸��Ч��.
 * Alpha : 0.0--1.0 ��ȫ͸������ȫ��͸��.
 * ʾ�� : new Effect({element:'layer',duration:3.0}).add(new Opacity({from:0.0,to:1.0})).start();
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
 * ������Ч:how = 'expand'��'fold'.
 * ����,direction : Effect.LEFT | Effect.RIGHT | Effect.UP | Effect.DOWN.
 */
var Fold = CC.create();

Fold.Swich = [ 0,-1,0,1,  0,0,0,1,  -1,0,1,0,  0,0,1,0, 0,0,0,-1, 0,1,0,-1, 0,0,-1,0, 1,0,-1,0];

CC.extend(Fold.prototype, ActionBase);
CC.extend(Fold.prototype,{
	//Ĭ������.
	direction : Effect.UP,
	
	//��������,expand,fold
	how : 'expand',
	
	//���Ŵ�С����,����[��,��,��,��]����.
	delta : false,
	
	initAction : function(e) {
		this._how = this.how == 'expand' ? 0 : 1;
		var element = e.element;
		
		//����ԭ����Сλ����Ϣ.
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
		
		//������Ŷ�δ���ã����Ŵ�С����Ϊ��߻��.
		
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
					if(false){//TODO:�Զ����������Ŀ���.
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
		//�ѱ仯��С.
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
 * ������Ч��.
 */
 
/**
 * ����ǳ���ƶ�ʽ�ر�.
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
      //��ʵֻ�Ǹı���������Щ.
      Effect.slowShow(this.view,true);
			var oThis = this;
      if (this.timeout) {
        setTimeout(function(){Effect.slowShow(oThis.view,false,oThis.clear.bind(oThis))}, this.timeout);
      }
    });
  }
}