/**
 * By www.865171.cn
 * q  q : 215288671
 * Date : 2010.4.22
 */

/**
 * 控件基类.
 * 控件即是本包中已实现的控件对象.
 */
var CBase =  {

  //类型,保留作为Component标识.
  type: 'CBase',

  //代表该控件的DOM结点.
  view: null,

  draggable: false,

  resizable: false,

  id: 0,

  title: '',

  tleLen: 15,

  //初始化.
  initialize: function(opts) {

    if ((typeof this.id) == 'undefined') {
      this.id = CC.uniqueID();
    }

    //所有赋值给view的属性通过用viewAttr传递.
    var viewAttr = null;

    if (opts && opts.viewAttr) {
      viewAttr = opts.viewAttr;
      delete opts.viewAttr;
    }

    CC.extend(this, opts);

    if (!this.view) {
      this.view = CC. $C('DIV');
    }

    if (this.parent) {
      this.parent.appendChild(this.view);
    }

    if (viewAttr) {
      CC.extend(this.view, viewAttr);
    }

    //view附加一个控件本身的引用.
    this.view.component = this;
  }
  ,

  //设置view的宽度.
  setWidth: function(width) {
    CC.style(this.view, 'width', width + 'px');
  }
  ,

  //设置view的高度.
  setHeight: function(height) {
    CC.style(this.view, 'width', height + 'px');
  }
  ,

  //设置view 的top.
  setTop: function(top) {
    CC.style(this.view, 'top', top + 'px');
  }
  ,

  setLeft: function(left) {
    CC.style(this.view, 'left', left + 'px');
  }
  ,

  //是否禁用该控件的view,参数:true或false.
  setDisabled: function(b) {
    CC.disabled(this.view, b);
  }
  ,

  //设置view的className.
  setClass: function(c) {
    CC.setClass(this.view, c);
  }
  ,

  //设置控件view的属性.
  //如设置高为setViewAttr('top',50);
  setViewAttr: function(n, v) {
    this.view[n] = v;
  }
  ,

  //设置view中任一子层id为childId的子结子的属性.
  //属性也可以多层次.
  //如存在一id为'_ico'子结点,设置其display属性为
  //inspectAttr('_ico','style.display','block');
  inspectViewAttr: function(childId, childAttrList, attrValue) {
    var obj = CC.inspect(this.view, childId);
    //??Shoud do this??
    if (obj == null) {
      return ;
    }
    CC.inspectAttr(obj, childAttrList, attrValue);
    return obj;
  }
  ,

  //设置控件的图标,如果存在的话.
  //图标结点id必须是'_ico'且在view中是唯一的.
  setIcon: function(cssIco) {
    this.inspectViewAttr('_ico', 'className', cssIco);
  }
  ,


  setVisible: function(b) {
    CC.display(this.view, b);
  }
  ,

  //设置控件的标题,如果存在的话.
  //标题结点id必须是'_tle'且在view中是唯一的.
  setTitle: function(ss) {
    this.title = ss;
    var tle = CC.inspect(this.view, '_tle');
    CC.style(tle.parentNode, 'title', tle);
    tle.innerHTML = ss.truncate(this.tleLen);
  }
  ,

  //设置控件view的宽度高度.
  setSize: function(width, height) {
    CC.style(this.view, 'width', width + 'px');
    CC.style(this.view, 'height', height + 'px');
  }
  ,

  setPosition: function(left, top) {
    CC.style(this.view, 'left', left + 'px');
    CC.style(this.view, 'top', top + 'px');
  }
  ,

  //控件view是否可见.
  isVisible: function() {
    return this.view.style.display != "none";
  }
  ,

  //为view添加DOM事件.
  //useCapture:否是支持冒泡.
  observe: function(name, observer, useCapture) {
    Event.observe(this.view, name, observer, useCapture);
  }
  ,


  stopObserving: function(name, observer, useCapture) {
    Event.stopObserving(this.view, observer, useCapture);
  }
  ,

  //设置控件是否可拖动.
  //moveObj:发生拖动时要移动的结点.
  setDragable: function(b, moveObj, fnOnMov, fnOnDrag, fnOnDrog) {
    this.draggable = b;
    (moveObj) ? Position.setDragable(this.view, moveObj, b, fnOnMov, fnOnDrag, fnOnDrog): Position.setDragable(this.view, this.view, b, fnOnMov, fnOnDrag, fnOnDrog);
  }
  ,

  //之前版本appendView已改为appendChild
  //为view添加一个子DOM结点.
  appendChild: function(v) {
    this.view.appendChild(v);
  }
  ,

  removeChild: function(v) {
    this.view.removeChild(v);
  }
};

