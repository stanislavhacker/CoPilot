/*global copilot */
/**
 * Skin
 */
(function () {

	copilot.data = copilot.data || {};

	/**
	 * Skin
	 * @constructor
	 */
	copilot.data.Skin = function () {
		/** @type {boolean}*/
		this.loading = true;
		/** @type {boolean}*/
		this.error = false;
		/** @type {copilot.data.Storage}*/
		this.storage = new copilot.data.Storage();
		//load skin
		this.load();
	};

	/**
	 * Load
	 */
	copilot.data.Skin.prototype.load = function () {
		var skin,
			what = "skin",
			self = this;

		//loading
		this.loading = true;
		this.error = false;

		//load
		this.get(function (data) {
			skin = data ? new copilot.model.Skin().clone.apply(data) : self.storage.getData(what, true);
			self.storage.setData(what, skin);
			self.loading = skin ? false : true;
			self.error = skin ? false : true;
		});
	};

	/**
	 * Get skin
	 * @returns {copilot.model.Skin}
	 */
	copilot.data.Skin.prototype.getSkin = function () {
		return this.storage.getData("skin") || new copilot.model.Skin();
	};

	/**
	 * Apply skin
	 */
	copilot.data.Skin.prototype.applySkin = function () {
		var data = this.getSkin();

		//background
		$('body').css('background', data.Background);
		$('#banner span').css('color', data.Background);

		//foreground
		$('.button, #menu .current_page_item a').css('background', data.Foreground);
		$('#featured .icon').css('color', data.Foreground);
	};

	/**
	 * @private
	 * get data
	 * @param {function} complete
	 */
	copilot.data.Skin.prototype.get = function (complete) {
		//noinspection JSUnresolvedFunction
		$.ajax({
			type: "GET",
			url: copilot.URL + "api/skin"
		}).done(function(data) {
			complete(data);
		}).fail(function () {
			complete(null);
		});
	};

}());