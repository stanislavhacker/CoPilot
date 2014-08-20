var copilot = {};
(function ($) {

	/**
	 * App
	 */
	(function () {
	
		//static variables
		copilot.URL = "";

		/**
		 * Main app
		 * @constructor
		 */
		copilot.App = function () {
			/** @type {string}*/
			copilot.URL = this.getUrl();
			//loader
			this.startLoader();
			//create and load skin
			this.skin = new copilot.data.Skin();
			//create and load language
			this.language = new copilot.data.Language();
			//create and load data
			this.data = new copilot.data.Data();

			//renderer
			this.renderer = new copilot.data.Renderer(this.language, this.skin);
		};

		/**
		 * Get url
		 * @returns {string}
		 */
		copilot.App.prototype.getUrl = function () {
			var index = window.location.href.indexOf("copilot/") + 8;
			return window.location.href.substr(0, index);
		};

		/**
		 * Start loader
		 */
		copilot.App.prototype.startLoader = function () {
			var interval,
				self = this,
				body = $('body');

			//hide
			body.hide();

			interval = setInterval(function () {
				if (self.skin.loading ||
					self.language.loading ||
					self.data.loading) {
					return;
				}
				clearInterval(interval);

				//render
				self.renderer.render();

				//show
				body.show();
			}, 100);
		};
	
	}());


	$(document).ready(function () {
		new copilot.App();
	});

}(jQuery));