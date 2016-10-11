/**
 * By www.865171.cn
 * q  q : 215288671
 * Date : 2010.4.22
 */

/**
 * 将通用函数封装在CC对象中.
 */
var CC =  {

  //用于产生唯一ID.
  uniqueId: 0,

  /**
   * <script></script>标签匹配.
   */
  ScriptExpAll: new RegExp('<script[^>]*>([\\S\\s]*?)<\/script>', 'img'), 
  ScriptExpOne: new RegExp('<script[^>]*>([\\S\\s]*?)<\/script>', 'im'),

  /**
   * <style></style>标签匹配.
   */
  StyleExpAll: new RegExp('<style[^>]*>([\\S\\s]*?)<\/style>', 'img'), 
  StyleExpOne: new RegExp('<style[^>]*>([\\S\\s]*?)<\/style>', 'im'),

  /**
   * object : 一个可枚举对象.
   * callback:枚举回调,如果有参数,效果如element.callback(args...).
   * 无参数无length属性element.callback(name,value)否则element.callback(i,element);
   * callback返回false中断枚举.
   */
  each: function(object, callback, args) {

    if (!object) {
      return object;
    }

    if (args) {
      if (object.length == undefined) {
        for (var name in object)
          if (callback.apply(object[name], args) === false)
            break;
      } else
        for (var i = 0, length = object.length; i < length; i++)
          if (callback.apply(object[i], args) === false)
            break;
    } else {
      if (object.length == undefined) {
        for (var name in object)
          if (callback.call(object[name], name, object[name]) === false)
            break;
      } else
      for (var i = 0, length = object.length, value = object[0]; i < length && callback.call(value, i, value) !== false; value = object[++i]){}
    }

    return object;
  }
  ,

  /**
   * 将src属性复制到des中,des已存在的属性不会被覆盖.
   */
  extendIf: function(des, src) {
    if (!des) {
      des = {}
      ;
    }
    if (src) {
      for (var i in src) {
        if (des[i] == undefined) {
          des[i] = src[i];
        }
      }
    }
    return des;
  }
  ,

  /**
   * 复制源对象src所有属性到目的对象des中.
   */
  extend: function(des, src) {
    if (!des) {
      des = {}
      ;
    }
    if (src) {
      for (var i in src) {
        des[i] = src[i];
      }
    }
    return des;
  }
  ,

  /**
   * 与extend方法不同的是,复制属性后并使得目的对象支持OOP的一些方法.
   */
  extendx: function(des, src) {

    if (!des || !src) {
      return des;
    }

    for (var prpty in src) {
      des[prpty] = src[prpty];
    }
    //
    if (!des.superclass) {
      CC.extend(des,  {
        _super: function() {
          var clz = this.superclass;
          //第cnt个Initialize调用.
          var cnt = this.__initIdx + 1; var st = 0;
          while (clz) {
            st++; if (st == cnt) {
              this.__initIdx++; clz.initialize.apply(this, arguments); return ;
            }
            clz = clz.superclass;
          }
        }
        ,

        //callType=before:父类方法调用在前,重写方法调用在后,after:相反,无时不调用父类方法.
        override: function(name, fncNew, callType) {
          var pfn = this[name]; if (!callType || callType == 'before') {
            this[name] = function() {
              pfn.apply(this, arguments); fncNew.apply(this, arguments);
            }
          }

           else if (callType == 'after') {
            this[name] = function() {
              fncNew.apply(this, arguments); pfn.apply(this, arguments);
            }
          } else {
            this[name] = function() {
              fncNew.apply(this, arguments);
            }
          }
        }
      }
      );
    };
    des.superclass = src;
    des.__initIdx = 0;
    return des;
  }
  ,

  /**
   * 返来一个类构造函数,该构造函数在new 时自动调用类的initialize()方法进行初始化.
   */
  create: function() {
    return function() {
      this.initialize.apply(this, arguments);
    }
  }
  ,

  /**
   * 生成一个类,clazzName:类名称,arrPropertyNames:属性名称,并作为构造函数初始化参数列表.
   * 如创建一个User类:CC.createBean('User',['id','name']);
   * var theUser = new User(someId,strName);
   */
  createBean: function(clazzName, arrPropertyNames) {
    var func = function() {
      for (var i = 0; i < arrPropertyNames.length; i++) {
        this[arrPropertyNames[i]] = arguments[i];
      }
    };
    window[clazzName] = func;
    return func;
  }
  ,

  debug: function(func, error) {
    var sTime = (new Date()).toLocaleString();
    var info = "FUNCTION:" + func + " --- Info:" + error;
    //TODO:error debug to server..
  }
  ,


  isIE: (document.all) ? true : false,

  /**
   * 获得浏览器XMLHttpRequest对象.
   */
  ajaxRequest: function() {
    try {
      if (window.XMLHttpRequest) {
        return new XMLHttpRequest();
      } else {
        if (window.ActiveXObject) {
          try {
            return new ActiveXObject("Msxml2.XMLHTTP");
          } catch (e) {
            try {
              return new ActiveXObject("Microsoft.XMLHTTP");
            } catch (e) {
              return null;
            }
          }
        }
      }
    } catch (ex) {
      this.debug('createXMLHttpRequest', ex);
      return false;
    }
  }
  ,

  //CC.$(id)等同于window.$('id').
  $: function(id) {
    return $(id);
  }
  ,

  /**
   * 以提交查询形式返回DOM结点f的字段数据.这些字段来自于f结点所包括的input类型子结点.
   * 当input类型结点的radio,checkbox属性checked==false时忽略该结点数据.
   */
  formQuery: function(f) {
    try {
      var formData = "", elem = "";
      var elements = f.elements;
      var length = elements.length;
      for (var s = 0; s < length; s++) {
        elem = elements[s];
        if (elem.tagName == 'INPUT') {
          if (elem.type == 'radio' || elem.type == 'checkbox') {
            if (!elem.checked) {
              continue;
            }
          }
        }
        if (formData != "") {
          formData += "&";
        }
        formData += elem.name + "=" + encodeURIComponent(elem.value);
      }
      return formData;
    } catch (ex) {
      this.debug('formQuery', ex);
      return false;
    }
  }
  ,

  /**
   * [id,msg,callback(value)],[id,msg,callback(value)],bQueryString
   * 返回一个Object.id传放通过验证的数据.
   * 如果bQueryString = true,返回由数据组成的查询字符串.
   * 如果验证出错,返回false;
   */
  validate: function() {
    var args = $A(arguments);
    var bQueryString = false;
    var b = args[args.length - 1];
    if (typeof(b) !== 'object') {
      bQueryString = b;
      args.remove(args.length - 1);
    }

    var result = (bQueryString) ? '' : {}
    ;
    CC.each(args, function(i, v) {
      var obj = $(v[0]); var value = obj.value; if (v[2]) {
        if (!v[2](value)) {
          alert(v[1]); obj.focus(); result = false; return false;
        }
      } else {
        if (value == null || value == '') {
          alert(v[1]); obj.focus(); result = false; return false;
        }
      }
      if (bQueryString) {
        result += (result == '') ? v[0] + '=' + value: '&' + v[0] + '=' + value;
      } else {
        result[v[0]] = value;
      }
    }
    );
    return result;
  }
  ,

  /**
   * 返回一个唯一ID以供程序使用.这里所指的唯一性由程序自身定义.
   */
  uniqueID: function() {
    return this.uniqueId++;
  }
  ,

  /**
   * 返回无提交参数的url.
   */
  normalizeURL: function(url) {
    var idx = url.indexOf('?');
    if (idx < 0) {
      return url;
    }
    return url.substring(0, idx);
  }
  ,

  isFunction: function(obj) {
    return (obj instanceof Function || typeof obj == "function");
  }
  ,


  isString: function(obj, canEmpty) {
    if (canEmpty) {
      return ((obj instanceof String || typeof obj == "string") && obj != "");
    } else {
      return (obj instanceof String || typeof obj == "string");
    }
  }
  ,

  isArray: function(obj) {
    return (ob instanceof Array || typeof ob == "array");
  }
  ,


  isDate: function() {
    return (ob instanceof Date);
  }
  ,

  alert: function(msg, title) {
    title = title || '提示';
    if (!window._sysWin) {
      window._sysWin = new CWin( {
        title: title
      }
      );
      CC.style(_sysWin.body, 'textAlign', 'center');
      document.body.appendChild(_sysWin.view);
      _sysWin.bottom.innerHTML = '<div class="fLe"></div><input value="确 定" class="btnFnt" onclick="_sysWin.setVisible(false);" onmouseover="this.className=\'btnFnt btnFntOn\'" onmouseout="this.className=\'btnFnt\'" type="button"><div class="fRi"></div>';
    }
    _sysWin.body.innerHTML = msg;
    _sysWin.centerWindow();
    _sysWin.setVisible(true);
  }
  ,

  tip: function(msg, title, proxy, timeout, getFocus) {
    proxy = CC. $(proxy);
    var tip = new CFloatTip( {
      parent: document.body, proxy: proxy, timeout: timeout, title: title, msg: msg
    }
    );
    tip.setVisible(true);

    if (getFocus) {
      proxy.focus();
    }
    return tip;
  }
  ,

  isNumber: function(ob) {
    var firstTest = (ob instanceof Number || typeof ob == "number");
    if (!firstTest) {
      var stI = "" + ob;
      if (stI.length == 0) {
        return false;
      }
      for (var i = 0; i < stI.length; i++) {
        var ch = stI.charAt(i);
        if ("0123456789.-%" .indexOf(ch) ==  - 1) {
          return false;
        }
      }
      return true;
    } else {
      return true;
    }
  }
  ,


 isMail : function(strMail) {
 	return /\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/.test(strMail);
 },
	
	isUndefined: function(object) {
    return typeof object == "undefined";
  }
  ,
  
  today: function() {
    return new Date();
  }
  ,


  ddmmyy: function(date) {
    var d = date.getDate();
    if (d < 10) {
      d = "0" + d;
    }
    var month = date.getMonth() + 1;
    if (month < 10) {
      month = "0" + month;
    }
    var fyear = date.getFullYear();
    return d + "/" + month + "/" + fyear;
  }
  ,


  addDate: function(field, date, delta) {
    var newDate = null;
    switch (field) {
      case "year":
        newDate = new Date(parseInt(date.getFullYear()) + 1, date.getMonth(), date.getDate());
        break;
      case "month":
        newDate = new Date(date.getFullYear(), date.getMonth() + delta, date.getDate());
        break;
      case "day":
        newDate = new Date(date.getFullYear(), date.getMonth(), date.getDate() + delta);
        break;
    }
    return newDate;
  }
  ,


  addClass: function(o, s) {
    var ss = o.className.replace(s, '');
    ss += ' ' + s;
    o.className = ss;
  }
  ,

  delClass: function(o, s) {
    o.className = o.className.replace(s, "");
  }
  ,

  /**
   * 交替样式.
   */
  switchClass: function(a, oldSty, newSty) {
    CC.delClass(a, oldSty);
    CC.addClass(a, newSty);
  }
  ,

  setClass: function(o, s) {
    o.className = s;
  }
  ,
	
  /**
   * usage : $C('div')或$C({tagName:'div',className:'style',onclick:func})
   */
  $C: function(a) {
    if (CC.isString(a)) {
      return document.createElement(a);
    }
    var tag = a.tagName;
    delete a.tagName;
    var b = CC.extend(document.createElement(tag), a);
    a.tagName = tag;
    return b;
  }
  ,


  $N: function(tagName) {
    return document.getElementsByTagName(tagName);
  }
  ,


  clearAttribute: function(obj) {
    for (var att in obj) {
      delete obj[att];
    }
  }
  ,

  /**
   * 清除DOM所有孩子结点.
   */
  removeChildren: function(node) {
    while (node.firstChild) {
      node.removeChild(node.firstChild);
    }
  }
  ,

  /**
   * 返回属性结点下标,这里把对象所有属性放在一个数组里看待.
   */
  getAttributeByIndex: function(obj, idx) {
    var i, k = 0;
    for (i in obj) {
      if (k++ == idx) {
        return obj[i];
      }
    }
    return null;
  }
  ,


  /**
   * 返回DOM结点对于兄弟结点的下标.
   */
  indexOfElement: function(parentNode, childNode) {
    var i, node;
    for (i = 0, node = parentNode.firstChild; (node != null) && (node != childNode); i++, node = node.nextSibling)
      ;
    return (node == null) ?  - 1: i;
  }
  ,

  /**
   * 加载一个Javascript文件.
   */
  loadScript: function(id, url) {
    var oHead = this. $N('head')[0];
    var script = this. $C( {
      tagName: 'SCRIPT', id: id, type: 'text/javascript', src: url
    }
    );
    oHead.appendChild(script);
    return script;
  }
  ,

  /**
   * 加载一个样式文件.
   */

  loadCSS: function(id, url) {
    var oHead = this. $N('head')[0];
    var css = this. $C( {
      tagName: 'link', id: id, rel: 'stylesheet', href: url, type: 'text/css'
    }
    );
    oHead.appendChild(css);
    return css;
  }
  ,

  /**
   * 加载一个样式字符串.
   */
  loadStyle: function(id, ss) {
    var o;
    if (CC.isIE) {
      window[id] = ss;
      o = document.createStyleSheet('javascript:' + id);
      return o;
    }
    var css = CC. $C( {
      tagName: 'style', id: id, type: 'text/css'
    }
    );
    css.innerHTML = ss;
    this. $N('head')[0].appendChild(css);
    return ss;
  }
  ,

  unloadHeader: function(id) {
    var h = this. $N('head')[0];
    h.removeChild(this.inspect(h, id));
  }
  ,

  /**
   * 使得提交的数据忽略浏览器缓存.
   */
  noCache: function() {
    return '&noCacheReq=' + (new Date()).getTime();
  }
  ,

  /**
   * 显示或隐藏某结点,v可以是结点id或结点.
   * 如果是id,则结点必须在dom树中.
   */
  display: function(v, b) {
    if (arguments.length == 1) {
      return $(v).style.display != 'none';
    }
    var ss = b ? 'block' : 'none';
    $(v).style.display = ss;
  }
  ,

  /**
   * 是否禁用DOM结点.
   */
  disabled: function(v, b) {
    $(v).disabled = b;
  }
  ,

  /**
   * 同dom 中 insertBefore相对.
   */
  insertAfter: function(oNew, oSelf) {
    var oNext = oSelf.nextSibling;
    if (oNext == null) {
      oSelf.parentNode.insertBefore(oNew, oSelf);
    } else {
      oSelf.parentNode.insertBefore(oNew, oNext);
    }
    return oNew;
  }
  ,

  /**
   * 返回包含在父结点中的属性id孩子结点,孩子结点可在深层,id在父结点中需唯一.
   */
  inspect: function(oParent, id) {
    var child = oParent.firstChild;
    while (child) {
      if (child.id == id) {
        return child;
      }
      var v = this.inspect(child, id);
      if (v) {
        return v;
      }
      child = child.nextSibling;
    }
    return null;
  }
  ,

  /**
   * 设置对象任意层次的属性值,如inspectAttr(obj,'firstChild.style.display','block');
   */
  inspectAttr: function(obj, attrList, value) {
    if (CC.isString(attrList)) {
      attrList = attrList.split('.');
    }

    for (var i = 0, idx = attrList.length - 1; i < idx; i++) {
      obj = obj[attrList[i]];
    }
    if (value == undefined) {
      return obj[attrList[i]];
    }
    obj[attrList[i]] = value;
  }
  ,

  select: function(dom, tagName) {
    return $(dom).getElementsByTagName(tagName);
  }
  ,

  /**
   * 设置或获取某结点风格,当style为一个对象时可设置多个属性.
   */
 style : function(element,style,value) {
 	//getter
 	if(CC.isUndefined(value)) {
		return CC.getStyle(element,style);
 	}
 	return CC.setStyle(element,style,value);
 },
  
	getOpacity : function (element) { 
     return CC.getStyle(element, 'opacity'); 
	},
	
  setOpacity : function (element, value) { 
  	element = $(element);
  	element.style.opacity = value == 1 ? '' : value < 0.00001 ? 0 : value; 
	},

 setStyle : function (element, key, value) { 
     if (key == 'opacity') { 
         CC.setOpacity(element, value) 
     } else { 
     		element = $(element);
         var elementStyle = element.style; 
         elementStyle[ 
             key == 'float' || key == 'cssFloat' ? ( 
                 elementStyle.styleFloat === undefined ? ( 
                     'cssFloat' 
                 ) : ( 
                     'styleFloat' 
                 ) 
             ) : (
                 key.camelize()
             )
         ] = value; 
     } 
 },
 
  getStyle : function (element, key) { 
  			 element = $(element);
         key = key == 'float' ? 'cssFloat' : key.camelize(); 
         var value = element.style[key]; 
         if (!value) { 
             var css = document.defaultView.getComputedStyle(element, null); 
             value = css ? css[key] : null; 
         } 
         if (key == 'opacity') { 
             return value ? parseFloat(value) : 1.0; 
         } 
         return value == 'auto' ? null : value; 
 	}
 
};

