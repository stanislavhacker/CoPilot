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

		/** @type {Array.<boolean>}*/
		copilot.Menus = [];

		/** @type {{}}*/
		copilot.Rates = null;

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
			this.data = new copilot.data.Data(this.language);

			//renderer
			this.renderer = new copilot.data.Renderer(this);
		};

		/**
		 * Get url
		 * @returns {string}
		 */
		copilot.App.prototype.getUrl = function () {
			//return "http://192.168.1.11/copilot/";
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
			//fill
			if (copilot.Rates === null) {
				copilot.Rates = {};
				copilot.Rates[copilot.model.Currency.CZK] = 1;
				copilot.Rates[copilot.model.Currency.USD] = 19.7132;
				copilot.Rates[copilot.model.Currency.GBP] = 35.306498;
				copilot.Rates[copilot.model.Currency.EUR] = 27.4733243;
				copilot.Rates[copilot.model.Currency.SEK] = 2.9258;
				copilot.Rates[copilot.model.Currency.CHF] = 23.1133;
				copilot.Rates[copilot.model.Currency.RUB] = 0.4299;
				copilot.Rates[copilot.model.Currency.TRY] = 9.8497;
				copilot.Rates[copilot.model.Currency.CNY] = 3.8680
			}
			//calculate
			var round = 10000000000,
				rate = copilot.Rates[from] / copilot.Rates[to];
			return Math.round(rate * round) / round;
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
		};

		/**
		 * GetFillsMonthlyExpenditure
		 * @param {copilot.data.Language} language
		 * @param {copilot.model.Fills} fills
		 * @return {copilot.model.Graph}
		 */
		copilot.App.GetFillsMonthlyExpenditure = function (language, fills) {
			var i,
				key,
				month,
				fillsTotal = {},
				graph = new copilot.model.Graph(),
				series = new copilot.model.Graph.Series();

			for (i = 0; i < fills.length; i++) {
				//month
				month = (fills[i].Date.getMonth() + 1) + "/" + fills[i].Date.getFullYear();
				//month
				fillsTotal[month] = fillsTotal[month] || 0;
				fillsTotal[month] += copilot.App.GetPriceWithRightValue(fills[i].Price);
			}

			//name
			series.name = language.getString("FuelPrice");
			series.lineWidth = 2;
			series.color = '';

			for (key in fillsTotal) {
				//noinspection JSUnfilteredForInLoop
				series.data.push([key, Math.round(fillsTotal[key] * 100) / 100]);
			}

			//create
			graph.name = language.getString("MonthlyExpenditure") + " " + language.getString('Fuels');
			graph.dataUnit = copilot.Currency;
			graph.type = 'pie';
			graph.addSeries(series);

			return graph;
		};

		/**
		 * GetMonthlyFuelConsumption
		 * @param {copilot.data.Language} language
		 * @param {copilot.model.Fills} fills
		 * @param {string} color
		 * @return {copilot.model.Graph}
		 */
		copilot.App.GetMonthlyFuelConsumption = function (language, fills, color) {
			var i,
				key,
				month,
				consumptionTotal = {},
				graph = new copilot.model.Graph(),
				series = new copilot.model.Graph.Series();

			for (i = 0; i < fills.length; i++) {
				//month
				month = (fills[i].Date.getMonth() + 1) + "/" + fills[i].Date.getFullYear();
				//month
				consumptionTotal[month] = consumptionTotal[month] || 0;
				consumptionTotal[month] += fills[i].Refueled;
			}

			//name
			series.name = language.getString("Consumed");
			series.lineWidth = 2;
			series.color = color;

			for (key in consumptionTotal) {
				//noinspection JSUnfilteredForInLoop
				series.data.push([key, Math.round(consumptionTotal[key] * 100) / 100]);
			}

			//create
			graph.name = language.getString("MonthlyFuelConsumption");
			graph.dataUnit = language.getString("FueledUnit");
			graph.type = 'column';
			graph.addSeries(series);

			return graph;
		};

		/**
		 * GetFillsMonthlyExpenditure
		 * @param {copilot.data.Language} language
		 * @param {copilot.model.Repairs} repairs
		 * @return {copilot.model.Graph}
		 */
		copilot.App.GetRepairsMonthlyExpenditure = function (language, repairs) {
			var i,
				key,
				month,
				repairsTotal = {},
				graph = new copilot.model.Graph(),
				series = new copilot.model.Graph.Series();

			for (i = 0; i < repairs.length; i++) {
				//month
				month = (repairs[i].Date.getMonth() + 1) + "/" + repairs[i].Date.getFullYear();
				//month
				repairsTotal[month] = repairsTotal[month] || 0;
				repairsTotal[month] += copilot.App.GetPriceWithRightValue(repairs[i].Price);
			}

			//name
			series.name = language.getString("RepairPrice");
			series.lineWidth = 2;
			series.color = '';

			for (key in repairsTotal) {
				//noinspection JSUnfilteredForInLoop
				series.data.push([key, Math.round(repairsTotal[key] * 100) / 100]);
			}

			//create
			graph.name = language.getString("MonthlyExpenditure") + " " + language.getString('Repairs');
			graph.dataUnit = copilot.Currency;
			graph.type = 'pie';
			graph.addSeries(series);

			return graph;
		};


		/**
		 * Get time difference
		 * @param {Date} laterDate
		 * @param {Date} earlierDate
		 * @returns {string}
		 */
		copilot.App.timeDifference = function (laterDate, earlierDate) {
			var difference = laterDate.getTime() - earlierDate.getTime(),
				days = 1000*60*60*24,
				hours = 1000*60*60,
				minutes = 1000*60,
				secondsDifference,
				minutesDifference,
				hoursDifference,
				daysDifference,
				string = "";

			daysDifference = Math.floor(difference / days);
			difference -= daysDifference * days;

			hoursDifference = Math.floor(difference / hours);
			difference -= hoursDifference * hours;

			minutesDifference = Math.floor(difference / minutes);
			difference -= minutesDifference * minutes;

			secondsDifference = Math.floor(difference / 1000);

			if (daysDifference > 0) {
				string += ("0" + daysDifference + ":").slice(-3);
			}
			if (hoursDifference > 0) {
				string += ("0" + hoursDifference + ":").slice(-3);
			}
			if (minutesDifference > 0) {
				string += ("0" + minutesDifference + ":").slice(-3);
			}
			if (secondsDifference > 0) {
				string += ("0" + secondsDifference).slice(-2);
			}

			return string;
		};

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
		//noinspection JSValidateTypes
		var js, fjs=d.getElementsByTagName(s)[0],p=/^http:/.test(d.location)?'http':'https';
		if(!d.getElementById(id)){
			js=d.createElement(s);js.id=id;js.src=p+'://platform.twitter.com/widgets.js';
			fjs.parentNode.insertBefore(js,fjs);
		}
	}(document, 'script', 'twitter-wjs'));

}(jQuery));