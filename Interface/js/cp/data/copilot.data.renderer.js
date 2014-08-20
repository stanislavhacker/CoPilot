/*global copilot */
/**
 * Skin
 */
(function () {

	copilot.data = copilot || {};

	/**
	 * Renderer
	 * @param {copilot.data.Language} language
	 * @param {copilot.data.Skin} skin
	 * @constructor
	 */
	copilot.data.Renderer = function (language, skin) {
		/** @type {copilot.data.Language}*/
		this.language = language;
		/** @type {copilot.data.Skin}*/
		this.skin = skin;
	};

	/**
	 * Render
	 */
	copilot.data.Renderer.prototype.render = function () {
		//title
		window.title = this.language.getString("AppName");
		//header, page
		this.renderHeaderWrapper();
		this.renderPage();
		//banners
		this.renderBanners();
		//featured
		this.renderFeatured();
		//footer
		this.renderCopyright();
	};


//	<div id="header" class="container">
//		<div id="logo">
//			<img src="images/logo.png">
//			<h1><a href="#" data-language="AppName">CoPilot</a></h1>
//		</div>
//		<div id="menu">
//			<ul>
//				<li class="current_page_item"><a href="#" accesskey="1" title="">Homepage</a></li>
//				<li><a href="#" accesskey="2" title="">Our Clients</a></li>
//				<li><a href="#" accesskey="3" title="">About Us</a></li>
//				<li><a href="#" accesskey="4" title="">Careers</a></li>
//				<li><a href="#" accesskey="5" title="">Contact Us</a></li>
//			</ul>
//		</div>
//	</div>
	/**
	 * Render header wrapper
	 */
	copilot.data.Renderer.prototype.renderHeaderWrapper = function () {
		var parent = $('#header-wrapper'),
			header,
			menu,
			logo,
			li,
			ul;

		//clear
		parent.empty();

		header = $('<div id="header" class="container" />');
		logo = $('<div id="logo" />').appendTo(header);

		$('<img src="images/logo.png">').appendTo(logo);
		$('<h1><a href="#" data-language="AppName">' + this.language.getString("AppName") + '</a></h1>').appendTo(logo);

		menu = $('<div id="menu" />').appendTo(header);
		ul = $('<ul></ul>').appendTo(menu);

		li = $('<li><a href="#" accesskey="1" title="">' + this.language.getString("Statistics") + '</a></li>').appendTo(ul);
		li.addClass("current_page_item");

		$('<li><a href="#" accesskey="2" title="">' + this.language.getString("Fuels") + '</a></li>').appendTo(ul);
		$('<li><a href="#" accesskey="3" title="">' + this.language.getString("Repairs") + '</a></li>').appendTo(ul);
		$('<li><a href="#" accesskey="4" title="">' + this.language.getString("Videos") + '</a></li>').appendTo(ul);
		$('<li><a href="#" accesskey="5" title="">' + this.language.getString("Images") + '</a></li>').appendTo(ul);

		//append
		parent.append(header);
	};

	/**
	 * Render page
	 */
	copilot.data.Renderer.prototype.renderPage = function () {
		var parent = $('#page');

		//clear
		parent.empty();

		//TODO
	};

	/**
	 * Render banners
	 */
	copilot.data.Renderer.prototype.renderBanners = function () {
		var parent = $('#banner-wrapper');

		//clear
		parent.empty();

		//TODO
	};

	/**
	 * Render features
	 */
	copilot.data.Renderer.prototype.renderFeatured = function () {
		var parent = $('#featured-wrapper');

		//clear
		parent.empty();

		//TODO
	};


//  <p>Copyright (c) 2013 Sitename.com. All rights reserved. | Photos by <a href="http://fotogrph.com/">Fotogrph</a> | Design by <a href="http://www.freecsstemplates.org/" rel="nofollow">FreeCSSTemplates.org</a>.</p>
	/**
	 * Render copyright
	 */
	copilot.data.Renderer.prototype.renderCopyright = function () {
		var parent = $('#copyright'),
			copyright;

		//clear
		parent.empty();

		copyright = $('<p>' + this.language.getString("Copyright") + ' | ' + this.language.getString("ContactEmail") + ' <a href="mailto:stanislav.hacker@live.com">stanislav.hacker@live.com</a> | ' + this.language.getString("DesignBy") + ' <a href="http://www.freecsstemplates.org/" rel="nofollow">FreeCSSTemplates.org</a>.</p>');

		//append
		copyright.appendTo(parent);
	};

}());