/**
 * XML处理类.
 */
var XML =  {

  //new xml document from string or url specified.
  newDoc: function() {
    if (arguments.length == 1) {
      return this.newDocFromString(arguments[0]);
    } else if (arguments.length == 3) {
      return this.newDocFromURL(arguments[0], arguments[1], arguments[2]);
    }
    return null;
  }
  ,

  newDocFromString: function(str) {
    try {
      if (window.ActiveXObject) {
        var arrSignatures = ["MSXML2.DOMDocument.5.0", "MSXML2.DOMDocument.4.0", "MSXML2.DOMDocument.3.0", "Microsoft.XmlDom"]; for (var i = 0; i < arrSignatures.length; i++) {
          try {
            var oXmlDom = new ActiveXObject(arrSignatures[i]); oXmlDom.async = false; oXmlDom.loadXML(str); return oXmlDom;
          } catch (ex){}
        }
      } else if (window.DOMParser) {
        var parser = new DOMParser(); return parser.parseFromString(str, "text/xml");
      }
    } catch (ex) {
      CC.debug('fXmlCreateDocFromString:' + ex);
    }
    return null;
  }
  ,

  newDocFromURL: function(url, strOnloadCallback, caller) {
    var xmlDoc = null; var callback = function() {
      caller[strOnloadCallback]();
    }; if (document.implementation && document.implementation.createDocument) {
      xmlDoc = document.implementation.createDocument("", "", null); if (strOnloadCallback) {
        xmlDoc.onload = callback;
      }
    } else if (window.ActiveXObject) {
      xmlDoc = new ActiveXObject("Microsoft.XMLDOM"); if (strOnloadCallback) {
        xmlDoc.onreadystatechange = function() {
          if (xmlDoc.readyState == 4) {
            callback();
          }
        };
      }
    }
    if (url) {
      xmlDoc.load(url);
    }
    return xmlDoc;
  }
  ,

  $: function(node, name) {
    return node.getElementsByTagName(name);
  }
  ,

  /**
   * 以JS对象形式返回xml结点属性.
   */
  parseAttr: function(node) {
    var objAttr = null; if (node.attributes) {
      objAttr = {}
      ; for (var i = 0; i < node.attributes.length; i++) {
        var attValue = node.attributes[i].value; var attName = node.attributes[i].name; objAttr[attName] = attValue;
      }
    }
    return objAttr;
  }
  ,

  outerXml: function(node) {
    if (node.xml) {
      return node.xml;
    } else {
      if (XMLSerializer != undefined) {
        return (new XMLSerializer()).serializeToString(node);
      }
    }
  }
  ,

  innerXml: function(node) {
    var str = this.outerXml(node); var start = str.indexOf('>'); var end = str.lastIndexOf('<') - 1; return str.substr(start + 1, end - start);
  }
  ,

  /**
   * 将xml数据转为json形式.
   */
  toJson: function(pnode) {
    if (pnode == null) {
      return null;
    }
    //属性
    var obj = {}
    ; obj.attributes = this.parseAttr(pnode);
    //叶子结点
    var node = pnode.firstChild; if (node == null) {
      return obj;
    }
    if (node.firstChild == null) {
      return node.data;
    }

    var tgName = null;
    while (node) {
      tgName = node.tagName; data = this.toJson(node);
      //数组
      if (obj[tgName] != null) {
        if (!(obj[tgName]instanceof Array)) {
          obj[tgName] = new Array(obj[tgName], data);
        } else {
          obj[tgName][obj[tgName].length] = data;
        }
      } else {
        obj[tgName] = data;
      }
      node = node.nextSibling;
    }
    return obj;
  }
};

