/*global copilot */
/**
 * Maintenances
 */
(function () {

	copilot.model = copilot || {};

	/**
	 * Paths
	 * @constructor
	 */
	copilot.model.Paths = function () {
		/** @type {number}*/
		this.length = 0;
	};

	/**
	 * Clone
	 * @returns {copilot.model.Paths}
	 */
	copilot.model.Paths.prototype.clone = function () {
		var clone = new copilot.model.Paths(),
			paths = this,
			path,
			key;

		for (key in paths) {
			if (paths.hasOwnProperty(key)) {
				//update length
				clone.length++;
				//paths
				path = paths[key];
				clone[key] = path.clone ? path.clone() : new copilot.model.Path().clone.apply(path);
			}
		}

		return clone;
	};





	/**
	 * Path
	 * @constructor
	 */
	copilot.model.Path = function () {
		/** @type {Date} */
		this.StartDate = new Date();
		/** @type {Date} */
		this.EndDate = new Date();
		/** @type {number}*/
		this.ConsumedFuel = 0;
		/** @type {number}*/
		this.TraveledDistance = 0;
		/** @type {string}*/
		this.Distance = "";
		/** @type {string}*/
		this.Uid = "";
	};

	/**
	 * Clone
	 * @returns {copilot.model.Path}
	 */
	copilot.model.Path.prototype.clone = function () {
		var clone = new copilot.model.Path();

		clone.StartDate = typeof this.StartDate === "string" ?  new Date(this.StartDate) : this.StartDate;
		clone.EndDate = typeof this.EndDate === "string" ?  new Date(this.EndDate) : this.EndDate;
		clone.ConsumedFuel = this.ConsumedFuel;
		clone.TraveledDistance = this.TraveledDistance;
		clone.Distance = this.Distance;
		clone.Uid = this.Uid;

		return clone;
	};

}());