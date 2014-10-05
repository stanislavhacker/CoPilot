/*global copilot */
/**
 * Maintenances
 */
(function () {

	copilot.model = copilot.model || {};

	/**
	 * Videos
	 * @constructor
	 */
	copilot.model.Videos = function () {
		/** @type {number}*/
		this.length = 0;
	};

	/**
	 * Clone
	 * @returns {copilot.model.Videos}
	 */
	copilot.model.Videos.prototype.clone = function () {
		var clone = new copilot.model.Videos(),
			videos = this,
			video,
			key;

		for (key in videos) {
			if (videos.hasOwnProperty(key)) {
				//update length
				clone.length++;
				//videos
				video = videos[key];
				clone[key] = video.clone ? video.clone() : new copilot.model.Video().clone.apply(video);
			}
		}

		return clone;
	};

	/**
	 * To json
	 * @returns {object}
	 */
	copilot.model.Videos.prototype.toJSON = function () {
		var data = [],
			i;
		for (i = 0; i < this.length; i++) {
			data.push(this[i]);
		}
		return data;
	};



	/**
	 * Video
	 * @constructor
	 */
	copilot.model.Video = function () {
		/** @type {object} */
		this.Data = null;
		/** @type {number}*/
		this.duration = "";
		/** @type {string}*/
		this._path = "";
		/** @type {string}*/
		this._preview = "";
		/** @type {boolean}*/
		this.Rotated = false;
		/** @type {Date}*/
		this.Time = new Date();
		/** @type {copilot.model.Backup}*/
		this.VideoBackup = null;
	};

	/**
	 * Is backuped
	 * @returns {boolean}
	 */
	copilot.model.Video.prototype.isBackuped = function () {
		return Boolean(this.VideoBackup && this.VideoBackup.Id);
	};

	/**
	 * Clone
	 * @returns {copilot.model.Video}
	 */
	copilot.model.Video.prototype.clone = function () {
		var clone = new copilot.model.Video();

		clone.Time = typeof this.Time === "string" ?  new Date(this.Time) : this.Time;
		clone.Data = this.Data;
		clone.duration = this.duration;
		clone._path = this._path;
		clone._preview = this._preview;
		clone.Rotated = this.Rotated;

		//backup
		if (this.VideoBackup) {
			clone.VideoBackup = this.VideoBackup.clone ? this.VideoBackup.clone() : new copilot.model.Backup().clone.apply(this.VideoBackup);
		}

		return clone;
	};

}());