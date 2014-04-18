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
using Data = CoPilot.Core.Data;
using System.Collections.ObjectModel;
using CoPilot.Core.Utils;

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
                    MediaWithProgress picture = (MediaWithProgress)param;
                    if (picture.IsChecked)
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
                    MediaWithProgress video = (MediaWithProgress)param;
                    if (video.IsChecked)
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
                    NavigationService.Navigate("/CoPilot/View/Pictures.xaml", this.GetPictureDataContainer(param as Data.Picture));
                }, param => !this.FtpController.IsMediaBackup);
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
                    NavigationService.Navigate("/CoPilot/View/Videos.xaml", this.GetVideoDataContainer(param as Data.Video));
                }, param => !this.FtpController.IsMediaBackup);
            }
        }

        /// <summary>
        /// Backup Command
        /// </summary>
        public ICommand BackupCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    String name = (String)param;
                    switch (name)
                    {
                        case "MainData":
                            FtpController.ProcessBackup(name);
                            break;
                        case "Images":
                            FtpController.ProcessBackup(this.Pictures);
                            break;
                        case "Videos":
                            FtpController.ProcessBackup(getVideosArray());
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
                    FtpController.ProcessShow((String)param);
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
        /// Focus file id Command
        /// </summary>
        public ICommand FocusFileIdCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (this.FileId == AppResources.EnterFileId)
                    {
                        this.FileId = "";
                    }
                    this.FileIdStyle = App.Current.Resources["Value"] as Style;
                }, param => true);
            }
        }

        /// <summary>
        /// Blur file id Command
        /// </summary>
        public ICommand BlurFileIdCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    updateFileIdField();
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
        private ObservableCollection<MediaWithProgress> pictures = new ObservableCollection<MediaWithProgress>();
        public ObservableCollection<MediaWithProgress> Pictures
        {
            get
            {
                return pictures;
            }
        }

        /// <summary>
        /// Videos with progress
        /// </summary>
        private ObservableCollection<MediaWithProgress> videos = new ObservableCollection<MediaWithProgress>();
        public ObservableCollection<MediaWithProgress> Videos
        {
            get
            {
                return videos;
            }
        }

        /// <summary>
        /// File id style
        /// </summary>
        private Style fileIdStyle = App.Current.Resources["ValueEmpty"] as Style;
        public Style FileIdStyle
        {
            get
            {
                return fileIdStyle;
            }
            set
            {
                fileIdStyle = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// File id
        /// </summary>
        private String fileId = null;
        public String FileId
        {
            get
            {
                return fileId;
            }
            set
            {
                fileId = value;
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

            updateFileIdField();

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
        /// Update file id fields
        /// </summary>
        private void updateFileIdField()
        {
            if (String.IsNullOrEmpty(this.FileId))
            {
                this.FileId = AppResources.EnterFileId;
                this.FileIdStyle = App.Current.Resources["ValueEmpty"] as Style;
            }
            else
            {
                this.FileIdStyle = App.Current.Resources["Value"] as Style;
            }
        }

        /// <summary>
        /// Downlaod backup
        /// </summary>
        /// <param name="name"></param>
        private void downloadBackup(string name)
        {
            switch (name)
            {
                case "MainData":
                    if (this.FileId != AppResources.EnterFileId && !String.IsNullOrEmpty(this.FileId))
                    {
                        FtpController.Download(this.FileId, "MainData");
                        this.FileId = null;
                        this.updateFileIdField();
                    }
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
                    url = DataController.Backup.DownloadUrl;
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
        /// Get videos
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<MediaWithProgress> getVideosArray()
        {
            var videos = this.Videos;
            var collection = new ObservableCollection<MediaWithProgress>();

            foreach (MediaWithProgress video in videos) 
            {
                var preview = new MediaWithProgress();
                preview.IsChecked = video.IsChecked;
                preview.Preview = video.Video;
                preview.Progress = video.Progress;
                preview.Size = "0";

                collection.Add(preview);
                collection.Add(video);
            }

            return collection;
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
        private DataContainer GetPictureDataContainer(Data.Picture picture)
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
        private DataContainer GetVideoDataContainer(Data.Video video)
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

            this.FtpController.UpdateSizes();

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