/**
 * 容器类控件.
 * 派生于该容器都具有appendChild,add,get,removeChild,remove,$,show,indexOf,contains方法.
 */
var CContainerBase = {};

//派生于基类.
CC.extendx(CContainerBase, CBase);

CC.extend(CContainerBase,  {

  children: null,

  container: null,

  initialize: function(opt) {
    //调用父类初始化.
    this._super(opt); if (!this.container) {
      this.container = this.view;
    }
    this.children = [];
  }
  ,

  //向容器中添加一个结点.
  appendChild: function(child) {
    this.container.appendChild(child);
  }
  ,

  //向容器中添加一个控件,注意是控件而不是DOM结点.
  //控件即是本包中已实现的控件,具有基本的view属性.
  add: function(a) {
    if (!this.contains(a)) {
      this.children.push(a); this.container.appendChild(a.view);
    }
    a.parentContainer = this; return a;
  }
  ,

  //获得容器中的一个控件.
  //参数a可为控件id,或控件自身.
  get: function(a) {
    a = this. $(a); if (this.children.indexOf(a) >= 0) {
      return a;
    }
    return null;
  }
  ,


  removeChild: function(child) {
    this.container.removeChild(child);
  }
  ,

  //参数a可为控件实例或控件ID.
  remove: function(a) {
    if (!a || !this.contains(a)) {
      return ;
    }

    a.parentContainer = null;

    this.children.remove(a); this.container.removeChild(a.view); return a;
  }
  ,

  //根据控件ID或控件自身安全返回容器中该控件对象.
  $: function(id) {
    if (id.type) {
      return id;
    }
    var chs = this.children;

    if (CC.isNumber(id)) {
      return this.children[id];
    }

    for (var i = 0, len = chs.length; i < len; i++) {
      if (chs[i].id == id) {
        return chs[i];
      }
    }
    return null;
  }
  ,

  //设置容器中指定控件的显示属性.
  //b为true或false.
  //参数a可为控件实例或控件ID.
  show: function(a, b) {
    a = this. $(a); a.setVisible(b); return a;
  }
  ,

  //返回窗口中控件的索引.
  //参数a可为控件实例或控件ID.
  indexOf: function(a) {
    if (!a.type) {
      a = this. $(a);
    }
    return this.children.indexOf(a);
  }
  ,

  size: function() {
    return this.children.length
  }
  ,
  //容器是否包含给出控件.
  //参数a可为控件实例或控件ID.
  contains: function(a) {
    if (!a.type) {
      a = this. $(a);
    }
    return this.children.indexOf(a) !=  - 1;
  }
});

//--基本Item控件
var CItem = CC.create();

//CItem.prototype派生于CBase类.
CC.extendx(CItem.prototype, CBase);

CC.extend(CItem.prototype,  {

  type: 'CItem',

  initialize: function(opt) {
    //传给父类初始化.
    this._super(CC.extendIf(opt,  {
      view: opt.view || CC. $C( {
        tagName: 'LI', innerHTML: CTemplate[this.type]
      }
      )
    }
    )); this.setTitle(this.title); this.setViewAttr('className', ''); this.setIcon(opt.icon || ''); Event.bindPassByStyle(this.view, 'on navOn', false);
  }
}

);


//--Tab Item 控件

var CTabItem = CC.create();

CC.extendx(CTabItem.prototype, CBase);

CC.extend(CTabItem.prototype,  {

  type: 'CTabItem',

  url: null,

  load: false,
  //每个TabItem包含一个面板作为其显示的主体.
  panel: null,

  onclose: null,

  //该TabItem是否可关闭,不能直接设置该值,应调用setCloseable().
  closeable: false,

  initialize: function(opt) {
    this._super(CC.extendIf(opt,  {
      view: CC. $C('TABLE')
    }
    )); var dv = CC. $C('DIV'); dv.innerHTML = CTemplate[this.type]; var tb = dv.firstChild; this.view.appendChild(tb.removeChild(tb.firstChild)); this.setTitle(this.title); this.setCloseable(this.closeable); if (this.url) {
      if (!this.load) {
        this.request();
      }
    }
  }
  ,

  //设置关闭后的回调.
  //fnc参数为一个回调函数.
  setOnclose: function(fnc) {
    var fon = function(ev) {
      Event.stop(window.event); fnc();
    }; this.inspectViewAttr('_cls', 'onclick', fon);
  }
  ,

  //该TabItem是否可关闭.
  setCloseable: function(b) {
    this.closeable = b; var obj = CC.inspect(this.view, '_cls'); CC.display(obj, b);
  }
  ,

  //设置TabItem显示的面板,为一个DOM结点.
  setPanel: function(p) {
    this.panel = p;
  }
  ,

  request: function() {
    if (this.url && this.panel) {
      var ajax = new Ajax( {
        url: this.url, displayPanel: this.panel, onSuccess: this.onContentLoaded, caller: this
      }
      ); ajax.open(); ajax.send(null);
    }
  }
  ,

  onContentLoaded: function(ajax) {
    ajax.invokeHtml(); this.load = true;
  }

});

