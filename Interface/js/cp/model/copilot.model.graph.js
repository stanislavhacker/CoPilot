/*global copilot */
/**
 * Maintenances
 */
(function () {

	copilot.model = copilot.model || {};

	/**
	 * Graph
	 * @constructor
	 */
	copilot.model.Graph = function () {
		/** @type {string}*/
		this.name = "";
		/** @type {string}*/
		this.dataName = "";
		/** @type {string}*/
		this.dataUnit = "";
		/** @type {Array.<string>}*/
		this.categories = [];
		/** @type {Array.<copilot.model.Graph.Series>}*/
		this.series = [];
	};

	/**
	 * Set categories
	 * @param {Array.<string>} categories
	 */
	copilot.model.Graph.prototype.setCategories = function (categories) {
		this.categories = categories;
	};

	/**
	 * Add series
	 * @param {copilot.model.Graph.Series} series
	 */
	copilot.model.Graph.prototype.addSeries = function (series) {
		this.series.push(series);
	};



	/**
	 * Graph
	 * @constructor
	 */
	copilot.model.Graph.Series = function () {
		/** @type {string}*/
		this.name = "";
		/** @type {Array.<object>}*/
		this.data = [];
		/** @type {string}*/
		this.color = undefined;
		/** @type {number}*/
		this.lineWidth = undefined;
	};


}());