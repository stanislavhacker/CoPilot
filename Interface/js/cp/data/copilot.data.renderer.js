/*global copilot, FB, twttr */
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
	 * Get map utl
	 * @param {copilot.model.Path} path
	 * @returns {string}
	 */
	function mapUrl(path) {
		var i,
			step,
			size,
			state,
			color,
			encoded,
			paths = [],
			url = "https://maps.googleapis.com/maps/api/staticmap?";

		//colors
		color = "path=color:0xff0000ff|weight:5";
		//step
		step = path.States.length > 100 ? Math.ceil(path.States.length / 100) : 1;

		//states
		for (i = 0; i < path.States.length; i += step) {
			state = path.States[i];
			if (state.Position) {
				//paths += "|" + state.Position.Latitude + "," + state.Position.Longitude;
				paths.push(new google.maps.LatLng(state.Position.Latitude, state.Position.Longitude));
			}
		}
		encoded = "|enc:" + google.maps.geometry.encoding.encodePath(paths);

		//size
		size = "&size=512x512";

		return url + color + encoded + size;
	}

	/**
	 * Renderer
	 * @param {copilot.App} app
	 * @constructor
	 */
	copilot.data.Renderer = function (app) {
		/** @type {copilot.data.Language}*/
		this.language = app.language;
		/** @type {copilot.data.Skin}*/
		this.skin = app.skin;
		/** @type {copilot.data.Data}*/
		this.data = app.data;
		/** @type {copilot.data.Map}*/
		this.mapRenderer = new copilot.data.Map(this);

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

		//loader
		parent.empty();
		this.loader(parent);

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

		//loader
		parent.empty();
		this.loader(parent);

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

	//////////////////////////////////////////////////
	//////////////////////////// VIDEOS
	//////////////////////////////////////////////////

	/**
	 * Render videos
	 * @param {jQuery} parent
	 */
	copilot.data.Renderer.prototype.renderVideos = function (parent) {
		var i,
			p,
			div,
			language = this.language,
			dataModel = this.data,
			videos = dataModel.videos() || null;

		//return
		if (videos === null) {
			//loader
			parent.empty();
			this.loader(parent);
			return;
		}

		//title
		this.videoTitle("div-video", parent, language);

		//zero length
		if (videos.length === 0) {
			$('<p>' + language.getString("EmptyVideos") + '</p>').appendTo(parent);
			return;
		}

		for (i = 0; i < videos.length; i++) {
			//video frame
			this.videoFrame(parent, videos[i]);
		}
	};

	/**
	 * @private
	 * Video title
	 * @param {string} id
	 * @param {jQuery} parent
	 * @param {copilot.data.Language} language
	 */
	copilot.data.Renderer.prototype.videoTitle = function (id, parent, language) {
		var title,
			exists = parent.find('#' + id).length;

		if (exists) {
			return;
		}

		//clear
		//noinspection JSUnresolvedFunction
		parent.empty();
		//title
		title = $('<div class="title" />').attr('id', id).appendTo(parent);
		//h2
		$('<h2>' + language.getString("Videos") + '</h2>').appendTo(title);
		$('<span class="byline">' + language.getString("Videos_Motto") + '</span>').appendTo(title);
	};

	/**
	 * @private
	 * Video frame
	 * @param {jQuery} parent
	 * @param {copilot.model.Video} video
	 */
	copilot.data.Renderer.prototype.videoFrame = function (parent, video) {
		var item,
			link,
			div,
			text,
			info,
			point,
			paths,
			height,
			button,
			iframe,
			data = [],
			description,
			self = this,
			language = this.language,
			dataModel = this.data;

		/**
		 * Description
		 * @returns {string|*}
		 */
		function getDescription(video, paths) {
			//data
			data[0] = video.Time.toLocaleDateString();
			data[1] = copilot.App.timeDifference(new Date(video.duration * 1000), new Date(0));
			data[2] = paths ? paths.States.length : 0;
			data[3] = paths ?  Math.round(paths.ConsumedFuel * 100) / 100 : 0;
			data[4] = paths ?  Math.round(paths.TraveledDistance * 100) / 100 : 0;
			data[5] = copilot.Distance;
			data[6] = language.getString('FueledUnit');
			//string
			return language.getString("VideoDescription", data);
		}

		//paths
		paths = dataModel.path(video.Time, new Date(video.Time.getTime() + (video.duration * 1000)));

		//data
		item = parent.find("#video-" + video.Time.getTime());
		info = item.find("p.info");
		description =  item.find("p.video");

		//not exists
		if (item.length === 0) {
			item = $('<p />').addClass('videos').attr('id', "video-" + video.Time.getTime()).appendTo(parent);
			item.css('height', '430px');
			link = $('<div class="odometer"/>').appendTo(item);
			link.append('<div class="value">' + video.Time.toLocaleDateString() + '<span class="small">' + video.Time.toLocaleTimeString() + '</span></div>');

			//info
			info = $('<p />').addClass("info").html(getDescription(video, paths));
			item.append(info);

			//description
			description = $('<p />').addClass("video");
			item.append(description);
			item.append($('<div />').css('clear', 'both'));
		} else {
			//set new html
			info.html(getDescription(video, paths));
		}

		//if paths exists
		if (paths) {
			if (description.find('.video-rendered').length === 0) {
				//empty
				description.empty();
				//first
				point = paths.States[0];
				//iframe
				if (video.isBackuped()) {
					//video is available on cloud storage
					iframe = this.videoElement(video);
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
				description.append(iframe);
				//description => helper
				description.append($('<span />').addClass('video-rendered'));

				if (point) {
					//map
					description.append(this.mapRenderer.renderMiniMapFrame(point ? point.Position.Latitude : 0, point ? point.Position.Longitude : 0, 15, 180, 240));
					//paths
					button = $('<a href="#' + language.getString("Videos") + '" class="button path">' + this.language.getString('ShowMap') + '</a>');
					button.data("video", video);
					button.click(function () {
						var video = $(this).data("video");
						self.renderMap(dataModel.path(video.Time, new Date(video.Time.getTime() + (video.duration * 1000))), video.isBackuped() ? video : null);
					});
					description.append(button);
					//share: fb
					button = $('<div class="fb-share-button" data-width="200" />').attr('data-href', mapUrl(dataModel.path(video.Time, new Date(video.Time.getTime() + (video.duration * 1000)))));
					description.append(button);
					//share: twitter
					button = $('<a href="https://twitter.com/share" class="twitter-share-button" data-url=" " data-count="none" data-text="' + language.getString("VideoDescription_Tweet", data, true) + '">Tweet</a>');
					description.append(button);

					//fb refresh
					FB.XFBML.parse(item[0]);
					//twitter
					twttr.widgets.load();
				}
			}
		} else {
			//empty
			this.loader(description);
		}
	};

	/**
	 * @private
	 * Video element
	 * @param {copilot.model.Video} videoData
	 * @param {function(video: jQuery)=} complete
	 * @returns {jQuery}
	 */
	copilot.data.Renderer.prototype.videoElement = function (videoData, complete) {
		var url,
			video,
			source,
			interval,
			data = this.data,
			language = this.language,
			div = $('<div class="mediaframe" />');

		interval = setInterval(function () {
			//get url
			url = data.videoUrl(videoData.VideoBackup.Id);
			//if url exists
			if (url) {
				clearInterval(interval);
				//empty
				div.empty();
				//video
				video = $('<video/>').appendTo(div);
				//css
				video.css({
					width: div.width(),
					height: div.height()
				});
				//events
				video.bind("canplay", function () {
					video.attr('controls', "true");
				});
				//add source
				source = $('<source src="' + url + '" type="video/mp4">');
				source.bind("error", function () {
					//not backuped
					var iframe = $('<div class="noiframe error"></div>'),
						text = $('<div></div>').appendTo(iframe);
					text.append(language.getString('MediaError'));
					div.replaceWith(iframe);
				});
				//append
				video.append(source);
				//complete
				if (complete) {
					complete(video);
				}
			}
		}, 1000);

		//load
		this.loader(div);

		return div;
	};


	//////////////////////////////////////////////////
	//////////////////////////// IMAGES
	//////////////////////////////////////////////////

	/**
	 * Render images
	 * @param {jQuery} parent
	 */
	copilot.data.Renderer.prototype.renderImages = function (parent) {
		var i,
			p,
			div,
			image,
			language = this.language,
			dataModel = this.data,
			images = dataModel.images() || null;

		//images
		if (images === null) {
			//loader
			parent.empty();
			this.loader(parent);
			return;
		}

		//title
		this.imageTitle("div-image", parent, language);

		//zero length
		if (images.length === 0) {
			$('<p>' + language.getString("EmptyImages") + '</p>').appendTo(parent);
			return;
		}

		for (i = 0; i < images.length; i++) {
			//image
			this.imageFrame(parent, images[i]);
		}

	};

	/**
	 * @private
	 * Image title
	 * @param {string} id
	 * @param {jQuery} parent
	 * @param {copilot.data.Language} language
	 */
	copilot.data.Renderer.prototype.imageTitle = function (id, parent, language) {
		var title,
			exists = parent.find('#' + id).length;

		if (exists) {
			return;
		}

		//clear
		//noinspection JSUnresolvedFunction
		parent.empty();
		//title
		title = $('<div class="title" />').attr('id', id).appendTo(parent);
		//h2
		$('<h2>' + language.getString("Images") + '</h2>').appendTo(title);
		$('<span class="byline">' + language.getString("Pictures_Motto") + '</span>').appendTo(title);
	};

	/**
	 * @private
	 * Image frame
	 * @param {jQuery} parent
	 * @param {copilot.model.Image} image
	 */
	copilot.data.Renderer.prototype.imageFrame = function (parent, image) {
		var item,
			link,
			div,
			text,
			info,
			point,
			paths,
			height,
			button,
			iframe,
			data = [],
			description,
			self = this,
			language = this.language,
			dataModel = this.data;

		/**
		 * Description
		 * @returns {string|*}
		 */
		function getDescription(image) {
			//data
			data[0] = image.Time.toLocaleDateString();
			//string
			return language.getString("ImageDescription", data);
		}

		//paths
		paths = dataModel.path(new Date(image.Time.getTime() - 10000), new Date(image.Time.getTime() + 10000));

		//data
		item = parent.find("#image-" + image.Time.getTime());
		info = item.find("p.info");
		description =  item.find("p.image");

		//not exists
		if (item.length === 0) {
			item = $('<p />').attr('id', "image-" + image.Time.getTime()).addClass('images').appendTo(parent);
			item.css('height', '400px');
			link = $('<div class="odometer"/>').appendTo(item);
			link.append('<div class="value">' + image.Time.toLocaleDateString() + '<span class="small">' + image.Time.toLocaleTimeString() + '</span></div>');

			//info
			info = $('<p />').addClass("info").html(getDescription(image));
			item.append(info);

			//description
			description = $('<p />').addClass("image");
			item.append(description);
			item.append($('<div />').css('clear', 'both'));
		} else {
			//set new html
			info.html(getDescription(image));
		}

		//if paths exists
		if (paths) {
			if (description.find('.image-rendered').length === 0) {
				//empty
				description.empty();
				//first
				point = paths.States[0];
				//iframe
				if (image.isBackuped()) {
					//image is available on cloud storage
					iframe = this.imageElement(image);
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
				description.append(iframe);
				//description => helper
				description.append($('<span />').addClass('image-rendered'));

				if (point) {
					//map
					description.append(this.mapRenderer.renderMiniMapFrame(point.Position.Latitude, point.Position.Longitude, 15, 180, 240));
					//paths
					button = $('<a href="#' + language.getString("Images") + '" class="button path">' + this.language.getString('ShowMap') + '</a>');
					button.data("image", image);
					button.click(function () {
						var image = $(this).data("image");
						self.renderMap(dataModel.path(new Date(image.Time.getTime() - 10000), new Date(image.Time.getTime() + 10000)));
					});
					description.append(button);
				}
			}
		} else {
			//empty
			this.loader(description);
		}
	};

	/**
	 * @private
	 * Image element
	 * @param {copilot.model.Image} imageData
	 * @returns {jQuery}
	 */
	copilot.data.Renderer.prototype.imageElement = function (imageData) {
		var url,
			image,
			interval,
			data = this.data,
			language = this.language,
			div = $('<div class="mediaframe" />');

		interval = setInterval(function () {
			//get url
			url = data.imageUrl(imageData.Backup.Id);
			//if url exists
			if (url) {
				clearInterval(interval);
				//empty
				div.empty();
				//image
				image = $('<image />').appendTo(div);
				//css
				image.css({
					width: div.width(),
					height: div.height()
				});
				image.bind("error", function () {
					//not backuped
					var iframe = $('<div class="noiframe error"></div>'),
						text = $('<div></div>').appendTo(iframe);
					text.append(language.getString('MediaError'));
					div.replaceWith(iframe);
				});
				//add source
				image.attr('src', url);
			}
		}, 1000);

		//load
		this.loader(div);

		return div;
	};

	//////////////////////////////////////////////////
	//////////////////////////// PATHS
	//////////////////////////////////////////////////

	/**
	 * Render paths
	 * @param {jQuery} parent
	 */
	copilot.data.Renderer.prototype.renderPaths = function (parent) {
		var i,
			p,
			div,
			language = this.language,
			dataModel = this.data,
			paths = dataModel.pathList() || null;

		//paths
		if (paths === null) {
			//loader
			parent.empty();
			this.loader(parent);
			return;
		}

		//title
		this.pathTitle("div-paths", parent, language);

		//zero length
		if (paths.length === 0) {
			$('<p>' + language.getString("EmptyPaths") + '</p>').appendTo(parent);
			return;
		}

		for (i = 0; i < paths.length; i++) {
			//path
			this.pathFrame(parent, /** @type {copilot.model.Path} */ paths[i]);
		}
	};

	/**
	 * @private
	 * Path title
	 * @param {string} id
	 * @param {jQuery} parent
	 * @param {copilot.data.Language} language
	 */
	copilot.data.Renderer.prototype.pathTitle = function (id, parent, language) {
		var title,
			exists = parent.find('#' + id).length;

		if (exists) {
			return;
		}

		//clear
		//noinspection JSUnresolvedFunction
		parent.empty();
		//title
		title = $('<div class="title" />').attr('id', id).appendTo(parent);
		//h2
		$('<h2>' + language.getString("Paths") + '</h2>').appendTo(title);
		$('<span class="byline">' + language.getString("Paths_Motto") + '</span>').appendTo(title);
	};

	/**
	 * @private
	 * Video frame
	 * @param {jQuery} parent
	 * @param {copilot.model.Path} path
	 */
	copilot.data.Renderer.prototype.pathFrame = function (parent, path) {
		var item,
			link,
			div,
			text,
			info,
			paths,
			height,
			button,
			data = [],
			pathStates,
			description,
			self = this,
			language = this.language,
			dataModel = this.data;

		/**
		 * Description
		 * @returns {string|*}
		 */
		function getDescription(path) {
			//data
			data[0] = path.StartDate.toLocaleDateString();
			data[1] = Math.round(path.ConsumedFuel * 1000) / 1000;
			data[2] = Math.round(path.TraveledDistance * 1000) / 1000;
			data[3] = path.Distance;
			data[4] = language.getString("FueledUnit");
			data[5] = copilot.App.timeDifference(path.EndDate, path.StartDate);
			//string
			return language.getString("PathDescription", data);
		}

		//paths
		pathStates = dataModel.path(path.StartDate, path.EndDate);

		//data
		item = parent.find("#path-" + path.Uid);
		info = item.find("p.info");
		description =  item.find("p.path");

		//not exists
		if (item.length === 0) {

			//item
			item = $('<p />').addClass('paths').attr('id', "path-" + path.Uid).appendTo(parent);
			item.css('height', '150px');
			link = $('<div class="odometer"/>').appendTo(item);
			link.append('<div class="value">' + path.StartDate.toLocaleDateString() + '<span class="small">' + path.StartDate.toLocaleTimeString() + '</span></div>');

			//info
			info = $('<p />').addClass("info").html(getDescription(path));
			item.append(info);

			//description
			description = $('<p />').addClass("path");
			item.append(description);
			item.append($('<div />').css('clear', 'both'));
		} else {
			//set new html
			info.html(getDescription(path));
		}

		//if pathStates exists
		if (pathStates) {
			//first
			if (pathStates.States[0] && description.find('.map-button').length === 0) {
				//empty
				description.empty();
				//paths
				button = $('<a href="#' + language.getString("Paths") + '" class="button path">' + this.language.getString('ShowMap') + '</a>');
				button.addClass('map-button');
				button.data("path", path);
				button.click(function () {
					var path = $(this).data("path");
					self.renderMap(dataModel.path(path.StartDate, path.EndDate));
				});
				description.append(button);
				//share: fb
				button = $('<div class="fb-share-button" data-width="200" />').attr('data-href', mapUrl(dataModel.path(path.StartDate, path.EndDate)));
				description.append(button);
				//share: twitter
				button = $('<a href="https://twitter.com/share" class="twitter-share-button" data-url=" " data-count="none" data-text="' + language.getString("PathDescription_Tweet", data, true) + '">Tweet</a>');
				description.append(button);

				//fb refresh
				FB.XFBML.parse(item[0]);
				//twitter
				twttr.widgets.load();
			}
		} else {
			//empty
			this.loader(description);
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
			skin = this.skin.getSkin(),
			language = this.language,
			fills = this.data.fills() || null;

		//loader
		parent.empty();
		this.loader(parent);

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
				graphData = fills.getFuelPriceTrendGraph(language, skin.Foreground);
				break;
			case 'TrendUnitsPerRefill':
				graphData = fills.getTrendUnitsPerRefillGraph(language, skin.Foreground);
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
			series: graphData.series
		});
	};

	/**
	 * Render map
	 * @param {copilot.model.Path} path
	 * @param {copilot.model.Video=} video
	 */
	copilot.data.Renderer.prototype.renderMap = function (path, video) {
		var frame;

		//clear
		if (path === null) {
			if (this.map) {
				this.map.remove();
			}
			this.map = null;
			return;
		}

		//frame
		frame = this.mapRenderer.renderMapFrame(path, video);
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
	};

	/**
	 * Loader
	 * @param {jQuery} parent
	 */
	copilot.data.Renderer.prototype.loader = function (parent) {
		var createLoader = parent.find('.spinner').length === 0;
		if (createLoader) {
			$('<div class="spinner"><div class="rect1"></div><div class="rect2"></div><div class="rect3"></div><div class="rect4"></div><div class="rect5"></div></div>').appendTo(parent);
		}
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