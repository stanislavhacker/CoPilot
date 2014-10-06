/*global copilot */
/**
 * Maintenances
 */
(function () {

	copilot.model = copilot.model || {};

	/**
	 * States
	 * @constructor
	 */
	copilot.model.States = function () {
		/** @type {number}*/
		this.length = 0;
	};

	/**
	 * Clone
	 * @returns {copilot.model.States}
	 */
	copilot.model.States.prototype.clone = function () {
		var clone = new copilot.model.States(),
			states = this,
			state,
			key;

		for (key in states) {
			if (states.hasOwnProperty(key)) {
				//update length
				clone.length++;
				//states
				state = states[key];
				clone[key] = state.clone ? state.clone() : new copilot.model.State().clone.apply(state);
			}
		}

		return clone;
	};

	/**
	 * To json
	 * @returns {object}
	 */
	copilot.model.States.prototype.toJSON = function () {
		var data = [],
			i;
		for (i = 0; i < this.length; i++) {
			data.push(this[i]);
		}
		return data;
	};


	/**
	 * State
	 * @constructor
	 */
	copilot.model.State = function () {
		/** @type {number} */
		this.AcceleratorPedalPosition = 0;
		/** @type {number}*/
		this.EngineLoad = 0;
		/** @type {number}*/
		this.EngineOilTemperature = 0;
		/** @type {number}*/
		this.EngineReferenceTorque = 0;
		/** @type {number}*/
		this.FuelInjectionTiming = 0;
		/** @type {number}*/
		this.MaxAirFlowRate = 0;
		/** @type {number}*/
		this.Rpm = 0;
		/** @type {number}*/
		this.Speed = 0;
		/** @type {number}*/
		this.Temperature = 0;
		/** @type {number}*/
		this.ThrottlePosition = 0;
		/** @type {Date}*/
		this.Uptime = new Date();
		/** @type {Date}*/
		this.Time = new Date();
		/** @type {copilot.model.Position}*/
		this.Position = new copilot.model.Position();
	};

	/**
	 * Clone
	 * @returns {copilot.model.State}
	 */
	copilot.model.State.prototype.clone = function () {
		var clone = new copilot.model.State();

		clone.AcceleratorPedalPosition = this.AcceleratorPedalPosition;
		clone.EngineLoad = this.EngineLoad;
		clone.EngineOilTemperature = this.EngineOilTemperature;
		clone.EngineReferenceTorque = this.EngineReferenceTorque;
		clone.FuelInjectionTiming = this.FuelInjectionTiming;
		clone.MaxAirFlowRate = this.MaxAirFlowRate;
		clone.Rpm = this.Rpm;
		clone.Speed = this.Speed;
		clone.Temperature = this.Temperature;
		clone.ThrottlePosition = this.ThrottlePosition;
		clone.Uptime = typeof this.Uptime === "string" ?  new Date(this.Uptime) : this.Uptime;
		clone.Time = typeof this.Time === "string" ?  new Date(this.Time) : this.Time;

		if (this.Position !== null) {
			//string
			if (typeof this.Position === "string") {
				clone.Position = new copilot.model.Position(this.Position);
			//object
			} else {
				clone.Position = this.Position.clone ? this.Position.clone() : new copilot.model.Position().clone.apply(this.Position);
			}
		} else {
			clone.Position = null;
		}

		return clone;
	};

}());