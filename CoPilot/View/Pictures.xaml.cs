using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CoPilot.Utils;
using Controllers = CoPilot.CoPilot.Controller;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CoPilot.Core.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.Windows.Input;
using CoPilot.Core.Utils;
using Microsoft.Phone.Tasks;

namespace CoPilot.CoPilot.View
{
    public partial class Pictures : PhoneApplicationPage, INotifyPropertyChanged
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
                    this.getNextPicture();
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
                    this.getPreviousPicture();
                }, param => true);
            }
        }

        /// <summary>
        /// Delete Command
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.deletePicture();
                    if (this.Max == 0)
                    {
                        NavigationService.GoBack();
                    }
                    else
                    {
                        this.closeMenuIfItsNecessary();
                        this.getPreviousPicture();
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
                    this.closeMenuIfItsNecessary();
                }, param => true);
            }
        }

        /// <summary>
        /// Oprn url Command
        /// </summary>
        public ICommand OpenUrlCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.openBackupInBrowser();
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
                    this.downloadBackup();
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
                this.Max = Convert.ToInt32(dataController.PicturesCount);
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
        /// Picture
        /// </summary>
        private Picture image;
        public Picture Image
        {
            get
            {
                return image;
            }
            set
            {
                //set image
                image = value;
                image.PropertyChanged -= imageStateChange;
                image.PropertyChanged += imageStateChange;

                //update brush and rotation
                setBrush(value);
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
        /// NotFound
        /// </summary>
        private Boolean notFound = false;
        public Boolean NotFound
        {
            get
            {
                return notFound;
            }
            set
            {
                if (notFound == value)
                {
                    return;
                }
                notFound = value;
                RaisePropertyChanged();
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
                return dataController != null && dataController.PicturesCount > 1;
            }
        }

        /// <summary>
        /// Progress
        /// </summary>
        private MediaWithProgress progress;
        public MediaWithProgress Progress
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

        #endregion

        #region PRIVATE

        private Dictionary<string, MediaWithProgress> progresses = new Dictionary<string, MediaWithProgress>();

        #endregion 


        /// <summary>
        /// Pictures
        /// </summary>
        public Pictures()
        {
            InitializeComponent();
            InitializeControllers();

            this.DataContext = this;
        }

        #region PICTURES

        /// <summary>
        /// Get next picture
        /// </summary>
        private void getNextPicture()
        {
            var pictures = dataController.Pictures;

            Position++;
            if (Position > pictures.Count)
            {
                Position = 1;
            }
            this.Image = pictures.ElementAt(Position - 1);
        }

        /// <summary>
        /// Get previous picture
        /// </summary>
        private void getPreviousPicture()
        {
            var pictures = dataController.Pictures;
            Position--;
            if (Position < 1)
            {
                Position = pictures.Count;
            }
            this.Image = pictures.ElementAt(Position - 1);
        }

        /// <summary>
        /// Get current picture
        /// </summary>
        private void getCurrentPicture()
        {
            var pictures = dataController.Pictures;
            if (Position <= pictures.Count && Position > 0)
            {
                this.Image = pictures.ElementAt(Position - 1);
            }
        }

        /// <summary>
        /// Delete picture
        /// </summary>
        private void deletePicture()
        {
            var picture = dataController.Pictures.ElementAt(Position - 1);
            dataController.RemovePicture(picture);
            this.Max = Convert.ToInt32(dataController.PicturesCount);
        }

        /// <summary>
        /// Set brush
        /// </summary>
        /// <param name="value"></param>
        private void setBrush(Picture value)
        {
            BitmapImage img = null;
            if (Storage.FileExists(value.Path)) 
            {
                var stream = Storage.OpenFile(value.Path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                img = new BitmapImage();
                img.SetSource(stream);
                this.NotFound = false;
            }
            else
            {
                this.NotFound = true;
            }
            this.Brush = img;
        }

        /// <summary>
        /// Open backup in browse
        /// </summary>
        private void openBackupInBrowser()
        {
            WebBrowserTask browser = new WebBrowserTask();
            browser.Uri = new Uri(this.Image.Backups.First().DownloadUrl);
            browser.Show();
        }

        /// <summary>
        /// Download backup
        /// </summary>
        private void downloadBackup()
        {
            var image = this.Image;
            var id = image.Backups.First().Id;

            this.Progress = new MediaWithProgress();
            this.Progress.Progress = new ProgressUpdater();
            this.Progress.Progress.IsIndetermine = true;
            this.Progress.Picture = this.Image;
            this.Progress.IsChecked = true;

            if (progresses.ContainsKey(image.Path))
            {
                progresses.Remove(image.Path);
            }
            progresses.Add(image.Path, this.Progress);
            FtpController.Download(id, this.Progress);
        }

        /// <summary>
        /// Image state change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imageStateChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != String.Empty)
            {
                return;
            }

            var image = this.Image;

            //brush
            this.setBrush(image);

            //progress
            //update progresss
            if (progresses.ContainsKey(image.Path))
            {
                this.Progress = progresses[image.Path];
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
        private void closeMenuIfItsNecessary()
        {
            if (MenuController.IsOpen)
            {
                MenuController.close();
            }
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

                //for given picture
                if (container.Picture != null)
                {
                    this.Position = this.DataController.Pictures.IndexOf(container.Picture) + 1;
                    this.getCurrentPicture();
                }
            }

            //on first load
            if (this.position == -1) {
                this.Position = Convert.ToInt32(this.DataController.PicturesCount);
                this.getCurrentPicture();
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