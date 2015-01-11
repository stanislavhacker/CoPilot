/*global copilot */
/**
 * Maintenances
 */
(function () {

	copilot.model = copilot.model || {};

	/**
	 * Circuits
	 * @constructor
	 */
	copilot.model.Circuits = function () {
		/** @type {number}*/
		this.length = 0;
	};

	/**
	 * Clone
	 * @returns {copilot.model.Circuits}
	 */
	copilot.model.Circuits.prototype.clone = function () {
		var clone = new copilot.model.Circuits(),
			times = this,
			circuit,
			key;

		for (key in times) {
			if (times.hasOwnProperty(key)) {
				//update length
				clone.length++;
				//times
				circuit = times[key];
				clone[key] = circuit.clone ? circuit.clone() : new copilot.model.Circuit().clone.apply(circuit);
			}
		}

		return clone;
	};

	/**
	 * To json
	 * @returns {object}
	 */
	copilot.model.Circuits.prototype.toJSON = function () {
		var data = [],
			i;
		for (i = 0; i < this.length; i++) {
			data.push(this[i]);
		}
		return data;
	};



	/**
	 * Circuit
	 * @constructor
	 */
	copilot.model.Circuit = function () {
		/** @type {Array.<copilot.model.States>}*/
		this.States = [];
		/** @type {Array.<number>}*/
		this.Laps = [];
		/** @type {string}*/
		this.Id = "";
		/** @type {string}*/
		this.Name = "";
		/** @type {Date}*/
		this.Start = new Date();
		/** @type {number}*/
		this.duration = 0;
	};

	/**
	 * Get path
	 * @returns {copilot.model.Path}
	 */
	copilot.model.Circuit.prototype.getPath = function () {
		var path = new copilot.model.Path();
		//null
		path.ConsumedFuel = null;
		path.Distance = null;
		path.TraveledDistance = null;
		//set
		path.StartDate = this.Start;
		path.EndDate = new Date(path.StartDate.getTime() + (this.duration * 1000));
		path.States = this.States;
		path.Uid = this.Id + "-" + this.Start.getTime();
		return path;
	};

	/**
	 * Clone
	 * @returns {copilot.model.Circuit}
	 */
	copilot.model.Circuit.prototype.clone = function () {
		var clone = new copilot.model.Circuit();

		clone.Start = typeof this.Start === "string" ?  new Date(this.Start) : this.Start;
		clone.duration = this.duration;
		clone.Name = this.Name;
		clone.Id = this.Id;
		clone.Laps = this.Laps.slice(0);
		clone.States = this.States.clone ? this.States.clone() : new copilot.model.States().clone.apply(this.States);
		return clone;
	};

}());
