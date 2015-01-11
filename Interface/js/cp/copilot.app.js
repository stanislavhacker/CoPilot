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
			return "http://192.168.1.11/copilot/";
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
					case copilot.model.Currency.SEK:
						return 0.343265699;
					case copilot.model.Currency.CHF:
						return 0.0432849734;
					case copilot.model.Currency.RUB:
						return 2.23066348;
					case copilot.model.Currency.TRY:
						return 0.101557864;
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
					case copilot.model.Currency.SEK:
						return 7.85071088;
					case copilot.model.Currency.CHF:
						return 0.987474869;
					case copilot.model.Currency.RUB:
						return 50.9190896;
					case copilot.model.Currency.TRY:
						return 2.31824926;
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
					case copilot.model.Currency.SEK:
						return 9.53781216;
					case copilot.model.Currency.CHF:
						return 1.20269502;
					case copilot.model.Currency.RUB:
						return 61.9990835;
					case copilot.model.Currency.TRY:
						return 2.82363606;
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
					case copilot.model.Currency.SEK:
						return 12.1850944;
					case copilot.model.Currency.CHF:
						return 1.53614633;
					case copilot.model.Currency.RUB:
						return 79.2301034;
					case copilot.model.Currency.TRY:
						return 3.60522533;
				}
			}

			//SEK => to
			if (from == copilot.model.Currency.SEK)
			{
				switch (to)
				{
					case copilot.model.Currency.CZK:
						return 2.9258;
					case copilot.model.Currency.USD:
						return 0.1284;
					case copilot.model.Currency.GBP:
						return 0.08253;
					case copilot.model.Currency.EUR:
						return 0.1051;
					case copilot.model.Currency.SEK:
						return 1;
					case copilot.model.Currency.CHF:
						return 0.1266;
					case copilot.model.Currency.RUB:
						return 6.8097;
					case copilot.model.Currency.TRY:
						return 0.298;
				}
			}

			//CHF => to
			if (from == copilot.model.Currency.CHF)
			{
				switch (to)
				{
					case copilot.model.Currency.CZK:
						return 23.1133;
					case copilot.model.Currency.USD:
						return 1.0144;
					case copilot.model.Currency.GBP:
						return 0.6519;
					case copilot.model.Currency.EUR:
						return 0.8318;
					case copilot.model.Currency.SEK:
						return 7.9321;
					case copilot.model.Currency.CHF:
						return 1;
					case copilot.model.Currency.RUB:
						return 53.7958;
					case copilot.model.Currency.TRY:
						return 2.3543;
				}
			}

			//RUB => to
			if (from == copilot.model.Currency.RUB)
			{
				switch (to)
				{
					case copilot.model.Currency.CZK:
						return 0.4299;
					case copilot.model.Currency.USD:
						return 0.01887;
					case copilot.model.Currency.GBP:
						return 0.01213;
					case copilot.model.Currency.EUR:
						return 0.01547;
					case copilot.model.Currency.SEK:
						return 0.1475;
					case copilot.model.Currency.CHF:
						return 0.0186;
					case copilot.model.Currency.RUB:
						return 1;
					case copilot.model.Currency.TRY:
						return 0.04379;
				}
			}

			//TRY => to
			if (from == copilot.model.Currency.TRY)
			{
				switch (to)
				{
					case copilot.model.Currency.CZK:
						return 9.8497;
					case copilot.model.Currency.USD:
						return 0.4323;
					case copilot.model.Currency.GBP:
						return 0.2778;
					case copilot.model.Currency.EUR:
						return 0.3545;
					case copilot.model.Currency.SEK:
						return 3.3803;
					case copilot.model.Currency.CHF:
						return 0.4263;
					case copilot.model.Currency.RUB:
						return 22.925;
					case copilot.model.Currency.TRY:
						return 1;
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