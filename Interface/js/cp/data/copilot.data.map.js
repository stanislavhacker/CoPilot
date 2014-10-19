/*global copilot, google */
/**
 * Skin
 */
(function () {

	copilot.data = copilot.data || {};

	/**
	 * Create state table
	 * @param {copilot.model.State} state
	 * @param {copilot.data.Language} language
	 * @param {number|string=} width
	 * @return {jQuery}
	 */
	function createStateTable(state, language, width) {
		//content
		var div = $('<table />').css({
			'width': width || 200
		});
		div.append($('<tr class="speed"><td><strong>' + language.getString('Speed')  + ':</strong></td><td class="value">' + state.Speed + '</td></tr>'));
		div.append($('<tr class="rpm"><td><strong>' + language.getString('ObdRpm')  + ':</strong></td><td class="value">' + state.Rpm + '</td></tr>'));
		div.append($('<tr class="temperature"><td><strong>' + language.getString('ObdTemperature')  + ':</strong></td><td class="value">' + state.Temperature + '</td></tr>'));
		div.append($('<tr><td><strong>' + language.getString('ObdAcceleratorPedalPosition')  + ':</strong></td><td class="value">' + state.AcceleratorPedalPosition + '</td></tr>'));
		div.append($('<tr><td><strong>' + language.getString('ObdEngineLoad')  + ':</strong></td><td class="value">' + state.EngineLoad + '</td></tr>'));
		div.append($('<tr><td><strong>' + language.getString('ObdEngineOilTemperature')  + ':</strong></td><td class="value">' + state.EngineOilTemperature + '</td></tr>'));
		div.append($('<tr><td><strong>' + language.getString('ObdEngineReferenceTorque')  + ':</strong></td><td class="value">' + state.EngineReferenceTorque + '</td></tr>'));
		div.append($('<tr><td><strong>' + language.getString('ObdFuelInjectionTiming')  + ':</strong></td><td class="value">' + state.FuelInjectionTiming + '</td></tr>'));
		div.append($('<tr><td><strong>' + language.getString('ObdMaxAirFlowRate')  + ':</strong></td><td class="value">' + state.MaxAirFlowRate + '</td></tr>'));
		div.append($('<tr><td><strong>' + language.getString('ObdThrottlePosition')  + ':</strong></td><td class="value">' + state.ThrottlePosition + '</td></tr>'));

		return div;
	}

	//noinspection JSValidateJSDoc
	/**
	 * Create marker
	 * @param {object} map
	 * @param {copilot.model.State} state
	 * @param {copilot.data.Language} language
	 * @return {google.maps.Marker} marker
	 */
	function createMarker(map, state, language) {
		var div,
			window,
			marker;

		//marker
		//noinspection JSUnresolvedFunction,JSUnresolvedVariable
		marker = new google.maps.Marker({
			position: new google.maps.LatLng(state.Position.Latitude, state.Position.Longitude),
			//map: map,
			title: language.getString('Speed') + ": "  + state.Speed
		});

		//content
		div = createStateTable(state, language);
		//window
		//noinspection JSUnresolvedFunction,JSUnresolvedVariable
		window = new google.maps.InfoWindow({
			content: div[0].outerHTML
		});
		//event
		//noinspection JSUnresolvedFunction,JSUnresolvedVariable
		google.maps.event.addListener(marker, 'click', function() {
			window.open(map, marker);
		});
		//return
		return marker;
	}


	/**
	 * Map
	 * @param {copilot.data.Renderer} renderer
	 * @constructor
	 */
	copilot.data.Map = function (renderer) {
		/** @type {copilot.data.Skin}*/
		this.skin = renderer.skin;
		/** @type {copilot.data.Language}*/
		this.language = renderer.language;
		/** @type {copilot.data.Data}*/
		this.data = renderer.data;
		/** @type {copilot.data.Renderer}*/
		this.renderer = renderer;

		/** @type {number}*/
		this.lastSpeed = null;
		/** @type {number}*/
		this.lastRpm = null;
		/** @type {number}*/
		this.lastTemperature = null;
	};

	//noinspection JSValidateJSDoc
	/**
	 * @private
	 * Get center
	 * @param {copilot.model.Path} path
	 * @returns {google.maps.LatLng}
	 */
	copilot.data.Map.prototype.getCenter = function (path) {
		var i;
		//states
		for (i = 0; i < path.States.length; i++) {
			if (path.States[i].Position) {
				//center
				//noinspection JSUnresolvedFunction,JSUnresolvedVariable
				return new google.maps.LatLng(path.States[i].Position.Latitude, path.States[i].Position.Longitude);
			}
		}
		//null
		return null;
	};

	/**
	 * @private
	 * Get full map view
	 * @param {copilot.model.Video=} video
	 * @returns {{map: jQuery, info: jQuery, inner: jQuery, video: jQuery, overlay: jQuery, table: jQuery}}
	 */
	copilot.data.Map.prototype.getView = function (video) {
		var info,
			inner,
			table,
			width,
			height,
			overlay,
			videoElement,
			videoMap = 300,
			win = $(window),
			infoHeight = 30,
			skin = this.skin,
			skinData = skin.getSkin(),
			header = $("#header").height(),
			map = $('<div id="mapviewer fullscreen" />');


		//info
		info = $('<div class="info-panel" />').css({
			width: '100%',
			height: infoHeight,
			background: skinData.Foreground
		});

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
		inner = $("<div></div>");
		inner.css({
			position: 'absolute',
			top: infoHeight,
			right: 0,
			width: video ? videoMap : width,
			height: height - header - infoHeight
		});

		//video element
		videoElement = $('<div class="full-video"></div>');
		videoElement.css({
			position: 'absolute',
			top: infoHeight,
			left: 0,
			width: video ? width - videoMap : 0,
			height: height - header - infoHeight
		});


		//graphs
		overlay = $('<div />').appendTo(videoElement);
		overlay.css({
			width: '100%',
			height: 300,
			position: 'absolute',
			top: 0,
			left: 0,
			zIndex: '3'
		});

		table = $('<div class="states-table" />').appendTo(videoElement);

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
				width: video ? videoMap : width,
				height: height - header - infoHeight
			});
			//inner
			videoElement.css({
				width: video ? width - videoMap : 0,
				height: height - header - infoHeight
			});
			videoElement.find('video').css({
				width: video ? width - videoMap : 0,
				height: height - header - infoHeight
			});
		});

		//append
		map.append(info);
		map.append(videoElement);
		map.append(inner);

		//return
		return {
			table: table,
			map: map,
			info: info,
			inner: inner,
			video: videoElement,
			overlay: overlay
		};
	};

	/**
	 * @private
	 * Speed graph
	 * @param {jQuery} parent
	 * @param {function(chart: Highcharts.Chart)} loaded
	 */
	copilot.data.Map.prototype.statesGraph = function (parent, loaded) {
		var graphData = new copilot.model.Graph(),
			language = this.language,
			graphElement,
			series,
			name;

		//name
		graphData.name = this.language.getString('Data');
		graphData.type = 'spline';

		//speed
		series = new copilot.model.Graph.Series();
		series.name = this.language.getString('Speed');
		series.color = '#F0A30A';
		series.lineWidth = 4;
		graphData.addSeries(series);

		//rpm
		series = new copilot.model.Graph.Series();
		series.name = this.language.getString('ObdRpm');
		series.color = '#A4C400';
		series.lineWidth = 3;
		graphData.addSeries(series);

		//temperature
		series = new copilot.model.Graph.Series();
		series.name = this.language.getString('ObdTemperature');
		series.color = '#E51400';
		series.lineWidth = 1;
		graphData.addSeries(series);

		//name
		name = $('<div class="title" />').appendTo(parent);
		name.text(graphData.name);
		name.css({
			position: 'absolute',
			top: 10,
			left: 10
		});

		//create
		graphElement = $('<div />').appendTo(parent);
		//noinspection JSUnresolvedFunction
		graphElement.highcharts({
			chart: {
				height: parent.height(),
				marginRight: 10,
				type: 'spline',
				animation: Highcharts.svg,
				events: {
					load: function () {
						if (loaded) {
							loaded(this);
						}
					}
				},
				backgroundColor: 'rgba(255, 255, 255, 0.01)'
			},
			credits: {
				enabled: false
			},
			title: {
				text: null
			},
			xAxis: {
				title: {
					enabled: false
				},
				tickPixelInterval: 150,
				type: 'datetime',

				lineWidth: 0,
				gridLineWidth: 0,
				minorGridLineWidth: 0,
				lineColor: 'transparent',
				labels: {
					enabled: false
				},
				minorTickLength: 0,
				tickLength: 0
			},
			yAxis: {
				title: {
					enabled: false
				},
				lineWidth: 0,
				gridLineWidth: 0,
				minorGridLineWidth: 0,
				lineColor: 'transparent',
				labels: {
					enabled: false
				},
				minorTickLength: 0,
				tickLength: 0
			},
			tooltip: {
				formatter: function () {
					var name = this.series.name,
						html = "";

					//html
					html += '<b>' + name + '</b><br/>';

					switch (name) {
						case language.getString('Speed'):
							html += Highcharts.numberFormat(this.y / 20, 0);
							break;
						case language.getString('ObdRpm'):
							html += Highcharts.numberFormat(this.y, 0);
							break;
						case language.getString('ObdTemperature'):
							html += Highcharts.numberFormat(this.y / 10, 0);
							break;
					}

					return html;
				}
			},
			legend: {
				enabled: false
			},
			exporting: {
				enabled: false
			},
			plotOptions: {
				series: {
					marker: {
						enabled: false
					}
				}
			},
			series: graphData.series
		});

	};

	//noinspection JSValidateJSDoc
	/**
	 * @private
	 * Get state
	 * @param {google.maps.Map} googleMap
	 * @param {copilot.model.Path} path
	 * @param {copilot.model.Video} video
	 * @returns {{positions: Array, markers: Array}}
	 */
	copilot.data.Map.prototype.getStates = function (googleMap, path, video) {
		var i,
			state,
			marker,
			position,
			markers = [],
			positions = [],
			language = this.language;

		//states
		for (i = 0; i < path.States.length; i++) {
			state = path.States[i];
			if (state.Position) {
				//create
				//noinspection JSUnresolvedFunction,JSUnresolvedVariable
				position = new google.maps.LatLng(state.Position.Latitude, state.Position.Longitude);
				positions.push(position);
				//marker
				if (!video) {
					marker = createMarker(googleMap, state, language);
					markers.push(marker);
				}
			}
		}

		return {
			positions: positions,
			markers: markers
		};
	};

	/**
	 * @private
	 * getStateForTime
	 * @param {copilot.model.Path} path
	 * @param {copilot.model.Video} video
	 * @param {number} time
	 * @returns {copilot.model.State}
	 */
	copilot.data.Map.prototype.getStateForTime = function (path, video, time) {
		var i,
			state,
			start = video.Time.getTime();

		//update time
		time = start + time;

		//states
		for (i = 0; i < path.States.length; i++) {
			state = path.States[i];
			if (time <= state.Time.getTime() && state.Position) {
				return state;
			}
		}
		return null;
	};

	//noinspection JSValidateJSDoc
	/**
	 * @private
	 * updateForState
	 * @param {copilot.model.State} state
	 * @param {google.maps.Marker} marker
	 * @param {google.maps.Map} googleMap
	 * @param {Array.<Highcharts.Series>} series
	 */
	copilot.data.Map.prototype.updateForState = function (state, marker, googleMap, series) {
		//get state
		var position,
			createPoint;

		//state exists
		if (state) {
			//noinspection JSUnresolvedFunction,JSUnresolvedVariable
			position = new google.maps.LatLng(state.Position.Latitude, state.Position.Longitude);
			//noinspection JSUnresolvedFunction,JSUnresolvedVariable
			marker.setPosition(position);
			//noinspection JSUnresolvedFunction,JSUnresolvedVariable
			googleMap.setCenter(position);

			//create point
			createPoint = this.lastSpeed !== state.Speed ||
				this.lastRpm !== state.Rpm ||
				this.lastTemperature !== state.Temperature;

			//Speed on graph
			if (series[0] && createPoint) {
				this.lastSpeed = state.Speed;
				//noinspection JSUnresolvedFunction
				series[0].addPoint([state.Time, state.Speed * 20], true, series[0].points.length > 30);
			}

			//Rpm on graph
			if (series[1] && createPoint) {
				this.lastRpm = state.Rpm;
				//noinspection JSUnresolvedFunction
				series[1].addPoint([state.Time, state.Rpm], true, series[1].points.length > 30);
			}

			//Temperature on graph
			if (series[2] && createPoint) {
				this.lastTemperature = state.Temperature;
				//noinspection JSUnresolvedFunction
				series[2].addPoint([state.Time, state.Temperature * 10], true, series[2].points.length > 30);
			}


		}
	};

	/**
	 * Clear
	 * @param {Array.<Highcharts.Series>} series
	 */
	copilot.data.Map.prototype.clearGraph = function (series) {
		//speed reset
		if (series[0]) {
			this.lastSpeed = null;
			series[0].setData([], true, true);
		}
		//rpm reset
		if (series[1]) {
			this.lastRpm = null;
			series[1].setData([], true, true);
		}
		//temperature reset
		if (series[2]) {
			this.lastTemperature = null;
			series[2].setData([], true, true);
		}
	};

	/**
	 * Get map frame
	 * @param {copilot.model.Path} path
	 * @param {copilot.model.Video=} video
	 * @returns {jQuery}
	 */
	copilot.data.Map.prototype.renderMapFrame = function (path, video) {
		var view,
			series,
			marker,
			center,
			data = [],
			googleMap,
			self = this,
			videoElement,
			skin = self.skin,
			language = self.language,
			skinData = skin.getSkin(),
			header = $("#header").height();

		//states
		center = this.getCenter(path);
		//view
		view = this.getView(video);

		//data
		data[0] = Math.round(path.TraveledDistance * 10) / 10; //distance
		data[1] = copilot.Distance;
		data[2] = copilot.App.timeDifference(path.EndDate, path.StartDate);
		data[3] = Math.round(path.ConsumedFuel * 100) / 100; //fuel
		data[4] = language.getString('FueledUnit');
		//info
		view.info.append(language.getString('RouteDescription', data));

		//video
		if (video) {
			//video
			videoElement = this.renderer.videoElement(video, "map", function (videoTag) {

				//graph
				self.statesGraph(view.overlay, function (graph) {
					//save graph series
					//noinspection JSUnresolvedVariable
					series = graph.series;
				});

				//bind
				videoTag.bind("timeupdate", function () {
					var state;
					//marker exists
					if (marker) {
						//state
						state = self.getStateForTime(path, video, videoTag[0].currentTime * 1000);
						//updateForState
						self.updateForState(state, marker, googleMap, series);
						//table of state
						if (state) {
							view.table.empty();
							view.table.append(createStateTable(state, language, 'auto'));
						}
					}
				});

				videoTag.bind("seeking", function () {
					self.clearGraph(series || []);
				});
			});
			//append
			view.video.append(videoElement);
		}

		//map
		setTimeout(function () {
			var route,
				mapData;

			//google map
			//noinspection JSUnresolvedFunction,JSUnresolvedVariable,JSCheckFunctionSignatures
			googleMap = new google.maps.Map(view.inner[0], {
				zoom: 16,
				center: center,
				disableDefaultUI: false,
				mapTypeId: google.maps.MapTypeId.ROADMAP
			});
			//get map data
			//noinspection JSValidateTypes
			mapData = self.getStates(googleMap, path, video);
			//route
			//noinspection JSUnresolvedFunction,JSUnresolvedVariable
			route = new google.maps.Polyline({
				path: mapData.positions,
				geodesic: false,
				strokeColor: skinData.Foreground,
				strokeOpacity: 1.0,
				strokeWeight: 10
			});
			//set map
			//noinspection JSUnresolvedFunction
			route.setMap(googleMap);
			//noinspection JSUnusedLocalSymbols,JSUnresolvedFunction
			var markerCluster = new MarkerClusterer(googleMap, mapData.markers, {
				gridSize: 100,
				maxZoom: 16
			});

			//for video
			if (video) {
				//noinspection JSUnusedAssignment, JSUnresolvedVariable,JSUnresolvedFunction
				marker = new google.maps.Marker({
					position: center,
					map: googleMap,
					icon: {
						path: google.maps.SymbolPath.CIRCLE,
						scale: 8,
						strokeColor: '#000000',
						strokeWeight: 8
					}
				});
			}

		}, 500);

		return view.map;
	};


	/**
	 * Get mini map frame
	 * @param {number} longitude
	 * @param {number} latitude
	 * @param {number=} mapZoom
	 * @param {number=} width
	 * @param {number=} height
	 * @returns {jQuery}
	 */
	copilot.data.Map.prototype.renderMiniMapFrame = function (longitude, latitude, mapZoom, width, height) {
		var inner,
			center,
			zoom = mapZoom || 13,
			map = $('<div class="mapviewer" />');

		width = width || 500;
		height = height || 400;

		//center
		//noinspection JSUnresolvedVariable,JSUnresolvedFunction
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
			//noinspection JSUnresolvedVariable,JSCheckFunctionSignatures
			var marker,
				googleMap = new google.maps.Map(inner[0], {
					zoom: zoom,
					center: center,
					disableDefaultUI: true,
					mapTypeId: google.maps.MapTypeId.ROADMAP
				});

			//noinspection JSUnusedAssignment, JSUnresolvedVariable,JSUnresolvedFunction
			marker = new google.maps.Marker({
				position: center,
				map: googleMap
			});

		}, 200);

		map.append(inner);

		return map;
	}

}());