//--Tab 控件,一个Tab控件由多个TabItem构成.
//监听select事件可用onselect(CTab)=...
var CTab = CC.create();

//派生于容器类.
CC.extendx(CTab.prototype, CContainerBase);

CC.extend(CTab.prototype,  {

  type: 'CTab',

  //默认TabItem长度.
  defItemLen: 130,

  //该面板存放当前显示的TabItem中的面板,TabItem中的面板将放入该面板内,即Tab显示主体.
  ctxPanel: null,

  selected: null,

  initialize: function(opt) {
    this._super(opt); if (!this.ctxPanel) {
      this.ctxPanel = CC. $C('DIV');
    }

    //重写容器类的add方法.
    this.override('add', function(it) {
      //selected event.
      it.view.onclick = this.select.bind(this, it);
      //close event.
      it.view.ondblclick = this.show.bind(this, it, false); it.setOnclose(this.show.bind(this, it, false)); this._ajust(); it.parent = this;
    }
    );
  }
  ,

  //设置Tab控件显示主板面板.
  setCtxPanel: function(v) {
    this.ctxPanel = v;
  }
  ,

  _ajust: function() {
    var sty = this.view.style; var w = this.view.clientWidth; var ch = this.children; var tcnt = this.children.length; var vcnt = 0; for (var i = 0; i < tcnt; i++) {
      if (ch[i].isVisible()) {
        vcnt++
      }
    }

    var perw = Math.floor(w / vcnt);

    if (perw > this.defItemLen) {
      perw = this.defItemLen;
    }

    for (var i = 0; i < tcnt; i++) {
      ch[i].setWidth(perw);
    }
  }
  ,

  //是否显示指定的TabItem,
  //参数a可为TabItem实例也可为TabItem的id,b为true或false.
  show: function(a, b) {
    //only one,can't set it.
    if (!b && this.getDisc() == 1) {
      return ;
    }
    a = this. $(a);

    //Cann't change this attribute.
    if (!a.closeable && !b) {
      return false;
    }

    var isv = a.isVisible();

    if (isv != b) {
      this._ajust();
    }

    a.setVisible(b);

    if (!b && this.selected == a) {
      var idx = this.indexOf(a); var tmp = idx - 1; var chs = this.children;
      while (tmp >= 0 && !chs[tmp].isVisible()) {
        tmp--;
      }
      if (tmp >= 0) {
        this.select(chs[tmp]); return ;
      }

      tmp = chs.length; idx += 1;
      while (idx < tmp && !chs[idx].isVisible()) {
        idx++;
      }
      if (idx < tmp) {
        this.select(chs[idx]);
      }
    }
  }
  ,

  //选择某个TabItem,
  //参数a可为TabItem实例也可为id.
  select: function(a) {

    if (a == '' || a == null) {
      var sel = this.selected; if (sel) {
        sel.setViewAttr('className', ''); this.ctxPanel.removeChild(sel.panel);
      }
      this.selected = ''; return ;
    }

    if (a == this.selected) {
      return ;
    }

    a = this.get(a); if (a == null) {
      return ;
    }

    this.show(a, true);

    var sel = this.selected; if (sel) {
      sel.setViewAttr('className', ''); this.ctxPanel.removeChild(sel.panel);
    }

    this.selected = a; a.setViewAttr('className', 'on'); this.ctxPanel.appendChild(a.panel); if (this.onselect) {
      this.onselect(a);
    }
  }
  ,

  //返回显示的TabItem个数.
  getDisc: function() {
    var cnt = 0; var chs = this.children; for (var i = 0, len = chs.length; i < len; i++) {
      if (chs[i].isVisible()) {
        cnt++;
      }
    }
    return cnt;
  }
});


//--CItemList,列表项控件.
var CItemList = CC.create();

//该控件为一个容器.
CC.extendx(CItemList.prototype, CContainerBase);

