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
		/** @type {{}}*/
		this.data = {};
		/** @type {{}}*/
		this.states = {};
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
			what = "setting";

		//return cached data
		if (self.data[what]) {
			return self.data[what];
		}

		//check for state
		if (self.states[what]) {
			return null;
		}
		self.states[what] = true;

		//loading
		this.loading = true;
		this.error = false;

		this.get({
			command : what
		}, function (data) {
			self.states[what] = false;
			self.data[what] = data ? new copilot.model.Settings().clone.apply(data) : null;
			self.loading = false;
			self.error = data === null;

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
		if (self.data[what]) {
			return self.data[what];
		}

		//check for state
		if (self.states[what]) {
			return null;
		}
		self.states[what] = true;

		this.get({
			command : what
		}, function (data) {
			self.states[what] = false;
			self.data[what] =  data ? new copilot.model.Maintenances().clone.apply(data) : null;

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
		if (self.data[what]) {
			return self.data[what];
		}

		//check for state
		if (self.states[what]) {
			return null;
		}
		self.states[what] = true;

		this.get({
			command : what
		}, function (data) {
			self.states[what] = false;
			self.data[what] =  data ? new copilot.model.Fills().clone.apply(data) : null;

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
		if (self.data[what]) {
			return self.data[what];
		}

		//check for state
		if (self.states[what]) {
			return null;
		}
		self.states[what] = true;

		this.get({
			command : what
		}, function (data) {
			self.states[what] = false;
			self.data[what] = data ? new copilot.model.Repairs().clone.apply(data) : null;

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
		if (self.data[what]) {
			return self.data[what];
		}

		//check for state
		if (self.states[what]) {
			return null;
		}
		self.states[what] = true;

		this.get({
			command : what
		}, function (data) {
			self.states[what] = false;
			self.data[what] = data ? new copilot.model.Videos().clone.apply(data) : null;

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
		if (self.data[what]) {
			return self.data[what];
		}

		//check for state
		if (self.states[what]) {
			return null;
		}
		self.states[what] = true;

		this.get({
			command : what
		}, function (data) {
			self.states[what] = false;
			self.data[what] = data ? new copilot.model.Images().clone.apply(data) : null;

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
	 * @returns {copilot.model.States}
	 */
	copilot.data.Data.prototype.paths = function (from, to) {
		var self = this,
			command = "paths",
			what = command + "-" +  from.getTime() + "-" +  to.getTime();

		//return cached data
		if (self.data[what]) {
			return self.data[what];
		}

		//check for state
		if (self.states[what]) {
			return null;
		}
		self.states[what] = true;

		this.get({
			command : command,
			fromDate: from.toISOString(),
			toDate: to.toISOString()
		}, function (data) {
			self.states[what] = false;
			self.data[what] = data ? new copilot.model.States().clone.apply(data) : null;

			if (data) {
				//re-render
				self.renderer.renderPageContent();
			} else {
				setTimeout(function () {
					self.paths(from, to);
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
		if (self.data[what]) {
			return self.data[what];
		}

		//check for state
		if (self.states[what]) {
			return null;
		}
		self.states[what] = true;

		this.get({
			command : command
		}, function (data) {
			self.states[what] = false;
			self.data[what] = data ? new copilot.model.Paths().clone.apply(data) : null;

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
				this.data["maintenances"] = null;
				break;
			case "add-fuel":
				this.data["fills"] = null;
				break;
			case "add-repair":
				this.data["repairs"] = null;
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
			odometer = repairs[0].Odometer.Value;
		}
		if (fills[0] && fills[0].Odometer.Value > odometer) {
			odometer = fills[0].Odometer.Value;
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