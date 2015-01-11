/*global copilot */
/**
 * Settings
 */
(function () {

	copilot.model = copilot.model || {};

	/**
	 * Settings
	 * @constructor
	 */
	copilot.model.Settings = function () {
		/** @type {string}*/
		this.Currency = "CZK";
		/** @type {copilot.model.Distance}*/
		this.Distance = copilot.model.Distance.Km;
		/** @type {string}*/
		this.Consumption = "";

		/** @type {number}*/
		this.Repairs = 0;
		/** @type {number}*/
		this.Maintenances = 0;
		/** @type {number}*/
		this.Fills = 0;
		/** @type {number}*/
		this.Videos = 0;
		/** @type {number}*/
		this.Pictures = 0;
		/** @type {number}*/
		this.Paths = 0;
		/** @type {number}*/
		this.SummaryFuelPrice = 0;
		/** @type {number}*/
		this.SummaryRepairPrice = 0;
		/** @type {number}*/
		this.Liters = 0;

		/** @type {number}*/
		this.Circuits = 0;
		/** @type {number}*/
		this.Times = 0;
	};

	/**
	 * Clone
	 * @returns {copilot.model.Settings}
	 */
	copilot.model.Settings.prototype.clone = function () {
		var clone = new copilot.model.Settings();

		clone.Currency = this.Currency;
		clone.Distance = this.Distance;
		clone.Consumption = this.Consumption;
		clone.Repairs = this.Repairs;
		clone.Maintenances = this.Maintenances;
		clone.Fills = this.Fills;
		clone.Videos = this.Videos;
		clone.Pictures = this.Pictures;
		clone.Paths = this.Paths;
		clone.SummaryFuelPrice = this.SummaryFuelPrice ;
		clone.SummaryRepairPrice = this.SummaryRepairPrice;
		clone.Liters = this.Liters;
		clone.Circuits = this.Circuits;
		clone.Times = this.Times;

		return clone;
	};

	/**
	 * To json
	 * @returns {object}
	 */
	copilot.model.Settings.prototype.toJSON = function () {
		return this;
	};

}());