CC.extend(CItemList.prototype,  {

  //invoke as onclick(itemObj)
  onselect: null,

  selected: null,

  initialize: function(opt) {
    this._super(CC.extendIf(opt,  {
      view: CC. $C('UL')
    }
    )); this.setViewAttr('className', 'gFdBdy');
    //@override
    this.override('add', function(it) {
      it.view.onclick = this.select.bind(this, it); it.parent = this;
    }
    );
  }
  ,

  //选择某个控件,
  //参数a可为控件实例也可为id.
  select: function(a) {
    if (a == '' || a == null) {
      var sel = this.selected; if (sel) {
        sel.setViewAttr('className', '');
      }
      this.selected = ''; return ;
    }
    a = this. $(a); if (a == null) {
      return ;
    }

    if (a == this.selected) {
      return ;
    }

    this.show(a, true);

    var sel = this.selected; if (sel) {
      sel.setViewAttr('className', '');
    }

    this.selected = a; a.setViewAttr('className', 'on'); if (this.onselect) {
      this.onselect(a);
    }
  }
});

//--CFolder 组控件
//CFolder控件其实由一个头部和一个CItemList控件组成.
var CFolder = CC.create();

CC.extendx(CFolder.prototype, CBase);

CC.extend(CFolder.prototype,  {

  type: 'CFolder',

  //存放一个CItemList.
  itemList: null,

  isFold: false,

  initialize: function(opt) {
    this._super(opt); this.view.innerHTML = CTemplate[this.type]; this.itemList = new CItemList(); this.view.appendChild(this.itemList.view); this.setViewAttr('className', 'gFd'); this.inspectViewAttr('_btnFN', 'onclick', this._btnFNOnclick.bind(this)); this.setTitle(this.title);
  }
  ,

  _btnFNOnclick: function() {
    var v = this.itemList.isVisible(); this.fold(v);
  }
  ,

  //添加一个CItem项.
  add: function(oItem) {
    this.itemList.add(oItem);
  }
  ,

  //折叠或展开.
  //b:true or false.
  fold: function(b) {
    this.inspectViewAttr('_btnFN', 'className', b ? 'opnFd' : 'clsFd'); this.itemList.setVisible(!b);
  }
}

);


//--CBarItem : Toolbar Item,工具栏项.
var CBarItem = CC.create();

CC.extendx(CBarItem.prototype, CBase);

CC.extend(CBarItem.prototype,  {
  type: 'CBarItem', initialize: function(opt) {
    this._super(opt); this.view.innerHTML = CTemplate[this.type]; this.setViewAttr('className', 'tlBtn2'); this.setTitle(this.title); this.setIcon(this.icon); Event.bindPassByStyle(this.view, 'on', false);
  }
}

);

//工具栏控件.
var CToolbar = CC.create();
CC.extendx(CToolbar.prototype, CContainerBase);

CC.extend(CToolbar.prototype,  {

  onselect: null,

  initialize: function(opt) {
    this._super(opt); this.setViewAttr('className', 'gTlBar');
    //@override
    this.override('add', function(it) {
      if (this.onselect) {
        it.view.onclick = this.onselect.bind(this, it);
      }

      Event.bindClickStyle(it.view, 'click', false);
    }
    );
  }
});

/**
 * window控件.
 */
var CWin = CC.create();

CC.extendx(CWin.prototype, CContainerBase);

CC.extend(CWin.prototype,  {

  type: 'CWin',

  body: null,

  bottom: null,

  initialize: function(opt) {
    this._super(opt); this.view.innerHTML = CTemplate[this.type]; this.setViewAttr('className', 'sysWin'); this.container = this.body = CC.inspect(this.view, '_bdy'); this.bottom = CC.inspect(this.view, '_bot'); this.setTitle(this.title || 'window ' + this.id); this.setIcon(this.icon || 'icoSw'); this.setDragable(true); this.setLeft(this.left || 283); this.setTop(this.top || 185); this.inspectViewAttr('_cls', 'onclick', this.setVisible.bind(this, false));
  }
  , centerWindow: function() {
    this.setLeft(283); this.setTop(185);
  }
  ,
  //@override
  setDragable: function(b) {
    if (this.draggable != b) {
      Position.setDragable(CC.inspect(this.view, '_tbar'), this.view, b); this.draggable = b;
    }
  }
});



/**
 * 浮动提示框,显示后即被回收.
 */
var CFloatTip = CC.create();

CC.extendx(CFloatTip.prototype, CBase);

