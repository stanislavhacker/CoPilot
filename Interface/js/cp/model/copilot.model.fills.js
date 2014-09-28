/*global copilot */
/**
 * Maintenances
 */
(function () {

	copilot.model = copilot || {};

	/**
	 * Fills
	 * @constructor
	 */
	copilot.model.Fills = function () {
		/** @type {number}*/
		this.length = 0;
	};

	/**
	 * Clone
	 * @returns {copilot.model.Fills}
	 */
	copilot.model.Fills.prototype.clone = function () {
		var clone = new copilot.model.Fills(),
			fills = this,
			fill,
			key;

		for (key in fills) {
			if (fills.hasOwnProperty(key)) {
				//update length
				clone.length++;
				//fills
				fill = fills[key];
				clone[key] = fill.clone ? fill.clone() : new copilot.model.Fill().clone.apply(fill);
			}
		}

		return clone;
	};

	/**
	 * Get graph
	 * @param {copilot.data.Language} language
	 * @returns {copilot.model.Graph}
	 */
	copilot.model.Fills.prototype.getFuelPriceTrendGraph = function (language) {
		var i,
			fill,
			categories = [],
			graph = new copilot.model.Graph(),
			series = new copilot.model.Graph.Series();

		series.name = language.getString("PricePerUnit");

		for (i = 0; i < this.length; i++) {
			//fill
			fill = this[i];
			//add categories
			categories.push(fill.Date.toLocaleDateString());
			//series
			series.data.push(fill.UnitPrice.Value);
		}

		//create
		graph.name = language.getString("FuelPriceTrend");
		graph.dataName = language.getString("PricePerUnit");
		graph.dataUnit = this.length ? fill.UnitPrice.Currency : "";
		graph.setCategories(categories);
		graph.addSeries(series);

		return graph;
	};

	/**
	 * Get graph
	 * @param {copilot.data.Language} language
	 * @returns {copilot.model.Graph}
	 */
	copilot.model.Fills.prototype.getTrendUnitsPerRefillGraph = function (language) {
		var i,
			fill,
			categories = [],
			graph = new copilot.model.Graph(),
			series = new copilot.model.Graph.Series();

		series.name = language.getString("Fueled");

		for (i = 0; i < this.length; i++) {
			//fill
			fill = this[i];
			//add categories
			categories.push(fill.Date.toLocaleDateString());
			//series
			series.data.push(fill.Refueled);
		}

		//create
		graph.name = language.getString("TrendUnitsPerRefill");
		graph.dataName = language.getString("Fueled");
		graph.setCategories(categories);
		graph.addSeries(series);

		return graph;
	};





	/**
	 * Fill
	 * @constructor
	 */
	copilot.model.Fill = function () {
		/** @type {Date} */
		this.Date = new Date();
		/** @type {copilot.model.Odometer}*/
		this.Odometer = new copilot.model.Odometer();
		/** @type {boolean}*/
		this.Full = false;
		/** @type {number}*/
		this.Refueled = 0;
		/** @type {copilot.model.Price}*/
		this.Price = new copilot.model.Price();
		/** @type {copilot.model.Price}*/
		this.UnitPrice = new copilot.model.Price();
	};

	/**
	 * Clone
	 * @returns {copilot.model.Fill}
	 */
	copilot.model.Fill.prototype.clone = function () {
		var clone = new copilot.model.Fill();

		clone.Date = typeof this.Date === "string" ?  new Date(this.Date) : this.Date;
		clone.Odometer = this.Odometer.clone ? this.Odometer.clone() : new copilot.model.Odometer().clone.apply(this.Odometer);
		clone.Full = this.Full;
		clone.Refueled = this.Refueled;
		clone.Price = this.Price.clone ? this.Price.clone() : new copilot.model.Price().clone.apply(this.Price);
		clone.UnitPrice = this.UnitPrice.clone ? this.UnitPrice.clone() : new copilot.model.Price().clone.apply(this.UnitPrice);

		return clone;
	};

}());