/*----------------------
给firefox 中添加
event.srcElement,	window.event,HtmlElement.runtimeStyle
---------------------*/
var IESIM =  {
  fixEventMeta: function() {
    HTMLElement.prototype.__defineGetter__("runtimeStyle", IESIM.getRuntimeStyle); window.constructor.prototype.__defineGetter__("event", IESIM.getEvent); Event.prototype.__defineGetter__("srcElement", IESIM.getSrcElement);
  }
  ,

  getRuntimeStyle: function() {
    return this.style;
  }
  ,

  getEvent: function() {
    return IESIM.searchEvent();
  }
  ,

  getSrcElement: function() {
    return this.target;
  }
  ,

  searchEvent: function() {
    if (CC.isIE) {
      return window.event;
    }
    var func = IESIM.searchEvent.caller;
    while (func != null) {
      var arg0 = func.arguments[0]; if (arg0) {
        if (arg0 instanceof Event) {
          return arg0;
        }
      }
      func = func.caller;
    }
    return null;
  }
  ,

  simulate: function() {
    this.fixEventMeta();
  }
};


var $A = Array.from = function(iterable) {
  if (!iterable)return []; if (iterable.toArray) {
    return iterable.toArray();
  } else {
    var results = []; for (var i = 0, length = iterable.length; i < length; i++)results.push(iterable[i]); return results;
  }
}

