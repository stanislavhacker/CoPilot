using CoPilot.Core.Utils;
using Interfaces = CoPilot.Interfaces;
using CoPilot.Utils;
using SendSpace;
using SendSpace.Responses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Controllers = CoPilot.CoPilot.Controller;
using CoPilot.Resources;

namespace CoPilot.CoPilot.Controller
{
    public class Ftp : Base
    {
        #region PRIVATE

        private const String SEND_SHARE_CLIENT_ID = "3ULHVRPN1L";

        #endregion

        #region EVENTS

        public event EventHandler<Interfaces.EventArgs.UploadEventArgs> onUploaded;
        public event EventHandler<Interfaces.EventArgs.UriEventArgs> onUrl;
        public event EventHandler<Interfaces.EventArgs.StreamEventArgs> onDownloaded;

        #endregion

        #region PROPERTY

        /// <summary>
        /// Is wifi enabled
        /// </summary>
        private Boolean isWifiEnabled = false;
        public Boolean IsWifiEnabled
        {
            get
            {
                return isWifiEnabled;
            }
            set
            {
                isWifiEnabled = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Is net enabled
        /// </summary>
        private Boolean isNetEnabled = false;
        public Boolean IsNetEnabled
        {
            get
            {
                return isNetEnabled;
            }
            set
            {
                isNetEnabled = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Is progress
        /// </summary>
        private Boolean isUpload = false;
        public Boolean IsUpload
        {
            get
            {
                return isUpload;
            }
            set
            {
                isUpload = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// Is download
        /// </summary>
        private Boolean isDownload = false;
        public Boolean IsDownload
        {
            get
            {
                return isDownload;
            }
            set
            {
                isDownload = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Is open
        /// </summary>
        private Boolean isOpen = false;
        public Boolean IsOpen
        {
            get
            {
                return isOpen;
            }
            set
            {
                isOpen = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Main data file
        /// </summary>
        private ProgressUpdater mainDataFile;
        public ProgressUpdater MainDataFile
        {
            get
            {
                return mainDataFile;
            }
            set
            {
                mainDataFile = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Is media backup
        /// </summary>
        private Boolean isMediaBackup = false;
        public Boolean IsMediaBackup
        {
            get
            {
                return isMediaBackup;
            }
            set
            {
                isMediaBackup = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Backup front
        /// </summary>
        private ObservableCollection<MediaWithProgress> backupFront = new ObservableCollection<MediaWithProgress>();
        public ObservableCollection<MediaWithProgress> BackupFront
        {
            get
            {
                return backupFront;
            }
            set
            {
                backupFront = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region CONTROLLERS

        /// <summary>
        /// Data controller
        /// </summary>
        private Controllers.Data dataController;
        public Controllers.Data DataController
        {
            get
            {
                return dataController;
            }
            set
            {
                dataController = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        /// <summary>
        /// Ftp controller
        /// </summary>
        public Ftp()
        {
            //progresses
            this.MainDataFile = new ProgressUpdater();
        }

        /// <summary>
        /// Upload file to stream
        /// </summary>
        /// <param name="file"></param>
        public void Upload(String name, String contentType, Stream file, ProgressUpdater updater, object state, bool sendEmail = false)
        {
            this.IsUpload = true;
            updater.IsIndetermine = true;

            Interfaces.NetClient client = this.getClient();

            //send email
            if (DataController.IsEmailOnBackup && DataController.BackupEmail != "" && sendEmail)
            {
                client.Email(DataController.BackupEmail, AppResources.SendByEmailSubject);
            }

            client.Upload(name, contentType, file);
            client.UploadProgress += (object sender, Interfaces.EventArgs.ProgressEventArgs e) =>
            {
                if (e.Progress.Status == "ok")
                {
                    updater.Eta = e.Progress.Eta;
                    updater.Percent = e.Progress.Percent;
                    updater.Speed = e.Progress.Speed;
                    updater.IsIndetermine = false;
                }
            };
            client.UploadComplete += (object sender, Interfaces.EventArgs.UploadEventArgs e) =>
            {
                updater.IsIndetermine = false;
                updater.Eta = "";
                updater.Percent = 100;
                updater.Speed = 0;

                if (onUploaded != null)
                {
                    onUploaded.Invoke(state, e);
                }

                this.IsUpload = false;
            };
            client.UploadError += (object sender, Interfaces.EventArgs.ExceptionEventArgs e) =>
            {
                this.IsUpload = false;
            };
        }

        /// <summary>
        /// Open url
        /// </summary>
        /// <param name="url"></param>
        public void Open(string url)
        {
            this.IsOpen = true;

            Interfaces.NetClient client = this.getClient();
            client.Get(url);
            client.GetComplete += (object sender, Interfaces.EventArgs.UriEventArgs e) =>
            {
                if (e != null && onUrl != null)
                {
                    onUrl.Invoke(this, e);
                }
                this.IsOpen = false;
            };
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="url"></param>
        public void Delete(string url)
        {
            Interfaces.NetClient client = this.getClient();
            client.Delete(url);
            client.DeleteComplete += (object sender, EventArgs e) =>
            {
                System.Diagnostics.Debug.WriteLine("Deleted old backup on " + url);
            };
        }

        /// <summary>
        /// Download by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        public void Download(string id, object state)
        {
            this.IsDownload = true;

            Interfaces.NetClient client = this.getClient();
            client.Download(id);
            client.DownloadComplete += (object sender, Interfaces.EventArgs.StreamEventArgs e) =>
            {
                this.IsDownload = false;
                if (e != null && onDownloaded != null)
                {
                    onDownloaded.Invoke(state, e);
                }
            };
            client.DownloadError += (object sender, Interfaces.EventArgs.ExceptionEventArgs e) =>
            {
                this.IsDownload = false;
                onDownloaded.Invoke(state, null);
            };
        }

        /// <summary>
        /// Update sizes
        /// </summary>
        public void UpdateSizes()
        {
            this.MainDataFile.Size = DataController.Size();
        }

        /// <summary>
        /// Backup now
        /// </summary>
        /// <param name="canBackup"></param>
        public async Task BackupNow(bool canBackup)
        {
            if (canBackup) 
            {
                await Task.Delay(3000);
                this.ProcessBackup("MainData");
            }
        }

        #region PROCESS

        /// <summary>
        /// Process backup
        /// </summary>
        /// <param name="p"></param>
        public void ProcessBackup(string name)
        {
            if (this.IsUpload)
            {
                return;
            }

            switch (name)
            {
                case "MainData":
                    MemoryStream stream = new MemoryStream();
                    DataController.Copy(stream);
                    var file = Controllers.Data.DATA_FILE;
                    this.Upload(file, "text/xml", stream, this.MainDataFile, file, true);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Process media backup
        /// </summary>
        /// <param name="observableCollection"></param>
        public void ProcessBackup(ObservableCollection<MediaWithProgress> media)
        {
            //add all
            foreach (MediaWithProgress medium in media) 
            {
                this.BackupFront.Add(medium);
            }

            //clear all
            media.Clear();

            if (!this.IsMediaBackup)
            {
                this.IsMediaBackup = true;
                //call first
                this.NextMedia();
            }
        }

        /// <summary>
        /// Next media upload
        /// </summary>
        public void NextMedia()
        {
            //end of front
            if (BackupFront.Count == 0)
            {
                this.IsMediaBackup = false;
                this.BackupFront = new ObservableCollection<MediaWithProgress>();
                return;
            }

            MediaWithProgress first = BackupFront.ElementAt(0);
            BackupFront.RemoveAt(0);

            //for picture
            if (first.Picture != null) {
                var stream = Storage.OpenFile(first.Picture.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                this.Upload(first.Picture.Path, "image/jpeg", stream, first.Progress, first);
            }

            //for video preview
            if (first.Preview != null)
            {
                var previewStream = Storage.OpenFile(first.Preview.Preview, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                this.Upload(first.Preview.Preview, "image/jpeg", previewStream, first.Progress, first);
            }

            //for video
            if (first.Video != null)
            {
                var videoStream = Storage.OpenFile(first.Video.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                this.Upload(first.Video.Path, "video/mp4", videoStream, first.Progress, first);
            }
        }



        /// <summary>
        /// Process show
        /// </summary>
        /// <param name="name"></param>
        public void ProcessShow(string name)
        {
            switch (name)
            {
                case "MainData":
                    this.Open(DataController.Backup.DownloadUrl);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region PRIVATE

        /// <summary>
        /// Get net client
        /// </summary>
        /// <returns></returns>
        private Interfaces.NetClient getClient()
        {
            return new SendSpaceClient(SEND_SHARE_CLIENT_ID);
        }

        #endregion
    }
}
