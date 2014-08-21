/*global copilot */
/**
 * Language
 */
(function () {

	copilot.data = copilot || {};

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
		/** @type {copilot.data.Renderer}*/
		this.renderer = null;
		//load data
		this.setting();
	};

	/**
	 * Load
	 * @return {{Currency:string,Distance:string,Consumption:string,Repairs:number,Maintenances:number,Fills:number,Videos:number,Pictures:number,Paths:number,SummaryFuelPrice:number,SummaryRepairPrice:number,Liters:number}}
	 */
	copilot.data.Data.prototype.setting = function () {
		var self = this,
			what = "setting";

		//return cached data
		if (self.data[what]) {
			return self.data[what];
		}

		//loading
		this.loading = true;
		this.error = false;

		this.get({
			command : what
		}, function (data) {
			self.data[what] = data;
			self.loading = false;
			self.error = data === null;
			//re-render
			self.renderer.renderHeaderWrapper();
			self.renderer.renderPageContent();
		});

		return null;
	};

	/**
	 * Settings
	 * @return {}
	 */
	copilot.data.Data.prototype.setting = function () {
		var self = this,
			what = "setting";

		//return cached data
		if (self.data[what]) {
			return self.data[what];
		}

		//loading
		this.loading = true;
		this.error = false;

		this.get({
			command : what
		}, function (data) {
			self.data[what] = data;
			self.loading = false;
			self.error = data === null;
			//re-render
			self.renderer.renderHeaderWrapper();
			self.renderer.renderPageContent();
		});

		return null;
	};

	/**
	 * Maintenances
	 * @return Array.<object>
	 */
	copilot.data.Data.prototype.maintenances = function () {
		var self = this,
			what = "maintenances";

		//return cached data
		if (self.data[what]) {
			return self.data[what];
		}

		this.get({
			command : what
		}, function (data) {
			self.data[what] = data;
			//re-render
			self.renderer.renderBanners();
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