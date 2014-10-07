/*global FB*/
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
		};

		/**
		 * GetExchangeRateFor
		 * @param {copilot.model.Currency} from
		 * @param {copilot.model.Currency} to
		 * @returns {number}
		 * @static
		 */
		copilot.App.GetExchangeRateFor = function(from, to) {
			//CZK => to
			if (from == copilot.model.Currency.CZK) {
				switch (to)
				{
					case copilot.model.Currency.CZK:
						return 1;
					case copilot.model.Currency.USD:
						return 0.050727399999999999;
					case copilot.model.Currency.GBP:
						return 0.0283233982;
					case copilot.model.Currency.EUR:
						return 0.0363989442;
				}
			}
			//USD => to
			if (from == copilot.model.Currency.USD)
			{
				switch (to)
				{
					case copilot.model.Currency.CZK:
						return 19.713200000000001;
					case copilot.model.Currency.USD:
						return 1;
					case copilot.model.Currency.GBP:
						return 0.616771878;
					case copilot.model.Currency.EUR:
						return 0.792625413;
				}
			}
			//EUR => to
			if (from == copilot.model.Currency.EUR)
			{
				switch (to)
				{
					case copilot.model.Currency.CZK:
						return 27.4733243;
					case copilot.model.Currency.USD:
						return 1.26163;
					case copilot.model.Currency.GBP:
						return 0.778137904;
					case copilot.model.Currency.EUR:
						return 1;
				}
			}

			//GBP => to
			if (from == copilot.model.Currency.GBP)
			{
				switch (to)
				{
					case copilot.model.Currency.CZK:
						return 35.306498;
					case copilot.model.Currency.USD:
						return 1.621345;
					case copilot.model.Currency.GBP:
						return 1;
					case copilot.model.Currency.EUR:
						return 1.28511925;
				}
			}

			return 1;
		};

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


	//FB
	(function(d, s, id) {
		var js, fjs = d.getElementsByTagName(s)[0];
		if (d.getElementById(id)) return;
		js = d.createElement(s); js.id = id;
		js.async = true;
		js.src = "//connect.facebook.net/cs_CZ/sdk.js#xfbml=1&version=v2.0";
		fjs.parentNode.insertBefore(js, fjs);
	}(document, 'script', 'facebook-jssdk'));
	//Twitter
	(function(d,s,id){
		var js, fjs=d.getElementsByTagName(s)[0],p=/^http:/.test(d.location)?'http':'https';
		if(!d.getElementById(id)){
			js=d.createElement(s);js.id=id;js.src=p+'://platform.twitter.com/widgets.js';
			fjs.parentNode.insertBefore(js,fjs);
		}
	}(document, 'script', 'twitter-wjs'));

}(jQuery));