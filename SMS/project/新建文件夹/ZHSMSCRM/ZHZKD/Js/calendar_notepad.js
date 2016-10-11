;(function($){
	var CalendarNotepad = function(obj,parameters){
		var setting=$.extend({},$.fn.calendarNotepad.defaults,parameters);
		//默认参数设置
		var Year =setting.Year==0?new Date().getFullYear():setting.Year;
		var Month = setting.Month==0?(new Date().getMonth()+1):setting.Month;
		var SelectDay = setting.SelectDay?new Date(setting.SelectDay):null;
		var onSelectDay = setting.onSelectDay;
		var onToday = setting.onToday;
		var onFinish = setting.onFinish;
		var Days=[];
		
		//当前月
		function m_current(){
			repert(new Date());
		};
		//上一月
		function m_previous(){
			repert(new Date(Year,Month-2,1));
		};
		//下一月
		function m_next(){
			repert(new Date(Year,Month,1));
		}
		//上一年
		function y_previous(){
			repert(new Date(Year-1,Month-1,1));
		}
		//下一年
		function y_next(){
			repert(new Date(Year+1,Month-1,1));
		}
		//重新画日历
		function repert(date){
			Month = date.getMonth()+1;
			Year = date.getFullYear();
			calendar_draw();//日历重画
		};
		//画日历
		function calendar_draw(){
			var date_arr=[];//保存日期列表
			//用当月第一天在一周中的日期值作为当月离第一天的天数
			for(var i = 1, firstDay = new Date(Year, Month - 1, 1).getDay(); i <= firstDay; i++){ date_arr.push(0); }
			//用当月最后一天在一个月中的日期值作为当月的天数
			for(var i = 1, monthDay = new Date(Year, Month, 0).getDate(); i <= monthDay; i++){ date_arr.push(i); }
			//清空原来的日期对象列表
			Days = [];
			//插入日期
			var frag = document.createDocumentFragment();
			while(date_arr.length){
				//每个星期插入一个tr
				var row = $("<tr></tr>");
				//每个星期有7天
				for(var i = 1; i <= 7; i++){
					var cell = $("<td></td>");
					cell.html('&nbsp;');
					if(date_arr.length){
						var d = date_arr.shift();
						if(d){
							cell.html(d);
							Days[d] = cell;
							var on = new Date(Year, Month - 1, d);
							//判断是否今日
							IsSame(on, new Date()) && onToday(cell);
							//判断是否选择日期
							SelectDay && IsSame(on, SelectDay) && onSelectDay(cell);
						}
					}
					row.append(cell);
				}
				frag.appendChild(row.get(0));
			}
			//先清空内容再插入(ie的table不能用innerHTML)
			setting.continer.empty();
			setting.continer.append(frag);
			//附加程序
			onFinish(_finish);
		};
		//判断是否同一日
		  function IsSame(d1, d2) {
			return (d1.getFullYear() == d2.getFullYear() && d1.getMonth() == d2.getMonth() && d1.getDate() == d2.getDate());
		  }; 
		  calendar_draw();
		  function _finish(){
			  $("#idCalendarYear").html(Year); 
			  $("#idCalendarMonth").text(Month); 
			  var flag = [10,12,14,15,20];//此处修改为ajax动态取数据,或者其他异步方式读取数据,返回json格式数据
			  for(var i = 0, len = flag.length; i < len; i++){
			   	Days[flag[i]].html("<a href='javascript:void(0);' class='calendar_open' title=\"'日期是:"+Year+"/"+Month+"/"+flag[i]+"'\">" + flag[i] + "</a>");
			  }
		  }
		  $("#idCalendarPre").bind("click",function(){m_previous();});
		  $("#idCalendarNext").bind("click",function(){m_next();});
		  $("#idCalendarPreYear").bind("click",function(){y_previous();});
		  $("#idCalendarNextYear").bind("click",function(){y_next();});
		  $("#idCalendarNow").bind("click",function(){m_current();});
		  
	};
	$.fn.calendarNotepad=function(parameters){
		new CalendarNotepad($(this),parameters);
	};
	$.fn.calendarNotepad.defaults={
			continer:$("#idCalendar"),//容器对象
			Year:0,//默认当前年
			Month:0,//默认当前月
			SelectDay:null,//选择日期
			onSelectDay:function(){},//选择日期触发
			onToday:function(){},//当前天触发
			onFinish:function(){}//日历完成触发
	};
})(jQuery);