var $ = function(a) {
  if (CC.isString(a)) {
    return document.getElementById(a);
  }
  return a;
};

var fGo = function(){};

if (!CC.isIE) {
  IESIM.simulate();
	}else {
     CC.getStyle = function (element, key) {
         key = key == 'float' ? 'styleFloat' : key.camelize(); 
         var value = element.style[key]; 
         if (!value && element.currentStyle) value = element.currentStyle[key]; 
         if (key == 'opacity') {
         		 if(element.filters[0]) {
         		 	return parseFloat(element.filters[0].Opacity/100.0);
         		 }
             value = ( CC.getStyle(element, 'filter') || '').match(/alpha\(opacity=(.*)\)/); 
             if (value) { 
                 if (value[1]) { 
                     return parseFloat(value[1]) / 100; 
                 }
             }
             return 1.0;  
         } else { 
             if (value == 'auto') { 
                 if ( 
                     (key == 'width' || key == 'height') &&  
                     CC.getStyle(element, 'display') != 'none' 
                 ) { 
                     return element['offset' + names.title(key)] + 'px'; 
                 } else { 
                     return undefined; 
                 } 
             } 
         } 
         return value; 
     };
     
     CC.setOpacity = function (element, value) {
     		element = $(element);
        if (element.filters && element.filters[0] && typeof (element.filters[0].opacity) == 'number')
        {
            element.style.zoom = 1;
            element.filters[0].opacity = value*100;
        }
        else
        {
            element.style.zoom = 1;
            element.style.filter = 'alpha(opacity=' + value*100 + ')';
        }
     }; 
 }

function fOnLoadStart() {
  Event.fireEvent('system', 'onload');
}

window.onload = fOnLoadStart;

