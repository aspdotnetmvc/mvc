/**
 * By www.865171.cn
 * q  q : 215288671
 * Date : 2010.4.22
 */

/**
 * �ؼ�����.
 * �ؼ����Ǳ�������ʵ�ֵĿؼ�����.
 */
var CBase =  {

  //����,������ΪComponent��ʶ.
  type: 'CBase',

  //����ÿؼ���DOM���.
  view: null,

  draggable: false,

  resizable: false,

  id: 0,

  title: '',

  tleLen: 15,

  //��ʼ��.
  initialize: function(opts) {

    if ((typeof this.id) == 'undefined') {
      this.id = CC.uniqueID();
    }

    //���и�ֵ��view������ͨ����viewAttr����.
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

    //view����һ���ؼ����������.
    this.view.component = this;
  }
  ,

  //����view�Ŀ��.
  setWidth: function(width) {
    CC.style(this.view, 'width', width + 'px');
  }
  ,

  //����view�ĸ߶�.
  setHeight: function(height) {
    CC.style(this.view, 'width', height + 'px');
  }
  ,

  //����view ��top.
  setTop: function(top) {
    CC.style(this.view, 'top', top + 'px');
  }
  ,

  setLeft: function(left) {
    CC.style(this.view, 'left', left + 'px');
  }
  ,

  //�Ƿ���øÿؼ���view,����:true��false.
  setDisabled: function(b) {
    CC.disabled(this.view, b);
  }
  ,

  //����view��className.
  setClass: function(c) {
    CC.setClass(this.view, c);
  }
  ,

  //���ÿؼ�view������.
  //�����ø�ΪsetViewAttr('top',50);
  setViewAttr: function(n, v) {
    this.view[n] = v;
  }
  ,

  //����view����һ�Ӳ�idΪchildId���ӽ��ӵ�����.
  //����Ҳ���Զ���.
  //�����һidΪ'_ico'�ӽ��,������display����Ϊ
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

  //���ÿؼ���ͼ��,������ڵĻ�.
  //ͼ����id������'_ico'����view����Ψһ��.
  setIcon: function(cssIco) {
    this.inspectViewAttr('_ico', 'className', cssIco);
  }
  ,


  setVisible: function(b) {
    CC.display(this.view, b);
  }
  ,

  //���ÿؼ��ı���,������ڵĻ�.
  //������id������'_tle'����view����Ψһ��.
  setTitle: function(ss) {
    this.title = ss;
    var tle = CC.inspect(this.view, '_tle');
    CC.style(tle.parentNode, 'title', tle);
    tle.innerHTML = ss.truncate(this.tleLen);
  }
  ,

  //���ÿؼ�view�Ŀ�ȸ߶�.
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

  //�ؼ�view�Ƿ�ɼ�.
  isVisible: function() {
    return this.view.style.display != "none";
  }
  ,

  //Ϊview���DOM�¼�.
  //useCapture:����֧��ð��.
  observe: function(name, observer, useCapture) {
    Event.observe(this.view, name, observer, useCapture);
  }
  ,


  stopObserving: function(name, observer, useCapture) {
    Event.stopObserving(this.view, observer, useCapture);
  }
  ,

  //���ÿؼ��Ƿ���϶�.
  //moveObj:�����϶�ʱҪ�ƶ��Ľ��.
  setDragable: function(b, moveObj, fnOnMov, fnOnDrag, fnOnDrog) {
    this.draggable = b;
    (moveObj) ? Position.setDragable(this.view, moveObj, b, fnOnMov, fnOnDrag, fnOnDrog): Position.setDragable(this.view, this.view, b, fnOnMov, fnOnDrag, fnOnDrog);
  }
  ,

  //֮ǰ�汾appendView�Ѹ�ΪappendChild
  //Ϊview���һ����DOM���.
  appendChild: function(v) {
    this.view.appendChild(v);
  }
  ,

  removeChild: function(v) {
    this.view.removeChild(v);
  }
};

/**
 * ������ؼ�.
 * �����ڸ�����������appendChild,add,get,removeChild,remove,$,show,indexOf,contains����.
 */
var CContainerBase = {};

//�����ڻ���.
CC.extendx(CContainerBase, CBase);

