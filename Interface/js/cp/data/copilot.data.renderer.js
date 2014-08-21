/*global copilot */
/**
 * Skin
 */
(function () {

	copilot.data = copilot || {};

	/**
	 * Renderer
	 * @param {copilot.App} copilot
	 * @constructor
	 */
	copilot.data.Renderer = function (copilot) {
		/** @type {copilot.data.Language}*/
		this.language = copilot.language;
		/** @type {copilot.data.Skin}*/
		this.skin = copilot.skin;
		/** @type {copilot.data.Data}*/
		this.data = copilot.data;

		//set renderers
		this.language.renderer = this;
		this.skin.renderer = this;
		this.data.renderer = this;

		this.registerEvents();
	};

	/**
	 * Render
	 */
	copilot.data.Renderer.prototype.render = function () {
		//title
		window.title = this.language.getString("AppName");

		//header, page
		this.renderHeaderWrapper();
		this.renderPageContent();
		//sidebar
		this.renderPageSideBar();
		//banners
		this.renderBanners();
		//featured
		this.renderFeatured();
		//footer
		this.renderCopyright();
	};

	/***********************************************************************/
	/******* HEADER */
	/***********************************************************************/

	/**
	 * Render header wrapper
	 */
	copilot.data.Renderer.prototype.renderHeaderWrapper = function () {
		var parent = $('#header-wrapper'),
			settings = this.data.setting(),
			language = this.language,
			selected = "current_page_item",
			hash = copilot.Hash,
			header,
			name,
			menu,
			logo,
			li,
			ul;

		//clear
		parent.empty();

		header = $('<div id="header" class="container" />');
		logo = $('<div id="logo" />').appendTo(header);

		$('<img src="images/logo.png">').appendTo(logo);
		$('<h1><a href="#" data-language="AppName">' + language.getString("AppName") + '</a></h1>').appendTo(logo);

		menu = $('<div id="menu" />').appendTo(header);
		ul = $('<ul></ul>').appendTo(menu);

		name = language.getString("Statistics");
		$('<li><a href="#' + name + '" accesskey="1" title="">' + name + '</a></li>')
			.toggleClass(selected, hash === "" || hash === name)
			.appendTo(ul);

		name = language.getString("Fuels");
		$('<li><a href="#' + name + '" accesskey="2" title="">' + name + ' (' + settings.Fills + ')</a></li>')
			.toggleClass(selected, hash === name)
			.appendTo(ul);

		name = language.getString("Repairs");
		$('<li><a href="#' + name + '" accesskey="3" title="">' + name + ' (' + settings.Repairs + ')</a></li>')
			.toggleClass(selected, hash === name)
			.appendTo(ul);

		name = language.getString("Videos");
		$('<li><a href="#' + name + '" accesskey="4" title="">' + name + ' (' + settings.Videos + ')</a></li>')
			.toggleClass(selected, hash === name)
			.appendTo(ul);

		name = language.getString("Images");
		$('<li><a href="#' + name + '" accesskey="5" title="">' + name + ' (' + settings.Pictures + ')</a></li>')
			.toggleClass(selected, hash === name)
			.appendTo(ul);

		//append
		parent.append(header);

		//apply skin
		this.skin.applySkin();
	};

	/***********************************************************************/
	/******* PAGE */
	/***********************************************************************/

	/**
	 * Render page
	 */
	copilot.data.Renderer.prototype.renderPageContent = function () {
		var parent = $('#page #content'),
			language = this.language;

		//clear
		parent.empty();

		switch(copilot.Hash) {
			case "":
			case language.getString("Statistics"):
				this.renderPageWelcome(parent);
				break;
			default:
				break;
		}

		//apply skin
		this.skin.applySkin();
	};

	/**
	 * Render welcome
	 * @param {jQuery} parent
	 */
	copilot.data.Renderer.prototype.renderPageWelcome = function (parent) {
		var language = this.language,
			setting = this.data.setting(),
			data = [];

		//title
		$('<div class="title"><h2>' + language.getString("Welcome_Title") + '</h2><span class="byline">' + language.getString("Welcome_Motto") + '</span></div>').appendTo(parent);

		//motto
		data[0] = setting.Liters;
		data[1] = setting.SummaryFuelPrice;
		data[2] = setting.Currency;
		data[3] = setting.Fills;
		data[4] = setting.Repairs;
		data[5] = setting.SummaryRepairPrice;
		data[6] = setting.Maintenances;
		data[7] = setting.Videos;
		data[8] = setting.Pictures;
		//add
		$('<p>' + language.getString("Welcome_Chat", data) + '</p>').appendTo(parent);

		//button more
		$('<a href="#" class="button">' + this.language.getString('Welcome_Warnings') + '</a>').appendTo(parent);
	};

	/***********************************************************************/
	/******* MENU */
	/***********************************************************************/

	/**
	 * Render page
	 */
	copilot.data.Renderer.prototype.renderPageSideBar = function () {
		var language = this.language,
			parent = $('#page #sidebar'),
			menuTwo,
			menuOne,
			menu,
			ul;

		//clear
		parent.empty();
		//menu div
		menu = $('<div id="stwo-col" />').appendTo(parent);

		//left menu
		menuOne = $('<div class="sbox1" />').appendTo(menu);
		$('<h2>' + language.getString("Graphs") + '</h2>').appendTo(menuOne);
		ul = $('<ul class="style2">').appendTo(menuOne);
			//items
			$('<li><a href="#' + language.getString('FuelPriceTrend') + '">' + language.getString('FuelPriceTrend') + '</a></li>').appendTo(ul);
			$('<li><a href="#' + language.getString('TrendUnitsPerRefill') + '">' + language.getString('TrendUnitsPerRefill') + '</a></li>').appendTo(ul);

		//menu right
		menuTwo = $('<div class="sbox2" />').appendTo(menu);
		$('<h2>' + language.getString("NewItems") + '</h2>').appendTo(menuTwo);
		ul = $('<ul class="style2">').appendTo(menuTwo);
			//items
			$('<li><a href="#' + language.getString('AddFuel') + '">' + language.getString('AddFuel') + '</a></li>').appendTo(ul);
			$('<li><a href="#' + language.getString('AddRepair') + '">' + language.getString('AddRepair') + '</a></li>').appendTo(ul);
			$('<li><a href="#' + language.getString('AddMaintenance') + '">' + language.getString('AddMaintenance') + '</a></li>').appendTo(ul);

		//apply skin
		this.skin.applySkin();
	};

	/***********************************************************************/
	/******* BANNERS */
	/***********************************************************************/

	/**
	 * Render banners
	 */
	copilot.data.Renderer.prototype.renderBanners = function () {
		var parent = $('#banner-wrapper'),
			maintenances = this.data.maintenances();

		//clear
		parent.empty();
		parent.hide();

		if (!maintenances) {
			return;
		}

		debugger;
		//TODO

		//apply skin
		this.skin.applySkin();
	};

	/***********************************************************************/
	/******* FEATURED */
	/***********************************************************************/

	/**
	 * Render features
	 */
	copilot.data.Renderer.prototype.renderFeatured = function () {
		var parent = $('#featured-wrapper');

		//clear
		parent.empty();

		//TODO

		//apply skin
		this.skin.applySkin();
	};

	/***********************************************************************/
	/******* FOOTER */
	/***********************************************************************/

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

		//apply skin
		this.skin.applySkin();
	};














	/**
	 * Render errors
	 */
	copilot.data.Renderer.prototype.renderErrors = function () {
		var parent = $("#errors"),
			errorText = this.language.getString("Error"),
			universal = "When loading a page the error occurred. Please reload the page and try to load it again. Check if application CoPilot still running and WiFi network is available.",
			error;

		//clear
		parent.empty();
		parent.show();

		error = $('<p>' + (errorText || universal) + '</p>');

		//append
		error.appendTo(parent);

		//apply skin
		this.skin.applySkin();
	};

	/**
	 * @private
	 * Register events
	 */
	copilot.data.Renderer.prototype.registerEvents = function () {
		var self = this;
		//hash change
		$(window).on('hashchange', function() {
			copilot.Hash = window.location.hash.substr(1);
			self.renderHeaderWrapper();
			self.renderPageContent();
		});
	};

}());