/*global copilot */
/**
 * Storage
 */
(function () {

	copilot.data = copilot.data || {};

	/**
	 * Storage
	 * @constructor
	 */
	copilot.data.Storage = function() {
		/** @type {map.<string, object>}*/
		this.data = {};
		/** @type {map.<string, object>}*/
		this.states = {};
		/** @type {boolean}*/
		this.isStorage = Boolean(typeof localStorage !== "undefined" && localStorage);
	};

	/**
	 * Get data
	 * @param {string} key
	 * @param {boolean} cached
	 * @returns {object|null}
	 */
	copilot.data.Storage.prototype.getData = function (key, cached) {
		var data = this.data[key];

		if (cached && this.isStorage) {
			//report it
			if (typeof console !== "undefined") {
				console.log('Load data for ' + key + ' from LocalStorage.');
			}
			//get data or null
			return this.getObject(key);
		}
		return data;
	};

	/**
	 * @private
	 * @param {string} key
	 * @returns {object}
	 */
	copilot.data.Storage.prototype.getObject = function (key) {
		var item,
			savedData = localStorage.getItem(key);

		if (!savedData) {
			localStorage.removeItem(key);
			return null;
		}

		//json parse
		item = JSON.parse(savedData);

		//switch
		switch(key) {
			case "path-list":
				return new copilot.model.Paths().clone.apply(item);
			case "pictures":
				return new copilot.model.Images().clone.apply(item);
			case "videos":
				return new copilot.model.Videos().clone.apply(item);
			case "repairs":
				return new copilot.model.Repairs().clone.apply(item);
			case "fills":
				return new copilot.model.Fills().clone.apply(item);
			case "maintenances":
				return new copilot.model.Maintenances().clone.apply(item);
			case "setting":
				return new copilot.model.Settings().clone.apply(item);
			case "skin":
				return new copilot.model.Skin().clone.apply(item);
			case "language":
				return item;
			default:
				//copilot.model.Path
				if (key.indexOf("path-") >= 0) {
					return new copilot.model.Path().clone.apply(item);
				}
				break;
		}

		return null;
	};

	/**
	 * Set data
	 * @param {string} key
	 * @param {object|null} data
	 */
	copilot.data.Storage.prototype.setData = function (key, data) {
		var json;
		if (this.isStorage) {
			//report it
			if (typeof console !== "undefined") {
				console.log('Save data for ' + key + ' into LocalStorage.');
			}
			//set data if null do nothing
			json = this.setObject(key, data);
			if (json) {
				localStorage.setItem(key, json);
			}
		}
		this.data[key] = data;
	};

	/**
	 * @private
	 * Set object
	 * @param {string} key
	 * @param {object} data
	 * @returns {string}
	 */
	copilot.data.Storage.prototype.setObject = function (key, data) {
		var string;

		//string
		if (!data) {
			localStorage.removeItem(key);
			return null;
		}

		//switch
		switch(key) {
			case "path-list":
				string = data.toJSON();
				break;
			case "pictures":
				string = data.toJSON();
				break;
			case "videos":
				string = data.toJSON();
				break;
			case "repairs":
				string = data.toJSON();
				break;
			case "fills":
				string = data.toJSON();
				break;
			case "maintenances":
				string = data.toJSON();
				break;
			case "setting":
				string = data.toJSON();
				break;
			case "skin":
				string = data.toJSON();
				break;
			case "language":
				string = data;
				break;
			default:
				//copilot.model.Path
				if (key.indexOf("path-") >= 0) {
					string = data.toJSON();
					break;
				}
		}

		return JSON.stringify(string);
	};

	/**
	 * Get state
	 * @param {string} key
	 * @returns {boolean}
	 */
	copilot.data.Storage.prototype.getState = function (key) {
		return this.states[key];
	};

	/**
	 * Set state
	 * @param {string} key
	 * @param {boolean} state
	 */
	copilot.data.Storage.prototype.setState = function (key, state) {
		this.states[key] = state;
	};

}());