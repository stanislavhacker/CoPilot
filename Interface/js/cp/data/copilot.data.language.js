/*global copilot */
/**
 * Language
 */
(function () {

	copilot.data = copilot || {};

	/**
	 * Language
	 * @constructor
	 */
	copilot.data.Language = function () {
		/** @type {boolean}*/
		this.loading = true;
		/** @type {{}}*/
		this.data = {};
		/** @type {{}}*/
		this.cache = {};
		//load language
		this.load();
	};

	/**
	 * Load
	 */
	copilot.data.Language.prototype.load = function () {
		var self = this;

		//loading
		this.loading = true;

		//noinspection JSUnresolvedFunction
		$.ajax(copilot.URL + "api/language")
			.done(function(data) {
				self.applyLanguage(data);
			}).always(function () {
				self.loading = false;
			});
	};

	/**
	 * Apply language
	 * @param {{}} data
	 */
	copilot.data.Language.prototype.applyLanguage = function (data) {
		this.data = data;
	};

	/**
	 * Apply language
	 * @param {string} name
	 * @return {string}
	 */
	copilot.data.Language.prototype.getString = function (name) {
		var i,
			key,
			value,
			array = this.data;

		//from cache
		if (this.cache[name]) {
			return this.cache[name];
		}

		//find
		for (i = 0; i < array.length; i++) {
			//noinspection JSUnresolvedVariable
			key = array[i].Key;
			if (key === name) {
				//noinspection JSUnresolvedVariable
				value =  array[i].Value;
				this.cache[key] = value;
				return value;
			}
		}

		return "";
	};

}());