CC.extend(CFloatTip.prototype,  {

  type: 'CFloatTip',

  timeout: 2500,

  //显示位置.相对proxy.
  proxy: null,

  initialize: function(opt) {
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
      CC.display(this.view,true);
			var oThis = this;
      if (this.timeout) {
        setTimeout(function(){CC.display(oThis.view,false);oThis.clear();}, this.timeout);
      }
    }
    );
  }
  ,

  clear: function() {
    document.body.removeChild(this.view); 
    this.proxy = null;
  }
  ,

  setMsg: function(msg) {
    this.inspectViewAttr('_msg', 'innerHTML', msg);
  }
});



var CUtil =  {

  _sysWin: function() {
    var w = window._sysWin;
    if (!w) {
      w = window._sysWin = new CWin({}
      );
      CC.style(_sysWin.body, 'textAlign', 'center');
      document.body.appendChild(_sysWin.view);
    }
    w.onClose = null;
    return w;
  }
  ,

  alert: function(msg, title) {
    title = title || '提示';
    var s = CUtil._sysWin();
    s.setTitle(title);
    s.bottom.innerHTML = CTemplate['CUtil.alert'];
    s.body.innerHTML = msg;
    s.centerWindow();
    s.setVisible(true);
  }
  ,

  //inputBox(msg,title,onClick(value,state));

  inputBox: function(msg, title, onClick) {
    var w = CUtil._sysWin();
    w.setTitle(title || '提示');
    w.body.innerHTML = CTemplate['CUtil.inputBox'].replace('@{msg}', msg);
    w.bottom.innerHTML = CTemplate['CUtil.inputBox.body'];
    var input = CC.inspect(w.body, '_input');
    var callback = function(state) {
      var v = input.value;
      onClick.call(this, v, state);
    }
    w.setVisible(true);
    input.focus();
    CC.inspectAttr(CC.inspect(w.bottom, '_ok'), 'onclick', function() {
      if (callback(1)) {
        w.setVisible(false);
      }
    });
    CC.inspectAttr(CC.inspect(w.bottom, '_cancel'), 'onclick', function() {
      w.setVisible(false);
    }
    );
  }
  ,

  tip: function(msg, title, proxy, timeout, getFocus) {
    proxy = CC. $(proxy);
    var tip = new CFloatTip( {proxy: proxy, timeout: timeout, title: title, msg: msg});
    tip.setVisible(true);

    if (getFocus) {
      proxy.focus();
    }
    return tip;
  }
};

//--MenuItem
var CMenuItem = CC.create();

//CMenuItem.prototype派生于CBase类.
CC.extendx(CMenuItem.prototype, CBase);

CC.extend(CMenuItem.prototype,  {

  type: 'CMenuItem',

  //子菜单
  subMenu: null,

  initialize: function(opt) {
    //最上层为A标签
    var dv = Cache.get('div'); dv.innerHTML = CTemplate[this.type]; this._super(CC.extendIf(opt,  {
      view: opt.view || dv.firstChild
    }
    ));

    // IE7中innerHTML=''时会删除子结点所有数据,因此改为removeChild
    dv.removeChild(dv.firstChild); Cache.put('div', dv); this.setTitle(this.title); this.setIcon(opt.icon || '');

    Event.bindPassByStyle(this.view, 'itemOn', false, (function() {
      //OnMouseOver : 隐藏上一菜单项的子菜单
      this.parentContainer._hidePre(); if (this.subMenu) {
        this.subMenu.show(true);
      }
      this.parentContainer._onItem = this;
    }
    ), (function() {
      //OnMouseOut :
      if (this.subMenu) {
        if (this.subMenu.isVisible()) {
          return true;
        }
      }
    }
    ), this);

    var oThis = this; var onClick = (function() {
      if (oThis.subMenu) {
        oThis.subMenu.show(true); return ;
      }
      //无子菜单
      oThis.parentContainer.hideAll(); Event.stopObserving(this, 'click', onClick);
    }
    );

    Event.observe(this.view, 'click', onClick, false);
  }
});


//--菜单控件
var CMenu = CC.create();

CC.extendx(CMenu.prototype, CContainerBase);

