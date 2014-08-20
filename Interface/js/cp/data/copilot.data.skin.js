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

		//noinspection JSUnresolvedFunction
		$.ajax(copilot.URL + "api/skin")
			.done(function(data) {
				self.data = data;
			}).always(function () {
				self.loading = false;
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