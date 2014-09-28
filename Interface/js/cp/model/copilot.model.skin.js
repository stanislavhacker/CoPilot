/*global copilot */
/**
 * Maintenances
 */
(function () {

	copilot.model = copilot.model || {};

	/**
	 * Skin
	 * @constructor
	 */
	copilot.model.Skin = function () {
		/** @type {string}*/
		this.Background = "#FFFFFF";
		/** @type {string}*/
		this.Foreground = "#000000";
	};

	/**
	 * Clone
	 * @returns {copilot.model.Skin}
	 */
	copilot.model.Skin.prototype.clone = function () {
		var clone = new copilot.model.Skin();

		clone.Background = this.Background;
		clone.Foreground = this.Foreground;

		return clone;
	};

}());