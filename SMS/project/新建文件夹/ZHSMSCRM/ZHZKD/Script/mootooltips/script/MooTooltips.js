/**
 * ToolTips - show tooltips on hover
 * @version		0.1
 * @MooTools version 1.2.1
 * @author Constantin Boiangiu <info [at] constantinb.com>
 */

var MooTooltips = new Class({
	
	Implements: [Options],
	
	options: {		
		container: null,	// hovered elements
		hovered:null,		// the element that when hovered shows the tip
		extra:null,
		ToolTipClass:'ToolTips',	// tooltip display class
		toolTipPosition:1, // -1 top; 1: bottom
		showDelay: 500,
		sticky:false,		// remove tooltip if closed
		fromTop: 0,		// distance from mouse or object
		fromLeft: 0,
		duration: 100,		// fade effect transition duration
		fadeDistance: 20    // the distance the tooltip starts fading in/out
	},
	
	initialize: function(options) {
		this.setOptions(options||null);
		if(!this.options.hovered && !this.options.extra) return;	
		
		if( this.options.hovered )
			this.elements = $(this.options.container||document.body).getElements(this.options.hovered);
		
		if(!$defined(this.elements)) this.elements = new Array();
		
		var e = new Hash(this.options.extra);
		e.each(function(el){
			$(el.id).set( 'rel', JSON.encode(el) );
			this.elements.include($(el.id));			
		},this);
		
		this.currentElement = null;
		this.attach();
	},
	
	attach: function(){
		this.elements.each(function(elem, key){
			var t = new Hash(JSON.decode(elem.getProperty('rel')));
			t.include('visible',0);
			
			var tooltip = this.createContainer(t.sticky||this.options.sticky);
			/* 
				set the tooltip content.  
				depending on where the content is, set the according parameters.
				
				Parameters for every element are:
				- content: element id to get the tip content from a HTML element ( a div within the page for example )
				- text: just input some text directly into the parameter and there you have it
				- ajax: get the content from a remote page				
			*/
			if( t.content )
				tooltip.message.set({'html':$(t.content).get('html')});
			else if( t.text )
				tooltip.message.set({'html':t.text});
			else if( t.ajax ){
				tooltip.message.set({'html':t.ajax_message||'Loading... please wait.'});
				new Element('div', {'class':'loading'}).injectInside(tooltip.message);
				/* the actual ajax call is made when element is hovered */
			}				
			/*
				by default, the tooltip is positioned below the element.
				if placed above, the script switches the CSS classes on the footer and header
				to make it point at the element hovered
			*/
			if( !t.position ) t.position = this.options.toolTipPosition;
			if( t.position == -1 ){
				tooltip.header.set({'class':'dockTopHeader'});
				tooltip.footer.set({'class':'dockTopFooter'});
			}
			
			tooltip.container.store('properties', t);
			elem.store('tip', tooltip.container);
			$(document.body).adopt(tooltip.container);
			elem.removeProperties('title','rel');
			
			var over = this.enter.bindWithEvent(this, elem);
			var out = this.leave.bindWithEvent(this, elem);
			
			var startEvent = t.focus ? 'focus' : 'mouseenter'; 
			var endEvent = t.focus ? 'blur' : 'mouseleave';			
			
			elem.addEvent(startEvent, over);
			if( t.sticky || this.options.sticky){
				tooltip.close.addEvent('click', this.hide.pass(tooltip.container).bind(this)  );
			}
			elem.addEvent(endEvent, out.pass(tooltip.container));				
			
			
		}, this);
	},
	
	enter: function(event, element){
		var tip = element.retrieve('tip');		
		/* all the tip properties are stored on the element */
		var elProperties = tip.retrieve('properties');
		if(elProperties.visible == 1) return;
		
		if( elProperties.ajax && !elProperties.loaded ){
			new Request.HTML({
				url: elProperties.ajax, 
				update: tip.getElement('.message'),
				/* if loading fails, set the loaded propety back to false so when the element is hovered, a new request is made */
				onFailure: function(){
					elProperties.set('loaded',0);
				}
			}).get();
			/* 
				set it as loaded when user hovers the element. 
				This way, while loading, if the user hovers the element again, it will not make a new request 
			*/
			elProperties.set('loaded',1);
		}
		
		/* if property target set on element, show tooltip after target */	
		var showAfter = elProperties.target ? $(elProperties.target) : element;
		var elSize = showAfter.getCoordinates();
		var tipSize = tip.getCoordinates();
		
		this.fromTop = 0;
		if( elProperties.position == -1 )
			this.fromTop = elSize.top - this.options.fromTop - tipSize.height;
		else
			this.fromTop = elSize.top + this.options.fromTop + elSize.height;
		
		var top_dist = this.fromTop + (elProperties.position||this.options.toolTipPosition) * this.options.fadeDistance ;
		
		tip.setStyles({
			'top': top_dist,
			'left': elSize.left + this.options.fromLeft,			
			'z-index':'110000'
		});		
		
		elProperties.set('leave', top_dist);		
		this.currentElement = tip;
		this.timer = $clear(this.timer);
		this.timer = this.show.delay(this.options.showDelay, this);
	},
	
	leave: function(element){
		var elProperties = element.retrieve('properties');
		/* if tooltip is visible and sticky, it closes when close button is clicked */
		if( (elProperties.sticky || this.options.sticky) && elProperties.visible ){
			return;	
		}
		this.hide(element);
	},
	
	hide: function(element){
		this.timer = $clear(this.timer);		
		var elProperties = element.retrieve('properties');
		element.morph({'opacity':0,'top': elProperties.leave});
		elProperties.visible = 0;
	},
	
	show: function(){
		this.currentElement.setStyles({'display':'block','opacity':0,'z-index':100000});
		this.currentElement.morph({'opacity':1, 'top':this.fromTop});		
		//this.setVisible.delay(this.options.duration, this);
		this.setVisible();
	},
	
	setVisible: function(){
		var elProperties = this.currentElement.retrieve('properties');
		elProperties.visible = 1;				
	},
	
	createContainer: function( sticky ){
		var container = new Element('div').set({
			'class':this.options.ToolTipClass, 
			'styles':{ 
				'position':'absolute',
				'top':0,
				'left':0,
				'opacity':0,
				'z-index':'100000' 
			},
			'morph':{
				duration:this.options.duration, 
				link:'cancel', 
				transition:Fx.Transitions.Sine.easeOut
			}
		});
		var header = new Element('div', {'class':'dockBottomHeader'});
		if( sticky ){
			var closeBtn = new Element('div', { 'class':'sticky_close' }).injectInside(header);
		}	
		var message = new Element('div', {'class':'message'});
		var footer = new Element('div', {'class':'dockBottomFooter'});
		container.adopt( header, message, footer );
		
		return {'container':container,'header':header,'message':message,'footer':footer,'close':closeBtn||null};		
	}	
});