CC.extend(CContainerBase,  {

  children: null,

  container: null,

  initialize: function(opt) {
    //���ø����ʼ��.
    this._super(opt); if (!this.container) {
      this.container = this.view;
    }
    this.children = [];
  }
  ,

  //�����������һ�����.
  appendChild: function(child) {
    this.container.appendChild(child);
  }
  ,

  //�����������һ���ؼ�,ע���ǿؼ�������DOM���.
  //�ؼ����Ǳ�������ʵ�ֵĿؼ�,���л�����view����.
  add: function(a) {
    if (!this.contains(a)) {
      this.children.push(a); this.container.appendChild(a.view);
    }
    a.parentContainer = this; return a;
  }
  ,

  //��������е�һ���ؼ�.
  //����a��Ϊ�ؼ�id,��ؼ�����.
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

  //����a��Ϊ�ؼ�ʵ����ؼ�ID.
  remove: function(a) {
    if (!a || !this.contains(a)) {
      return ;
    }

    a.parentContainer = null;

    this.children.remove(a); this.container.removeChild(a.view); return a;
  }
  ,

  //���ݿؼ�ID��ؼ�����ȫ���������иÿؼ�����.
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

  //����������ָ���ؼ�����ʾ����.
  //bΪtrue��false.
  //����a��Ϊ�ؼ�ʵ����ؼ�ID.
  show: function(a, b) {
    a = this. $(a); a.setVisible(b); return a;
  }
  ,

  //���ش����пؼ�������.
  //����a��Ϊ�ؼ�ʵ����ؼ�ID.
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
  //�����Ƿ���������ؼ�.
  //����a��Ϊ�ؼ�ʵ����ؼ�ID.
  contains: function(a) {
    if (!a.type) {
      a = this. $(a);
    }
    return this.children.indexOf(a) !=  - 1;
  }
});

//--����Item�ؼ�
var CItem = CC.create();

//CItem.prototype������CBase��.
CC.extendx(CItem.prototype, CBase);

