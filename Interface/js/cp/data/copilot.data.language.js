/*global copilot */
/**
 * Language
 */
(function () {

	copilot.data = copilot.data || {};

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
		this.cache = {};
		/** @type {copilot.data.Storage}*/
		this.storage = new copilot.data.Storage();
		/** @type {copilot.data.Renderer}*/
		this.renderer = null;
		//load language
		this.load();
	};

	/**
	 * Load
	 */
	copilot.data.Language.prototype.load = function () {
		var language,
			self = this,
			what = "language";

		//loading
		this.loading = true;
		this.error = false;

		this.get(function (data) {
			language = data ? data : self.storage.getData(what, true);
			self.storage.setData(what, language);
			self.loading = language ? false : true;
			self.error = language ? false : true;
		});
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
			array = this.storage.getData("language") || [];

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

		return null;
	};

	/**
	 * @private
	 * get data
	 * @param {function} complete
	 */
	copilot.data.Language.prototype.get = function (complete) {
		//noinspection JSUnresolvedFunction
		$.ajax({
			type: "GET",
			url: copilot.URL + "api/language"
		}).done(function(data) {
			complete(data);
		}).fail(function () {
			complete(null);
		});
	};

}());