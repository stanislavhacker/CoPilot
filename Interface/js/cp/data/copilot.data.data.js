/*global copilot */
/**
 * Language
 */
(function () {

	copilot.data = copilot || {};

	/**
	 * Language
	 * @constructor
	 */
	copilot.data.Data = function () {
		/** @type {boolean}*/
		this.loading = true;
		/** @type {{}}*/
		this.data = {};
		//load data
		this.load();
	};

	/**
	 * Load
	 */
	copilot.data.Data.prototype.load = function () {
		var self = this;

		//loading
		this.loading = true;

		//noinspection JSUnresolvedFunction
		$.ajax(copilot.URL + "api/data")
			.done(function(data) {
				self.data = data;
				self.loading = false;
				debugger;
			}).fail(function () {
				self.loading = false;
			});
	};

}());