CC.extend(CMenu.prototype,  {

  type: 'CMenu',

  //父菜单项
  parentItem: null,

  //上一掠过菜单项
  _onItem: null,

  initialize: function(opt) {
    this._super(CC.extendIf(opt,  {
      view: CC. $C( {
        tagName: 'DIV', className: 'gMenu'
      }
      )
    }
    )); this.view.innerHTML = CTemplate[this.type]; this.setWidth(120);

    //撤消菜单内的onclick事件上传
    this.setViewAttr('onclick', (function() {
      Event.stop(window.event);
    }
    ));

    //@override
    //重写容器类的add方法.
    this.override('add', function(it) {
      this.view.firstChild.appendChild(it.view);
    }
    );

    //默认为不显示
    this.setVisible(false);
    //默认将菜单添加到DOM中
    document.body.appendChild(this.view);
  }
  ,

  //把子菜单menu添加到tar项上,tar可为一个index,或一个CMenuItem对象,还可为CMenuItem的id
  attach: function(menu, tar) {
    tar = this. $(tar); CC.addClass(tar.view, 'sub'); menu.parentItem = tar; tar.subMenu = menu;
  }
  ,

  //撤消菜单项上的子菜单
  detach: function(tar) {
    tar = this. $(tar); CC.delClass(tar.view, 'sub'); tar.subMenu.parentItem = null; tar.subMenu = null;
  }
  ,

  //显示/隐藏菜单,当点击菜单外时菜单并不用消失.
  show: function(b) {
    if (b) {
      this._autoPositioned();
    }
    if (!b) {
      CC.each(this.children, (function(i, e) {
        if (this.subMenu) {
          if (this.subMenu.isVisible()) {
            this.subMenu.show(false);
          }
        }
      }
      ));
    }
    this.setVisible(b);
  }
  ,

  //显示界面菜单,与show不同,该方法在指定处显示菜单，并且当点击菜单范围外时自动消失.
  //showMenu(obj,'bottom')或showMenu(120,230)
  showMenu: function(a, b) {
    var oThis = this; var onClick = (function() {
      if (oThis.isVisible()) {
        oThis.hideAll();
      }
      Event.stopObserving(document, 'click', onClick);
    }
    ); Event.observe(document, 'click', onClick); var ev = window.event; if (ev) {
      Event.stop(window.event);
    }
    if (!CC.isNumber(a)) {
      var off = Position.cumulativeOffset(a); switch (b) {
        case 'right':
        off[0] += a.offsetWidth; break; default:
          off[1] += a.offsetHeight; break;
      }
      this.setPosition(off[0], off[1]);
    } else {
      this.setPosition(a, b);
    }
    this.show(true);
  }
  ,

  //隐藏与该菜单相联的所有菜单(包括子菜单和父菜单).
  hideAll: function() {
    this.setVisible(false);

    if (this._onItem) {
      CC.delClass(this._onItem.view, 'itemOn');
    }

    CC.each(this.children, (function(i, e) {
      if (this.subMenu) {
        if (this.subMenu.isVisible()) {
          this.subMenu.hideAll();
        }
      }
    }
    )); if (this.parentItem) {
      if (this.parentItem.parentContainer.isVisible()) {
        this.parentItem.parentContainer.hideAll();
      }
    }
  }
  ,

  //在适当位置显示菜单.
  _autoPositioned: function() {
    if (this.parentItem) {
      var off = Position.cumulativeOffset(this.parentItem.view); CC.style(this.view, 'top', off[1]);
      //回缩2px.
      CC.style(this.view, 'left', off[0] + this.parentItem.view.offsetWidth - 2);
    }
  }
  ,

  _hidePre: function() {
    if (this._onItem) {
      var pre = this._onItem; CC.delClass(pre.view, 'itemOn'); if (pre.subMenu) {
        pre.subMenu.show(false);
      }
    }
  }
});


var CTreeItem = CC.create();