CC.extend(String.prototype,  {
  trim: function() {
    return this.replace(new RegExp("(^[\\s]*)|([\\s]*$)", "g"), "");
  }
  ,

  escape: function() {
    return escape(this);
  }
  ,

  unescape: function() {
    return unescape(this);
  }
  ,

  checkSpecialChar : function(flag,oObj){
    var reg=/[%\'\"\/\\]/;
    if( this.search( reg )!=-1){
        if(flag){
        	//CC.showSysMsg('error',"Be careful of characters such as ＂ % \' \" \\ \/ etc.",oObj,3000);
        }
        return false;
    }
    return true;
	},

  //截短
  truncate: function(length, truncation) {
    length = length || 30; truncation = truncation === undefined ? '...' : truncation; return this.length > length ? this.slice(0, length - truncation.length) + truncation: this;
  }
  ,

  //除去字符串中JS,fncb为可选回调项，回调传递<script>...</script>.
  stripScript: function(fncb) {
    if (!fncb) {
      return this.replace(CC.ScriptExpAll, '');
    }
    return this.replace(CC.ScriptExpAll, function(sMatch) {
      fncb(sMatch); return '';
    }
    );
  }
  ,

  stripStyle: function(fncb) {
    if (!fncb) {
      return this.replace(CC.StyleExpAll, '');
    }
    return this.replace(CC.StyleExpAll, function(sMatch) {
      fncb(sMatch); return '';
    }
    );
  }
  ,

  innerScript: function() {
    this.match(CC.ScriptExpOne); return RegExp. $1;
  }
  ,

  innerStyle: function() {
    this.match(CC.StyleExpOne); return RegExp. $1;
  }
  ,

  //运行并除去JS,...<script>content..</script>...,可含多个<style></style>
  execScript: function() {
    return this.replace(CC.ScriptExpAll, function(ss) {
      //IE 不直接支持RegExp.$1??.
      ss.match(CC.ScriptExpOne); if (window.execScript) {
        execScript(RegExp. $1);
      } else {
        window.eval(RegExp. $1);
      }
      return '';
    }
    );
  }
  ,

  execStyle: function() {
    return this.replace(CC.StyleExpAll, function(ss) {
      //IE 不直接支持RegExp.$1??.
      ss.match(CC.StyleExpOne); CC.loadStyle(RegExp. $1); return '';
    }
    );
  },
  
  camelize: function() {
    var parts = this.split('-'), len = parts.length;
    if (len == 1) return parts[0];

    var camelized = this.charAt(0) == '-'
      ? parts[0].charAt(0).toUpperCase() + parts[0].substring(1)
      : parts[0];

    for (var i = 1; i < len; i++)
      camelized += parts[i].charAt(0).toUpperCase() + parts[i].substring(1);

    return camelized;
  }
}

);

CC.extend(Array.prototype,  {
  clone: function() {
    var copArr = []; copArr = (copArr.concat(this)); return copArr;
  }
  ,

  //p:the index or the obj in arr,return the length
  remove: function(p) {
    if (CC.isNumber(p)) {
      if (p < 0 || p >= this.length) {
        throw "Index Of Bounds:" + this.length + "," + p;
      }
      this.splice(p, 1)[0]; return this.length;
    }
    if (this.length > 0 && this[this.length - 1] == p) {
      this.pop();
    } else {
      var pos = this.indexOfArray(this, p); if (pos !=  - 1) {
        this.splice(pos, 1)[0];
      }
    }
    return this.length;
  }
  ,

  indexOf: function(obj) {
    for (var i = 0, length = this.length; i < length; i++) {
      if (this[i] == obj)return i;
    }
    return  - 1;
  }
  ,

  insert: function(idx, obj) {
    return this.splice(idx, 0, obj);
  }
  ,

  include: function(obj) {
    return (this.indexOf(obj) !=  - 1);
  }
}

);


function StringBuilder(ss) {
  this._buff = new Array();
}

StringBuilder.prototype =  {
  append: function(str) {
    this._buff[this._buff.length] = str;
  }
  ,

  rollBack: function() {
    this._buff.pop();
  }
  ,

  toString: function() {
    return this._buff.join('');
  }
};

//Exception Set

function Exception(str) {
  this.msg = str;
}

Exception.prototype =  {
  toString: function() {
    if (this.msg) {
      return this.msg.toString();
    }
    return "";
  }
};

function IndexOutOfBoundsException(len, acutal) {
  this.len = len; this.acutal = acutal;
}

IndexOutOfBoundsException.prototype = new Exception(); IndexOutOfBoundsException.prototype.toString = function() {
  return ("Index Out Of Bounds(length,index):" + this.len + "," + this.acutal);
};

function NullPointerException(msg) {
  if (msg)this.msg = msg;
}

NullPointerException.prototype = new Exception("Null Pointer Exception");


//Event Util
if (!window.Event) {
  window.Event = new Object;
}

CC.extend(Event,  {

  eventList: [],

  KEY_BACKSPACE: 8, KEY_TAB: 9, KEY_ENTER: 13, KEY_ESC: 27, KEY_LEFT: 37, KEY_UP: 38, KEY_RIGHT: 39, KEY_DOWN: 40, KEY_DELETE: 46,
  //添加事件监听
  addListener: function(sid, eid, caller, handler) {
    try {
      var eids = this.eventList[sid]; if (eids == null) {
        eids = {}
        ; this.eventList[sid] = eids;
      }
      var handlers = this.eventList[sid][eid]; if (handlers == null) {
        eids[eid] = [[caller, handler]]; return ;
      }
      for (var i = 0; i < handlers.length; i++) {
        if (handlers[i][1] == handler && handlers[i][0] == caller) {
          return ;
        }
      }
      handlers[handlers.length] = [caller, handler];
    } catch (ex) {
      CC.debug('EH.addListener', ex); return false;
    }
  }
  ,

  //发送事件
  fireEvent: function(sid, eid) {
    try {
      var ss = this.eventList[sid]; if (!ss) {
        return ;
      }
      var handlers = ss[eid]; if (handlers == null) {
        return ;
      }
      var args = []; for (var j = 2; j < arguments.length; j++) {
        args[j - 2] = arguments[j];
      }
      var length = handlers.length; for (var i = 0; i < length; ++i) {
        var caller = handlers[i][0]; var handler = handlers[i][1]; try {
          if (caller == null) {
            handler(args);
          } else {
            if (CC.isFunction(handler)) {
              handler.call(caller, args);
            } else {
              caller[handler](args);
            }
          }
        } catch (e){}
      }
    } catch (ex) {
      CC.debug("fireEvent(" + sid + "," + eid + ")", ex); return false;
    }
  }
  ,

  //移除事件
  removeListener: function(sid, eid, caller, theHandler) {
    try {
      var ss = this.eventList[sid]; if (!ss) {
        return ;
      }
      var handlers = ss[eid]; var length = handlers.length; for (var i = 0; i < length; ++i) {
        if (handlers[i][1] == theHandler && handlers[i][0] == caller) {
          var arr = new Array(); handlers[i] = null; this.eventList[sid][eid] = arr.concat(handlers.slice(0, i), handlers.slice(i + 1));
        }
      }
    } catch (ex) {
      CC.debug('EH.removeListener', ex); return false;
    }
  }
  ,

  //检查是否按下回车
  isEnterKey: function(ev) {
    return (ev.keyCode == 13);
  }
  ,

  //get event.srcElement
  element: function(ev) {
    return ev.srcElement;
  }
  ,

  //mouse left click
  isLeftClick: function(ev) {
    return (((ev.which)
     && (ev.which == 1)) || ((ev.button) && (ev.button == 1)));
  }
  ,

  //stop event bubble up
  stop: function(ev) {
    if (ev.preventDefault) {
      ev.preventDefault(); ev.stopPropagation();
    } else {
      ev.returnValue = false; ev.cancelBubble = true;
    }
  }
  ,

  bindPassByStyle: function(obj, cssStyle, capture, onBack, offBack, proxy) {
    this.bindAlternateStyle(obj, 'onmouseover', 'onmouseout', cssStyle, capture, onBack, offBack, proxy);
  }
  ,

  bindClickStyle: function(obj, cssStyle, capture, downBack, upBack, proxy) {
    this.bindAlternateStyle(obj, 'onmousedown', 'onmouseup', cssStyle, capture, downBack, upBack, proxy);
  }
  ,

  //当callback返回true时不改变样式.
  bindAlternateStyle: function(obj, strEvtOn, strEvtOff, cssStyle, capture, onBack, offBack, proxy) {
    obj = $(obj); obj[strEvtOn] = (function(ev) {
      ev = ev || window.event;

      if (!capture) {
        Event.stop(ev);
      }

      if (onBack) {
        if (proxy) {
          if (onBack.call(proxy, ev)
          ) {
            return ;
          }
        } else {
          if (onBack(ev)
          ) {
            return ;
          }
        }
      }
      CC.addClass(obj, cssStyle);
    }
    );

    obj[strEvtOff] = (function(ev) {
      ev = ev || window.event;

      if (!capture) {
        Event.stop(ev);
      }

      if (offBack) {
        if (proxy) {
          if (offBack.call(proxy, ev)
          ) {
            return ;
          }
        } else {
          if (offBack(ev)
          ) {
            return ;
          }
        }
      }
      CC.delClass(obj, cssStyle);
    }
    );
  }
  ,

  observers: false,

  _observeAndCache: function(element, name, observer, useCapture) {
    if (!this.observers) {
      this.observers = [];
    }
    if (element.addEventListener) {
      this.observers.push([element, name, observer, useCapture]); element.addEventListener(name, observer, useCapture);
    } else if (element.attachEvent) {
      this.observers.push([element, name, observer, useCapture]); element.attachEvent('on' + name, observer);
    }
  }
  ,

  unloadCache: function() {
    if (!this.observers) {
      return ;
    }

    for (var i = 0; i < this.observers.length; i++) {
      this.stopObserving.apply(this, this.observers[i]); this.observers[i][0] = null;
    }
    this.observers = false;
  }
  ,

  observe: function(element, name, observer, useCapture) {
    useCapture = useCapture || false;

    if (name == 'keypress' && (navigator.appVersion.match( / Konqueror | Safari | KHTML / )
     || element.attachEvent)) {
      name = 'keydown';
    }
    this._observeAndCache(element, name, observer, useCapture);
  }
  ,

  stopObserving: function(element, name, observer, useCapture) {
    var element = $(element); useCapture = useCapture || false;

    if (name == 'keypress' && (navigator.appVersion.match( / Konqueror | Safari | KHTML / )
     || element.detachEvent)) {
      name = 'keydown';
    }

    if (element.removeEventListener) {
      element.removeEventListener(name, observer, useCapture);
    } else if (element.detachEvent) {
      element.detachEvent('on' + name, observer);
    }
  }
  ,

  noSelect: function() {
    return false;
  }
}

);

/* 防止IE内存泄漏 */
if (CC.isIE) {
  Event.observe(window, 'unload', Event.unloadCache, false);
}


//即返回函数对象 function func(caller,method,args){caller.method(args)}
//当外部调用bind返回的函数传入参数时,参数将加入args数组一起传入所绑定的函数.
Function.prototype.bind = function() {
  var __method = this, args = $A(arguments), object = args.shift(); return function() {
    return __method.apply(object, args);
  }
}

Function.prototype.bindAsListener = function() {
  var __method = this, args = $A(arguments), object = args.shift(); return function(event) {
    return __method.apply(object, [(event || window.event)].concat(args));
  }
}

Function.prototype.bindAsNoCaptureListener = function() {
  var __method = this, args = $A(arguments), object = args.shift(); return function(event) {
    event = event || window.event; Event.stop(event); return __method.apply(object, [event].concat(args));
  }
}

/**
 *
 */
var Ajax = CC.create();

Ajax.S_OK = "ok"; Ajax.S_ERROR = "error"; Ajax.S_NA = "na"; Ajax.S_TIMEOUT = "timeout";

Ajax.prototype =  {

  initialize: function(options) {
    CC.extend(this,  {
      method: 'POST', asynchronous: true, contentType: 'application/x-www-form-urlencoded', msg: "数据加载中,请稍候...", timeout: 10000, disabledComp: null, xmlReq: CC.ajaxRequest(), caller: null, onFailed: null, onSuccess: function(ajax) {
        ajax.invokeHtml();
      }
      , callBackParam: null, state: Ajax.S_NA, displayPanel: null, ui: true
    }
    ); CC.extend(this, options);
  }
  ,

  setOption: function(opts) {
    CC.extend(this, opts);
  }
  ,

  setMsg: function(msg) {
    var pb = CC. $('ajaxProgressBar'); if (!pb) {
      var dv = Cache.get('div'); dv.innerHTML = '<div id="ajaxProgressBar" style="top: 24px; right: 10px; z-index: 60; left: 814px; display: block;" class="gSLoading"><span id="_msg"></span></div>';
      pb = dv.firstChild; Cache.put('div', dv);
      document.body.appendChild(pb);
    }
    CC.inspect(pb, '_msg').innerHTML = msg; CC.display(pb, this.ui);
  }
  ,

  onTimeout: function() {
    if (this.xmlReq.readyState >= 4) {
      return ;
    }
    this.abort(); this.setMsg("time out.");
  }
  ,

  _close: function() {
    if (this.disabledComp) {
      CC.disabled(this.disabledComp, false)
    }; this.disabledComp = null; CC.display(CC. $('ajaxProgressBar'), false);
  }
  ,

  abort: function() {
    this.xmlReq.abort(); this._close();
  }
  ,

  _setHeaders: function() {
    if (this.method.toLowerCase() == 'post') {
      this.xmlReq.setRequestHeader('Content-type', this.contentType + (this.encoding ? '; charset=' + this.encoding: ''));
    }
  }
  ,
  //timeout start here
  open: function() {
    if (this.timeout) {
      setTimeout(this.onTimeout.bind(this), this.timeout);
    }

    if (this.disabledComp) {
      CC.disabled(this.disabledComp, true);
    }
    var theUrl = null; if (this.url.indexOf('?') > 0) {
      theUrl = this.url + '&__uid=' + CC.uniqueID();
    } else {
      theUrl = this.url + '?&__uid=' + CC.uniqueID();
    }
    this.xmlReq.open(this.method.toUpperCase(), theUrl, this.asynchronous);
  }
  ,

  send: function(data) {
    this._setHeaders(); this.xmlReq.onreadystatechange = this.onReadyStateChange.bind(this); this.xmlReq.send(data); this.setMsg(this.msg);
  }
  ,

  setRequestHeader: function(key, value) {
    this.xmlReq.setRequestHeader(key, value);
  }
  ,

  getResponseHeader: function(key) {
    return this.xmlReq.getResponseHeader(key);
  }
  ,

  onReadyStateChange: function() {
    var req = this.xmlReq; if (req.readyState == 4) {
      var onSuccess = this.onSuccess; var onFailure = this.onFailure;

      if (req.status == 200) {
        if (CC.isFunction(onSuccess)) {
          if (this.caller) {
            onSuccess.apply(this.caller, [this]);
          } else {
            onSuccess(this);
          }
        } else {
          if (this.caller) {
            if (onSuccess) {
              this.caller[onSuccess](this);
            }
          } else {
            if (onSuccess) {
              if (CC.isString(onSuccess)) {
                window[onSuccess](this);
              }
            }
          }
        }
      } else {
        //failed
        this.state = Ajax.ERROR; if (CC.isFunction(onFailure)) {
          if (this.caller) {
            onSuccess.apply(this.caller, [this]);
          } else {
            onSuccess(this);
          }
        } else {
          if (this.caller) {
            if (onFailure) {
              this.caller[onFailure](this);
            }
          } else {
            if (onFailure) {
              if (CC.isString(onFailure)) {
                window[onFailure](this);
              }
            }
          }
        }
      }
      this._close();
    }
  }
  ,

  //get value of <state>....</state>
  getState: function() {
    try {
      var xmlDoc = this.xmlReq.responseXML.documentElement; var state = xmlDoc.getElementsByTagName('state').item(0).firstChild.data; if (state == Ajax.S_OK)return Ajax.S_OK; if (state == Ajax.S_ERROR)return Ajax.S_ERROR; return Ajax.S_NA;
    } catch (e) {
      return Ajax.S_NA;
    }
  }
  ,

  //运行返回内容中的JS,方法返回已剔除JS后的内容.
  invokeScript: function() {
    return eval(this.xmlReq.responseText);
  }
  ,

  //方法先提取JS(如果存在),style(如果存在),将剩下内容放入displayPanel(innerHTML)中,再运行提取的JS,style.
  invokeHtml: function() {
    var cache = []; var ss = this.xmlReq.responseText.stripScript(function(sMatch) {
      cache[cache.length] = sMatch.innerScript();
    }
    );

    cache.join('').execScript();

    cache = []; ss = ss.stripStyle(function(sMatch) {
      cache[cache.length] = sMatch.innerStyle();
    }
    );

    cache.join('').execStyle();

    if (this.displayPanel) {
      this.displayPanel.innerHTML = ss;
    }
    cache = null; ss = null;
  }
};

/**
 * 以下部分来自Prototype库.
 */
var Position =  {
  realOffset: function(element) {
    var valueT = 0, valueL = 0;
    do {
      valueT += element.scrollTop || 0; valueL += element.scrollLeft || 0; element = element.parentNode;
    } while (element); return [valueL, valueT];
  }
  ,

  //get absolute location of element
  cumulate: function(element, offset) {
    var c = 0;
    while (element) {
      c += element[offset]; element = element.offsetParent;
    }
    return c;
  }
  ,

  cumulativeOffset: function(element) {
    var valueT = 0, valueL = 0;
    do {
      valueT += element.offsetTop || 0; valueL += element.offsetLeft || 0; element = element.offsetParent;
    } while (element); 
    return [valueL, valueT];
  }
  ,

  positionedOffset: function(element) {
    var valueT = 0, valueL = 0;
    do {
      valueT += element.offsetTop || 0;
       valueL += element.offsetLeft || 0; element = element.offsetParent;
        if (element) {
        if (element.tagName == 'BODY')break; 
        var p = CC.getStyle(element, 'position'); 
        if (p == 'relative' || p == 'absolute') {
          break;
        }
      }
    } while (element); return [valueL, valueT];
  }
  ,

  //whether hide or not ,get the demensions
  getDimensions: function(element) {
    var display = CC.style(element, 'display'); 
    if (display != 'none' && display != null) // Safari bug
    return  {width: element.offsetWidth, height: element.offsetHeight};

    // All *Width and *Height properties give 0 on elements with display none,
    // so enable the element temporarily
    var els = element.style; 
    var originalVisibility = els.visibility; 
    var originalPosition = els.position; 
    var originalDisplay = els.display; 
    els.visibility = 'hidden'; 
    els.position = 'absolute'; 
    els.display = 'block'; 
    var originalWidth = element.clientWidth; 
    var originalHeight = element.clientHeight; 
    els.display = originalDisplay;
     els.position = originalPosition; 
     els.visibility = originalVisibility; 
     return  {
      width: originalWidth, height: originalHeight
    };
  }
  ,

  makePositioned: function(element) {
    var pos = CC.style(element, 'position'); 
    if (pos == 'static' || !pos) {
      element._madePositioned = true; 
      element.style.position = 'relative';
      // Opera returns the offset relative to the positioning context, when an
      // element is position relative but top and left have not been defined
    }
    return element;
  }
  ,

  undoPositioned: function(element) {
    if (element._madePositioned) {
      delete element._madePositioned; 
      element.style.position = ''; 
      element.style.top = ''; 
      element.style.left = ''; 
      element.style.bottom = ''; 
      element.style.right = '';
    }
    return element;
  }
  ,

  makeClipping: function(element) {
    if (element._overflow) {
      return element;
    }
    element._overflow = element.style.overflow || 'auto'; if ((CC.style(element, 'overflow') || 'visible') != 'hidden') {
      element.style.overflow = 'hidden';
    }
    return element;
  }
  ,

  undoClipping: function(element) {
    if (!element._overflow) {
      return element;
    }
    element.style.overflow = element._overflow == 'auto' ? '' : element._overflow; element._overflow = null; return element;
  }
  ,

  /**
   * fnOnMov : 移动回回调函数fn(ev,moveObj),当返回false不移动对象.
   */
  setDragable: function(dragObj, moveObj, b, fnOnMov, fnOnDrag, fnOnDrog) {
    if (!b) {
      dragObj.onmousedown = dragObj.onmouseup = null; return ;
    }

    var fnMoving = function() {
      var ev = window.event; if (!Event.isLeftClick(ev)) {
        dragObj.onmouseup(); return ;
      }

      if (moveObj && fnOnDrag && !moveObj.__ondraged) {
        fnOnDrag(ev, moveObj); moveObj.__ondraged = true;
      }

      if (fnOnMov) {
        if (!fnOnMov(ev, moveObj)) {
          return false;
        }
      }

      var x = ev.clientX; var y = ev.clientY; var x1 = x - moveObj._x; var y1 = y - moveObj._y; moveObj._x = x; moveObj._y = y;

      moveObj.style.left = moveObj.offsetLeft + x1; moveObj.style.top = moveObj.offsetTop + y1;
    };

    dragObj.onmousedown = function() {
      var ev = window.event; var x = ev.clientX; var y = ev.clientY; moveObj._x = x; moveObj._y = y; window.document.ondragstart = function() {
        window.event.returnValue = false;
      }
      Event.observe(document, "mousemove", fnMoving); Event.observe(document, "selectstart", Event.noSelect);
    };

    dragObj.onmouseup = function() {
      if (moveObj && moveObj.__ondraged) {
        fnOnDrog(window.event, moveObj); moveObj.__ondraged = false;
      }
      window.document.ondragstart = function() {
        window.event.returnValue = true;
      }
      Event.stopObserving(document, "mousemove", fnMoving); Event.stopObserving(document, "selectstart", Event.noSelect);
    };
  }
};

/**
 * 缓存类.
 */
var Cache =  {

  /**某类设置的最大缓存数量.*/
  MAX_ITEM_SIZE: 5,

  /**获取缓存数据的回调函数.*/
  callbacks: [],


  register: function(k, callback) {
    this[k] = [callback, [callback()]];
  }
  ,

  get: function(k) {
    var a = this[k]; var b = this[k][1]; return (b.length == 0) ? (a[0])(): b.shift();
  }
  ,

  put: function(k, v) {
    var c = this[k][1]; if (c.length >= this.MAX_ITEM_SIZE) {
      return ;
    }
    c.push(v);
  }
};

/**
 * 缓存DIV结点,该结点可方便复用其innerHTML功能.
 */
Cache.register('div', function() {
  return CC. $C('DIV')
}

);


//-----Debug
var _debug = true; function assert(b) {
  if (!b && _debug) {
    alert("Assert Failed.\n");
  }
}

var Console =  {
  panel: CC. $C( {
    tagName: 'div', id: 'console', className: 'console', innerHTML: '<div class="bar"><div class="tle">控制台 </div><input title="输入要查看的值." id="_in" class="input" size="15" /></div><div id="_out" title="双击清除." class="out"></div>', ondblclick: function() {
      Console.out.innerHTML = '';
    }
  }
  ),

  trace: function(msg) {
    this.out.appendChild(CC. $C( {
      tagName: 'div', innerHTML: msg
    }
    ));
  }
  ,

  initialize: function() {
    var oThis = this; CC.loadStyle('console', '.console{display:none;over-float-y:auto;left:450px;top:100px;height:400px;background:white;position:absolute;width:350px;border-top:1px solid #CCC;border-bottom:1px solid #CCC;} .console .bar{height:20px;background:blue;padding-top:3px;padding-left:4px;border : 1px solid white; font-size:12px;} .console .tle{float:left;} .console .out {border:1px solid white;padding:2px 3px 2px 3px;font-size:13px;over-flow-y:auto;} .console .bar .input{height:18px;float:right;margin:1px; border:1px solid black; background:white;}');

    Event.addListener('system', 'onload', this, function() {
      document.body.appendChild(this.panel);
    }
    );

    this.out = CC.inspect(this.panel, '_out'); Position.setDragable(this.panel, this.panel, true); this.inval = CC.inspect(this.panel, '_in'); this.inval.onkeydown = function() {
      if (Event.isEnterKey(window.event)) {
        oThis._cal();
      }
    }
  }
  ,

  _cal: function() {
    try {
      var o = eval(this.inval.value); this.trace(this.inval.value + ":"); if (typeof o == 'object') {
        CC.each(o, function(n, v) {
          Console.trace("<b> " + n + " : </b><xmp style='border:1px dotted #CCC;'>" + v + "</xmp>");
        }
        );
      } else {
        Console.trace("<xmp style='border:1px dotted #CCC;'>" + o + "</xmp>");
      }
    } catch (e) {
      Console.trace("<font color='red'>" + e+"</font>");
    }
    this.inval.value = '';
  }
}; Console.initialize();
