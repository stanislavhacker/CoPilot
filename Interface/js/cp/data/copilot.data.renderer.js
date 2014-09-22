/*global copilot */
/**
 * Skin
 */
(function () {

	copilot.data = copilot || {};

	/**
	 * Calculate height
	 * @param {copilot.model.Odometer} odometerA
	 * @param {copilot.model.Odometer} odometerB
	 * @param {number} ration
	 * @param {number=} minimalValue
	 * @returns {number}
	 */
	function calculateHeight(odometerA, odometerB, ration, minimalValue) {
		if (odometerB) {
			return (odometerA.Value - odometerB.Value) / ration;
		}
		return minimalValue || 200;
	}

	/**
	 * Calculate distance
	 * @param {copilot.model.Odometer} odometerA
	 * @param {copilot.model.Odometer} odometerB
	 * @returns {number}
	 */
	function calculateDistance(odometerA, odometerB) {
		if (odometerB) {
			return odometerA.Value - odometerB.Value;
		}
		return 0;
	}

	/**
	 * Split url
	 * @param {string} url
	 * @returns {{url: string, data: {}}}
	 */
	function splitUrl(url) {
		var i,
			info,
			params,
			data = {},
			index = url.indexOf("?"),
			www = url.substr(0, index),
			query = url.substr(index, url.length - index);

		params = query.split("&");
		for (i = 0; i < params.length; i++) {
			info = params[i].split("=");
			data[info[0]] = info[1];
		}

		return {
			url: www,
			data: data
		}
	}

	/**
	 * Video iframe
	 * @param {copilot.model.Video} video
	 * @returns {jQuery}
	 */
	function videoIframe(video) {
		var url,
			cid,
			resid,
			iframe;

		//url
		url = splitUrl(video.VideoBackup.Url);
		//cid
		//noinspection JSUnresolvedVariable
		cid = url.data.cid;
		//noinspection JSUnresolvedVariable
		resid = url.data.resid;
		//iframe
		return $('<iframe src="https://onedrive.live.com/embed?cid=' + cid + '&resid=' + resid + '" width="320" height="240" frameborder="0" scrolling="no"></iframe>');
	}

	/**
	 * Get map frame
	 * @param {number} longitude
	 * @param {number} latitude
	 * @param {number=} mapZoom
	 * @param {number=} width
	 * @param {number=} height
	 * @returns {jQuery}
	 */
	function mapFrame(longitude, latitude, mapZoom, width, height) {
		var iframe,
			mapLink,
			zoom = mapZoom || 13,
			map = $('<div id="mapviewer" />');

		width = width || 500;
		height = height || 400;

		//map link
		mapLink = "http://www.bing.com/maps/embed/?v=2&cp=" + longitude + "~" + latitude + "&lvl=" + zoom + "&sty=r&w=" + width + "&h=" + height;

		//map
		iframe = $('<iframe id="map" scrolling="no" width="' + width + '" height="' + height + '" frameborder="0" src="' + mapLink + '"></iframe>');
		map.append(iframe);

		return map;
	}

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
			.toggleClass(selected, hash === "" || hash === name || hash === "Warnings")
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
		var parent = $('#content'),
			language = this.language;

		//clear
		parent.empty();

		//loader
		this.loader(parent);

		switch(copilot.Hash) {
			case "":
			case "Warnings":
			case language.getString("Statistics"):
				this.renderPageWelcome(parent);
				break;
			case language.getString("Fuels"):
				this.renderFills(parent);
				break;
			case language.getString("Repairs"):
				this.renderRepairs(parent);
				break;
			case language.getString("Videos"):
				this.renderVideos(parent);
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
			maintenances = this.data.maintenances() || null,
			setting = this.data.setting(),
			data = [];

		//noinspection JSUnresolvedFunction
		parent.empty();

		//title
		$('<div class="title"><h2>' + language.getString("Welcome_Title") + '</h2><span class="byline">' + language.getString("Welcome_Motto") + '</span></div>').appendTo(parent);

		//motto
		data[0] = Math.round(setting.Liters * 10) / 10;
		data[1] = Math.round(setting.SummaryFuelPrice * 10) / 10;
		data[2] = setting.Currency;
		data[3] = setting.Fills;
		data[4] = setting.Repairs;
		data[5] = Math.round(setting.SummaryRepairPrice * 10) / 10;
		data[6] = setting.Maintenances;
		data[7] = setting.Videos;
		data[8] = setting.Pictures;
		//add
		$('<p>' + language.getString("Welcome_Chat", data) + '</p>').appendTo(parent);

		//button warnings
		if (maintenances && maintenances.length > 0 && maintenances.getWarnings(this.data.odometer()).length > 0) {
			$('<a href="#Warnings" class="button">' + this.language.getString('Welcome_Warnings') + '</a>').appendTo(parent);
		}
	};

	/**
	 * Render fills
	 * @param {jQuery} parent
	 */
	copilot.data.Renderer.prototype.renderFills = function (parent) {
		var i,
			p,
			a,
			div,
			fill,
			title,
			height,
			info,
			distance,
			data = [],
			distanceEl,
			language = this.language,
			fills = this.data.fills() || null;

		if (fills === null) {
			return;
		}

		//clear
		//noinspection JSUnresolvedFunction
		parent.empty();

		//title
		title = $('<div class="title" />').appendTo(parent);

		//h2
		$('<h2>' + language.getString("Fuels") + '</h2>').appendTo(title);
		$('<span class="byline">' + language.getString("Fuels_Motto") + '</span>').appendTo(title);

		for (i = 0; i < fills.length; i++) {
			//fill
			fill = fills[i];
			//height
			height = calculateHeight(fill.Odometer, fills[i + 1] ? fills[i + 1].Odometer : null, 1, 70);

			p = $('<p></p>').addClass('fills').appendTo(parent);
			p.css('height', height);
			a = $('<div class="odometer" />').appendTo(p);
			a.append('<div class="value">' + fill.Odometer.Value.toFixed(1) + ' ' + fill.Odometer.Distance + '</div>');

			//distance
			distance = calculateDistance(fill.Odometer, fills[i + 1] ? fills[i + 1].Odometer : null);
			if (distance > 0) {
				distanceEl = $('<div class="distance">' + Math.round(distance)  + ' ' + fill.Odometer.Distance + '</div>');
				distanceEl.css('top', (height / 2) + 15);
				a.append(distanceEl);
			}

			//data
			data[0] = fill.Date.toLocaleDateString();
			data[1] = fill.Refueled;
			data[2] = language.getString("FueledUnit");
			data[3] = Math.round(fill.Price.Value * 10) / 10;
			data[4] = fill.Price.Currency;
			data[5] = Math.round(fill.UnitPrice.Value * 10) / 10;


			//info
			info = $('<p />').addClass("info").html(language.getString("FillDescription", data));

			p.append(info);
			p.append($('<div />').css('clear', 'both'));
		}

		if (fills.length === 0) {
			$('<p>' + language.getString("EmptyData") + '</p>').appendTo(parent);
			$('<a href="#' + language.getString('AddFuel') + '" class="button">' + language.getString('AddFuel') + '</a>').appendTo(parent);
		}

	};

	/**
	 * Render repairs
	 * @param {jQuery} parent
	 */
	copilot.data.Renderer.prototype.renderRepairs = function (parent) {
		var i,
			p,
			a,
			div,
			height,
			repair,
			title,
			info,
			distance,
			data = [],
			distanceEl,
			description,
			language = this.language,
			repairs = this.data.repairs() || null;

		if (repairs === null) {
			return;
		}

		//clear
		//noinspection JSUnresolvedFunction
		parent.empty();

		//title
		title = $('<div class="title" />').appendTo(parent);

		//h2
		$('<h2>' + language.getString("Repairs") + '</h2>').appendTo(title);


		$('<span class="byline">' + language.getString("Repairs_Motto") + '</span>').appendTo(title);

		for (i = 0; i < repairs.length; i++) {
			//repair
			repair = repairs[i];
			//height
			height = calculateHeight(repair.Odometer, repairs[i + 1] ? repairs[i + 1].Odometer : null, 10, 100);

			p = $('<p></p>').addClass('repairs').appendTo(parent);
			p.css('height', height);
			a = $('<div class="odometer"/>').appendTo(p);
			a.append('<div class="value">' + repair.Odometer.Value.toFixed(1)  + ' ' + repair.Odometer.Distance + '</div>');

			//distance
			distance = calculateDistance(repair.Odometer, repairs[i + 1] ? repairs[i + 1].Odometer : null);
			if (distance > 0) {
				distanceEl = $('<div class="distance">' + Math.round(distance)  + ' ' + repair.Odometer.Distance + '</div>');
				distanceEl.css('top', (height / 2) + 15);
				a.append(distanceEl);
			}

			//data
			data[0] = repair.Date.toLocaleDateString();
			data[1] = repair.ServiceName;
			data[2] = Math.round(repair.Price.Value * 10) / 10;
			data[3] = repair.Price.Currency;

			//info
			info = $('<p />').addClass("info").html(language.getString("RepairDescriptionWeb", data));

			//description
			description = $('<p />').addClass("description").html(repair.Description.replace("\n", "<br />"));

			p.append(info);
			p.append(description);

			p.append($('<div />').css('clear', 'both'));
		}

		if (repairs.length === 0) {
			$('<p>' + language.getString("EmptyData") + '</p>').appendTo(parent);
			$('<a href="#' + language.getString('AddRepair') + '" class="button">' + language.getString('AddRepair') + '</a>').appendTo(parent);
		}

	};

	/**
	 * Render videos
	 * @param {jQuery} parent
	 */
	copilot.data.Renderer.prototype.renderVideos = function (parent) {
		var i,
			p,
			a,
			div,
			text,
			info,
			point,
			video,
			title,
			paths,
			height,
			button,
			iframe,
			data = [],
			description,
			language = this.language,
			dataModel = this.data,
			videos = dataModel.videos() || null;

		if (videos === null) {
			return;
		}

		//clear
		//noinspection JSUnresolvedFunction
		parent.empty();

		//title
		title = $('<div class="title" />').appendTo(parent);

		//h2
		$('<h2>' + language.getString("Videos") + '</h2>').appendTo(title);


		$('<span class="byline">' + language.getString("Videos_Motto") + '</span>').appendTo(title);

		for (i = 0; i < videos.length; i++) {
			//video
			video = videos[i];
			//paths
			paths = dataModel.paths(video.Time, new Date(video.Time.getTime() + (video.duration * 1000)));

			p = $('<p></p>').addClass('videos').appendTo(parent);
			p.css('height', '400px');
			a = $('<div class="odometer"/>').appendTo(p);
			a.append('<div class="value">' + video.Time.toLocaleDateString() + '<span class="small">' + video.Time.toLocaleTimeString() + '</span></div>');

			//data
			data[0] = video.Time.toLocaleDateString();
			data[1] = new Date(video.duration * 1000).toLocaleTimeString();
			data[2] = paths ? paths.length : 0;

			//info
			info = $('<p />').addClass("info").html(language.getString("VideoDescription", data));
			p.append(info);

			//if paths exists
			if (paths) {
				//first
				point = paths[0];
				//iframe
				if (video.isBackuped()) {
					//video is available on cloud storage
					iframe = videoIframe(video);
				} else {
					//not backuped
					iframe = $('<div class="noiframe"></div>');
					text = $('<div></div>').appendTo(iframe);
					text.append(language.getString('VideoNotBackuped', data));
					text.append($('<br />'));
					$('<a href="' + window.location.hash + '" data-video="' + video._path + '"></a>').text(language.getString('VideoPlayIt')).click(function () {
						dataModel.run('play', $(this).attr('data-video'));
					}).appendTo(text);
					text.append($('<br />'));
					text.append(language.getString('Or'));
					text.append($('<br />'));
					$('<a href="' + window.location.hash + '"></a>').text(language.getString('VideoBackupIt')).click(function () {
						dataModel.run('backup');
					}).appendTo(text);
				}

				//description =. video, map
				description = $('<p />').addClass("video");
				description.append(iframe);
				description.append(mapFrame(point ? point.Position.Latitude : 0, point ? point.Position.Longitude : 0, 13, 180, 240));

				//paths
				button = $('<a href="#" class="button path">' + this.language.getString('ShowMap') + '</a>'); //TODO: Show map with all points
				description.append(button);
			} else {
				//description
				description = $('<p />').addClass("video");
				this.loader(description);
			}

			//div
			p.append(description);
			p.append($('<div />').css('clear', 'both'));
		}

		if (videos.length === 0) {
			$('<p>' + language.getString("EmptyVideos") + '</p>').appendTo(parent);
		}

	};

	/***********************************************************************/
	/******* MENU */
	/***********************************************************************/

	/**
	 * Render page
	 */
	copilot.data.Renderer.prototype.renderPageSideBar = function () {
		var language = this.language,
			parent = $('#sidebar'),
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
			maintenances = this.data.maintenances() || [],
			warnings,
			i;

		//clear
		parent.empty();
		parent.hide();

		if (maintenances.length === 0) {
			return;
		}

		warnings = maintenances.getWarnings(this.data.odometer());
		for (i = 0; i < warnings.length; i++) {
			banner.call(this, warnings[i]).appendTo(parent);
		}

		if (warnings.length > 0) {
			parent.prepend('<a name="Warnings" />');
			parent.show();
		}

		//apply skin
		this.skin.applySkin();
	};

	/**
	 * Banner
	 * @param {copilot.model.Maintenance} maintenance
	 * @return {jQuery}
	 */
	function banner(maintenance) {
		var icon,
			language = this.language,
			container = $('<div id="banner" class="container" />'),
			boxLeft;


		switch(maintenance.Type) {
			case "Filters":
				icon = "icon-filter";
				break;
			case "Oil":
				icon = "icon-exclamation-sign";
				break;
			case "Maintenance":
				icon = "icon-medkit";
				break;
			case "Insurance":
				icon = "icon-briefcase";
				break;
			case "TechnicalInspection":
				icon = "icon-check";
				break;
			default:
				icon = "icon-question-sign";
				break;
		}

		//right
		if (maintenance.IsOdometer) {
			$('<div class="box-right"> <a class="button button-big">' + maintenance.Odometer.Value + ' ' +  maintenance.Odometer.Distance + '</a></div>').appendTo(container);
		} else {
			$('<div class="box-right"> <a class="button button-big">' + maintenance.getDate() + '</a></div>').appendTo(container);
		}


		//left
		boxLeft = $('<div class="box-left" />').appendTo(container);
		$('<span class="icon ' + icon + '"></span>').appendTo(boxLeft);
		$('<h2>' + language.getString('MaintenanceType_' + maintenance.Type) + '</h2>').appendTo(boxLeft);
		$('<span>' + language.getString('MaintenanceType_' + maintenance.Type + '_Description') + '</span>').appendTo(boxLeft);
		$('<span class="description"><strong>' + language.getString('YourDescription') + ':</strong> ' + maintenance.Description + '</span>').appendTo(boxLeft);

		return container;
	}

	/***********************************************************************/
	/******* FEATURED */
	/***********************************************************************/

	/**
	 * Render features
	 */
	copilot.data.Renderer.prototype.renderFeatured = function () {
		var parent = $('#featured-wrapper'),
			language = this.language,
			container;

		//clear
		parent.empty();

		//title
		container = $('<div id="featured" class="container">').appendTo(parent);
		$('<div class="major"><h2>' + language.getString('Social') + '</h2><span class="byline">' + language.getString('SocialDescription') + '</span></div>').appendTo(container);

		feature('facebook', language.getString('Facebook'), language.getString('FacebookDescription'), "https://www.facebook.com/carcopilot").appendTo(parent);
		feature('twitter', language.getString('Twitter'), language.getString('TwitterDescription'), "https://twitter.com/carcopilot").appendTo(parent);
		feature('google-plus', language.getString('Google'), language.getString('GoogleDescription'), "https://plus.google.com/b/115628070739534024707/115628070739534024707/posts").appendTo(parent);
		feature('youtube', language.getString('Youtube'), language.getString('YoutubeDescription'), "https://www.youtube.com/channel/UCx7xoEA0NPlLwdA68G8_orQ/videos").appendTo(parent);


		//apply skin
		this.skin.applySkin();
	};

	/**
	 * Feature
	 * @param {string} ico
	 * @param {string} name
	 * @param {string} description
	 * @param {string} link
	 * @returns {*|jQuery|HTMLElement}
	 */
	function feature(ico, name, description, link) {
		var p,
			url,
			parent = $('<div class="column1" />');

		$('<span class="icon icon-' + ico + '"></span>').appendTo(parent);
		$('<div class="title"><h2>' + name + '</h2></div>').appendTo(parent);

		url = link.length > 33 ? link.substr(0, 30) + "..." : link;
		p = $('<p>' + description + '</p>').appendTo(parent);
		$('<br /><a href="' + link + '" target="_blank">' + url + '</a>').appendTo(p);

		return parent;
	}

	/***********************************************************************/
	/******* FOOTER */
	/***********************************************************************/

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
	 * Loader
	 * @param {jQuery} parent
	 */
	copilot.data.Renderer.prototype.loader = function (parent) {
		$('<div class="spinner"><div class="rect1"></div><div class="rect2"></div><div class="rect3"></div><div class="rect4"></div><div class="rect5"></div></div>').appendTo(parent);
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