CC.extend(CTreeItem.prototype,  {

  type: 'CTreeItem',

  //根结点指针
  root: false,

  isFolder: null,

  parentItem: null,

  initialize: function(opt) {

    CC.extendx(this, (opt.isFolder) ? CContainerBase : CBase);

    this._super(CC.extendIf(opt,  {
      view: CC. $C( {
        tagName: 'LI', className: 'x-tree-node'
      }
      )
    }
    ));

    if (this.root !== false) {
      this._createView();
    }
    if (this.isFolder) {
      //重写容器类的add方法,插入后再更新视图.
      this.add = (function(it) {
        it.parentItem = this; it.root = this.root == null ? this : this.root;

        if (!this.contains(it)) {
          this.children.push(it);
        }

        it.parentContainer = this;
        //添加后才能确定界面.
        it._createView();

        this.container.appendChild(it.view);

      }
      );
    }
  }
  ,

  _createView: function() {
    var space = this.getLevel() - 1; var ss = CTemplate['CTreeItem.part0']; var arr = [];
    //
    var p = this.parentItem; if (p) {
      while (p) {
        if (!p._isLastOne()) {
          arr[arr.length] = CTemplate['CTreeItem.part1'];
        } else {
          arr[arr.length] = CTemplate['CTreeItem.part2'];
        }
        p = p.parentItem;
      }
      arr.reverse();
    }
    arr[arr.length] = CTemplate['CTreeItem.part3']; this.view.innerHTML = ss + arr.join(''); this.setTitle(this.title);
    //
    var head = CC.inspect(this.view, '_head');

    Event.bindPassByStyle(head, 'x-tree-node-over', false);

    head.ondblclick = this.expand.bind(this); 
    head.onclick = this._onselect.bind(this);

    var fold = CC.inspect(this.view, '_fold'); var last = this._isLastOne(); if (this.isFolder) {
      CC.addClass(head, 'x-tree-node-collapsed'); fold.onclick = this.expand.bindAsNoCaptureListener(this); if (last) {
        var ptm = this.parentItem; if (ptm != null && ptm.size() >= 2) {
          var pre = ptm.get(ptm.indexOf(this) - 1); var prfld = CC.inspect(pre.view, '_fold'); CC.setClass(prfld, 'x-tree-ec-icon'); if (pre.isFolder) {
            pre.expand(); 
            pre.expand();
          } else {
            CC.switchClass(prfld, 'x-tree-elbow-plus', 'x-tree-elbow');
          }
        }
        CC.addClass(fold, 'x-tree-elbow-end-plus');
      } else {
        CC.addClass(fold, 'x-tree-elbow-plus');
      }
    } else {
      CC.addClass(head, 'x-tree-node-leaf'); if (last) {
        var ptm = this.parentItem; if (ptm != null && ptm.size() >= 2) {
          var pre = ptm.get(ptm.indexOf(this) - 1); var prfld = CC.inspect(pre.view, '_fold'); CC.setClass(prfld, 'x-tree-ec-icon'); if (pre.isFolder) {
            pre.expand(); 
            pre.expand();
          } else {
            CC.addClass(prfld, 'x-tree-elbow');
          }
        }
        CC.addClass(fold, 'x-tree-elbow-end');
      } else {
        CC.addClass(fold, 'x-tree-elbow');
      }
    }

    //folder
    if (this.isFolder) {
      //设置正确的容器.
      this.container = CC.inspect(this.view, '_ctx');
    }
  }
  ,

  _isLastOne: function() {
    var ppt = this.parentContainer; return (ppt == null) || (ppt.indexOf(this) == (ppt.size() - 1));
  }
  ,

  _onselect: function() {
    var root = this.root; 
    if (!root) {
      root = this;
    }

    if (root.onselect) {
      root.onselect(this);
    }

    var pre = root._preSelect; 
    if (pre) {
      CC.delClass(CC.inspect(pre.view, '_head'), 'x-tree-selected');
    }

    CC.addClass(CC.inspect(this.view, '_head'), 'x-tree-selected'); 
    root._preSelect = this;
  }
  ,

  expand: function(b) {

    if (!this.isFolder) {
      return ;
    }

    var ctx = CC.inspect(this.view, '_ctx');

    if (arguments[0] !== true || arguments[0] !== false) {
      b = !(ctx.style.display != 'none');
    }

    var fold = CC.inspect(this.view, '_fold'); 
    var head = CC.inspect(this.view, '_head'); 
    var last = this._isLastOne(); 
    if (b) {
      (last) ? CC.switchClass(fold, 'x-tree-elbow-end-plus', 'x-tree-elbow-end-minus')
      : CC.switchClass(fold, 'x-tree-elbow-plus', 'x-tree-elbow-minus'); 
      CC.switchClass(head, 'x-tree-node-collapsed', 'x-tree-node-expanded');
    } else {
      (last) ? CC.switchClass(fold, 'x-tree-elbow-end-minus', 'x-tree-elbow-end-plus')
      : CC.switchClass(fold, 'x-tree-elbow-minus', 'x-tree-elbow-plus'); 
      CC.switchClass(head, 'x-tree-node-expanded', 'x-tree-node-collapsed');
    }
		CC.display(ctx,b);
  }
  ,

  //从第1层起
  getLevel: function() {
    var l = 1; var p = this.parentItem;
    while (p) {
      l++; p = p.parentItem;
    }
    return l;
  }
});

