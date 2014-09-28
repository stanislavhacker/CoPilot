/*global copilot */
/**
 * Maintenances
 */
(function () {

	copilot.model = copilot.model || {};

	/**
	 * Odometer
	 * @constructor
	 */
	copilot.model.Odometer = function () {
		/** @type {string}*/
		this.Distance = "";
		/** @type {number}*/
		this.Value = 0;
	};

	/**
	 * Clone
	 * @returns {copilot.model.Odometer}
	 */
	copilot.model.Odometer.prototype.clone = function () {
		var clone = new copilot.model.Odometer();
		clone.Distance = this.Distance;
		clone.Value = this.Value;
		return clone;
	};

	/**
	 * Price
	 * @constructor
	 */
	copilot.model.Price = function () {
		/** @type {string}*/
		this.Currency = "";
		/** @type {number}*/
		this.Value = 0;
	};

	/**
	 * Clone
	 * @returns {copilot.model.Price}
	 */
	copilot.model.Price.prototype.clone = function () {
		var clone = new copilot.model.Price();
		clone.Currency = this.Currency;
		clone.Value = this.Value;
		return clone;
	};

	/**
	 * Backup
	 * @constructor
	 */
	copilot.model.Backup = function () {
		/** @type {Date}*/
		this.Date = new Date();
		/** @type {string}*/
		this.Id = "";
		/** @type {string}*/
		this.Url = "";
	};

	/**
	 * Clone
	 * @returns {copilot.model.Backup}
	 */
	copilot.model.Backup.prototype.clone = function () {
		var clone = new copilot.model.Backup();
		clone.Date = typeof this.Date === "string" ? new Date(this.Date) : this.Date;
		clone.Id = this.Id;
		clone.Url = this.Url;
		return clone;
	};

	/**
	 * Positions
	 * @param {string} position
	 * @constructor
	 */
	copilot.model.Position = function (position) {
		var data = position && position.split(", ");
		/** @type {number}*/
		this.Latitude = position ? data[0] : 0;
		/** @type {number}*/
		this.Longitude = position ? data[1] : 0;
	};

	/**
	 * Clone
	 * @returns {copilot.model.Position}
	 */
	copilot.model.Position.prototype.clone = function () {
		var clone = new copilot.model.Position();
		clone.Latitude = this.Latitude;
		clone.Longitude = this.Longitude;
		return clone;
	};

}());