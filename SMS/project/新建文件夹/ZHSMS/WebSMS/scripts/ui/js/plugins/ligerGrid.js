/**
* jQuery ligerUI  1.0.2
* 
* Author leoxie [ gd_star@163.com ] 
* 
*/

if (typeof (LigerUIManagers) == "undefined") LigerUIManagers = {};
(function($)
{
    $.fn.ligerGetGridManager = function()
    {
        return LigerUIManagers[this[0].id + "_Grid"];
    }; 

    $.ligerDefaults = $.ligerDefaults || {};
    $.ligerDefaults.Grid = { 
        title: null, 
        width: 'auto',                          //宽度值
        columnWidth: 120,                      //默认列宽度
        resizable: true,                        //table是否可伸缩
        url: false,                             //ajax url
        usePager: true,                         //是否分页
        page: 1,                                //默认当前页
        total: 1,                               //总页面数
        pageSize: 10,                           //每页默认的结果数
        pageSizeOptions: [10, 20, 30, 40, 50],  //可选择设定的每页结果数
        parms : [],                         //提交到服务器的参数
        columns: [],                          //数据源
        minColToggle: 1,                        //最小显示的列
        dataType: 'server',                     //数据源：本地(local)或(server),本地是将读取p.data
        dataAction: 'server',                    //提交数据的方式：本地(local)或(server),选择本地方式时将在客服端分页、排序
        showTableToggleBtn: false,              //是否显示'显示隐藏Grid'按钮 
        switchPageSizeApplyComboBox : true,     //切换每页记录数是否应用ligerComboBox
        allowAdjustColWidth: true,              //是否允许调整列宽     
        checkbox: false,                         //是否显示复选框
        allowHideColumn: true,                 //是否显示'切换列层'按钮
        enabledEdit: false,                      //是否允许编辑
        isScroll: true,                         //是否滚动
        onDragCol: null,                       //拖动列事件
        onToggleCol: null,                     //切换列事件
        onChangeSort: null,                    //改变排序事件
        onSuccess: null,                       //成功事件
        onDblClickRow:null,                     //双击行事件
        onSelectRow: null,                    //选择行事件
        onUnSelectRow : null,                   //取消选择行事件
        onBeforeCheckRow: null,                 //选择前事件，可以通过return false阻止操作(复选框)
        onCheckRow: null,                    //选择事件(复选框) 
        onBeforeCheckAllRow: null,              //选择前事件，可以通过return false阻止操作(复选框 全选/全不选)
        onCheckAllRow: null,                    //选择事件(复选框 全选/全不选)
        onBeforeShowData:null,                  //显示数据前事件，可以通过reutrn false阻止操作
        onAfterShowData: null,                 //显示完数据事件
        onError: null,                         //错误事件
        onSubmit: null,                         //提交前事件
        dateFormat: 'yyyy-MM-dd',              //默认时间显示格式
        InWindow : true,                        //是否以窗口的高度为准 height设置为百分比时可用
        statusName : '__status',                    //状态名
        method :'post',                         //提交方式
        fixedCellHeight:true,                       //是否固定单元格的高度
        heightDiff : 0,                         //高度补差,当设置height:100%时，可能会有高度的误差，可以通过这个属性调整
        cssClass : null,                    //类名
        root :'Rows',                       //数据源字段名
        record:'Total',                     //数据源记录数字段名
        pageParmName :'page',               //页索引参数名，(提交给服务器)
        pagesizeParmName:'pagesize',        //页记录数参数名，(提交给服务器)
        sortnameParmName:'sortname',        //页排序列名(提交给服务器)
        sortorderParmName:'sortorder',      //页排序方向(提交给服务器)
        onReload : null,                    //刷新事件，可以通过return false来阻止操作
        onToFirst:null,                     //第一页，可以通过return false来阻止操作
        onToPrev:null,                      //上一页，可以通过return false来阻止操作
        onToNext:null,                      //下一页，可以通过return false来阻止操作
        onToLast:null,                      //最后一页，可以通过return false来阻止操作
        allowUnSelectRow : false,           //是否允许反选行
        dblClickToEdit : false,            //是否双击的时候才编辑
        alternatingRow : true,
        mouseoverRowCssClass:'l-grid-row-over',
        enabledSort: true,                      //是否允许排序
        rowAttrRender : null,                  //行自定义属性渲染器(包括style，也可以定义)
        //获取时间
        renderDate: function(value)
        {
            var da;
            if (!value) return null;
            if (typeof value == 'object')
            {
                return value;
            }
            if (value.indexOf('Date') > -1)
            {
                da = eval('new ' + value.replace('/', '', 'g').replace('/', '', 'g'));
            } else
            {
                da = eval('new Date("' + value + '");');
            }
            return da;
        }
    };
    $.ligerDefaults.GridString = {
        errorMessage: '发生错误',
        pageStatMessage: '显示记录从{from}到{to}，总数 {total} 条',
        pageTextMessage: 'Page',
        loadingMessage: '加载中...',
        findTextMessage: '查找',
        noRecordMessage: '没有符合条件的记录存在', 
        isContinueByDataChanged: '数据已经改变,如果继续将丢失数据,是否继续?'
    };
    ///	<param name="$" type="jQuery"></param>
    $.ligerAddGrid = function(grid, p)
    {
        if (grid.usedGrid) return;
        $(grid).hasClass("l-panel") || $(grid).addClass("l-panel");
        var gridhtmlarr = [];
        gridhtmlarr.push("        <div class='l-panel-header'><span class='l-panel-header-text'></span></div>");
        gridhtmlarr.push("                    <div class='l-grid-loading'></div>");
        gridhtmlarr.push("                    <div class='l-grid-editor'></div>");
        gridhtmlarr.push("        <div class='l-panel-bwarp'>");
        gridhtmlarr.push("            <div class='l-panel-body'>");
        gridhtmlarr.push("                <div class='l-grid'>");
        gridhtmlarr.push("                    <div class='l-grid-dragging-line'></div>");
        gridhtmlarr.push("                    <div class='l-grid-popup'><table cellpadding='0' cellspacing='0'><tbody></tbody></table></div>");
        gridhtmlarr.push("                    <div class='l-grid-header'>");
        gridhtmlarr.push("                        <div class='l-grid-header-inner'><table class='l-grid-header-table' cellpadding='0' cellspacing='0'><tbody><tr></tr></tbody></table></div>");
        gridhtmlarr.push("                    </div>"); 
        gridhtmlarr.push("                    <div class='l-grid-body l-scroll'>");
        gridhtmlarr.push("                    </div>"); 
        gridhtmlarr.push("                 </div>");
        gridhtmlarr.push("              </div>");
        gridhtmlarr.push("         </div>");
        gridhtmlarr.push("         <div class='l-panel-bar'>");
        gridhtmlarr.push("            <div class='l-panel-bbar-inner'>");
        gridhtmlarr.push("            <div class='l-bar-group l-bar-selectpagesize'></div>");
        gridhtmlarr.push("                <div class='l-bar-separator'></div>");
        gridhtmlarr.push("                <div class='l-bar-group'>");
        gridhtmlarr.push("                    <div class='l-bar-button l-bar-btnfirst'><span></span></div>");
        gridhtmlarr.push("                    <div class='l-bar-button l-bar-btnprev'><span></span></div>");
        gridhtmlarr.push("                </div>");
        gridhtmlarr.push("                <div class='l-bar-separator'></div>");
        gridhtmlarr.push("                <div class='l-bar-group'><span class='pcontrol'> <input type='text' size='4' value='1' style='width:20px' maxlength='3' /> / <span></span></span></div>");
        gridhtmlarr.push("                <div class='l-bar-separator'></div>");
        gridhtmlarr.push("                <div class='l-bar-group'>");
        gridhtmlarr.push("                     <div class='l-bar-button l-bar-btnnext'><span></span></div>");
        gridhtmlarr.push("                    <div class='l-bar-button l-bar-btnlast'><span></span></div>");
        gridhtmlarr.push("                </div>");
        gridhtmlarr.push("                <div class='l-bar-separator'></div>");
        gridhtmlarr.push("                <div class='l-bar-group'>");
        gridhtmlarr.push("                     <div class='l-bar-button l-bar-btnload'><span></span></div>");
        gridhtmlarr.push("                </div>");
        gridhtmlarr.push("                <div class='l-bar-separator'></div>");
        gridhtmlarr.push("                <div class='l-bar-group l-bar-right'><span class='l-bar-text'></span></div>");
        gridhtmlarr.push("                <div class='l-clear'></div>");
        gridhtmlarr.push("            </div>");
        gridhtmlarr.push("         </div>");
        $(grid).html(gridhtmlarr.join(''));
        var g = {
            //刷新数据
            loadData: function(loadService)
            {
                //参数初始化
                if (!p.newPage) p.newPage = 1;
                if (p.dataAction == "server")
                {
                    if (!p.sortOrder) p.sortOrder = "asc";
                }
                var param = [];
                if(p.parms && p.parms.length)
                {
                    $(p.parms).each(function()
                    {
                        param.push({ name: this.name, value: this.value });
                    });
                } 
                if (p.dataAction == "server")
                {
                    if (p.usePager)
                    {
                        param.push({ name: p.pageParmName, value: p.newPage });
                        param.push({ name: p.pagesizeParmName, value: p.pageSize });
                    }
                    if (p.sortName)
                    {
                        param.push({ name: p.sortnameParmName, value: p.sortName });
                        param.push({ name: p.sortorderParmName, value: p.sortOrder });
                    }
                }; 
                //loading状态 
                g.gridloading.show();
                $(".l-bar-btnload span",g.toolbar).addClass("l-disabled");
                this.loading = true;
                if (p.dataType == "local")
                {
                    if (!g.data) g.data = $.extend({}, p.data); 
                    if (p.usePager)
                        g.currentData = g.getCurrentPageData(g.data);
                    else
                    {
                        g.currentData = $.extend({}, g.data);
                    }
                    g.showData(g.currentData);
                } else if (p.dataAction == "local" && g.data && g.data.Rows && g.data.Rows.length && !loadService)
                {
                    g.currentData = g.getCurrentPageData(g.data);
                    g.showData(g.currentData);
                }
                else
                {  
                    //请求服务器
                    $.ajax({
                        type: p.method,
                        url: p.url,
                        data: param,
                        async:false,
                        dataType: 'json', 
                        success: function(data)
                        {  
                            g.data = $.extend({}, data);
                            if (p.dataAction == "server")
                            {
                                g.currentData = g.data;  
                                g.showData(g.currentData);   
                            } else
                            {
                                g.currentData = g.getCurrentPageData(g.data);
                                g.showData(g.currentData);
                            }
                        },
                        error: function(XMLHttpRequest, textStatus, errorThrown) 
                        {   
                            g.gridloading.hide();
                            $(".l-bar-btnload span",g.toolbar).removeClass("l-disabled");
                            try { if (p.onError) p.onError(XMLHttpRequest, textStatus, errorThrown); } catch (e) { } 
                        }
                    });
                }
            },
            setOptions:function(parms)
            { 
                $.extend(p, parms);
                if(parms.data)
                {
                    g.data = $.extend({}, parms.data);
                    p.dataType = "local";
                }
            },
            showData: function(data)
            {  
                //加载中
                $('.l-bar-btnloading:first', this.toolbar).removeClass('l-bar-btnloading'); 
                g.gridloading.hide();
                if(!data || !data[p.root]) return ; 
                if (p.onBeforeShowData && p.onBeforeShowData(grid,data) == false)
                {
                    return false;
                }
                g.isDataChanged = false; 
                $(".l-bar-btnload:first span",g.toolbar).removeClass("l-disabled");
                this.loading = false;
                if (p.usePager)
                {
                    //更新分页
                    p.total = data[p.record];
                    p.page = p.newPage;
                    p.pageCount = Math.ceil(p.total / p.pageSize);
                    this.buildPager();
                }
                //清空数据
                g.gridbody.html("");
                //$(".l-grid-row,.l-grid-detailpanel,.l-grid-totalsummary", g.gridbody).remove();
                //加载数据 
                var gridhtmlarr = ['<div class="l-grid-body-inner"><table class="l-grid-body-table" cellpadding=0 cellspacing=0><tbody>']; 
                var rowlenth = data[p.root].length;  
                $(data[p.root]).each(function(i, item)
                {
                    if (!item) return;
                    if (!p.usePager && i == rowlenth - 1 && !g.isTotalSummary()) 
                        gridhtmlarr.push('<tr class="l-grid-row l-grid-row-last');
                    else
                        gridhtmlarr.push('<tr class="l-grid-row');
                    if(i%2 == 1 && p.alternatingRow)
                        gridhtmlarr.push(' l-grid-row-alt'); 
                    gridhtmlarr.push('" '); 
                    if(p.rowAttrRender) gridhtmlarr.push(p.rowAttrRender(item,i));
                    gridhtmlarr.push(' rowindex="' + i + '">');
                    $(g.headers).each(function(headerCellIndex,headerInfor)
                    {  
                        //如果是复选框(系统列)
                        if(this.ischeckbox)
                        {
                            gridhtmlarr.push('<td class="l-grid-row-cell l-grid-row-cell-checkbox" style="width:'+this.width+'px"><div class="l-grid-row-cell-inner"><span class="l-grid-row-cell-btn-checkbox"></span></div></td>'); 
                            return;
                        }
                        //如果是明细列(系统列)
                        else if (this.isdetail)
                        { 
                            gridhtmlarr.push('<td class="l-grid-row-cell l-grid-row-cell-detail" style="width:'+this.width+'px"><div class="l-grid-row-cell-inner"><span class="l-grid-row-cell-detailbtn"></span></div></td>'); 
                            return;
                        }
                        var column = p.columns[this.columnindex];
                        var colwidth = this.width;
                        if (!this.islast) 
                            gridhtmlarr.push('<td class="l-grid-row-cell" columnindex="' + this.columnindex + '" ');
                        else
                            gridhtmlarr.push('<td class="l-grid-row-cell l-grid-row-cell-last" columnindex="' + this.columnindex + '" ');
                        if(this.columnname) gridhtmlarr.push('columnname="'+this.columnname+'"'); 
                        gridhtmlarr.push(' style = "'); 
                        gridhtmlarr.push('width:'+ colwidth +'px" ');  
                        if(p.fixedCellHeight)
                            gridhtmlarr.push('><div class="l-grid-row-cell-inner l-grid-row-cell-inner-fixedheight" ');
                        else
                            gridhtmlarr.push('><div class="l-grid-row-cell-inner" '); 
                        gridhtmlarr.push(' style = "width:'+parseInt(colwidth-8)+'px; '); 
                        if(column && column.align) gridhtmlarr.push('text-align:'+column.align+';'); 
                        
                        var content = '';
                         
                        if (column && column.render)
                        {
                            content = column.render(item, i,item[this.columnname]);
                        }
                        else if (this.columnname)
                        {
                            if (column.type && column.type == "date")
                            {
                                var date = p.renderDate(item[this.columnname]);
                                item[this.columnname] = date;
                                if (date != null)
                                {
                                    if (column.format) content = g.getFormatDate(date, column.format);
                                    else content = g.getFormatDate(date, p.dateFormat);
                                }
                            }
                            else
                            {
                                content = item[this.columnname];
                            }
                        } 
                        gridhtmlarr.push('">'+content + '</div></td>');     
                    });
                    gridhtmlarr.push('</tr>'); 
                });
                gridhtmlarr.push('</tbody></table></div>');
                g.gridbody.html(gridhtmlarr.join('')); 
                //创建汇总行
                g.bulidTotalSummary(); 

                $("> div:first",g.gridbody).width(g.gridtablewidth);

                g.onResize();

                //表体 - 行经过事件
                $("tbody:first > .l-grid-row", g.gridbody).each(function(){g.setRowEven(this);});
                if (p.onAfterShowData)
                {
                    p.onAfterShowData(grid,data);
                }
            },
            setRowEven : function(rowobj)
            { 
                $(rowobj).hover(function(e)
                {  
                    if(!p.mouseoverRowCssClass)
                         $(this).addClass(p.mouseoverRowCssClass); 

                }, function(e)
                {
                    if(!p.mouseoverRowCssClass)
                         $(this).removeClass(p.mouseoverRowCssClass);
                }).click(function(e)
                { 
                    if(p.checkbox)
                    {
                        var row = $(this);
                        var index = row.attr('rowindex');
                        var uncheck = row.hasClass("l-checked");
                        if(p.onBeforeCheckRow)
                        {
                        if(p.onBeforeCheckRow(!uncheck,g.getRowByRowIndex(index),row,index) == false) return false;
                        }
                        if(uncheck)
                           row.removeClass("l-checked");
                        else
                           row.addClass("l-checked"); 
                        p.onCheckRow && p.onCheckRow(!uncheck,g.getRowByRowIndex(index),row,index);
                        return ;
                    }
                    var index = $(this).attr('rowindex'); 
                    if ($(this).hasClass("l-selected"))
                    {
                        if(!p.allowUnSelectRow)
                        {
                            $(this).addClass("l-selected-again");
                             return ;
                        }
                        $(this).removeClass("l-selected l-selected-again");
                        if(p.onUnSelectRow)
                        { 
                            p.onUnSelectRow(g.getRowByRowIndex(index),this,index);
                        }
                    }
                    else
                    {
                        $(this).siblings(".l-selected").each(function(){
                            if(p.allowUnSelectRow || $(this).hasClass("l-selected-again"))
                                g.endEdit();
                            $(this).removeClass("l-selected l-selected-again");
                        });
                        $(this).addClass("l-selected");
                        if(p.onSelectRow)
                        { 
                            p.onSelectRow(g.getRowByRowIndex(index),this,index);
                        }
                    } 
                    
                }).dblclick(function(){
			        var index = $(this).attr('rowindex');  
			        if (p.onDblClickRow){
				        p.onDblClickRow(this, index , g.getRowByRowIndex(index));
			        }
		        }); 
            },
            applyEditor: function(obj)
            { 
                if (obj.href || obj.type) return true;
                var rowcell;
                if ($(obj).hasClass("l-grid-row-cell")) rowcell = obj;
                else if ($(obj).parent().hasClass("l-grid-row-cell")) rowcell = $(obj).parent()[0];
                if (!rowcell) return;
                var row = $(rowcell).parent();
                var rowindex = $(row).attr("rowindex");
                var columnindex = $(rowcell).attr("columnindex");
                var columnname = $(rowcell).attr("columnname");
                var column = p.columns[columnindex];
                var left = $(rowcell).offset().left - g.body.offset().left;
                var top = $(rowcell).offset().top - $(grid).offset().top;
                var rowdata = g.getRowByRowIndex(rowindex);
                var currentdata = rowdata[columnname]; 
                g.grideditor.css({ left: left, top: top, width: $(rowcell).css('width'), height: $(rowcell).css('height') }).html("");
                g.grideditor.editingCell = null;
                if (column.editor && column.editor.type == 'date')
                { 
                    var $inputText = $("<input type='text'/>");
                    g.grideditor.append($inputText);
                    $inputText.val(g.getFormatDate(currentdata, p.dateFormat));
                    $inputText.ligerDateEditor(
                            {
                                width: $(rowcell).width(),
                                onChangeDate: function(newValue)
                                {
                                    g.grideditor.editingValue = newValue;
                                    $(rowcell).addClass("l-grid-row-cell-edited");
                                    $(obj).html(newValue);
                                    g.updateData(rowcell, newValue);

                                }
                            }
                             );
                    g.grideditor.editingCell = rowcell;
                    g.grideditor.show();
                }
                else if (column.editor && column.editor.type == 'select')
                {
                    var $inputText = $("<input type='text'/>");
                    g.grideditor.append($inputText);
                    $inputText.val(currentdata);
                    var options = {
                        width: $(rowcell).width(),
                        data: column.editor.data,
                        isMultiSelect: false,
                        onSelected: function(newValue, newText)
                        {
                            g.grideditor.editingValue = newValue;
                            $(rowcell).addClass("l-grid-row-cell-edited");
                            if (column.editor.valueColumnName && columnname)
                                g.currentData[p.root][rowindex][columnname] = newText;
                            g.updateData(rowcell, newValue);
                            if (column.editor.render)
                                $(obj).html(column.editor.render(g.currentData[p.root][rowindex]));
                            else
                                $(obj).html(newText);
                        }
                    };
                    if (column.editor.dataValueField) options.valueField = column.editor.dataValueField;
                    if (column.editor.dataDisplayField) options.displayField = options.textField = column.editor.dataDisplayField;
                    if (column.editor.valueColumnName)
                        options.initValue = g.currentData[p.root][rowindex][column.editor.valueColumnName];
                    else if (columnname)
                        options.initText = g.currentData[p.root][rowindex][columnname];
                    $inputText.ligerComboBox(options);
                    g.grideditor.editingCell = rowcell;
                    g.grideditor.show();
                }
                else if (column.editor && column.editor.type == 'int')
                {
                    var $inputText = $("<input type='text'/>");
                    g.grideditor.append($inputText);
                    $inputText.attr({ style: 'border:#6E90BE' }).val(currentdata);
                    $inputText.ligerSpinner(
                            {
                                width: $(rowcell).width(),
                                height: $(rowcell).height(),
                                type: 'int',
                                onChangeValue: function(newValue)
                                {
                                    g.grideditor.editingValue = newValue;
                                    $(rowcell).addClass("l-grid-row-cell-edited");
                                    $(obj).html(newValue);
                                    g.updateData(rowcell, newValue);
                                }
                            }
                             );
                    g.grideditor.editingCell = rowcell;
                    g.grideditor.show();
                }
                else if (column.editor && (column.editor.type == 'string' || column.editor.type == 'text'))
                {  
                    var $inputText = $("<input type='text' class='l-text-editing'/>");
                    g.grideditor.append($inputText);
                    $inputText.val(currentdata);
                    $inputText.ligerTextBox(
                            {
                                width: $(rowcell).width()-1,
                                 height: $(rowcell).height(),
                                onChangeValue: function(newValue)
                                {
                                    g.grideditor.editingValue = newValue;
                                    $(rowcell).addClass("l-grid-row-cell-edited"); 
                                    g.updateData(rowcell, newValue);
                                    if (column.render)
                                        $(obj).html(column.render(g.currentData[p.root][rowindex],rowindex,g.currentData[p.root][rowindex][columnname]));
                                    else
                                        $(obj).html(newValue);
                                }
                            }
                    ).bind('keydown', function (e) {
                        var key = e.which;
                        if (key == 13) {
                            $inputText.trigger("change");
                            g.endEdit();
                        }
                    }); 
                    $inputText.parent().addClass("l-text-editing");
                    g.grideditor.editingCell = rowcell;
                    g.grideditor.show();
                }
                else if (column.editor && (column.editor.type == 'chk' || column.editor.type == 'checkbox'))
                {
                    var $input = $("<input type='checkbox'/>");
                    g.grideditor.append($input); 
                    $input[0].checked = currentdata==1 ? true : false;
                    $input.ligerCheckBox();
                    $input.change(function(){
                        g.updateData(rowcell, this.checked?1:0);
                        if (column.render)
                            $(obj).html(column.render(g.currentData[p.root][rowindex],rowindex,g.currentData[p.root][rowindex][columnname]));
                        else 
                             $(obj).html(this.checked?'Y':'N');
                    });
                    g.grideditor.editingCell = rowcell;
                    g.grideditor.show();
                }
            },
            endEdit:function(){
                var cell = g.grideditor.editingCell; 
                var value = g.grideditor.editingValue; 
                g.grideditor.html("").hide();
                if (p.onAfterEdit) p.onAfterEdit(value,cell);
            },
            getFormatDate: function(date, dateformat)
            {
                if (date == "NaN") return null;
                var format = dateformat;
                var o = {
                    "M+": date.getMonth() + 1,
                    "d+": date.getDate(),
                    "h+": date.getHours(),
                    "m+": date.getMinutes(),
                    "s+": date.getSeconds(),
                    "q+": Math.floor((date.getMonth() + 3) / 3),
                    "S": date.getMilliseconds()
                }
                if (/(y+)/.test(format))
                {
                    format = format.replace(RegExp.$1, (date.getFullYear() + "")
            .substr(4 - RegExp.$1.length));
                }
                for (var k in o)
                {
                    if (new RegExp("(" + k + ")").test(format))
                    {
                        format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k]
                : ("00" + o[k]).substr(("" + o[k]).length));
                    }
                }
                return format;
            },
            deleteSelectedRow: function()
            {
                var row = $(".l-selected", g.gridbody);
                g.deleteRow(row);
            },
            deleteRow: function(row)
            {
                g.popup.hide();
                g.endEdit();
                var rowindex = row.attr("rowindex");
                $(row).remove();
                g.deleteData(rowindex);
                g.isDataChanged = true;
            },
            deleteData: function(rowindex)
            {
                g.currentData[p.root][rowindex][p.statusName] = 'delete';
            },
            updateData: function(cell, value)
            {
                var columnindex = $(cell).attr("columnindex");
                var column = p.columns[columnindex];
                var columnname = column.name;
                var row = $(cell).parents(".l-grid-row:eq(0)");
                var rowindex = row.attr("rowindex");
                if (column.type && column.type == 'int')
                    g.currentData[p.root][rowindex][columnname] = parseInt(value);
                else if (column && column.editor && column.editor.type == 'select')
                    g.currentData[p.root][rowindex][column.editor.valueColumnName ? column.editor.valueColumnName : columnname] = value;
                else
                    g.currentData[p.root][rowindex][columnname] = value;
                if (g.currentData[p.root][rowindex][p.statusName] == undefined)
                    g.currentData[p.root][rowindex][p.statusName] = 'update';
                g.isDataChanged = true;
            },
            addRow: function()
            { 
                var rowindex = g.currentData[p.root].length;
                g.currentData[p.root][rowindex] = {};
                var rowdata = g.currentData[p.root][rowindex];
                if (!p.usePager && !g.isTotalSummary())
                    $("tbody:first > .l-grid-row:last", g.gridbody).removeClass("l-grid-row-last");
                var row = $("<tr class='l-grid-row' rowindex='" + rowindex + "'></tr>");
                if($("tbody",g.gridbody).length==0)
                {
                    g.gridbody.html('<div class="l-grid-body-inner"><table class="l-grid-body-table" cellpadding=0 cellspacing=0><tbody></tbody></table></div>');
                }
                $("tbody:first",g.gridbody).append(row); 
                if (!p.usePager && !g.isTotalSummary())
                    row.addClass("l-grid-row-last");
                var celllength = $("tr > .l-grid-hd-cell", g.gridheader).length;
                $("tr > .l-grid-hd-cell", g.gridheader).each(function(headerCellIndex, headerCell)
                { 
                    var columnname = $(headerCell).attr("columnname");
                    var columnindex = $(headerCell).attr("columnindex");
                    var column = p.columns[columnindex];
                    var rowCell = $("<td class='l-grid-row-cell' columnindex='" + columnindex + "'><div class='l-grid-row-cell-inner'></div></td>");
                    if (celllength == headerCellIndex + 1) rowCell.addClass("l-grid-row-cell-last");
                    if (columnname)
                    {
                        rowdata[columnname] = "";
                        if (column.type && column.type == 'int') rowdata[columnname] = 0;
                        rowCell.attr({ columnname: columnname });
                    }
                    $(".l-grid-row-cell-inner", rowCell).html(rowdata[columnname]);
                    row.append(rowCell);
                    rowCell.css('width', $(headerCell).css('width'));
                    if (column.align) $(".l-grid-row-cell-inner", rowCell).css({ textAlign: column.align });
                    if ($(headerCell).is(":visible") == false) rowCell.hide();
                });
                rowdata[p.statusName] = 'add';
                //添加事件
                g.setRowEven(row[0]);
                //标识状态
                g.isDataChanged = true;
            },
            getData: function()
            {
                if (g.currentData == null) return null;
                return g.currentData[p.root];
            },
            getCurrentPageData: function(jsonObj)
            {
                var data = {
                    Rows: new Array()
                };
                if(!jsonObj || !jsonObj[p.root] || !jsonObj[p.root].length) 
                {
                    data[p.record] = 0;
                    return data;
                }
                data[p.record] = jsonObj[p.root].length ? jsonObj[p.root].length : 0;
                if (!p.newPage) p.newPage = 1;
                for (i = (p.newPage - 1) * p.pageSize; i < jsonObj[p.root].length && i < p.newPage * p.pageSize; i++)
                {
                    var obj = $.extend({}, jsonObj[p.root][i]);
                    data[p.root].push(obj);
                }
                return data;
            },
            getColumn: function(columnname)
            {
                for (i = 0; i < p.columns.length; i++)
                {
                    if (p.columns[i].name == columnname)
                    {
                        return p.columns[i];
                    }
                }
                return null;
            },
            getColumnType: function(columnname)
            {
                for (i = 0; i < p.columns.length; i++)
                {
                    if (p.columns[i].name == columnname)
                    {
                        if (p.columns[i].type) return p.columns[i].type;
                        return "string";
                    }
                }
                return null;
            },
            //比较某一列两个数据
            compareData: function(data1, data2, columnName, columnType)
            {
                switch (columnType)
                {
                    case "int":
                        return parseInt(data1[columnName]) < parseInt(data2[columnName]) ? -1 : parseInt(data1[columnName]) > parseInt(data2[columnName]) ? 1 : 0;
                    case "float":
                        return parseFloat(data1[columnName]) < parseFloat(data2[columnName]) ? -1 : parseFloat(data1[columnName]) > parseFloat(data2[columnName]) ? 1 : 0;
                    case "string":
                        return data1[columnName].localeCompare(data2[columnName]);
                    case "date":
                        return data1[columnName] < data2[columnName] ? -1 : data1[columnName] > data2[columnName] ? 1 : 0;
                }
                return data1[columnName].localeCompare(data2[columnName]);
            },
            //是否包含汇总
            isTotalSummary: function()
            {
                for (var i = 0; i < p.columns.length; i++)
                {
                    if (p.columns[i].totalSummary) return true;
                }
                return false;
            },
            bulidTotalSummary: function()
            {
                if (!g.isTotalSummary()) return false;
                if (!g.currentData || g.currentData[p.root].length == 0) return false;
                if (g.gridbody.totalsummary) g.gridbody.totalsummary.remove();
                g.gridbody.totalsummary = $("<tr class='l-grid-totalsummary'></tr>");
                if (!p.usePager) g.gridbody.totalsummary.addClass("l-grid-totalsummary-nobottom");
                $("tbody:first",g.gridbody).append(g.gridbody.totalsummary);
                $(g.headers).each(function()
                {
                    var cell = $("<td class='l-grid-totalsummary-cell'><div class='l-grid-totalsummary-cell-inner'></div></td>");
                    g.gridbody.totalsummary.append(cell);
                    cell.css('width', this.width); 
                    if(this.islast) 
                       cell.addClass("l-grid-totalsummary-cell-last");
                    columnname = this.columnname;
                    columnindex = this.columnindex; 
                    if (columnname)
                    {
                        $("div:first",cell).attr({ columnname: columnname });
                    }
                    if (!columnindex) return;
                    cell.attr({ columnindex: columnindex });
                    var column = p.columns[columnindex];
                    if (column.align) 
                        $(".l-grid-totalsummary-cell-inner", cell).css({ textAlign: column.align });
                    if (column.totalSummary)
                    {
                        var isExist = function(type)
                        {
                            for (var i = 0; i < types.length; i++)
                                if (types[i].toLowerCase() == type.toLowerCase()) return true;
                            return false;
                        };
                        var sum = 0, count = 0, avg = 0;
                        var max = parseFloat(g.currentData[p.root][0][column.name]);
                        var min = parseFloat(g.currentData[p.root][0][column.name]);
                        for (var i = 0; i < g.currentData[p.root].length; i++)
                        {
                            var value = parseFloat(g.currentData[p.root][i][column.name]);
                            sum += value;
                            count += 1;
                            if (value > max) max = value;
                            if (value < min) min = value;
                        }
                        avg = sum * 1.0 / g.currentData[p.root].length;
                        if(column.totalSummary.render)
                        { 
                            var renderhtml = column.totalSummary.render({
                                sum:sum,
                                count:count,
                                avg:avg,
                                min:min,
                                max:max
                             },column,cell); 
                             $(".l-grid-totalsummary-cell-inner:first", cell).append(renderhtml);
                        }
                        else if (column.totalSummary.type)
                        {
                            var types = column.totalSummary.type.split(','); 
                            if (isExist('sum'))
                                $(".l-grid-totalsummary-cell-inner:first", cell).append("<div>Sum=" + sum.toFixed(2) + "</div>");
                            if (isExist('count'))
                                $(".l-grid-totalsummary-cell-inner:first", cell).append("<div>Count=" + count + "</div>");
                            if (isExist('max'))
                                $(".l-grid-totalsummary-cell-inner:first", cell).append("<div>Max=" + max.toFixed(2) + "</div>");
                            if (isExist('min'))
                                $(".l-grid-totalsummary-cell-inner:first", cell).append("<div>Min=" + min.toFixed(2) + "</div>");
                            if (isExist('avg'))
                                $(".l-grid-totalsummary-cell-inner:first", cell).append("<div>Avg=" + avg.toFixed(2) + "</div>");
                        }
                        if(column.totalSummary.align)
                        {
                            $(".l-grid-totalsummary-cell-inner:first", cell).css("textAlign",column.totalSummary.align);
                        }
                    }

                });
            },
            //改变排序
            changeSort: function(columnName, sortOrder)
            {
                if (this.loading) return true;
                if (p.dataAction == "local")
                {
                    var columnType = g.getColumnType(columnName);
                    if (!g.sortedData)
                        g.sortedData = $.extend({}, g.data);
                    if (p.sortName == columnName)
                    {
                        g.sortedData[p.root].reverse();
                    } else
                    {
                        g.sortedData[p.root].sort(function(data1, data2)
                        {
                            return g.compareData(data1, data2, columnName, columnType);
                        });
                    }
                    if (p.usePager)
                        g.currentData = g.getCurrentPageData(g.sortedData);
                    else
                        g.currentData = g.sortedData;
                    g.showData(g.currentData);
                }
                p.sortName = columnName;
                p.sortOrder = sortOrder;
                if (p.dataAction == "server")
                {
                    g.loadData();
                }
            },
            //改变分页
            changePage: function(ctype)
            {
                if (this.loading) return true;
                if (g.isDataChanged && !confirm(p.isContinueByDataChanged))
                    return false;
                //计算新page
                switch (ctype)
                {
                    case 'first': if (p.page == 1) return; p.newPage = 1; break;
                    case 'prev': if (p.page == 1) return; if (p.page > 1) p.newPage = parseInt(p.page) - 1; break;
                    case 'next': if (p.page >= p.pageCount) return; p.newPage = parseInt(p.page) + 1; break;
                    case 'last': if (p.page >= p.pageCount) return; p.newPage = p.pageCount; break;
                    case 'input':
                        var nv = parseInt($('.pcontrol input', this.toolbar).val());
                        if (isNaN(nv)) nv = 1;
                        if (nv < 1) nv = 1;
                        else if (nv > p.pageCount) nv = p.pageCount;
                        $('.pcontrol input', this.toolbar).val(nv);
                        p.newPage = nv;
                        break;
                }
                if (p.newPage == p.page) return false; 
                if(p.newPage==1)
                {
                     $(".l-bar-btnfirst span",g.toolbar).addClass("l-disabled");
                     $(".l-bar-btnprev span",g.toolbar).addClass("l-disabled");
                }
                else
                {
                    $(".l-bar-btnfirst span",g.toolbar).removeClass("l-disabled");
                     $(".l-bar-btnprev span",g.toolbar).removeClass("l-disabled");
                }
                if(p.newPage == p.pageCount)
                {
                    $(".l-bar-btnlast span",g.toolbar).addClass("l-disabled");
                     $(".l-bar-btnnext span",g.toolbar).addClass("l-disabled");
                }
                else
                {
                    $(".l-bar-btnlast span",g.toolbar).removeClass("l-disabled");
                     $(".l-bar-btnnext span",g.toolbar).removeClass("l-disabled");
                }
                if (p.onChangePage)
                    p.onChangePage(p.newPage);
                if (p.dataAction == "server")
                {
                    this.loadData();
                }
                else
                {
                    g.currentData = g.getCurrentPageData(g.data);
                    g.showData(g.currentData);
                }
            },
            buildPager: function()
            {
                $('.pcontrol input', this.toolbar).val(p.page);
                $('.pcontrol span', this.toolbar).html(p.pageCount);
                var r1 = parseInt((p.page - 1) * p.pageSize) + 1.0;
                var r2 = parseInt(r1) + parseInt(p.pageSize) - 1;
                if (p.total < r2) r2 = p.total;
                var stat = p.pageStatMessage;
                stat = stat.replace(/{from}/, r1);
                stat = stat.replace(/{to}/, r2);
                stat = stat.replace(/{total}/, p.total);
                $('.l-bar-text', this.toolbar).html(stat);
                if(p.page==1)
                {
                     $(".l-bar-btnfirst span",g.toolbar).addClass("l-disabled");
                     $(".l-bar-btnprev span",g.toolbar).addClass("l-disabled");
                }
                else
                {
                    $(".l-bar-btnfirst span",g.toolbar).removeClass("l-disabled");
                     $(".l-bar-btnprev span",g.toolbar).removeClass("l-disabled");
                }
                if(p.page == p.pageCount)
                {
                    $(".l-bar-btnlast span",g.toolbar).addClass("l-disabled");
                     $(".l-bar-btnnext span",g.toolbar).addClass("l-disabled");
                }
                else
                {
                    $(".l-bar-btnlast span",g.toolbar).removeClass("l-disabled");
                     $(".l-bar-btnnext span",g.toolbar).removeClass("l-disabled");
                }
            },
            getCheckedRows: function()
            {
                var rows = $("tbody:first > .l-checked", g.gridbody);
                var rowdata = [];
                $("tbody:first > .l-checked", g.gridbody).each(function(i,row){
                    var rowindex = $(row).attr("rowindex");
                    rowdata.push(g.getRowByRowIndex(parseInt(rowindex)));
                }); 
                return rowdata;
            },
            getSelectedRow: function()
            {
                var row = $("tbody:first > .l-selected", g.gridbody);
                var rowindex = row.attr("rowindex");
                return g.getRowByRowIndex(parseInt(rowindex));
            },
            getRowByRowIndex: function(rowindex)
            {
                if (g.currentData == null) return null;
                return g.currentData[p.root][rowindex];
            },
            onResize: function()
            {
                if (p.height && p.height != 'auto')
                {
                    var windowHeight = $(window).height(); 
                    //if(g.windowHeight != undefined && g.windowHeight == windowHeight) return;
                    
                    var h = 0;
                    var parentHeight = null;
                    if (typeof(p.height) == "string" && p.height.indexOf('%') > 0)
                    { 
                        var gridparent = $(grid).parent(); 
                        if (p.InWindow || gridparent[0].tagName.toLowerCase() == "body") { 
                            parentHeight = windowHeight; 
                            parentHeight -= parseInt($('body').css('paddingTop'));
                            parentHeight -= parseInt($('body').css('paddingBottom'));
                        }
                        else{ 
                            parentHeight = gridparent.height();
                        }  
                        h =  parentHeight * parseFloat(p.height) * 0.01;  
                        if(p.InWindow || gridparent[0].tagName.toLowerCase() == "body") 
                            h -= ($(grid).offset().top - parseInt($('body').css('paddingTop')));
                    } 
                    else
                    { 
                        h = parseInt(p.height);
                    }   
                   
                    h += p.heightDiff; 
                    g.windowHeight = windowHeight;
                    g.setHeight(h);
                } 
            },
            setHeight : function(h)
            {  
                if(p.title) h -= 24;
                if(p.usePager) h -= 32;
                h -= 22;   
                h>0 && g.gridbody.height(h);
            },
            dragStart: function(dragtype, e, toDragHeaderIndex)
            {
                if (dragtype == 'colresize') //列宽调整
                { 
                    var columnindex = g.headers[g.toDragHeaderIndex].columnindex;
                    var width = g.headers[g.toDragHeaderIndex].width; 
                    if (columnindex == undefined) return; 
                    g.colresize = { startX: e.pageX, width: width, columnindex: columnindex };
                    $('body').css('cursor', 'e-resize');
                    g.draggingline.css({ height: g.body.height(), left: e.pageX - $(grid).offset().left + parseInt(g.body[0].scrollLeft), top: 0 }).show();

                    $('body').bind('selectstart', function () { return false; });
                }
                $.fn.ligerNoSelect && $('body').ligerNoSelect();
            },
            dragMove: function(e)
            {
                if (g.colresize) //列 调整
                {
                    var diff = e.pageX - g.colresize.startX;
                    var newwidth = g.colresize.width + diff;
                    g.colresize.newwidth = newwidth;
                    $('body').css('cursor', 'e-resize');
                    g.draggingline.css({ left: e.pageX - $(grid).offset().left + parseInt(g.body[0].scrollLeft) });

                    $('body').unbind('selectstart');
                }
            },
            dragEnd: function(e)
            {
                if (g.colresize)
                { 
                    if(g.colresize.newwidth == undefined){
                        $('body').css('cursor', 'default');
                         return false;
                    }
                    var mincolumnwidth = 80;
                    var columnindex = g.colresize.columnindex;
                    var column = p.columns[columnindex];
                    if (column && column.minWidth) mincolumnwidth = column.minWidth;
                    var newwidth = g.colresize.newwidth;
                    newwidth = newwidth < mincolumnwidth ? mincolumnwidth : newwidth; 
                    var diff = newwidth - g.colresize.width; 
                    g.headers[g.toDragHeaderIndex].width += diff; 
                    g.gridtablewidth += diff;
                    $("div:first",g.gridheader).width(g.gridtablewidth+40);
                     $("div:first",g.gridbody).width(g.gridtablewidth); 
                    $('.l-grid-hd-cell[columnindex=' + columnindex + ']', this.gridheader).css('width', newwidth);
                    $('tbody:first > .l-grid-row > td[columnindex='+columnindex+'],tbody:first > .l-grid-totalsummary > td[columnindex='+columnindex+']', this.gridbody).each(function(){
                        $(this).css('width', newwidth);
                        $("div:first",this).css('width', newwidth-8);
                    });
                    g.onResize();
                    g.draggingline.hide();
                    g.colresize = false;
                }

                $('body').css('cursor', 'default');
                $.fn.ligerNoSelect && $('body').ligerNoSelect(false);
            },
            onClick: function(e)
            {
                var obj = (e.target || e.srcElement);
                var tagName = obj.tagName.toLowerCase();
                if (g.grideditor.editingCell)
                {
                    if (tagName == 'html' || tagName == 'body' || $(obj).hasClass("l-grid-body") || $(obj).hasClass("l-grid-row"))
                    {
                        g.endEdit();
                    }
                }
                if (p.allowHideColumn)
                {
                    if (tagName == 'html' || tagName == 'body' || $(obj).hasClass("l-grid-body") || $(obj).hasClass("l-grid-row") || $(obj).hasClass("l-grid-row-cell-inner") || $(obj).hasClass("l-grid-header"))
                    {
                        g.popup.hide();
                    }
                }
            },
            toggleCol: function(columnindex, visible)
            {
                var headercell = $(".l-grid-hd-cell[columnindex='" + columnindex + "']", this.gridheader);
                if (!headercell) return;
                if (visible)
                {
                    headercell.show();
                    $(".l-grid-row-cell[columnindex='" + columnindex + "']", this.gridbody).show();
                } else
                {
                    headercell.hide();
                    $(".l-grid-row-cell[columnindex='" + columnindex + "']", this.gridbody).hide();
                }
            }
        };
        //头部
        g.header = $(".l-panel-header:first", grid); 
        //主体
        g.body = $(".l-panel-body:first", grid);
        //底部工具条         
        g.toolbar = $(".l-panel-bar:first", grid);
        //显示/隐藏列      
        g.popup = $(".l-grid-popup:first", grid);
        //编辑层   
        g.grideditor = $(".l-grid-editor:first", grid);
        //加载中
        g.gridloading = $(".l-grid-loading:first", grid);
        //调整列宽层 
        g.draggingline = $(".l-grid-dragging-line", grid);
        //表头     
        g.gridheader = $(".l-grid-header:first", grid);
        //表主体     
        g.gridbody = $(".l-grid-body:first", grid);
        g.currentData = null;
        
        
        p.cssClass && $(grid).addClass(p.cssClass);
        /*--------------------------------
        --------------创建头部------------
        ---------------------------------*/
        if (p.title)
            $(".l-panel-header-text", g.header).html(p.title);
        else
            g.header.hide();
        /*----------------------------------
        --------------创建表头--------------
        ----------------------------------*/
        g.headers = [];
        g.gridtablewidth = 0;
        //如果有复选框列 
        if(p.checkbox)
        {
            var headerCell = $("<td class='l-grid-hd-cell l-grid-hd-cell-checkbox'><div class='l-grid-hd-cell-inner'><div class='l-grid-hd-cell-text l-grid-hd-cell-btn-checkbox'></div></td>");
            headerCell.css({ width: 27 });
            $("tr:first", g.gridheader).append(headerCell);
            g.headers.push({
                width : 27,   
                ischeckbox : true
            });
            g.gridtablewidth += 28;
        }
        //如果有明细，创建列
        if (p.detail && p.detail.onShowDetail)
        {
            var detailHeaderCell = $("<td class='l-grid-hd-cell l-grid-hd-cell-detail'><div class='l-grid-hd-cell-inner'><div class='l-grid-hd-cell-text'></div></td>");
            detailHeaderCell.css({ width: 29 });
            $("tr:first", g.gridheader).append(detailHeaderCell);
            g.headers.push({
                width : 29,   
                isdetail : true
            });
            g.gridtablewidth += 30;
        }  
        
        $(p.columns).each(function(i, item)
        {
            var $headerCell = $("<td class='l-grid-hd-cell' columnindex='" + i + "'><div class='l-grid-hd-cell-inner'><span class='l-grid-hd-cell-text'> </span></div></td>"); 
            if (i == p.columns.length - 1)
            {
                //$(".l-grid-hd-cell-drophandle", $headerCell).remove();
                $headerCell.addClass("l-grid-hd-cell-last");
            }
            if (item.name)
                $headerCell.attr({ columnname: item.name });
            if (item.isSort != undefined)
                $headerCell.attr({ isSort: item.isSort });
            if (item.isAllowHide != undefined) 
                $headerCell.attr({ isAllowHide: item.isAllowHide });
            var headerText = "";
            if (item.display && item.display != "")
                headerText = item.display;
            else if (item.headerRender)
                headerText = item.headerRender(item);
            else
                headerText = "&nbsp;";
            $(".l-grid-hd-cell-text", $headerCell).html(headerText);
            //$headerCell.prepend(headerText);
            $("tr:first", g.gridheader).append($headerCell);
            var colwidth = item.width;
            if (item.minWidth)
            { 
                if (item.width && item.width > item.minWidth)
                    colwidth = item.width;
                else 
                    colwidth = item.minWidth; 
            } else if (item.width)
            {
                colwidth = item.width;
            } else if (p.columnWidth)
            {
                colwidth = p.columnWidth;
            }
            g.gridtablewidth += colwidth+1;
            $headerCell.width(colwidth); 
            g.headers.push({
                width : colwidth,
                columnname : item.name,
                columnindex : i, 
                islast : i == p.columns.length - 1,
                isdetail : false
            }); 
        }); 
        $("div:first",g.gridheader).width(g.gridtablewidth+40);
        //创建 显示/隐藏 列 列表
        $("tr:first .l-grid-hd-cell", g.gridheader).each(function(i, td)
        {
            if ($(this).hasClass("l-grid-hd-cell-detail")) return;
            var isAllowHide = $(this).attr("isAllowHide");
            if (isAllowHide != undefined && isAllowHide.toLowerCase() == "false") return;
            var chk = 'checked="checked"';
            var columnindex = $(this).attr("columnindex");
            var columnname = $(this).attr("columnname");
            if (!columnindex || !columnname) return;
            var header = $(".l-grid-hd-cell-text", this).html();
            if (this.style.display == 'none') chk = '';
            $('tbody', g.popup).append('<tr><td class="l-column-left"><input type="checkbox" ' + chk + ' class="l-checkbox" columnindex="' + columnindex + '"/></td><td class="l-column-right">' + header + '</td></tr>');
        }); 
       $.fn.ligerCheckBox &&  $('input:checkbox', g.popup).ligerCheckBox(
                {
                    onBeforeClick: function(obj)
                    {
                        if (!obj.checked) return true;
                        if ($('input:checked', g.popup).length <= p.minColToggle)
                            return false;
                        return true;
                    }
                });
        //创建 显示/隐藏 列 
        $(".l-grid-hd-cell",g.gridheader).bind("contextmenu",function(e)
                { 
                    if (g.colresize) return true; 
                    if (!p.allowHideColumn) return true;
                    var columnindex = $(this).attr("columnindex");
                    if(columnindex == undefined) return true;
                    var left = (e.pageX - g.body.offset().left + parseInt(g.body[0].scrollLeft)); 
                    if(columnindex== p.columns.length-1) left-= 80;
                    g.popup.css({ left: left, top: g.gridheader.height() + 1 });
                    g.popup.toggle();
                    return false;
                }
        );
        /*----------------------------------
        ----------宽度高度初始化------------
        ----------------------------------*/
        if(p.isScroll == false) p.height = 'auto';
        if (p.height == 'auto')
        {
            g.gridbody.height('auto');
        }
        if (p.width)
        { 
            $(grid).width(p.width);
        } 

        g.onResize();
        /*----------------------------------
        --------------创建表体--------------
        ----------------------------------*/
        g.loadData();
        /*----------------------------------------
        --------------创建底部工具条--------------
        ----------------------------------------*/
        if (p.usePager)
        {
            //创建底部工具条 - 选择每页显示记录数
            var optStr = "";
            var selectedIndex = -1;
            $(p.pageSizeOptions).each(function(i, item)
            {
                var selectedStr = "";
                if (p.pageSize == item) selectedIndex = i;
                optStr += "<option value='" + item + "' " + selectedStr + " >" + item + "</option>";
            });

            $('.l-bar-selectpagesize', g.toolbar).append("<select name='rp'>" + optStr + "</select>");
            if (selectedIndex != -1) $('.l-bar-selectpagesize select', g.toolbar)[0].selectedIndex = selectedIndex;
            if (p.switchPageSizeApplyComboBox && $.fn.ligerComboBox)
            {
                $(".l-bar-selectpagesize select", g.toolbar).ligerComboBox(
                {
                    onBeforeSelect: function()
                    {
                        if (g.isDataChanged && !confirm(p.isContinueByDataChanged))
                            return false;
                        return true;
                    },
                    width: 45
                });
            }
            //创建底部工具条 - 各个按钮状态
            //创建底部工具条 - 当前页数
            //创建底部工具条 - 当前分页信息
        }
        else
        {
            g.toolbar.hide();
        }
        /*----------------------------------
        ------------创建Loading------------
        ----------------------------------*/
        g.gridloading.html(p.loadingMessage);
        /*----------------------------------
        --------------创建事件--------------
        ----------------------------------*/
        g.header.click(function()
        {
            g.popup.hide();
            g.endEdit();
        });
        //表头 - 经过和点击事件
        $(".l-grid-hd-cell", g.gridheader).hover(function()
        { 
        }, function()
        {
        }).mousedown(function(e)
        {
            if(g.colresize) return false; //如果正在调整列宽
        });
        $(".l-grid-hd-cell-text",g.gridheader).click(function(e)
        {
            var obj = (e.target || e.srcElement);
            var row = $(this).parent().parent();
            if (!row.attr("columnname")) return;
            if(g.colresize) return false; //如果正在调整列宽
            if(!p.enabledSort) return ;
            if (row.attr("isSort") != undefined && row.attr("isSort").toLowerCase() == "false") return;  
            if (g.isDataChanged && !confirm(p.isContinueByDataChanged))
                return false;
            var sort = $(".l-grid-hd-cell-sort", row);
            var columnName = $(row).attr("columnname");
            if (sort.length > 0)
            {
                if (sort.hasClass("l-grid-hd-cell-sort-asc"))
                {
                    sort.removeClass("l-grid-hd-cell-sort-asc").addClass("l-grid-hd-cell-sort-desc");
                    row.removeClass("l-grid-hd-cell-asc").addClass("l-grid-hd-cell-desc");
                    g.changeSort(columnName, 'desc');
                }
                else if (sort.hasClass("l-grid-hd-cell-sort-desc"))
                {
                    sort.removeClass("l-grid-hd-cell-sort-desc").addClass("l-grid-hd-cell-sort-asc");
                    row.removeClass("l-grid-hd-cell-desc").addClass("l-grid-hd-cell-asc");
                    g.changeSort(columnName, 'asc');
                }
            }
            else
            {
                row.removeClass("l-grid-hd-cell-desc").addClass("l-grid-hd-cell-asc");
                $(this).after("<span class='l-grid-hd-cell-sort l-grid-hd-cell-sort-asc'>&nbsp;&nbsp;</span>");
                g.changeSort(columnName, 'asc');
            }
            $(".l-grid-hd-cell-sort", row.siblings()).remove();
            return false;
        });
        g.gridheader.click(function()
        {
            g.endEdit();
        });
        //调整列宽
        if (p.allowAdjustColWidth)
        {
            g.gridheader.mousemove(function(e)
            {
                if(g.colresize) return; //如果正在调整列宽
                var posLeft = e.pageX - $(grid).offset().left;//当前鼠标位置
                var currentLeft = 0;  
                for(var i=0;i<g.headers.length;i++)
                { 
                    if(g.headers[i].width) currentLeft+= g.headers[i].width + 1;
                    if(g.headers[i].isdetail || g.headers[i].ischeckbox) continue; 
                    if(posLeft >= currentLeft-2-g.gridbody[0].scrollLeft && posLeft <= currentLeft+2-g.gridbody[0].scrollLeft)
                    { 
                        $('body').css({cursor:'e-resize'});
                        g.toDragHeaderIndex = i;
                        return;
                    }
                }
                $('body').css({cursor:'default'});
                g.toDragHeaderIndex = null; 
            }).mouseout(function(e)
            {
                if(g.colresize) return; //如果正在调整列宽
                $('body').css({cursor:'default'});
            }).mousedown(function(e)
            {
                if(g.colresize) return; //如果正在调整列宽
                if(g.toDragHeaderIndex == null) return ;//如果不在位置上
                g.dragStart('colresize', e, g.toDragHeaderIndex);
            });
        }

        //表头 - 显示/隐藏'列控制'按钮事件
        if (p.allowHideColumn)
        {

            $('tr', g.popup).hover(function() { $(this).addClass('l-popup-row-over'); },
            function() { $(this).removeClass('l-popup-row-over'); });
            var onPopupCheckboxChange = function()
            {
                if ($('input:checked', g.popup).length + 1 <= p.minColToggle)
                {
                    return false;
                }
                g.toggleCol($(this).attr("columnindex"), this.checked);
            };
            if ($.fn.ligerCheckBox)
                $(':checkbox', g.popup).change(onPopupCheckboxChange);
            else
                $(':checkbox', g.popup).click(onPopupCheckboxChange);
        }
        //表头 - 调整列宽层事件
        //表体 - 滚动联动事件
        g.gridbody.scroll(function(){  
           
            var scrollLeft = g.gridbody.scrollLeft();
            if(scrollLeft == undefined) return ; 
            g.gridheader[0].scrollLeft = scrollLeft;
        }); 
        //表体 - 数据 单元格事件
        $(grid).click(function(e){ 
            var obj = (e.target || e.srcElement); 
            if(obj.tagName.toLowerCase()=="span" && $(obj).hasClass("l-grid-row-cell-detailbtn"))
            {    
                var row = $(obj).parent().parent().parent();
                //确保不是在内嵌表格点击的 
                if(row.parent().parent()[0] != $("table:first",g.gridbody)[0]) return ; 
                var rowindex = parseInt($(row).attr("rowindex"));
                var item = g.currentData[p.root][rowindex];
                if ($(obj).hasClass("l-open"))
                { 
                    row.next(".l-grid-detailpanel").remove();
                    $(obj).removeClass("l-open"); 
                }
                else
                { 
                    var detailRow = $("<tr class='l-grid-detailpanel'><td><div class='l-grid-detailpanel-inner' style='display:none'></div></td></tr>"); 
                    var detailRowInner = $("div:first",detailRow);
                    detailRowInner.width(g.gridtablewidth-1);
                    detailRowInner.parent().attr("colSpan",g.headers.length).width(g.gridtablewidth-1);
                    row.after(detailRow);
                    if(p.detail.onShowDetail)
                    {
                        p.detail.onShowDetail(item, detailRowInner[0]);
                        detailRowInner.show();
                    }
                    else if(p.detail.render)
                    {
                        detailRowInner.append(p.detail.render());
                        detailRowInner.show();
                    }
                    $(obj).addClass("l-open");
                } 
                return ;
            }   
            if(obj.tagName.toLowerCase()=="div" && $(obj).hasClass("l-grid-hd-cell-btn-checkbox"))
            {
                var row = $(obj).parent().parent().parent();
                var uncheck = row.hasClass("l-checked");
                if(p.onBeforeCheckAllRow)
                 {
                    if(p.onBeforeCheckAllRow(!uncheck,grid)==false) return false;
                 }
                if(uncheck)
                {
                    row.removeClass("l-checked");
                    $("tbody:first > tr",g.gridbody).removeClass("l-checked");
                }
                else
                {
                    row.addClass("l-checked");
                    $("tbody:first > tr",g.gridbody).addClass("l-checked");
                }
                p.onCheckAllRow && p.onCheckAllRow(!uncheck,grid);
            }
            if(obj.tagName.toLowerCase()=="div" || $(obj).hasClass("l-grid-row-cell-inner") || $(obj).hasClass("l-grid-row-cell"))
            {    
                if (p.enabledEdit && !p.dblClickToEdit)
                { 
                    var row = null;
                    if ($(obj).hasClass("l-grid-row-cell")) row = $(obj).parent();
                    else row = $(obj).parent().parent();
                    //第一次选择的时候不允许编辑，第二次才允许
                    if(p.allowUnSelectRow || row.hasClass("l-selected-again"))
                        g.applyEditor(obj);
                } 
            }
        });
        //工具条 - 切换每页记录数事件
        $('select', g.toolbar).change(function()
        {
            if (g.isDataChanged && !confirm(p.isContinueByDataChanged))
                return false;
            p.newPage = 1;
            p.pageSize = this.value;
            g.loadData();
        });
        //工具条 - 切换当前页事件
        $('.pcontrol input', g.toolbar).keydown(function(e) { if (e.keyCode == 13) g.changePage('input') });
        //工具条 - 按钮事件
        $(".l-bar-button", g.toolbar).hover(function()
        {
            $(this).addClass("l-bar-button-over");
        }, function()
        {
            $(this).removeClass("l-bar-button-over");
        }).click(function()
        {
            if ($(this).hasClass("l-bar-btnfirst"))
            { 
                if(p.onToFirst && p.onToFirst(grid) == false) return false;
                g.changePage('first');
            }
            else if ($(this).hasClass("l-bar-btnprev"))
            {
                if(p.onToPrev && p.onToPrev(grid) == false) return false;
                g.changePage('prev');
            }
            else if ($(this).hasClass("l-bar-btnnext"))
            {
                if(p.onToNext && p.onToNext(grid) == false) return false;
                g.changePage('next');
            }
            else if ($(this).hasClass("l-bar-btnlast"))
            {
                 if(p.onToLast && p.onToLast(grid) == false) return false; 
                g.changePage('last');
            }
            else if ($(this).hasClass("l-bar-btnload"))
            {
                if($("span",this).hasClass("l-disabled")) return false;
                if(p.onReload && p.onReload(grid) == false) return false;
                if (g.isDataChanged && !confirm(p.isContinueByDataChanged))
                    return false;
                g.loadData();
            }
        });
        g.toolbar.click(function()
        {
            g.popup.hide();
            g.endEdit();
        });
        //全局事件
        $(document).mousemove(function(e) { g.dragMove(e) }).mouseup(function(e) { g.dragEnd() }).hover(function() { }, function() { g.dragEnd() }).click(function(e) { g.onClick(e) });
        //$(grid).click(function (e) { g.onClick(e) });
        
        if (grid.id == undefined) grid.id = "LigerUI_" + new Date().getTime();
        LigerUIManagers[grid.id + "_Grid"] = g;
        grid.usedGrid = true;
        
        $(window).resize(function()
        {
            g.onResize();
        });
    };
    $.ligerGridSetParms = function(p, fixedP)
    {
        p = $.extend({}, $.ligerDefaults.Grid,$.ligerDefaults.GridString, p || {});
        if (p.url && p.data)
        {
            p.dataType = "local";
        }
        else if (p.url && !p.data)
        {
            p.dataType = "server";
        }
        else if (!p.url && p.data)
        {
            p.dataType = "local";
        }
        else if (!p.url && !p.data)
        {
            p.dataType = "local";
            p.data = [];
        }
        if (p.dataType == "local")
            p.dataAction = "local";
        if (fixedP)
        {
            p = $.extend(p, fixedP);
        }
        return p;
    };

    $.fn.ligerGrid = function(p)
    {
        var fixedP = {};
        p = p || {};
        p = $.ligerGridSetParms(p, fixedP);
        this.each(function()
        {
            $.ligerAddGrid(this, p);
        });
        if (this.length == 0) return null;
        if (this.length == 1) return $(this[0]).ligerGetGridManager();
        var managers = [];
        this.each(function() {
            managers.push($(this).ligerGetGridManager());
        });
        return managers;
    };
 
   
})(jQuery);