var CTree = CC.create();
CC.extendx(CTree.prototype, CBase);
CC.extend(CTree.prototype,  {
  type: 'CTree', root: null, initialize: function(opt) {
    this._super( {
      view: CC. $C( {
        tagName: 'DIV', className: 'x-tree', innerHTML: CTemplate[this.type]
      }
      )
    }
    ); opt.isFolder = true;
    //Root Flag,子结点初始为false.
    opt.root = null; this.root = new CTreeItem(opt); CC.inspect(this.view, '_ctx').appendChild(this.root.view);
  }
}

);

var CTemplate = {};
CTemplate['CItem'] = '<b id="_ico" class=""></b><a id="_tle" class="gfNm"></a>';
CTemplate['CTabItem'] = '<table><tbody><tr><td class="tLe" /><td class="bdy"><nobr id="_tle"></nobr></td><td class="btn"><a class="TabCls" id="_cls" title="关闭" href="javascript:fGo()"></a></td><td class="tRi" /></tr></tbody></table>';
CTemplate['CFolder'] = '<h3 class="gfTit"><a id="_btnFN" onclick="" class="clsFd" href="javascript:fGo()"/><a id="_tle" class="gfName" onclick="" href="javascript:fGo()"></a></h3>';
CTemplate['CBarItem'] = '<div class="bLe"></div><b id="_ico" class=""></b><span id="_tle" class="bTxt"></span><div class="bRi"></div>';
CTemplate['CWin'] = '<h2 id="_tbar"><div class="fLe"></div><b id="_ico" class="icoSw"></b><span id="_tle"></span><div class="fRi"></div><a id="_cls" class="btnCls" title="关闭" href="javascript:fGo();"></a></h2><div  class="bdy"><div class="fLe"></div><div id="_bdy" class="bdyCtn"></div><div class="fRi"></div></div><div id="_bot" class="bot"><div class="fLe"></div><div class="fRi"></div></div>';
CTemplate['CFloatTip'] = '<div class="bdy"><div id="_tle" class="important_txt"></div><div id="_msg" class="important_subtxt"></div></div><div class="btm_cap"></div>';
CTemplate['CUtil.alert'] = '<div class="fLe"></div><input value="确 定" class="btnFnt" onclick="_sysWin.setVisible(false);" onmouseover="this.className=\'btnFnt btnFntOn\'" onmouseout="this.className=\'btnFnt\'" type="button"><div class="fRi"></div>';
CTemplate['CUtil.inputBox'] = '<table class="swTb"><tbody><tr><th valign="top"><b id="icoIfo" class="icoIfo"/></th><td><span class="swTit" id="contextSpan"><br/>@{msg}<input type="text" id="_input" class="gIpt" style=""/><p class="swEroMsg"><span id="_err"/></p></span></td></tr></tbody></table><div class="clear"/>';
CTemplate['CUtil.inputBox.body'] = '<div class="fLe"></div><input value="取 消" class="btnFnt" id="_cancel" onmouseover="this.className=\'btnFnt btnFntOn\'" onmouseout="this.className=\'btnFnt\'" type="button"><input value="确 定" class="btnFnt" id="_ok" onmouseover="this.className=\'btnFnt btnFntOn\'" onmouseout="this.className=\'btnFnt\'" type="button"><div class="fRi"></div>';
CTemplate['CMenuItem'] = '<a href="javascript:fGo()"><span><b id="_ico" class="icos"></b><span id="_tle"></span></span></a>';
CTemplate['CMenu'] = '<div class="bdy gMenuOpt" id="_bdy"></div>';
CTemplate['CTreeItem.part0'] = '<div id="_head" class="x-tree-node-el x-unselectable" unselectable="on"><span class="x-tree-node-indent">';
CTemplate['CTreeItem.part1'] = '<img src="s.gif" class="x-tree-icon x-tree-elbow-line"></img>';
CTemplate['CTreeItem.part2'] = '<img src="s.gif" class="x-tree-icon"></img>';
CTemplate['CTreeItem.part3'] = '<img id="_fold" src="s.gif" class="x-tree-ec-icon"></img><img id="_ico" src="s.gif" class="x-tree-node-icon"></img></span><a href="javascript:fGo()" class="x-tree-node-anchor" hidefocus="on"><span unselectable="on" id="_tle"></span></a></div><ul id="_ctx" class="x-tree-node-ct  x-tree-lines" style="position: static; display:none; left: auto; top: auto; z-index: auto;"></ul>';
CTemplate['CTree'] = '<div class="x-panel-bwrap"><div class="x-panel-body x-panel-body-noheader" style="over-flow:auto"><ul id="_ctx" class="x-tree-node-ct  x-tree-lines"></ul></div></div>';
