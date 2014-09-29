var copilot = {};
(function ($) {

	/**
	 * App
	 */
	(function () {
	
		//static variables
		/** @type {string}*/
		copilot.URL = "";
		/** @type {string}*/
		copilot.Hash = window.location.hash.substr(1);
		/** @type {copilot.model.Distance}*/
		copilot.Distance = null;
		/** @type {copilot.model.Currency}*/
		copilot.Currency = null;

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
			this.renderer = new copilot.data.Renderer(this);
		};

		/**
		 * Get url
		 * @returns {string}
		 */
		copilot.App.prototype.getUrl = function () {
			//return "http://192.168.1.10/copilot/";
			var index = window.location.href.indexOf("copilot/") + 8;
			return window.location.href.substr(0, index);
		};

		/**
		 * Is loading
		 * @returns {boolean}
		 */
		copilot.App.prototype.isLoading = function () {
			return this.skin.loading ||
				this.language.loading ||
				this.data.loading;
		};

		/**
		 * Is error
		 * @returns {boolean}
		 */
		copilot.App.prototype.isError = function () {
			return this.skin.error ||
				this.language.error ||
				this.data.error;
		};

		/**
		 * Start loader
		 */
		copilot.App.prototype.startLoader = function () {
			var interval,
				self = this,
				body = $("body"),
				errors = $("#errors"),
				clazz = "loading",
				items = $("#header-wrapper, #wrapper, #copyright");

			//hide
			body.addClass(clazz);
			items.hide();
			errors.hide();

			interval = setInterval(function () {
				//loading and not error
				if (self.isLoading() && !self.isError()) {
					return;
				}
				clearInterval(interval);

				//error
				if (self.isError()) {
					self.renderer.renderErrors();

				//ok
				} else {
					//render
					self.renderer.render();

					//show
					items.show();
					body.removeClass(clazz);
				}
			}, 100);
		};













		///// CONVERTERS

		/**
		 * GetExchangeDistanceFor
		 * @param {copilot.model.Distance} from
		 * @param {copilot.model.Distance} to
		 * @returns {number}
		 * @static
		 */
		copilot.App.GetExchangeDistanceFor = function(from, to) {
			//Mi => Km
			if (from === copilot.model.Distance.Mi && to === copilot.model.Distance.Km) {
				return 1.609344;
			}
			//Km => Mi
			if (from === copilot.model.Distance.Km && to === copilot.model.Distance.Mi) {
				return 0.621371192;
			}
			return 1;
		};

		/**
		 * GetOdometerWithRightDistance
		 * @param {copilot.model.Odometer} odometer
		 * @returns {number}
		 * @static
		 */
		copilot.App.GetOdometerWithRightDistance = function(odometer) {
			var rate,
				currentDistance = copilot.Distance;

			if (odometer.Distance === currentDistance) {
				return odometer.Value;
			}
			//recalculate
			rate = copilot.App.GetExchangeDistanceFor(odometer.Distance, currentDistance);
			return Math.round(odometer.Value * rate * 100) / 100;
		}

		/**
		 * GetExchangeRateFor
		 * @param {copilot.model.Currency} from
		 * @param {copilot.model.Currency} to
		 * @returns {number}
		 * @static
		 */
		copilot.App.GetExchangeRateFor = function(from, to) {
			//CZK => USD
			//{"to": "USD", "rate": 0.050727399999999999, "from": "CZK"}
			if (from == copilot.model.Currency.CZK && to == copilot.model.Currency.USD) {
				return 0.050727399999999999;
			}
			//USD => CZK
			//{"to": "CZK", "rate": 19.713200000000001, "from": "USD"}
			if (from == copilot.model.Currency.USD && to == copilot.model.Currency.CZK) {
				return 19.713200000000001;
			}

			return 1;
		}

		/**
		 * GetPriceWithRightValue
		 * @param {copilot.model.Price} price
		 * @returns {number}
		 * @static
		 */
		copilot.App.GetPriceWithRightValue = function(price) {
			var rate,
				currentCurrency = copilot.Currency;

			//recalculate
			rate = copilot.App.GetExchangeRateFor(price.Currency, currentCurrency);
			return Math.round(price.Value * rate * 100) / 100;
		}

	}());


	$(document).ready(function () {
		new copilot.App();
	});

}(jQuery));