CC.extend(CItem.prototype,  {

  type: 'CItem',

  initialize: function(opt) {
    //���������ʼ��.
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


//--Tab Item �ؼ�

var CTabItem = CC.create();

CC.extendx(CTabItem.prototype, CBase);

CC.extend(CTabItem.prototype,  {

  type: 'CTabItem',

  url: null,

  load: false,
  //ÿ��TabItem����һ�������Ϊ����ʾ������.
  panel: null,

  onclose: null,

  //��TabItem�Ƿ�ɹر�,����ֱ�����ø�ֵ,Ӧ����setCloseable().
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

  //���ùرպ�Ļص�.
  //fnc����Ϊһ���ص�����.
  setOnclose: function(fnc) {
    var fon = function(ev) {
      Event.stop(window.event); fnc();
    }; this.inspectViewAttr('_cls', 'onclick', fon);
  }
  ,

  //��TabItem�Ƿ�ɹر�.
  setCloseable: function(b) {
    this.closeable = b; var obj = CC.inspect(this.view, '_cls'); CC.display(obj, b);
  }
  ,

  //����TabItem��ʾ�����,Ϊһ��DOM���.
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

//--Tab �ؼ�,һ��Tab�ؼ��ɶ��TabItem����.
//����select�¼�����onselect(CTab)=...
var CTab = CC.create();

//������������.
CC.extendx(CTab.prototype, CContainerBase);

CC.extend(CTab.prototype,  {

  type: 'CTab',

  //Ĭ��TabItem����.
  defItemLen: 130,

  //������ŵ�ǰ��ʾ��TabItem�е����,TabItem�е���彫����������,��Tab��ʾ����.
  ctxPanel: null,

  selected: null,

  initialize: function(opt) {
    this._super(opt); if (!this.ctxPanel) {
      this.ctxPanel = CC. $C('DIV');
    }

    //��д�������add����.
    this.override('add', function(it) {
      //selected event.
      it.view.onclick = this.select.bind(this, it);
      //close event.
      it.view.ondblclick = this.show.bind(this, it, false); it.setOnclose(this.show.bind(this, it, false)); this._ajust(); it.parent = this;
    }
    );
  }
  ,

  //����Tab�ؼ���ʾ�������.
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

  //�Ƿ���ʾָ����TabItem,
  //����a��ΪTabItemʵ��Ҳ��ΪTabItem��id,bΪtrue��false.
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

  //ѡ��ĳ��TabItem,
  //����a��ΪTabItemʵ��Ҳ��Ϊid.
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

  //������ʾ��TabItem����.
  getDisc: function() {
    var cnt = 0; var chs = this.children; for (var i = 0, len = chs.length; i < len; i++) {
      if (chs[i].isVisible()) {
        cnt++;
      }
    }
    return cnt;
  }
});


//--CItemList,�б���ؼ�.
var CItemList = CC.create();

//�ÿؼ�Ϊһ������.
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

  //ѡ��ĳ���ؼ�,
  //����a��Ϊ�ؼ�ʵ��Ҳ��Ϊid.
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

//--CFolder ��ؼ�
//CFolder�ؼ���ʵ��һ��ͷ����һ��CItemList�ؼ����.
var CFolder = CC.create();

CC.extendx(CFolder.prototype, CBase);

CC.extend(CFolder.prototype,  {

  type: 'CFolder',

  //���һ��CItemList.
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

  //���һ��CItem��.
  add: function(oItem) {
    this.itemList.add(oItem);
  }
  ,

  //�۵���չ��.
  //b:true or false.
  fold: function(b) {
    this.inspectViewAttr('_btnFN', 'className', b ? 'opnFd' : 'clsFd'); this.itemList.setVisible(!b);
  }
}

);


//--CBarItem : Toolbar Item,��������.
var CBarItem = CC.create();

CC.extendx(CBarItem.prototype, CBase);

CC.extend(CBarItem.prototype,  {
  type: 'CBarItem', initialize: function(opt) {
    this._super(opt); this.view.innerHTML = CTemplate[this.type]; this.setViewAttr('className', 'tlBtn2'); this.setTitle(this.title); this.setIcon(this.icon); Event.bindPassByStyle(this.view, 'on', false);
  }
}

);

//�������ؼ�.
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
 * window�ؼ�.
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
 * ������ʾ��,��ʾ�󼴱�����.
 */
var CFloatTip = CC.create();

CC.extendx(CFloatTip.prototype, CBase);

CC.extend(CFloatTip.prototype,  {

  type: 'CFloatTip',

  timeout: 2500,

  //��ʾλ��.���proxy.
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
    title = title || '��ʾ';
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
    w.setTitle(title || '��ʾ');
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

//CMenuItem.prototype������CBase��.
CC.extendx(CMenuItem.prototype, CBase);

CC.extend(CMenuItem.prototype,  {

  type: 'CMenuItem',

  //�Ӳ˵�
  subMenu: null,

  initialize: function(opt) {
    //���ϲ�ΪA��ǩ
    var dv = Cache.get('div'); dv.innerHTML = CTemplate[this.type]; this._super(CC.extendIf(opt,  {
      view: opt.view || dv.firstChild
    }
    ));

    // IE7��innerHTML=''ʱ��ɾ���ӽ����������,��˸�ΪremoveChild
    dv.removeChild(dv.firstChild); Cache.put('div', dv); this.setTitle(this.title); this.setIcon(opt.icon || '');

    Event.bindPassByStyle(this.view, 'itemOn', false, (function() {
      //OnMouseOver : ������һ�˵�����Ӳ˵�
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
      //���Ӳ˵�
      oThis.parentContainer.hideAll(); Event.stopObserving(this, 'click', onClick);
    }
    );

    Event.observe(this.view, 'click', onClick, false);
  }
});


//--�˵��ؼ�
var CMenu = CC.create();

CC.extendx(CMenu.prototype, CContainerBase);

CC.extend(CMenu.prototype,  {

  type: 'CMenu',

  //���˵���
  parentItem: null,

  //��һ�ӹ��˵���
  _onItem: null,

  initialize: function(opt) {
    this._super(CC.extendIf(opt,  {
      view: CC. $C( {
        tagName: 'DIV', className: 'gMenu'
      }
      )
    }
    )); this.view.innerHTML = CTemplate[this.type]; this.setWidth(120);

    //�����˵��ڵ�onclick�¼��ϴ�
    this.setViewAttr('onclick', (function() {
      Event.stop(window.event);
    }
    ));

    //@override
    //��д�������add����.
    this.override('add', function(it) {
      this.view.firstChild.appendChild(it.view);
    }
    );

    //Ĭ��Ϊ����ʾ
    this.setVisible(false);
    //Ĭ�Ͻ��˵���ӵ�DOM��
    document.body.appendChild(this.view);
  }
  ,

  //���Ӳ˵�menu��ӵ�tar����,tar��Ϊһ��index,��һ��CMenuItem����,����ΪCMenuItem��id
  attach: function(menu, tar) {
    tar = this. $(tar); CC.addClass(tar.view, 'sub'); menu.parentItem = tar; tar.subMenu = menu;
  }
  ,

  //�����˵����ϵ��Ӳ˵�
  detach: function(tar) {
    tar = this. $(tar); CC.delClass(tar.view, 'sub'); tar.subMenu.parentItem = null; tar.subMenu = null;
  }
  ,

  //��ʾ/���ز˵�,������˵���ʱ�˵���������ʧ.
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

  //��ʾ����˵�,��show��ͬ,�÷�����ָ������ʾ�˵������ҵ�����˵���Χ��ʱ�Զ���ʧ.
  //showMenu(obj,'bottom')��showMenu(120,230)
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

  //������ò˵����������в˵�(�����Ӳ˵��͸��˵�).
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

  //���ʵ�λ����ʾ�˵�.
  _autoPositioned: function() {
    if (this.parentItem) {
      var off = Position.cumulativeOffset(this.parentItem.view); CC.style(this.view, 'top', off[1]);
      //����2px.
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

  //�����ָ��
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
      //��д�������add����,������ٸ�����ͼ.
      this.add = (function(it) {
        it.parentItem = this; it.root = this.root == null ? this : this.root;

        if (!this.contains(it)) {
          this.children.push(it);
        }

        it.parentContainer = this;
        //��Ӻ����ȷ������.
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
      //������ȷ������.
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

  //�ӵ�1����
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
    //Root Flag,�ӽ���ʼΪfalse.
    opt.root = null; this.root = new CTreeItem(opt); CC.inspect(this.view, '_ctx').appendChild(this.root.view);
  }
}

);

var CTemplate = {};
CTemplate['CItem'] = '<b id="_ico" class=""></b><a id="_tle" class="gfNm"></a>';
CTemplate['CTabItem'] = '<table><tbody><tr><td class="tLe" /><td class="bdy"><nobr id="_tle"></nobr></td><td class="btn"><a class="TabCls" id="_cls" title="�ر�" href="javascript:fGo()"></a></td><td class="tRi" /></tr></tbody></table>';
CTemplate['CFolder'] = '<h3 class="gfTit"><a id="_btnFN" onclick="" class="clsFd" href="javascript:fGo()"/><a id="_tle" class="gfName" onclick="" href="javascript:fGo()"></a></h3>';
CTemplate['CBarItem'] = '<div class="bLe"></div><b id="_ico" class=""></b><span id="_tle" class="bTxt"></span><div class="bRi"></div>';
CTemplate['CWin'] = '<h2 id="_tbar"><div class="fLe"></div><b id="_ico" class="icoSw"></b><span id="_tle"></span><div class="fRi"></div><a id="_cls" class="btnCls" title="�ر�" href="javascript:fGo();"></a></h2><div  class="bdy"><div class="fLe"></div><div id="_bdy" class="bdyCtn"></div><div class="fRi"></div></div><div id="_bot" class="bot"><div class="fLe"></div><div class="fRi"></div></div>';
CTemplate['CFloatTip'] = '<div class="bdy"><div id="_tle" class="important_txt"></div><div id="_msg" class="important_subtxt"></div></div><div class="btm_cap"></div>';
CTemplate['CUtil.alert'] = '<div class="fLe"></div><input value="ȷ ��" class="btnFnt" onclick="_sysWin.setVisible(false);" onmouseover="this.className=\'btnFnt btnFntOn\'" onmouseout="this.className=\'btnFnt\'" type="button"><div class="fRi"></div>';
CTemplate['CUtil.inputBox'] = '<table class="swTb"><tbody><tr><th valign="top"><b id="icoIfo" class="icoIfo"/></th><td><span class="swTit" id="contextSpan"><br/>@{msg}<input type="text" id="_input" class="gIpt" style=""/><p class="swEroMsg"><span id="_err"/></p></span></td></tr></tbody></table><div class="clear"/>';
CTemplate['CUtil.inputBox.body'] = '<div class="fLe"></div><input value="ȡ ��" class="btnFnt" id="_cancel" onmouseover="this.className=\'btnFnt btnFntOn\'" onmouseout="this.className=\'btnFnt\'" type="button"><input value="ȷ ��" class="btnFnt" id="_ok" onmouseover="this.className=\'btnFnt btnFntOn\'" onmouseout="this.className=\'btnFnt\'" type="button"><div class="fRi"></div>';
CTemplate['CMenuItem'] = '<a href="javascript:fGo()"><span><b id="_ico" class="icos"></b><span id="_tle"></span></span></a>';
CTemplate['CMenu'] = '<div class="bdy gMenuOpt" id="_bdy"></div>';
CTemplate['CTreeItem.part0'] = '<div id="_head" class="x-tree-node-el x-unselectable" unselectable="on"><span class="x-tree-node-indent">';
CTemplate['CTreeItem.part1'] = '<img src="s.gif" class="x-tree-icon x-tree-elbow-line"></img>';
CTemplate['CTreeItem.part2'] = '<img src="s.gif" class="x-tree-icon"></img>';
CTemplate['CTreeItem.part3'] = '<img id="_fold" src="s.gif" class="x-tree-ec-icon"></img><img id="_ico" src="s.gif" class="x-tree-node-icon"></img></span><a href="javascript:fGo()" class="x-tree-node-anchor" hidefocus="on"><span unselectable="on" id="_tle"></span></a></div><ul id="_ctx" class="x-tree-node-ct  x-tree-lines" style="position: static; display:none; left: auto; top: auto; z-index: auto;"></ul>';
CTemplate['CTree'] = '<div class="x-panel-bwrap"><div class="x-panel-body x-panel-body-noheader" style="over-flow:auto"><ul id="_ctx" class="x-tree-node-ct  x-tree-lines"></ul></div></div>';
