/*global copilot */
/**
 * Maintenances
 */
(function () {

	copilot.model = copilot.model || {};

	/**
	 * Images
	 * @constructor
	 */
	copilot.model.Images = function () {
		/** @type {number}*/
		this.length = 0;
	};

	/**
	 * Clone
	 * @returns {copilot.model.Images}
	 */
	copilot.model.Images.prototype.clone = function () {
		var clone = new copilot.model.Images(),
			images = this,
			image,
			key;

		for (key in images) {
			if (images.hasOwnProperty(key)) {
				//update length
				clone.length++;
				//images
				image = images[key];
				clone[key] = image.clone ? image.clone() : new copilot.model.Image().clone.apply(image);
			}
		}

		return clone;
	};





	/**
	 * Image
	 * @constructor
	 */
	copilot.model.Image = function () {
		/** @type {object} */
		this.Data = null;
		/** @type {string}*/
		this._path = "";
		/** @type {boolean}*/
		this.Rotated = false;
		/** @type {Date}*/
		this.Time = new Date();
		/** @type {copilot.model.Backup}*/
		this.Backup = null;
	};

	/**
	 * Is backuped
	 * @returns {boolean}
	 */
	copilot.model.Image.prototype.isBackuped = function () {
		return Boolean(this.Backup && this.Backup.Id);
	};

	/**
	 * Clone
	 * @returns {copilot.model.Image}
	 */
	copilot.model.Image.prototype.clone = function () {
		var clone = new copilot.model.Image();

		clone.Time = typeof this.Time === "string" ? new Date(this.Time) : this.Time;
		clone.Data = this.Data;
		clone._path = this._path;
		clone.Rotated = this.Rotated;

		//backup
		if (this.Backup) {
			clone.Backup = this.Backup.clone ? this.Backup.clone() : new copilot.model.Backup().clone.apply(this.Backup);
		}

		return clone;
	};

}());