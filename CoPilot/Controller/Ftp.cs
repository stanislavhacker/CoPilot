using CoPilot.Core.Utils;
using Interfaces = CoPilot.Interfaces;
using CoPilot.Utils;
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
using CoPilot.Interfaces.EventArgs;
using CoPilot.OneDrive;
using CoPilot.Interfaces.Types;
using CoPilot.Interfaces;
using System.Threading;
using CoPilot.Core.Data;

namespace CoPilot.CoPilot.Controller
{
    public class Ftp : Base
    {
        #region EVENTS

        public event EventHandler<StateEventArgs> OnStateChange;
        public event EventHandler<ErrorEventArgs> Error;

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
                RaisePropertyChanged("IsOneDriveAvailable");
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
                return isNetEnabled && isLogged;
            }
            set
            {
                isNetEnabled = value;
                RaisePropertyChanged();
                RaisePropertyChanged("IsOneDriveAvailable");
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
        /// IsLogged
        /// </summary>
        public Boolean isLogged = false;
        public Boolean IsLogged
        {
            get
            {
                return isLogged;
            }
            set
            {
                isLogged = value;

                RaisePropertyChanged();
                RaisePropertyChanged("IsWifiEnabled");
                RaisePropertyChanged("IsNetEnabled");
                RaisePropertyChanged("IsOneDriveAvailable");
            }
        }

        /// <summary>
        /// Is OneDrive available
        /// </summary>
        public Boolean isOneDriveAvailable = false;
        public Boolean IsOneDriveAvailable
        {
            get
            {
                return !IsLogged && Settings.Get("StorageConnected") != true.ToString() && isNetEnabled;
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

        #region PRIVATE

        private NetClient client = null;

        #endregion

        /// <summary>
        /// Ftp controller
        /// </summary>
        public Ftp()
        {
            this.client = new OneDriveClient();

            ///State change
            this.client.State += (object sender, StateEventArgs e) =>
            {
                this.IsLogged = e.State == ConnectionStatus.Connected;
                if (OnStateChange != null)
                {
                    OnStateChange.Invoke(this, e);
                }
            };

            ///Error
            this.client.Error += (object sender, ErrorEventArgs e) =>
            {
                if (Error != null)
                {
                    Error.Invoke(this, e);
                }
            };
        }

        /// <summary>
        /// Try login
        /// </summary>
        public void TryLogin()
        {
            this.client.TryLogin();
        }

        /// <summary>
        /// Login into service
        /// </summary>
        /// <param name="serviceType"></param>
        public async Task Login()
        {
            await this.client.Login();
        }

        /// <summary>
        /// Check if is progress
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<Boolean> Progress(Uri url)
        {
            return await client.Progress(url);
        }

        /// <summary>
        /// Connect
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        public async void Connect(Progress progress)
        {
            var exists = await this.Progress(progress.Url);
            if (exists)
            {
                await this.Upload(progress);
            }
        }

        /// <summary>
        /// Upload
        /// </summary>
        /// <param name="location"></param>
        /// <param name="type"></param>
        /// <param name="token"></param>
        /// <param name="bar"></param>
        /// <returns></returns>
        public async Task<Response> Upload(Progress bar)
        {
            Response response = await client.Upload(bar);
            return response;
        }

        /// <summary>
        /// get video url
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response> VideoUrl(String id)
        {
            Response response = await client.Url(id);
            return response;
        }

        /// <summary>
        /// Download
        /// </summary>
        /// <param name="id"></param>
        /// <param name="location"></param>
        /// <param name="type"></param>
        /// <param name="token"></param>
        /// <param name="bar"></param>
        /// <returns></returns>
        public async Task<DownloadStatus> Download(String id, Progress bar)
        {
            DownloadStatus response = await client.Download(id, bar);
            return response;
        }

        /// <summary>
        /// Download main backup
        /// </summary>
        /// <param name="DownloadProgress"></param>
        /// <returns></returns>
        public async Task<DownloadStatus> Download(Progress bar)
        {
            bar.InProgress = true;
            var response = await client.BackupId(Data.DATA_FILE_NAME);
            if (response != null)
            {
                return await client.Download(response.Id, bar);
            }
            else
            {
                return DownloadStatus.Fail;
            }
        }

        /// <summary>
        /// Download video preview
        /// </summary>
        /// <param name="DownloadProgress"></param>
        /// <returns></returns>
        public async Task<DownloadStatus> Preview(String id, Progress bar)
        {
            return await client.Preview(id, bar);
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

                var progress = new Progress();
                progress.BytesTransferred = 0;
                progress.ProgressPercentage = 0;
                progress.Selected = true;
                progress.TotalBytes = 0;
                progress.Url = new Uri(Controllers.Data.DATA_FILE, UriKind.Relative);
                progress.Cancel = new System.Threading.CancellationTokenSource();
                progress.Type = Interfaces.Types.FileType.Data;
                progress.Preferences = ProgressPreferences.AllowOnCelluralAndBatery;

                await this.ProcessBackup(progress);
            }
        }

        #region PROCESS

        /// <summary>
        /// Process backup
        /// </summary>
        /// <param name="p"></param>
        public async Task ProcessBackup(Interfaces.Progress progress)
        {
            Response response = await this.Upload(progress);
            if (response != null)
            {
                DataController.Backup = this.createBackupInfo(response);
            }
        }


        /// <summary>
        /// createBackupInfo
        /// </summary>
        /// <param name="response"></param>
        private BackupInfo createBackupInfo(Response response)
        {
            //create new
            BackupInfo info = new BackupInfo();
            info.Url = response.Url;
            info.Date = DateTime.Now;
            info.Id = response.Id;

            return info;
        }

        /// <summary>
        /// Process media backup
        /// </summary>
        /// <param name="observableCollection"></param>
        public async Task ProcessBackup(ObservableCollection<Progress> data)
        {
            foreach (Progress progress in data) 
            {
                //response
                Response response = await this.Upload(progress);
                Ftp.ProcessUploadResponse(progress, response);
            }

            data.Clear();
        }

        #endregion


        #region PRIVATE STATIC

        /// <summary>
        /// Process upload response
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="response"></param>
        private static void ProcessUploadResponse(Progress progress, Response response)
        {
            if (response != null)
            {
                //info
                BackupInfo info = new BackupInfo();
                info.Date = DateTime.Now;
                info.Id = response.Id;
                info.Url = response.Url;

                //save
                if (progress.Data.GetType() == typeof(Video))
                {
                    (progress.Data as Video).VideoBackup = info;
                    (progress.Data as Video).CallPropertyChangedOnAll();
                }
                if (progress.Data.GetType() == typeof(Picture))
                {
                    (progress.Data as Picture).Backup = info;
                    (progress.Data as Picture).CallPropertyChangedOnAll();
                }
            }
        }

        #endregion
    }
}
