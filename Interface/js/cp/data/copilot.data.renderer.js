/*global copilot */
/**
 * Skin
 */
(function () {

	copilot.data = copilot.data || {};

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
			var value = (copilot.App.GetOdometerWithRightDistance(odometerA) - copilot.App.GetOdometerWithRightDistance(odometerB)) / ration;
			return Math.max(value, minimalValue);
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
			return copilot.App.GetOdometerWithRightDistance(odometerA) - copilot.App.GetOdometerWithRightDistance(odometerB);
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
	 * Resource iframe
	 * @param {copilot.model.Backup} backup
	 * @returns {jQuery}
	 */
	function resourceIframe(backup) {
		var url,
			cid,
			resid,
			iframe;

		//url
		url = splitUrl(backup.Url);
		//cid
		//noinspection JSUnresolvedVariable
		cid = url.data.cid;
		//noinspection JSUnresolvedVariable
		resid = url.data.resid;
		//iframe
		return $('<iframe src="https://onedrive.live.com/embed?cid=' + cid + '&resid=' + resid + '" width="320" height="240" frameborder="0" scrolling="no"></iframe>');
	}

	/**
	 * Get mini map frame
	 * @param {number} longitude
	 * @param {number} latitude
	 * @param {number=} mapZoom
	 * @param {number=} width
	 * @param {number=} height
	 * @returns {jQuery}
	 */
	function miniMapFrame(longitude, latitude, mapZoom, width, height) {
		var inner,
			center,
			zoom = mapZoom || 13,
			map = $('<div class="mapviewer" />');

		width = width || 500;
		height = height || 400;

		//center
		center = new google.maps.LatLng(longitude, latitude);

		//map
		map.css({
			width: width,
			height: height
		});

		//map
		inner = $('<div class="map"></div>');
		inner.css({
			width: width,
			height: height
		});

		//map
		setTimeout(function () {
			var marker,
				googleMap = new google.maps.Map(inner[0], {
					zoom: zoom,
					center: center,
					disableDefaultUI: true,
					mapTypeId: google.maps.MapTypeId.ROADMAP
				});

			//noinspection JSUnusedAssignment
			marker = new google.maps.Marker({
				position: center,
				map: googleMap
			});

		}, 200);

		map.append(inner);

		return map;
	}

	/**
	 * Get map frame
	 * @param {Array.<copilot.model.State>} paths
	 * @param {copilot.data.Skin} skin
	 * @param {copilot.data.Language} language
	 * @returns {jQuery}
	 */
	function mapFrame(paths, skin, language) {
		var i,
			div,
			state,
			center,
			width,
			height,
			inner,
			positions = [],
			win = $(window),
			header = $("#header").height(),
			map = $('<div id="mapviewer fullscreen" />');

		//states
		for (i = 0; i < paths.length; i++) {
			if (paths[i].Position) {
				//center
				center = new google.maps.LatLng(paths[i].Position.Latitude, paths[i].Position.Longitude);
				break;
			}
		}

		//sizes
		width = win.width();
		height = win.height();

		//map
		map.css({
			position: "fixed",
			top: header,
			left: 0,
			width: width,
			height: height - header,
			background: 'white',
			zIndex: '1000'
		});

		//inner
		inner = $("<div></div>").appendTo(map);
		inner.css({
			width: width,
			height: height - header
		});

		//events
		win.unbind("resize.map");
		win.bind("resize.map", function () {
			//sizes
			width = win.width();
			height = win.height();
			//map
			map.css({
				width: width,
				height: height - header
			});
			//inner
			inner.css({
				width: width,
				height: height - header
			});
		});

		//map
		setTimeout(function () {
			var route,
				position,
				googleMap = new google.maps.Map(inner[0], {
					zoom: 16,
					center: center,
					disableDefaultUI: false,
					mapTypeId: google.maps.MapTypeId.ROADMAP
				});

			//states
			for (i = 0; i < paths.length; i++) {
				state = paths[i];
				if (state.Position) {
					//create
					position = new google.maps.LatLng(state.Position.Latitude, state.Position.Longitude);
					//marker
					createMarker(googleMap, state, language);
					//push
					positions.push(position);
				}
			}

			route = new google.maps.Polyline({
				path: positions,
				geodesic: false,
				strokeColor: skin.data.Foreground,
				strokeOpacity: 1.0,
				strokeWeight: 10
			});

			route.setMap(googleMap);

		}, 500);

		return map;
	}

	/**
	 * Create marker
	 * @param {object} map
	 * @param {copilot.model.State} state
	 * @param {copilot.data.Language} language
	 */
	function createMarker(map, state, language) {
		var div,
			window,
			marker;

		//marker
		marker = new google.maps.Marker({
			position: new google.maps.LatLng(state.Position.Latitude, state.Position.Longitude),
			map: map,
			title: language.getString('Speed') + ": "  + state.Speed
		});

		//content
		div = $('<table />').css({
			'width': 200
		});
		div.append($('<tr><td><strong>' + language.getString('Speed')  + ':</strong></td><td>' + state.Speed + '</td></tr>'));
		div.append($('<tr><td><strong>' + language.getString('ObdRpm')  + ':</strong></td><td>' + state.Rpm + '</td></tr>'));
		div.append($('<tr><td><strong>' + language.getString('ObdTemperature')  + ':</strong></td><td>' + state.Temperature + '</td></tr>'));
		div.append($('<tr><td><strong>' + language.getString('ObdAcceleratorPedalPosition')  + ':</strong></td><td>' + state.AcceleratorPedalPosition + '</td></tr>'));
		div.append($('<tr><td><strong>' + language.getString('ObdEngineLoad')  + ':</strong></td><td>' + state.EngineLoad + '</td></tr>'));
		div.append($('<tr><td><strong>' + language.getString('ObdEngineOilTemperature')  + ':</strong></td><td>' + state.EngineOilTemperature + '</td></tr>'));
		div.append($('<tr><td><strong>' + language.getString('ObdEngineReferenceTorque')  + ':</strong></td><td>' + state.EngineReferenceTorque + '</td></tr>'));
		div.append($('<tr><td><strong>' + language.getString('ObdFuelInjectionTiming')  + ':</strong></td><td>' + state.FuelInjectionTiming + '</td></tr>'));
		div.append($('<tr><td><strong>' + language.getString('ObdMaxAirFlowRate')  + ':</strong></td><td>' + state.MaxAirFlowRate + '</td></tr>'));
		div.append($('<tr><td><strong>' + language.getString('ObdThrottlePosition')  + ':</strong></td><td>' + state.ThrottlePosition + '</td></tr>'));


		//window
		window = new google.maps.InfoWindow({
			content: div[0].outerHTML
		});

		//event
		google.maps.event.addListener(marker, 'click', function() {
			window.open(map, marker);
		});
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
		/** @type {jQuery}*/
		this.map = null;

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
		document.title = this.language.getString("AppName");

		//header, page
		this.renderHeaderWrapper();
		this.renderPageContent();
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
			self = this,
			header,
			name,
			menu,
			logo,
			h1,
			li,
			ul;

		//clear
		parent.empty();

		header = $('<div id="header" class="container" />');
		logo = $('<div id="logo" />').appendTo(header);

		$('<img src="images/logo.png">').appendTo(logo);
		h1 = $('<h1><a href="#" data-language="AppName">' + language.getString("AppName") + '</a></h1>').appendTo(logo);
		h1.toggleClass(selected, hash === "" || hash === name || hash === "Warnings");

		menu = $('<div id="menu" />').appendTo(header);
		ul = $('<ul></ul>').appendTo(menu);

		name = language.getString("Fuels");
		$('<li><a href="#' + name + '" accesskey="1" title="">' + name + ' (' + settings.Fills + ')</a></li>')
			.toggleClass(selected, hash === name)
			.appendTo(ul);

		name = language.getString("Repairs");
		$('<li><a href="#' + name + '" accesskey="2" title="">' + name + ' (' + settings.Repairs + ')</a></li>')
			.toggleClass(selected, hash === name)
			.appendTo(ul);

		name = language.getString("Videos");
		$('<li><a href="#' + name + '" accesskey="3" title="">' + name + ' (' + settings.Videos + ')</a></li>')
			.toggleClass(selected, hash === name)
			.appendTo(ul);

		name = language.getString("Images");
		$('<li><a href="#' + name + '" accesskey="4" title="">' + name + ' (' + settings.Pictures + ')</a></li>')
			.toggleClass(selected, hash === name)
			.appendTo(ul);

		name = language.getString("Paths");
		$('<li><a href="#' + name + '" accesskey="5" title="">' + name + ' (' + settings.Paths + ')</a></li>')
			.toggleClass(selected, hash === name)
			.appendTo(ul);

		//clear map screen on click
		header.find('a').click(function () {
			self.renderMap(null);
		});

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

		//sidebar
		this.renderPageSideBar();

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
			case language.getString("Images"):
				this.renderImages(parent);
				break;
			case language.getString("Paths"):
				this.renderPaths(parent);
				break;
			case language.getString("AddItem"):
				this.renderAddItem(parent);
				break;
			case language.getString('FuelPriceTrend'):
				this.renderGraph(parent, 'FuelPriceTrend');
				break;
			case language.getString('TrendUnitsPerRefill'):
				this.renderGraph(parent, 'TrendUnitsPerRefill');
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
		data[2] = copilot.Currency;
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
			a.append('<div class="value">' + copilot.App.GetOdometerWithRightDistance(fill.Odometer).toFixed(1) + ' ' + copilot.Distance + '</div>');

			//distance
			distance = calculateDistance(fill.Odometer, fills[i + 1] ? fills[i + 1].Odometer : null);
			if (distance > 0) {
				distanceEl = $('<div class="distance">' + Math.round(distance)  + ' ' + copilot.Distance + '</div>');
				distanceEl.css('top', (height / 2) + 15);
				a.append(distanceEl);
			}

			//data
			data[0] = fill.Date.toLocaleDateString();
			data[1] = fill.Refueled;
			data[2] = language.getString("FueledUnit");
			data[3] = copilot.App.GetPriceWithRightValue(fill.Price);
			data[4] = copilot.Currency;
			data[5] = copilot.App.GetPriceWithRightValue(fill.UnitPrice);


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
			height = calculateHeight(repair.Odometer, repairs[i + 1] ? repairs[i + 1].Odometer : null, 10, 120);

			p = $('<p></p>').addClass('repairs').appendTo(parent);
			p.css('height', height);
			a = $('<div class="odometer"/>').appendTo(p);
			a.append('<div class="value">' + copilot.App.GetOdometerWithRightDistance(repair.Odometer).toFixed(1)  + ' ' + copilot.Distance + '</div>');

			//distance
			distance = calculateDistance(repair.Odometer, repairs[i + 1] ? repairs[i + 1].Odometer : null);
			if (distance > 0) {
				distanceEl = $('<div class="distance">' + Math.round(distance)  + ' ' + copilot.Distance + '</div>');
				distanceEl.css('top', (height / 2) + 15);
				a.append(distanceEl);
			}

			//data
			data[0] = repair.Date.toLocaleDateString();
			data[1] = repair.ServiceName;
			data[2] = copilot.App.GetPriceWithRightValue(repair.Price);
			data[3] = copilot.Currency;

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
			self = this,
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
					iframe = resourceIframe(video.VideoBackup);
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

				//description => video, map
				description = $('<p />').addClass("video");
				description.append(iframe);

				if (point) {
					//map
					description.append(miniMapFrame(point ? point.Position.Latitude : 0, point ? point.Position.Longitude : 0, 15, 180, 240));
					//paths
					button = $('<a href="#' + language.getString("Videos") + '" class="button path">' + this.language.getString('ShowMap') + '</a>');
					button.data("video", video);
					button.click(function () {
						var video = $(this).data("video");
						self.renderMap(dataModel.paths(video.Time, new Date(video.Time.getTime() + (video.duration * 1000))));
					});
					description.append(button);
				}

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

	/**
	 * Render images
	 * @param {jQuery} parent
	 */
	copilot.data.Renderer.prototype.renderImages = function (parent) {
		var i,
			p,
			a,
			div,
			text,
			info,
			point,
			image,
			title,
			paths,
			height,
			button,
			iframe,
			data = [],
			self = this,
			description,
			language = this.language,
			dataModel = this.data,
			images = dataModel.images() || null;

		if (images === null) {
			return;
		}

		//clear
		//noinspection JSUnresolvedFunction
		parent.empty();

		//title
		title = $('<div class="title" />').appendTo(parent);

		//h2
		$('<h2>' + language.getString("Images") + '</h2>').appendTo(title);


		$('<span class="byline">' + language.getString("Pictures_Motto") + '</span>').appendTo(title);

		for (i = 0; i < images.length; i++) {
			//image
			image = images[i];
			//paths
			paths = dataModel.paths(new Date(image.Time.getTime() - 10000), new Date(image.Time.getTime() + 10000));

			p = $('<p></p>').addClass('images').appendTo(parent);
			p.css('height', '400px');
			a = $('<div class="odometer"/>').appendTo(p);
			a.append('<div class="value">' + image.Time.toLocaleDateString() + '<span class="small">' + image.Time.toLocaleTimeString() + '</span></div>');

			//data
			data[0] = image.Time.toLocaleDateString();

			//info
			info = $('<p />').addClass("info").html(language.getString("ImageDescription", data));
			p.append(info);

			//if paths exists
			if (paths) {
				//first
				point = paths[0];
				//iframe
				if (image.isBackuped()) {
					//image is available on cloud storage
					iframe = resourceIframe(image.Backup);
				} else {
					//not backuped
					iframe = $('<div class="noiframe"></div>');
					text = $('<div></div>').appendTo(iframe);
					text.append(language.getString('ImageNotBackuped', data));
					text.append($('<br />'));
					$('<a href="' + window.location.hash + '" data-image="' + image._path + '"></a>').text(language.getString('ImageShowIt')).click(function () {
						dataModel.run('show', $(this).attr('data-image'));
					}).appendTo(text);
					text.append($('<br />'));
					text.append(language.getString('Or'));
					text.append($('<br />'));
					$('<a href="' + window.location.hash + '"></a>').text(language.getString('VideoBackupIt')).click(function () {
						dataModel.run('backup');
					}).appendTo(text);
				}

				//description => image, map
				description = $('<p />').addClass("image");
				description.append(iframe);

				if (point) {
					//map
					description.append(miniMapFrame(point.Position.Latitude, point.Position.Longitude, 15, 180, 240));
					//paths
					button = $('<a href="#' + language.getString("Images") + '" class="button path">' + this.language.getString('ShowMap') + '</a>');
					button.data("image", image);
					button.click(function () {
						var image = $(this).data("image");
						self.renderMap(dataModel.paths(new Date(image.Time.getTime() - 10000), new Date(image.Time.getTime() + 10000)));
					});
					description.append(button);
				}

			} else {
				//description
				description = $('<p />').addClass("image");
				this.loader(description);
			}

			//div
			p.append(description);
			p.append($('<div />').css('clear', 'both'));
		}

		if (images.length === 0) {
			$('<p>' + language.getString("EmptyImages") + '</p>').appendTo(parent);
		}

	};

	/**
	 * Render paths
	 * @param {jQuery} parent
	 */
	copilot.data.Renderer.prototype.renderPaths = function (parent) {
		var i,
			p,
			a,
			div,
			info,
			path,
			title,
			height,
			button,
			data = [],
			self = this,
			description,
			pathStates,
			language = this.language,
			dataModel = this.data,
			paths = dataModel.pathList() || null;

		if (paths === null) {
			return;
		}

		//clear
		//noinspection JSUnresolvedFunction
		parent.empty();

		//title
		title = $('<div class="title" />').appendTo(parent);

		//h2
		$('<h2>' + language.getString("Paths") + '</h2>').appendTo(title);


		$('<span class="byline">' + language.getString("Paths_Motto") + '</span>').appendTo(title);

		for (i = 0; i < paths.length; i++) {
			//path
			path = /** @type {copilot.model.Path} */ paths[i];
			//paths
			pathStates = dataModel.paths(path.StartDate, path.EndDate);

			p = $('<p></p>').addClass('paths').appendTo(parent);
			p.css('height', '150px');
			a = $('<div class="odometer"/>').appendTo(p);
			a.append('<div class="value">' + path.StartDate.toLocaleDateString() + '<span class="small">' + path.StartDate.toLocaleTimeString() + '</span></div>');

			//data
			data[0] = path.StartDate.toLocaleDateString();
			data[1] = Math.round(path.ConsumedFuel * 1000) / 1000;
			data[2] = Math.round(path.TraveledDistance * 1000) / 1000;
			data[3] = path.Distance;
			data[4] = language.getString("FueledUnit");

			//info
			info = $('<p />').addClass("info").html(language.getString("PathDescription", data));
			p.append(info);

			//description
			description = $('<p />').addClass("path");

			//if paths exists
			if (pathStates) {
				//first
				if (pathStates[0]) {
					//paths
					button = $('<a href="#' + language.getString("Paths") + '" class="button path">' + this.language.getString('ShowMap') + '</a>');
					button.data("path", path);
					button.click(function () {
						var path = $(this).data("path");
						self.renderMap(dataModel.paths(path.StartDate, path.EndDate));
					});
					description.append(button);
				}

			} else {
				//description
				description = $('<p />').addClass("path");
				this.loader(description);
			}

			//div
			p.append(description);
			p.append($('<div />').css('clear', 'both'));
		}

		if (paths.length === 0) {
			$('<p>' + language.getString("EmptyPaths") + '</p>').appendTo(parent);
		}

	};

	/**
	 * Render graph
	 * @param {jQuery} parent
	 * @param {string} graph
	 */
	copilot.data.Renderer.prototype.renderGraph = function (parent, graph) {
		var graphData,
			graphElement,
			skin = this.skin.data,
			language = this.language,
			fills = this.data.fills() || null;

		if (fills === null) {
			return;
		}

		//clear
		//noinspection JSUnresolvedFunction
		parent.empty();

		//create container
		graphElement = $('<div id="#graph">').appendTo(parent);

		//graph
		switch(graph) {
			case 'FuelPriceTrend':
				graphData = fills.getFuelPriceTrendGraph(language);
				break;
			case 'TrendUnitsPerRefill':
				graphData = fills.getTrendUnitsPerRefillGraph(language);
				break;
			default:
				break;
		}

		//create
		graphElement.highcharts({
			chart: {
				zoomType: 'x'
			},
			title: {
				text: graphData.name
			},
			xAxis: {
				minRange: 5,
				categories: graphData.categories,
				labels: {
					rotation: 90
				}
			},
			yAxis: {
				title: {
					text: graphData.dataName + " " + graphData.dataUnit
				}
			},
			tooltip: {
				valueSuffix: graphData.dataUnit
			},
			legend: {
				enabled: false
			},
			plotOptions: {
				series: {
					value: 0,
					width: 2,
					color: skin.Foreground
				}
			},
			series: graphData.series
		});
	};

	/**
	 * Render map
	 * @param {Array.<copilot.model.State>} states
	 */
	copilot.data.Renderer.prototype.renderMap = function (states) {
		var skin = this.skin,
			language = this.language,
			frame;

		//clear
		if (states === null) {
			if (this.map) {
				this.map.remove();
			}
			this.map = null;
			return;
		}

		//frame
		frame = mapFrame(states, skin, language);
		//set current map
		this.map = frame;
		//append
		$('body').append(frame);
	};

	/**
	 * Render add item
	 * @param {jQuery} parent
	 */
	copilot.data.Renderer.prototype.renderAddItem = function (parent) {
		var div,
			title,
			language = this.language;

		//clear
		//noinspection JSUnresolvedFunction
		parent.empty();

		//title
		title = $('<div class="title" />').appendTo(parent);

		//h2
		$('<h2>' + language.getString("AddItem") + '</h2>').appendTo(title);

		$('<span class="byline">' + language.getString("AddItem_Motto") + '</span>').appendTo(title);

		//add
		$('<p>' + language.getString("AddItem_Description") + '</p>').appendTo(parent);
	};


	/***********************************************************************/
	/******* MENU */
	/***********************************************************************/

	/**
	 * Render page
	 */
	copilot.data.Renderer.prototype.renderPageSideBar = function () {
		var language = this.language,
			data = this.data,
			parent = $('#sidebar'),
			content = $('#content'),
			showAddMenu = true,
			min = 'minimalized',
			max = 'maximalized',
			menuTwo,
			menuOne,
			menu,
			li,
			ul;

		//clear
		parent.empty();
		//menu div
		menu = $('<div id="stwo-col" />').appendTo(parent);

		//show add
		switch(copilot.Hash) {
			case language.getString('FuelPriceTrend'):
			case language.getString('TrendUnitsPerRefill'):
				showAddMenu = false;
				break;
			default:
				showAddMenu = true;
				break;
		}

		//left menu
		menuOne = $('<div class="sbox1" />').appendTo(menu);
		$('<h2>' + language.getString("Graphs") + '</h2>').appendTo(menuOne);
		ul = $('<ul class="style2">').appendTo(menuOne);
			//items
			$('<li><a href="#' + language.getString('FuelPriceTrend') + '">' + language.getString('FuelPriceTrend') + '</a></li>').appendTo(ul);
			$('<li><a href="#' + language.getString('TrendUnitsPerRefill') + '">' + language.getString('TrendUnitsPerRefill') + '</a></li>').appendTo(ul);

		if (showAddMenu) {
			//menu right
			menuTwo = $('<div class="sbox2" />').appendTo(menu);
			$('<h2>' + language.getString("NewItems") + '</h2>').appendTo(menuTwo);
			ul = $('<ul class="style2">').appendTo(menuTwo);
			//items
			li = $('<li><a href="#' + language.getString('AddItem') + '">' + language.getString('AddFuel') + '</a></li>').appendTo(ul);
			li.click(function () {
				data.run("add-fuel");
			});

			li = $('<li><a href="#' + language.getString('AddItem') + '">' + language.getString('AddRepair') + '</a></li>').appendTo(ul);
			li.click(function () {
				data.run("add-repair");
			});

			li = $('<li><a href="#' + language.getString('AddItem') + '">' + language.getString('AddMaintenance') + '</a></li>').appendTo(ul);
			li.click(function () {
				data.run("add-maintenance");
			});

			//remove class
			parent.removeClass(min);
			content.removeClass(max);
		} else {
			//add min
			parent.addClass(min);
			content.addClass(max);
		}

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
			$('<div class="box-right"> <a class="button button-big">' + copilot.App.GetOdometerWithRightDistance(maintenance.Odometer) + ' ' +  copilot.Distance + '</a></div>').appendTo(container);
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
			self.renderMap(null);
			self.renderHeaderWrapper();
			self.renderPageContent();
		});
	};

}());