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
using System.Windows.Input;
using CoPilot.Utils;
using Controllers = CoPilot.CoPilot.Controller;
using CoPilot.Core.Data;
using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CoPilot.Core.Utils;
using Microsoft.Phone.Tasks;
using CoPilot.Interfaces;
using CoPilot.Interfaces.Types;
using System.Threading.Tasks;
using CoPilot.CoPilot.Controller;
using CoPilot.Resources;

namespace CoPilot.CoPilot.View
{
    public partial class Videos : PhoneApplicationPage, INotifyPropertyChanged
    {
        #region COMMANDS

        /// <summary>
        /// Next Command
        /// </summary>
        public ICommand NextCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.IsPlaying = false;
                    this.getNextVideo();
                }, param => true);
            }
        }

        /// <summary>
        /// Previous Command
        /// </summary>
        public ICommand PreviousCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.IsPlaying = false;
                    this.getPreviousVideo();
                }, param => true);
            }
        }

        /// <summary>
        /// Delete Command
        /// </summary>
        public ICommand SoftDeleteCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (!string.IsNullOrEmpty(this.Video.VideoBackup.Id))
                    {
                        this.IsPlaying = false;
                        this.deleteVideo(RemoveType.SoftDelete);
                        this.getCurrentVideo();
                    }
                }, param => true);
            }
        }

        /// <summary>
        /// Delete Command
        /// </summary>
        public ICommand HardDeleteCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.IsPlaying = false;
                    this.deleteVideo(RemoveType.HardDelete);
                    if (this.Max == 0)
                    {
                        NavigationService.GoBack();
                    }
                    else
                    {
                        this.closeMenuIfItsNecessary();
                        this.getPreviousVideo();
                    }
                }, param => true);
            }
        }

        /// <summary>
        /// Cancel Command
        /// </summary>
        public ICommand CancelCommand
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
        /// Confirm delete Command
        /// </summary>
        public ICommand ConfirmDeleteCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.openMenuIfItsNecessary();
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
                    if (!this.closeMenuIfItsNecessary() && this.IsPlayEnabled) 
                    {
                        this.IsPlaying = !this.isPlaying;
                    }
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
                    this.downloadBackupPreview();
                }, param => true);
            }
        }


        /// <summary>
        /// Command login
        /// </summary>
        public ICommand CommandLogin
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    NavigationService.Navigate("/CoPilot/View/Backup.xaml", this.GetDefaultDataContainer());
                }, param => true);
            }
        }

        /// <summary>
        /// Connetion
        /// </summary>
        public ICommand CommandConnection
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    ConnectionSettingsTask connectionSettingsTask = new ConnectionSettingsTask();
                    connectionSettingsTask.ConnectionSettingsType = ConnectionSettingsType.WiFi;
                    connectionSettingsTask.Show();
                }, param => true);
            }
        }

        /// <summary>
        /// Open video command
        /// </summary>
        public ICommand OpenVideoCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    WebBrowserTask task = new WebBrowserTask();
                    task.Uri = new Uri(this.Video.VideoBackup.Url);
                    task.Show();
                }, param => true);
            }
        }

        /// <summary>
        /// BufferingCommando command
        /// </summary>
        public ICommand BufferingCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    MediaElement media = param as MediaElement;
                    this.BufferingProgress = Math.Round(media.BufferingProgress * 100);
                    if (this.BufferingProgress == 100)
                    {
                        this.BufferingProgress = 0;
                    }
                }, param => true);
            }
        }

        /// <summary>
        /// MediaOpenedCommand command
        /// </summary>
        public ICommand MediaOpenedCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    MediaElement media = param as MediaElement;
                    this.IsPlayEnabled = true;
                }, param => true);
            }
        }

        /// <summary>
        /// MediaFailedCommand command
        /// </summary>
        public ICommand MediaFailedCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.IsPlayEnabled = false;
                    MessageBox.Show(AppResources.VideoError_Description, AppResources.VideoError_Title, MessageBoxButton.OK);
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
                this.Max = Convert.ToInt32(DataController.VideosCount);
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
        /// Video
        /// </summary>
        private Video video;
        public Video Video
        {
            get
            {
                return video;
            }
            set
            {
                //set video
                video = value;
                video.PropertyChanged -= videoStateChange;
                video.PropertyChanged += videoStateChange;

                //update brush and rotation
                setBrush(value);
                setVideoPath(value);
                this.Rotation = value.Rotated ? 180 : 0;

                //update progresss
                if (progresses.ContainsKey(value.Path))
                {
                    this.Progress = progresses[value.Path];
                }
                else
                {
                    this.Progress = null;
                }

                RaisePropertyChanged();
                RaisePropertyChanged("IsSoftDeleteEnabled");
                RaisePropertyChanged("IsHardDeleteEnabled");
            }
        }

        /// <summary>
        /// Rotation
        /// </summary>
        private Double rotation = 0;
        public Double Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Error
        /// </summary>
        private Boolean error = false;
        public Boolean Error
        {
            get
            {
                return error;
            }
            set
            {
                error = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Completed
        /// </summary>
        private Boolean completed = true;
        public Boolean Completed
        {
            get
            {
                return completed;
            }
            set
            {
                completed = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Position
        /// </summary>
        private int position = -1;
        public int Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Max
        /// </summary>
        private int max = 0;
        public int Max
        {
            get
            {
                return max;
            }
            set
            {
                max = value;
                OnPropertyChanged("IsButtonsEnabled");
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Enable buttons
        /// </summary>
        public Boolean IsButtonsEnabled
        {
            get
            {
                return dataController != null && dataController.VideosCount > 1;
            }
        }

        /// <summary>
        /// Poasition
        /// </summary>
        private TimeSpan videoPosition = TimeSpan.Zero;
        public TimeSpan VideoPosition
        {
            get
            {
                return videoPosition;
            }
            set
            {
                videoPosition = value;
                OnPropertyChanged("Time");
            }
        }

        /// <summary>
        /// BufferingProgress
        /// </summary>
        private Double bufferingProgress = 0;
        public Double BufferingProgress
        {
            get
            {
                return bufferingProgress;
            }
            set
            {
                bufferingProgress = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Video path
        /// </summary>
        private string videoPath = "";
        public string VideoPath
        {
            get
            {
                return videoPath;
            }
            set
            {
                videoPath = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Timer video
        /// </summary>
        public string Time
        {
            get
            {
                return videoPosition.ToString(@"hh\:mm\:ss");
            }
        }

        /// <summary>
        /// Is video playing
        /// </summary>
        private bool isPlaying = false;
        public bool IsPlaying
        {
            get
            {
                return isPlaying;
            }
            set
            {
                if (isPlaying == value)
                {
                    return;
                }

                isPlaying = value;
                TextOpacity = isPlaying ? 0.3 : 1;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Is video playing
        /// </summary>
        private bool isPlayEnabled = false;
        public bool IsPlayEnabled
        {
            get
            {
                return isPlayEnabled;
            }
            set
            {
                isPlayEnabled = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Is from cloud
        /// </summary>
        private bool isFromCloud = true;
        public bool IsFromCloud
        {
            get
            {
                return isFromCloud;
            }
            set
            {
                isFromCloud = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Timer opacity
        /// </summary>
        private double textOpacity = 1;
        public double TextOpacity
        {
            get
            {
                return textOpacity;
            }
            set
            {
                if (textOpacity == value)
                {
                    return;
                }
                textOpacity = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Brush
        /// </summary>
        private ImageSource brush;
        public ImageSource Brush
        {
            get
            {
                return brush;
            }
            set
            {
                brush = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Progress
        /// </summary>
        private Progress progress;
        public Progress Progress
        {
            get
            {
                return progress;
            }
            set
            {
                progress = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// IsSoftDeleteEnabled
        /// </summary>
        public bool IsSoftDeleteEnabled
        {
            get
            {
                var progress = this.FtpController.Progress(new Uri(this.Video.Path, UriKind.Relative));
                var backup = !string.IsNullOrEmpty(this.Video.VideoBackup.Id);
                return !progress && backup;
            }
        }

        /// <summary>
        /// IsHardDeleteEnabled
        /// </summary>
        public bool IsHardDeleteEnabled
        {
            get
            {
                var progress = this.FtpController.Progress(new Uri(this.Video.Path, UriKind.Relative));
                return !progress;
            }
        }

        #endregion

        #region PRIVATE

        private Dictionary<string, Progress> progresses = new Dictionary<string, Progress>();

        #endregion 


        /// <summary>
        /// Videos
        /// </summary>
        public Videos()
        {
            InitializeComponent();
            InitializeControllers();

            this.DataContext = this;
        }

        #region VIDEOS

        /// <summary>
        /// Video change
        /// </summary>
        private void videoChange()
        {
            this.Error = false;
            this.IsPlayEnabled = false;
            this.BufferingProgress = 0;
        }

        /// <summary>
        /// Get next video
        /// </summary>
        private void getNextVideo()
        {
            var videos = dataController.Videos;

            Position++;
            if (Position > videos.Count)
            {
                Position = 1;
            }
            this.videoChange();
            this.Video = videos.ElementAt(Position - 1);
        }

        /// <summary>
        /// Get previous video
        /// </summary>
        private void getPreviousVideo()
        {
            var videos = dataController.Videos;

            Position--;
            if (Position < 1)
            {
                Position = videos.Count;
            }
            this.videoChange();
            this.Video = videos.ElementAt(Position - 1);
        }

        /// <summary>
        /// Get current video
        /// </summary>
        private void getCurrentVideo()
        {
            var videos = dataController.Videos;
            if (Position <= videos.Count && Position > 0)
            {
                this.Video = videos.ElementAt(Position - 1);
            }
        }

        /// <summary>
        /// Delete video
        /// </summary>
        private void deleteVideo(RemoveType type)
        {
            var video = dataController.Videos.ElementAt(Position - 1);
            dataController.RemoveVideo(video, type);
            this.videoChange();
            this.Max = Convert.ToInt32(dataController.VideosCount);
        }

        /// <summary>
        /// Set brush
        /// </summary>
        /// <param name="value"></param>
        private void setBrush(Video value)
        {
            BitmapImage img = null;
            if (Storage.FileExists(value.Preview))
            {
                var stream = Storage.OpenFile(value.Preview, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                try
                {
                    img = new BitmapImage();
                    img.SetSource(stream);
                    this.Completed = true;
                }
                catch
                {
                    stream.Dispose();
                    this.downloadBackupPreview();
                }
            }
            else
            {
                this.downloadBackupPreview();
            }
            this.Brush = img;
        }

        /// <summary>
        /// Set video path
        /// </summary>
        /// <param name="value"></param>
        private void setVideoPath(Video value)
        {
            if (Storage.FileExists(value.Path))
            {
                this.IsFromCloud = false;
                this.VideoPath = value.Path;
            }
            else
            {
                this.IsFromCloud = true;
                downloadBackupVideo();
            }
        }

        /// <summary>
        /// Download backup
        /// </summary>
        private async void downloadBackupPreview()
        {
            this.Completed = false;
            this.Error = false;

            var video = this.Video;
            var id = video.VideoBackup.Id;

            //progress
            if (progresses.ContainsKey(video.Path))
            {
                return;
            }

            this.Progress = new Progress();
            this.Progress.Cancel = new System.Threading.CancellationToken();
            this.Progress.Selected = true;
            this.Progress.Type = FileType.Video;
            this.Progress.Url = new Uri(video.Preview, UriKind.Relative);
            this.Progress.Data = this.video;
            this.Progress.InProgress = true;

            //download
            progresses.Add(video.Path, this.Progress);
            await Task.Delay(200);
            DownloadStatus success = await FtpController.Preview(id, this.Progress);
            progresses.Remove(video.Path);

            //not this video
            if (video != this.Video)
            {
                return;
            }

            if (success == DownloadStatus.Complete)
            {
                this.Video = video;
            }
            else if (success == DownloadStatus.Fail)
            {
                this.Error = true;
            }
            this.Progress = null;
        }

        /// <summary>
        /// Download backup
        /// </summary>
        private async void downloadBackupVideo()
        {
            //video
            var video = this.Video;
            var tries = 6;
            this.IsPlayEnabled = false;

            //not logged, try to wait
            while (!FtpController.IsLogged && tries > 0)
            {
                await Task.Delay(500);
                tries--;
            }

            //download
            Response success = await FtpController.VideoUrl(this.Video.VideoBackup.Id);
            var url = success != null ? success.Url : "";

            //save
            if (video == this.Video)
            {
                this.VideoPath = url;
            }
        }

        /// <summary>
        /// Video state change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void videoStateChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != String.Empty)
            {
                return;
            }

            var video = this.Video;

            //brush
            this.setBrush(video);

            //update progresss
            if (progresses.ContainsKey(video.Path))
            {
                this.Progress = progresses[video.Path];
            }
        }

        #endregion

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

        #endregion

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
                this.DataController = container.DataController;
                this.CameraController = container.CameraController;
                this.DriveModeController = container.DriveModeController;
                this.FtpController = container.FtpController;

                //for given video
                if (container.Video != null)
                {
                    this.Position = this.DataController.Videos.IndexOf(container.Video) + 1;
                    this.getCurrentVideo();
                }
            }

            //on first load
            if (this.position == -1) {
                this.Position = Convert.ToInt32(this.DataController.VideosCount);
                this.getCurrentVideo();
            }

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
                DataController.Save(true);
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