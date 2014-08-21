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
		/** @type {boolean}*/
		this.error = false;
		/** @type {{}}*/
		this.data = {};
		/** @type {{}}*/
		this.cache = {};
		/** @type {copilot.data.Renderer}*/
		this.renderer = null;
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
		this.error = false;

		//noinspection JSUnresolvedFunction
		$.ajax(copilot.URL + "api/language")
			.done(function(data) {
				self.applyLanguage(data);
				self.loading = false;
			}).fail(function () {
				self.error = true;
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
	 * Apply data
	 * @param {string} text
	 * @param {Array.<string>} data
	 * @returns {string}
	 */
	function applyData(text, data) {
		var i;

		if (!data || data.length === 0) {
			return text;
		}

		for (i = 0; i < data.length; i++) {
			text = text.replace(new RegExp("\\{" + i + "\\}", "g"), "<strong>" + data[i] + "</strong>");
		}

		return text;
	}

	/**
	 * Apply language
	 * @param {string} name
	 * @param {Array.<string>=} data
	 * @return {string}
	 */
	copilot.data.Language.prototype.getString = function (name, data) {
		var i,
			key,
			value,
			array = this.data;

		//from cache
		if (this.cache[name]) {
			return applyData(this.cache[name], data);
		}

		//find
		for (i = 0; i < array.length; i++) {
			//noinspection JSUnresolvedVariable
			key = array[i].Key;
			if (key === name) {
				//noinspection JSUnresolvedVariable
				value =  array[i].Value;
				this.cache[key] = value;
				return applyData(value, data);
			}
		}

		return "";
	};

}());