/*global copilot */
/**
 * Maintenances
 */
(function () {

	copilot.model = copilot.model || {};

	/**
	 * Repairs
	 * @constructor
	 */
	copilot.model.Repairs = function () {
		/** @type {number}*/
		this.length = 0;
	};

	/**
	 * Clone
	 * @returns {copilot.model.Repairs}
	 */
	copilot.model.Repairs.prototype.clone = function () {
		var clone = new copilot.model.Repairs(),
			repairs = this,
			repair,
			key;

		for (key in repairs) {
			if (repairs.hasOwnProperty(key)) {
				//update length
				clone.length++;
				//repairs
				repair = repairs[key];
				clone[key] = repair.clone ? repair.clone() : new copilot.model.Repair().clone.apply(repair);
			}
		}

		return clone;
	};





	/**
	 * Repair
	 * @constructor
	 */
	copilot.model.Repair = function () {
		/** @type {Date} */
		this.Date = new Date();
		/** @type {copilot.model.Odometer}*/
		this.Odometer = new copilot.model.Odometer();
		/** @type {copilot.model.Price}*/
		this.Price = new copilot.model.Price();
		/** @type {string}*/
		this.Description = "";
		/** @type {string}*/
		this.ServiceName = "";
	};

	/**
	 * Clone
	 * @returns {copilot.model.Repair}
	 */
	copilot.model.Repair.prototype.clone = function () {
		var clone = new copilot.model.Repair();

		clone.Date = typeof this.Date === "string" ?  new Date(this.Date) : this.Date;
		clone.Odometer = this.Odometer.clone ? this.Odometer.clone() : new copilot.model.Odometer().clone.apply(this.Odometer);
		clone.Price = this.Price.clone ? this.Price.clone() : new copilot.model.Price().clone.apply(this.Price);
		clone.Description = this.Description;
		clone.ServiceName = this.ServiceName;

		return clone;
	};

}());