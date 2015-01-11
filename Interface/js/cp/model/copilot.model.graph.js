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
		/** @type {string}*/
		this.type = 'line';
	};

	/**
	 * Get chart
	 * @returns {*}
	 */
	copilot.model.Graph.prototype.getChart = function () {
		switch(this.type) {
			case "line":
				return {
					zoomType: 'x'
				};
			case "pie":
				return {
					plotBackgroundColor: null,
					plotBorderWidth: 1,
					plotShadow: false
				};
			case "column":
				return {
					type: 'column'
				};
		}
		return null;
	};

	/**
	 * Get title
	 * @returns {*}
	 */
	copilot.model.Graph.prototype.getTitle = function () {
		return {
			text: this.name
		};
	};

	/**
	 * Get tooltip
	 * @returns {*}
	 */
	copilot.model.Graph.prototype.getTooltip = function () {
		switch(this.type) {
			case "line":
				return {
					valueSuffix: this.dataUnit
				};
			case "pie":
				return {
					pointFormat: '{series.name}: <b>{point.y} ' + this.dataUnit + '</b>'
				};
			case "column":
				return {
					headerFormat: '<div><span style="font-size:10px">{point.key}</span>',
					pointFormat: '<br />{series.name}: <b>{point.y:.1f} ' + this.dataUnit + '</b>',
					footerFormat: '</div>',
					shared: true
				};
		}
		return null;
	};

	/**
	 * Get plot options
	 * @returns {*}
	 */
	copilot.model.Graph.prototype.getPlotOptions = function () {
		switch(this.type) {
			case "line":
				return {};
			case "pie":
				return {
					pie: {
						allowPointSelect: true,
						cursor: 'pointer',
						dataLabels: {
							enabled: true,
							format: '<b>{point.name}</b>: {point.y} ' + this.dataUnit
						}
					}
				};
			case "column":
				return {
					column: {
						pointPadding: 0.2,
						borderWidth: 0
					}
				}
		}
		return null;
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
		series.type = this.type;
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
		/** @type {string}*/
		this.type = 'line';
	};


}());