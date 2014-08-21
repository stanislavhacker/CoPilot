/*global copilot */
/**
 * Skin
 */
(function () {

	copilot.data = copilot || {};

	/**
	 * Skin
	 * @constructor
	 */
	copilot.data.Skin = function () {
		/** @type {boolean}*/
		this.loading = true;
		/** @type {boolean}*/
		this.error = false;
		/** @type {{}}*/
		this.data = {};
		//load skin
		this.load();
	};

	/**
	 * Load
	 */
	copilot.data.Skin.prototype.load = function () {
		var self = this;

		//loading
		this.loading = true;
		this.error = false;

		//noinspection JSUnresolvedFunction
		$.ajax(copilot.URL + "api/skin")
			.done(function(data) {
				self.data = data;
				self.loading = false;
			}).fail(function () {
				self.error = true;
			});
	};

	/**
	 * Apply skin
	 */
	copilot.data.Skin.prototype.applySkin = function () {
		var data = this.data;

		//background
		$('body').css('background', data.Background);
		$('#banner span').css('color', data.Background);

		//foreground
		$('.button, #menu .current_page_item a').css('background', data.Foreground);
		$('#featured .icon').css('color', data.Foreground);
	};

}());