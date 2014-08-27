/*global copilot */
/**
 * Maintenances
 */
(function () {

	copilot.model = copilot || {};

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

}());