using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using Controllers = CoPilot.CoPilot.Controller;
using CoPilot.Utils;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.IO;
using CoPilot.Resources;
using Microsoft.Phone.Tasks;
using CoreData = CoPilot.Core.Data;
using System.Collections.ObjectModel;
using CoPilot.Core.Utils;
using CoPilot.Interfaces;
using CoPilot.CoPilot.Controller;
using System.Threading.Tasks;
using CoPilot.Core.Data;
using CoPilot.Data;

namespace CoPilot.CoPilot.View
{
    public partial class Backup : PhoneApplicationPage, INotifyPropertyChanged
    {

        #region COMMANDS

        /// <summary>
        /// Cehck picutre command
        /// </summary>
        public ICommand ChekcPictureCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    Progress picture = (Progress)param;
                    if (picture.Selected)
                    {
                        Pictures.Add(picture);
                    }
                    else
                    {
                        Pictures.Remove(picture);
                    }
                }, param => true);
            }
        }

        /// <summary>
        /// Cehck video command
        /// </summary>
        public ICommand ChekcVideoCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    Progress video = (Progress)param;
                    if (video.Selected)
                    {
                        Videos.Add(video);
                    }
                    else
                    {
                        Videos.Remove(video);
                    }
                }, param => true);
            }
        }

        /// <summary>
        /// View picutre command
        /// </summary>
        public ICommand ViewPictureCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    NavigationService.Navigate("/CoPilot/View/Pictures.xaml", this.GetPictureDataContainer(param as CoreData.Picture));
                }, param => true);
            }
        }

        /// <summary>
        /// View video command
        /// </summary>
        public ICommand ViewVideoCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    NavigationService.Navigate("/CoPilot/View/Videos.xaml", this.GetVideoDataContainer(param as CoreData.Video));
                }, param => true);
            }
        }

        /// <summary>
        /// Backup Command
        /// </summary>
        public ICommand BackupCommand
        {
            get
            {
                return new RelayCommand(async (param) =>
                {
                    String name = (String)param;
                    switch (name)
                    {
                        case "MainData":

                            if (UploadProgress.InProgress)
                            {
                                return;
                            }

                            await ProcessBackupUpload();
                            break;
                        case "Images":

                            if (this.IsImageBackup)
                            {
                                return;
                            }

                            this.IsImageBackup = true;
                            await FtpController.ProcessBackup(this.Pictures);
                            this.IsImageBackup = false;
                            break;
                        case "Videos":

                            if (this.IsVideoBackup)
                            {
                                return;
                            }

                            this.IsVideoBackup = true;
                            await FtpController.ProcessBackup(this.Videos);
                            this.IsVideoBackup = false;
                            break;
                        default:
                            break;
                    }
                }, param => true);
            }
        }

        /// <summary>
        /// Show Command
        /// </summary>
        public ICommand ShowCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.ProcessShow((String)param);
                }, param => true);
            }
        }

        /// <summary>
        /// Tap Command
        /// </summary>
        public ICommand TapCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.closeMenuIfItsNecessary();
                }, param => true);
            }
        }

        /// <summary>
        /// Main backup Command
        /// </summary>
        public ICommand MainBackupTap
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.openMainBackupMenu();
                }, param => true);
            }
        }

        /// <summary>
        /// Photo backup Command
        /// </summary>
        public ICommand PhotoBackupTap
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.openMainPhotoMenu();
                }, param => true);
            }
        }

        /// <summary>
        /// Video backup Command
        /// </summary>
        public ICommand VideoBackupTap
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.openMainVideoMenu();
                }, param => true);
            }
        }

        /// <summary>
        /// Download Command
        /// </summary>
        public ICommand DownloadCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.downloadBackup((String)param);
                }, param => true);
            }
        }

        /// <summary>
        /// Email Command
        /// </summary>
        public ICommand EmailCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.sendEmailLink((String)param);
                }, param => true);
            }
        }

        /// <summary>
        /// OneDriveCommand Command
        /// </summary>
        public ICommand OneDriveCommand
        {
            get
            {
                return new RelayCommand(async (param) =>
                {
                    await showLoginScreen();
                }, param => true);
            }
        }

        /// <summary>
        /// CancelCommand
        /// </summary>
        public ICommand CancelCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    Progress prg = param as Progress;
                    if (prg != null && !prg.Cancel.IsCancellationRequested && prg.Cancel.Token.CanBeCanceled)
                    {
                        prg.Cancel.Cancel();
                        prg.InProgress = false;
                        prg.Selected = false;
                    }
                }, param => true);
            }
        }

        #endregion

        #region PROPERTY MENU

        /// <summary>
        /// Menu controller
        /// </summary>
        private Controllers.Menu menuController;
        public Controllers.Menu MenuController
        {
            get
            {
                return menuController;
            }
            set
            {
                menuController = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region PROPERTY

        /// <summary>
        /// Pictures with progress
        /// </summary>
        private ObservableCollection<Progress> pictures = new ObservableCollection<Progress>();
        public ObservableCollection<Progress> Pictures
        {
            get
            {
                return pictures;
            }
        }

        /// <summary>
        /// Videos with progress
        /// </summary>
        private ObservableCollection<Progress> videos = new ObservableCollection<Progress>();
        public ObservableCollection<Progress> Videos
        {
            get
            {
                return videos;
            }
        }

        /// <summary>
        /// IsImageBackup
        /// </summary>
        public Boolean isImageBackup = false;
        public Boolean IsImageBackup
        {
            get
            {
                return isImageBackup;
            }
            set
            {
                isImageBackup = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// IsVideoBackup
        /// </summary>
        public Boolean isVideoBackup = false;
        public Boolean IsVideoBackup
        {
            get
            {
                return isVideoBackup;
            }
            set
            {
                isVideoBackup = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Download progress
        /// </summary>
        public Progress downloadProgress = new Progress();
        public Progress DownloadProgress
        {
            get
            {
                return downloadProgress;
            }
            set
            {
                downloadProgress = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Upload progress
        /// </summary>
        public Progress uploadProgress = new Progress();
        public Progress UploadProgress
        {
            get
            {
                return uploadProgress;
            }
            set
            {
                uploadProgress = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Ftp controller
        /// </summary>
        private Controllers.Ftp ftpController;
        public Controllers.Ftp FtpController
        {
            get
            {
                return ftpController;
            }
            set
            {
                ftpController = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Camera controller
        /// </summary>
        private Controllers.Camera cameraController;
        public Controllers.Camera CameraController
        {
            get
            {
                return cameraController;
            }
            set
            {
                cameraController = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Drive Mode controller
        /// </summary>
        private Controllers.DriveMode driveModeController;
        public Controllers.DriveMode DriveModeController
        {
            get
            {
                return driveModeController;
            }
            set
            {
                driveModeController = value;
                RaisePropertyChanged();
            }
        }

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
        /// Backup
        /// </summary>
        public Backup()
        {
            InitializeComponent();
            InitializeControllers();

            this.createUploadProgress();
            this.createDownloadProgress();
            this.DataContext = this;
        }

        /// <summary>
        /// Stop recording now
        /// </summary>
        private void StopRecordingNow()
        {
            if (CameraController.IsRecording)
            {
                CameraController.RecordStop(true);
            }
        }

        #region CONTROLLERS

        /// <summary>
        /// Initialize all controllers
        /// </summary>
        private void InitializeControllers()
        {
            //create controllers
            this.createMenuController();
        }

        /// <summary>
        /// Create menu controller
        /// </summary>
        private void createMenuController()
        {
            ///CONTROLLER
            MenuController = new Controllers.Menu();
        }

        #endregion

        #region PRIVATE

        /// <summary>
        /// Show login screen
        /// </summary>
        /// <returns></returns>
        private async Task showLoginScreen()
        {
            //visibility
            this.Visibility = System.Windows.Visibility.Collapsed;
            
            this.SupportedOrientations = SupportedPageOrientation.Portrait;
            await this.FtpController.Login();
            this.SupportedOrientations = SupportedPageOrientation.Landscape;
            
            //show
            await Task.Delay(500);
            this.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Process show
        /// </summary>
        /// <param name="name"></param>
        private void ProcessShow(string name)
        {
            switch (name)
            {
                case "MainData":
                    if (!String.IsNullOrEmpty(this.DataController.Backup.Url))
                    {
                        WebBrowserTask task = new WebBrowserTask();
                        task.Uri = new Uri(DataController.Backup.Url);
                        task.Show();
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Downlaod backup
        /// </summary>
        /// <param name="name"></param>
        private async void downloadBackup(string name)
        {
            switch (name)
            {
                case "MainData":
                    await ProcessBackupDownload();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Send email link
        /// </summary>
        /// <param name="p"></param>
        private void sendEmailLink(string name)
        {
            String url = "";
            String id = "";
            switch (name)
            {
                case "MainData":
                    url = DataController.Backup.Url;
                    id = DataController.Backup.Id;
                    break;
                default:
                    break;
            }

            var text = AppResources.SendByEmailBody.Replace("{E}", Environment.NewLine);

            //email
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.Subject = AppResources.SendByEmailSubject;
            emailComposeTask.Body = String.Format(text, id, url);
            emailComposeTask.Show();
        }

        /// <summary>
        /// Open main backup menu
        /// </summary>
        private void openMainBackupMenu()
        {
            openMenuIfItsNecessary();
            MenuController.Context = Controllers.MenuContext.MainBackup;
        }

        /// <summary>
        /// Open photo backup menu
        /// </summary>
        private void openMainPhotoMenu()
        {
            openMenuIfItsNecessary();
            MenuController.Context = Controllers.MenuContext.PhotoBackup;
        }

        /// <summary>
        /// Open video backup menu
        /// </summary>
        private void openMainVideoMenu()
        {
            openMenuIfItsNecessary();
            MenuController.Context = Controllers.MenuContext.VideoBackup;
        }



        /// <summary>
        /// ProcessBackupDownload
        /// </summary>
        /// <returns></returns>
        private async Task ProcessBackupDownload()
        {
            if (DownloadProgress.InProgress)
            {
                return;
            }

            //check backup
            var result = MessageBox.Show(AppResources.BackupApplyDescription, AppResources.BackupApplyTitle, MessageBoxButton.OKCancel);
            if (result != MessageBoxResult.OK)
            {
                return;
            }

            DownloadProgress.BytesTransferred = 0;

            //stop saving data
            DataController.Stop();

            //download and apply backup
            DownloadStatus state = await FtpController.Download(DownloadProgress);

            //unselect
            DownloadProgress.Selected = false;
            DownloadProgress.InProgress = false;

            if (state == DownloadStatus.Complete)
            {
                //load data
                DataController.FromBackup();
            }
            else
            {
                MessageBox.Show(AppResources.BackupNotFoundDescription, AppResources.BackupNotFoundTitle, MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// ProcessBackupUpload
        /// </summary>
        /// <returns></returns>
        private async Task ProcessBackupUpload()
        {
            UploadProgress.BytesTransferred = 0;

            //upload and save backup
            await FtpController.ProcessBackup(UploadProgress);
        }

        /// <summary>
        /// Create uplaod progress
        /// </summary>
        private void createUploadProgress()
        {
            UploadProgress.BytesTransferred = 0;
            UploadProgress.ProgressPercentage = 0;
            UploadProgress.Selected = true;
            UploadProgress.TotalBytes = 0;
            UploadProgress.Url = new Uri(Controllers.Data.DATA_FILE, UriKind.Relative);
            UploadProgress.Cancel = new System.Threading.CancellationTokenSource();
            UploadProgress.Type = Interfaces.Types.FileType.Data;
            UploadProgress.Preferences = ProgressPreferences.AllowOnCelluralAndBatery;
        }

        /// <summary>
        /// Create downlaod progress
        /// </summary>
        private void createDownloadProgress()
        {
            DownloadProgress.BytesTransferred = 0;
            DownloadProgress.Cancel = new System.Threading.CancellationTokenSource();
            DownloadProgress.ProgressPercentage = 0;
            DownloadProgress.Selected = true;
            DownloadProgress.TotalBytes = 0;
            DownloadProgress.Type = Interfaces.Types.FileType.Data;
            DownloadProgress.Url = new Uri(Controllers.Data.DATA_FILE, UriKind.Relative);
            DownloadProgress.Preferences = ProgressPreferences.AllowOnCelluralAndBatery;
        }

        #endregion 

        #region DATA CONTAINER

        /// <summary>
        /// Get default data container
        /// </summary>
        /// <returns></returns>
        private DataContainer GetDefaultDataContainer()
        {
            DataContainer data = new DataContainer();
            data.DataController = this.DataController;
            data.FtpController = this.FtpController;
            data.CameraController = this.CameraController;
            data.DriveModeController = this.DriveModeController;
            return data;
        }

        /// <summary>
        /// Get Picture data container
        /// </summary>
        /// <param name="fill"></param>
        /// <returns></returns>
        private DataContainer GetPictureDataContainer(CoreData.Picture picture)
        {
            DataContainer data = this.GetDefaultDataContainer();
            data.Picture = picture;
            return data;
        }

        /// <summary>
        /// Get Videos data container
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        private DataContainer GetVideoDataContainer(CoreData.Video video)
        {
            DataContainer data = this.GetDefaultDataContainer();
            data.Video = video;
            return data;
        }

        #endregion

        #region OPEN MENU

        /// <summary>
        /// Open menu if its closed
        /// </summary>
        private void openMenuIfItsNecessary()
        {
            if (!MenuController.IsOpen)
            {
                MenuController.open();
            }
        }

        /// <summary>
        /// Close menu if its closed
        /// </summary>
        private Boolean closeMenuIfItsNecessary()
        {
            if (MenuController.IsOpen)
            {
                MenuController.close();
                return true;
            }
            return false;
        }

        #endregion

        #region GLOBAL EVENTS

        /// <summary>
        /// On navigated to
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var data = NavigationService.GetLastNavigationData();
            if (data != null)
            {
                DataContainer container = data as DataContainer;
                this.CameraController = container.CameraController;
                this.FtpController = container.FtpController;
                this.DriveModeController = container.DriveModeController;
                this.DataController = container.DataController;
            }

            //show message
            if (FtpController.IsOneDriveAvailable)
            {
                MessageBox.Show(AppResources.Backup_NotSignedIn_Description, AppResources.Backup_NotSignedIn, MessageBoxButton.OK);
            }

            //connect upload
            FtpController.Connect(this.UploadProgress);

            if (App.IsInactiveMode)
            {
                CameraController.CameraStart();
            }
            App.IsInactiveMode = false;
            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// On navigate from
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            App.IsInactiveMode = (e.Uri.ToString() == "app://external/");
            if (App.IsInactiveMode)
            {
                StopRecordingNow();
            }
            base.OnNavigatingFrom(e);
        }

        /// <summary>
        /// On back key press
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            //try end drive mode
            CoPilot.DriveModeEnd(this.DriveModeController, e);

            if (e.Cancel == false && MenuController.IsOpen)
            {
                MenuController.close();
                e.Cancel = true;
            }
            base.OnBackKeyPress(e);
        }

        #endregion

        #region PROPERTY CHANGE

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// On property change
        /// </summary>
        /// <param name="name"></param>
        public void OnPropertyChanged(string name)
        {
            App.RootFrame.Dispatcher.BeginInvoke(() =>
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            });
        }

        /// <summary>
        /// Raise proeprty change
        /// </summary>
        /// <param name="caller"></param>
        public void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            App.RootFrame.Dispatcher.BeginInvoke(() =>
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(caller));
                }
            });
        }

        #endregion
    }
}