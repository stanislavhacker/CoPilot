/*global copilot */
/**
 * Language
 */
(function () {

	copilot.data = copilot.data || {};

	//url: api/data?command=setting&from=&to=&page=

	/**
	 * Language
	 * @constructor
	 */
	copilot.data.Data = function () {
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
			}
			//set
			self.storage.setState(what, false);
			self.storage.setData(what, settings);
			self.loading = settings ? false : true;
			self.error = settings ? false : true;

			if (data) {
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
			self.storage.setState(what, false);
			self.storage.setData(what, data ? new copilot.model.Maintenances().clone.apply(data) : self.storage.getData(what, true));

			if (data) {
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
		var self = this,
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
			self.storage.setState(what, false);
			self.storage.setData(what, data ? new copilot.model.Fills().clone.apply(data) : self.storage.getData(what, true));

			if (data) {
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
		var self = this,
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
			self.storage.setState(what, false);
			self.storage.setData(what, data ? new copilot.model.Repairs().clone.apply(data) : self.storage.getData(what, true));

			if (data) {
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
		var self = this,
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
			self.storage.setState(what, false);
			self.storage.setData(what, data ? new copilot.model.Videos().clone.apply(data) : self.storage.getData(what, true));

			if (data) {
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
	 * Images
	 * @return {copilot.model.Images}
	 */
	copilot.data.Data.prototype.images = function () {
		var self = this,
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
			self.storage.setState(what, false);
			self.storage.setData(what, data ? new copilot.model.Images().clone.apply(data) : self.storage.getData(what, true));

			if (data) {
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
	 * Get paths for dates
	 * @param {Date} from
	 * @param {Date} to
	 * @returns {copilot.model.Path}
	 */
	copilot.data.Data.prototype.path = function (from, to) {
		var self = this,
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
			self.storage.setState(what, false);
			self.storage.setData(what, data ? new copilot.model.Path().clone.apply(data) : self.storage.getData(what, true));

			if (data) {
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
		var self = this,
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
			self.storage.setState(what, false);
			self.storage.setData(what, data ? new copilot.model.Paths().clone.apply(data) : self.storage.getData(what, true));

			if (data) {
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