/*global copilot */
/**
 * Maintenances
 */
(function () {

	copilot.model = copilot.model || {};

	/**
	 * CircuitGroups
	 * @constructor
	 */
	copilot.model.CircuitGroups = function () {
		/** @type {number}*/
		this.length = 0;
	};

	/**
	 * Clone
	 * @returns {copilot.model.CircuitGroups}
	 */
	copilot.model.CircuitGroups.prototype.clone = function () {
		var clone = new copilot.model.CircuitGroups(),
			times = this,
			circuit,
			key;

		for (key in times) {
			if (times.hasOwnProperty(key)) {
				//update length
				clone.length++;
				//times
				circuit = times[key];
				clone[key] = circuit.clone ? circuit.clone() : new copilot.model.CircuitGroup().clone.apply(circuit);
			}
		}

		return clone;
	};

	/**
	 * To json
	 * @returns {object}
	 */
	copilot.model.CircuitGroups.prototype.toJSON = function () {
		var data = [],
			i;
		for (i = 0; i < this.length; i++) {
			data.push(this[i]);
		}
		return data;
	};



	/**
	 * CircuitGroup
	 * @constructor
	 */
	copilot.model.CircuitGroup = function () {
		/** @type {Array.<copilot.model.States>}*/
		this.States = [];
		/** @type {Array.<number>}*/
		this.Laps = [];
		/** @type {copilot.model.Circuit}*/
		this.Circuit = new copilot.model.Circuit();
		/** @type {string}*/
		this.Name = "";
		/** @type {number}*/
		this.FastestLap = 0;
	};

	/**
	 * Clone
	 * @returns {copilot.model.CircuitGroup}
	 */
	copilot.model.CircuitGroup.prototype.clone = function () {
		var clone = new copilot.model.CircuitGroup();

		clone.Circuit = this.Circuit.clone ? this.Circuit.clone() : new copilot.model.Circuit().clone.apply(this.Circuit);
		clone.FastestLap = this.FastestLap;
		clone.Name = this.Name;
		clone.Laps = this.Laps.slice(0);
		clone.States = this.States.clone ? this.States.clone() : new copilot.model.States().clone.apply(this.States);
		return clone;
	};

}());
