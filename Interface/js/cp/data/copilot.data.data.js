/*global copilot */
/**
 * Language
 */
(function () {

	copilot.data = copilot.data || {};

	//url: api/data?command=setting&from=&to=&page=

	/**
	 * Data
	 * @param {copilot.data.Language} language
	 * @constructor
	 */
	copilot.data.Data = function (language) {
		/** @type {copilot.data.Language}*/
		this.language = language;
		/** @type {boolean}*/
		this.loading = true;
		/** @type {boolean}*/
		this.error = false;
		/** @type {copilot.data.Storage}*/
		this.storage = new copilot.data.Storage();
		/** @type {copilot.data.Renderer}*/
		this.renderer = null;
		//load data
		this.setting();
	};

	/**
	 * Load
	 * @return {copilot.model.Settings}
	 */
	copilot.data.Data.prototype.setting = function () {
		var self = this,
			settings,
			what = "setting";

		//return cached data
		if (self.storage.getData(what)) {
			return self.storage.getData(what);
		}

		//check for state
		if (self.storage.getState(what)) {
			return null;
		}
		self.storage.setState(what, true);

		//loading
		this.loading = true;
		this.error = false;

		this.get({
			command : what
		}, function (data) {
			//settings
			settings = data ? new copilot.model.Settings().clone.apply(data) : self.storage.getData(what, true);
			//set
			if (settings) {
				copilot.Distance = settings.Distance;
				copilot.Currency = settings.Currency;
				copilot.Unit = settings.Unit;
			}
			//set
			self.storage.setState(what, false);
			self.storage.setData(what, settings);
			self.loading = settings ? false : true;
			self.error = settings ? false : true;

			if (settings) {
				//re-render
				self.renderer.renderHeaderWrapper();
				self.renderer.renderPageContent();
			} else {
				setTimeout(function () {
					self.setting();
				}, 1000);
			}
		});

		return null;
	};

	/**
	 * Maintenances
	 * @return {copilot.model.Maintenances}
	 */
	copilot.data.Data.prototype.maintenances = function () {
		var self = this,
			maintenances,
			what = "maintenances";

		//return cached data
		if (self.storage.getData(what)) {
			return self.storage.getData(what);
		}

		//check for state
		if (self.storage.getState(what)) {
			return null;
		}
		self.storage.setState(what, true);

		this.get({
			command : what
		}, function (data) {
			//maintenances
			maintenances = data ? new copilot.model.Maintenances().clone.apply(data) : self.storage.getData(what, true);
			//set data
			self.storage.setState(what, false);
			self.storage.setData(what, maintenances);

			if (maintenances) {
				//re-render
				self.renderer.renderPageContent();
				self.renderer.renderBanners();
			} else {
				setTimeout(function () {
					self.maintenances();
				}, 1000);
			}
		});

		return null;
	};

	/**
	 * Fills
	 * @return {copilot.model.Fills}
	 */
	copilot.data.Data.prototype.fills = function () {
		var fills,
			self = this,
			what = "fills";

		//return cached data
		if (self.storage.getData(what)) {
			return self.storage.getData(what);
		}

		//check for state
		if (self.storage.getState(what)) {
			return null;
		}
		self.storage.setState(what, true);

		this.get({
			command : what
		}, function (data) {
			//fills
			fills = data ? new copilot.model.Fills().clone.apply(data) : self.storage.getData(what, true);
			//set data
			self.storage.setState(what, false);
			self.storage.setData(what, fills);

			if (fills) {
				//re-render
				self.renderer.renderPageContent();
				self.renderer.renderBanners();
			} else {
				setTimeout(function () {
					self.fills();
				}, 1000);
			}
		});

		return null;
	};

	/**
	 * Repairs
	 * @return {copilot.model.Repairs}
	 */
	copilot.data.Data.prototype.repairs = function () {
		var repairs,
			self = this,
			what = "repairs";

		//return cached data
		if (self.storage.getData(what)) {
			return self.storage.getData(what);
		}

		//check for state
		if (self.storage.getState(what)) {
			return null;
		}
		self.storage.setState(what, true);

		this.get({
			command : what
		}, function (data) {
			//repairs
			repairs = data ? new copilot.model.Repairs().clone.apply(data) : self.storage.getData(what, true);
			//set data
			self.storage.setState(what, false);
			self.storage.setData(what, repairs);

			if (repairs) {
				//re-render
				self.renderer.renderPageContent();
				self.renderer.renderBanners();
			} else {
				setTimeout(function () {
					self.repairs();
				}, 1000);
			}
		});

		return null;
	};

	/**
	 * Videos
	 * @return {copilot.model.Videos}
	 */
	copilot.data.Data.prototype.videos = function () {
		var videos,
			self = this,
			what = "videos";

		//return cached data
		if (self.storage.getData(what)) {
			return self.storage.getData(what);
		}

		//check for state
		if (self.storage.getState(what)) {
			return null;
		}
		self.storage.setState(what, true);

		this.get({
			command : what
		}, function (data) {
			//videos
			videos = data ? new copilot.model.Videos().clone.apply(data) : self.storage.getData(what, true);
			//set data
			self.storage.setState(what, false);
			self.storage.setData(what, videos);

			if (videos) {
				//re-render
				self.renderer.renderPageContent();
			} else {
				setTimeout(function () {
					self.videos();
				}, 1000);
			}
		});

		return null;
	};

	/**
	 * Video url
	 * @param {string} id
	 * @return {string|null}
	 */
	copilot.data.Data.prototype.videoUrl = function (id) {
		var videoUrl,
			self = this,
			command = "video-url",
			what = command + "-" + id;

		//return cached data
		if (self.storage.getData(what)) {
			return self.storage.getData(what);
		}

		//check for state
		if (self.storage.getState(what)) {
			return null;
		}
		self.storage.setState(what, true);

		this.get({
			command : command,
			what: id
		}, function (data) {
			//videoUrl
			videoUrl = data ? data : self.storage.getData(what, true);
			//set data
			self.storage.setState(what, false);
			self.storage.setData(what, videoUrl);

			if (videoUrl) {
				//re-render
				self.renderer.renderPageContent();
			} else {
				setTimeout(function () {
					self.videoUrl(id);
				}, 1000);
			}
		});

		return null;
	};

	/**
	 * Images
	 * @return {copilot.model.Images}
	 */
	copilot.data.Data.prototype.images = function () {
		var images,
			self = this,
			what = "pictures";

		//return cached data
		if (self.storage.getData(what)) {
			return self.storage.getData(what);
		}

		//check for state
		if (self.storage.getState(what)) {
			return null;
		}
		self.storage.setState(what, true);

		this.get({
			command : what
		}, function (data) {
			//images
			images = data ? new copilot.model.Images().clone.apply(data) : self.storage.getData(what, true);
			//set data
			self.storage.setState(what, false);
			self.storage.setData(what, images);

			if (images) {
				//re-render
				self.renderer.renderPageContent();
			} else {
				setTimeout(function () {
					self.images();
				}, 1000);
			}
		});

		return null;
	};

	/**
	 * Image url
	 * @param {string} id
	 * @return {string|null}
	 */
	copilot.data.Data.prototype.imageUrl = function (id) {
		var imageUrl,
			self = this,
			command = "image-url",
			what = command + "-" + id;

		//return cached data
		if (self.storage.getData(what)) {
			return self.storage.getData(what);
		}

		//check for state
		if (self.storage.getState(what)) {
			return null;
		}
		self.storage.setState(what, true);

		this.get({
			command : command,
			what: id
		}, function (data) {
			//imageUrl
			imageUrl = data ? data : self.storage.getData(what, true);
			//set data
			self.storage.setState(what, false);
			self.storage.setData(what, imageUrl);

			if (imageUrl) {
				//re-render
				self.renderer.renderPageContent();
			} else {
				setTimeout(function () {
					self.imageUrl(id);
				}, 1000);
			}
		});

		return null;
	};

	/**
	 * Get paths for dates
	 * @param {Date} from
	 * @param {Date} to
	 * @returns {copilot.model.Path}
	 */
	copilot.data.Data.prototype.path = function (from, to) {
		var path,
			self = this,
			command = "path",
			what = command + "-" +  from.getTime() + "-" +  to.getTime();

		//return cached data
		if (self.storage.getData(what)) {
			return self.storage.getData(what);
		}

		//check for state
		if (self.storage.getState(what)) {
			return null;
		}
		self.storage.setState(what, true);

		this.get({
			command : command,
			fromDate: from.toISOString(),
			toDate: to.toISOString()
		}, function (data) {
			//path
			path = data ? new copilot.model.Path().clone.apply(data) : self.storage.getData(what, true);
			//set data
			self.storage.setState(what, false);
			self.storage.setData(what, path);

			if (path) {
				//re-render
				self.renderer.renderPageContent();
			} else {
				setTimeout(function () {
					self.path(from, to);
				}, 1000);
			}
		});

		return null;
	};

	/**
	 * Get path list
	 * @returns {copilot.model.Paths}
	 */
	copilot.data.Data.prototype.pathList = function () {
		var pathList,
			self = this,
			command = "path-list",
			what = command;

		//return cached data
		if (self.storage.getData(what)) {
			return self.storage.getData(what);
		}

		//check for state
		if (self.storage.getState(what)) {
			return null;
		}
		self.storage.setState(what, true);

		this.get({
			command : command
		}, function (data) {
			//path list
			pathList = data ? new copilot.model.Paths().clone.apply(data) : self.storage.getData(what, true);
			//set data
			self.storage.setState(what, false);
			self.storage.setData(what, pathList);

			if (pathList) {
				//re-render
				self.renderer.renderPageContent();
			} else {
				setTimeout(function () {
					self.pathList();
				}, 1000);
			}
		});

		return null;
	};

	/**
	 * Run on device
	 * @param {string} what
	 * @param {string=} whatData
	 */
	copilot.data.Data.prototype.run = function (what, whatData) {

		//clear data
		switch(what) {
			case "add-maintenance":
				this.storage.setData("maintenances", null);
				break;
			case "add-fuel":
				this.storage.setData("fills", null);
				break;
			case "add-repair":
				this.storage.setData("repairs", null);
				break;
			default:
				break;
		}

		this.get({
			command : 'run',
			what: what,
			data: whatData
		}, function (data) {
			if (!data) {
				console.log('Command run for "' + what + '" cannot be invoke.');
			}
		});
	};

	/**
	 * Get maximum odometer
	 * @returns {number|null}
	 */
	copilot.data.Data.prototype.odometer = function () {
		var repairs = this.repairs(),
			fills = this.fills(),
			odometer = null;

		if (!repairs || !fills) {
			return odometer;
		}

		if (repairs[0]) {
			odometer = copilot.App.GetOdometerWithRightDistance(repairs[0].Odometer);
		}
		if (fills[0] && copilot.App.GetOdometerWithRightDistance(fills[0].Odometer) > odometer) {
			odometer = copilot.App.GetOdometerWithRightDistance(fills[0].Odometer);
		}

		return odometer;
	};

	/**
	 * Circuits
	 * @return {copilot.model.Circuits}
	 */
	copilot.data.Data.prototype.circuits = function () {
		var circuits,
			self = this,
			what = "circuits";

		//return cached data
		if (self.storage.getData(what)) {
			return self.storage.getData(what);
		}

		//check for state
		if (self.storage.getState(what)) {
			return null;
		}
		self.storage.setState(what, true);

		this.get({
			command : what
		}, function (data) {
			//fills
			circuits = data ? new copilot.model.Circuits().clone.apply(data) : self.storage.getData(what, true);
			//set data
			self.storage.setState(what, false);
			self.storage.setData(what, circuits);

			if (circuits) {
				//re-render
				self.renderer.renderPageSideBar();
				//page content
				if (copilot.Hash === "speedway/" + self.language.getString('Circuits')) {
					self.renderer.renderPageContent();
				}
			} else {
				setTimeout(function () {
					self.circuits();
				}, 1000);
			}
		});

		return null;
	};

	/**
	 * Times
	 * @return {copilot.model.CircuitGroups}
	 */
	copilot.data.Data.prototype.times = function () {
		var circuits,
			self = this,
			what = "times";

		//return cached data
		if (self.storage.getData(what)) {
			return self.storage.getData(what);
		}

		//check for state
		if (self.storage.getState(what)) {
			return null;
		}
		self.storage.setState(what, true);

		this.get({
			command : what
		}, function (data) {
			//fills
			circuits = data ? new copilot.model.CircuitGroups().clone.apply(data) : self.storage.getData(what, true);
			//set data
			self.storage.setState(what, false);
			self.storage.setData(what, circuits);

			if (circuits) {
				//re-render
				self.renderer.renderPageSideBar();
				//page content
				if (copilot.Hash === "speedway/" + self.language.getString('Times')) {
					self.renderer.renderPageContent();
				}
			} else {
				setTimeout(function () {
					self.times();
				}, 1000);
			}
		});

		return null;
	};

	/**
	 * @private
	 * get data
	 * @param {object} data
	 * @param {function} complete
	 */
	copilot.data.Data.prototype.get = function (data, complete) {
		//noinspection JSUnresolvedFunction
		$.ajax({
			type: "GET",
			url: copilot.URL + "api/data",
			data: data
		}).done(function(data) {
			complete(data);
		}).fail(function () {
			complete(null);
		});
	};

}());