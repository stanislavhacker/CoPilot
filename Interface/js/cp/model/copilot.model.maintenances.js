/*global copilot */
/**
 * Maintenances
 */
(function () {

	copilot.model = copilot.model || {};

	/**
	 * Maintenances
	 * @constructor
	 */
	copilot.model.Maintenances = function () {
		/** @type {number}*/
		this.length = 0;
	};

	/**
	 * Clone
	 * @returns {copilot.model.Maintenances}
	 */
	copilot.model.Maintenances.prototype.clone = function () {
		var clone = new copilot.model.Maintenances(),
			maintenances = this,
			maintenance,
			key;

		for (key in maintenances) {
			if (maintenances.hasOwnProperty(key)) {
				//update length
				clone.length++;
				//maintenances
				maintenance = maintenances[key];
				clone[key] = maintenance.clone ? maintenance.clone() : new copilot.model.Maintenance().clone.apply(maintenance);
			}
		}

		return clone;
	};

	/**
	 * Get warnings
	 * @param {number|null} odometer
	 * @returns {Array.<copilot.model.Maintenance>}
	 */
	copilot.model.Maintenances.prototype.getWarnings = function (odometer) {
		var maintenances = this,
			maintenanceOdometer,
			current = new Date().getTime(),
			filtered = [],
			maintenance,
			before,
			date,
			i;

		for (i = 0; i < maintenances.length; i++) {
			//maintenances
			maintenance = maintenances[i];
			//check state
			if (!maintenance.IsOdometer) {
				//data
				date = maintenance.Date.getTime();
				//maintenance
				before = maintenance.WarningDays * 86400000;
				//check before
				if (date - current < before) {
					filtered.push(maintenance);
				}
			}

			//odometer
			if (maintenance.IsOdometer && odometer !== null) {
				maintenanceOdometer = copilot.App.GetOdometerWithRightDistance(maintenance.Odometer);
				var sub = maintenanceOdometer - odometer;
				if (sub >= 0 && sub < maintenance.WarningDistance) {
					filtered.push(maintenance);
				}
			}
		}

		return filtered;
	};

	/**
	 * To json
	 * @returns {object}
	 */
	copilot.model.Maintenances.prototype.toJSON = function () {
		var data = [],
			i;
		for (i = 0; i < this.length; i++) {
			data.push(this[i]);
		}
		return data;
	};



	/**
	 * Settings
	 * @constructor
	 */
	copilot.model.Maintenance = function () {
		/** @type {Date} */
		this.Date = new Date();
		/** @type {copilot.model.Odometer}*/
		this.Odometer = new copilot.model.Odometer();
		/** @type {boolean}*/
		this.IsOdometer = false;
		/** @type {string}*/
		this.Distance = "";
		/** @type {string}*/
		this.Type = "";
		/** @type {string}*/
		this.Description = "";
		/** @type {string} */
		this.Id = "";
		/** @type {number}*/
		this.WarningDays = 0;
		/** @type {number}*/
		this.WarningDistance = 1000;
	};

	/**
	 * Clone
	 * @returns {copilot.model.Maintenance}
	 */
	copilot.model.Maintenance.prototype.clone = function () {
		var clone = new copilot.model.Maintenance();

		clone.Date = typeof this.Date === "string" ?  new Date(this.Date) : this.Date;
		clone.Odometer = this.Odometer.clone ? this.Odometer.clone() : new copilot.model.Odometer().clone.apply(this.Odometer);
		clone.IsOdometer = this.IsOdometer;
		clone.Distance = this.Distance;
		clone.Type = this.Type;
		clone.Description = this.Description;
		clone.Id = this.Id;
		clone.WarningDays = this.WarningDays;
		clone.WarningDistance = this.WarningDistance;

		return clone;
	};

	/**
	 * Get date
	 * @returns {string}
	 */
	copilot.model.Maintenance.prototype.getDate = function () {
		var date = this.Date;
		return date.getDate() + "." + (date.getMonth() + 1) + " " + date.getFullYear();
	};

}());