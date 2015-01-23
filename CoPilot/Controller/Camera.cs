using CoPilot.Utils;
using Microsoft.Phone.Controls;
using System;
using System.Linq;
using System.IO.IsolatedStorage;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using CoPilot.Core.Data;
using CoPilot.Utils.Events;
using System.Collections.ObjectModel;
using CoPilot.Core.Utils;
using System.Threading.Tasks;

namespace CoPilot.CoPilot.Controller
{
    public class Camera : Base
    {

        #region PRIVATES

        private VideoCaptureDevice camera;
        private CaptureSource source;
        private DispatcherTimer videoTimer;
        private DispatcherTimer shotTimer;
        private Video video = null;

        private const int MAX_VIDEO_LENGTH = 5 * 60;

        #endregion

        #region COMMANDS

        /// <summary>
        /// Record Command
        /// </summary>
        public ICommand RecordCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (!this.IsRecording)
                    {
                        this.RecordStart();
                    }
                    else if (this.IsRecording)
                    {
                        this.RecordStop();
                    }
                    OnPropertyChanged("IsSupported");
                },
                param => true
               );
            }
        }

        /// <summary>
        /// Shot Command
        /// </summary>
        public ICommand ShotCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.TakeShot();
                },
                param => true
               );
            }
        }

        #endregion

        #region PROPERTY

        /// <summary>
        /// Is supported
        /// </summary>
        public bool IsSupported
        {
            get
            {
                return camera != null;
            }
        }

        /// <summary>
        /// Is camera enable
        /// </summary>
        public bool IsCameraEnabled
        {
            get
            {
                return IsSupported && IsAvailableSpaceForVideo && !this.startRecording && !this.stopRecording;
            }
        }

        /// <summary>
        /// Is shot enable
        /// </summary>
        public bool IsShotEnabled
        {
            get
            {
                return IsSupported && !this.IsRecording && this.IsAvailableSpaceForPhoto;
            }
        }

        /// <summary>
        /// Is camera record
        /// </summary>
        private bool isRecording = false;
        public bool IsRecording
        {
            get
            {
                return this.isRecording;
            }
            private set
            {
                this.timeStartRecording = DateTime.Now;
                this.isRecording = value;
                OnPropertyChanged("IsShotEnabled");
                if (RecordingStateChange != null)
                {
                    RecordingStateChange.Invoke(this, EventArgs.Empty);
                }
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Is neccesarry to start
        /// </summary>
        public bool IsNecessaryToStart
        {
            get
            {
                return source != null && (source.State == CaptureState.Stopped || source.State == CaptureState.Failed);
            }
        }

        /// <summary>
        /// Is picture shot
        /// </summary>
        private bool isShot = false;
        public bool IsShot
        {
            get
            {
                return this.isShot;
            }
            private set
            {
                this.isShot = value;

                if (value)
                {
                    shotTimer = new DispatcherTimer();
                    shotTimer.Interval = TimeSpan.FromMilliseconds(400);
                    shotTimer.Tick += delegate
                    {
                        this.IsShot = false;
                    };
                    shotTimer.Start();
                }
                else
                {
                    if (shotTimer != null)
                    {
                        shotTimer.Stop();
                    }
                }
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Video brush
        /// </summary>
        private VideoBrush videoBrush = null;
        public VideoBrush VideoBrush
        {
            get
            {
                return this.videoBrush;
            }
            set
            {
                this.videoBrush = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Get record time
        /// </summary>
        /// <returns></returns>
        public String RecordLength
        {
            get
            {
                if (this.isRecording)
                {
                    return this.RecordDuration.ToString("hh':'mm':'ss");
                }
                return "";
            }
        }

        /// <summary>
        /// Record duration
        /// </summary>
        public TimeSpan RecordDuration
        {
            get
            {
                if (this.isRecording)
                {
                    return DateTime.Now.Subtract(this.timeStartRecording);
                }
                return TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Set orientation of page
        /// </summary>
        private PageOrientation orientation = PageOrientation.LandscapeLeft;
        public PageOrientation Orientation
        {
            get
            {
                return this.orientation;
            }
            set
            {
                this.orientation = value;
                this.Rotation = this.orientation == PageOrientation.LandscapeRight ? 180 : 0;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Set rotation of camera
        /// </summary>
        private Double rotation = 0;
        public Double Rotation
        {
            get
            {
                return this.rotation;
            }
            set
            {
                this.rotation = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// Space for photo
        /// </summary>
        public Boolean IsAvailableSpaceForPhoto
        {
            get
            {
                var max = 5 * 1024 * 1024;
                return Storage.Get().AvailableFreeSpace > max;
            }
        }

        /// <summary>
        /// Space for video
        /// </summary>
        public Boolean IsAvailableSpaceForVideo
        {
            get
            {
                var max = 150 * 1024 * 1024;
                return Storage.Get().AvailableFreeSpace > max;
            }
        }

        #endregion

        #region EVENTS

        public event EventHandler<VideoSaveEventArgs> VideoSave;
        public event EventHandler<PictureSaveEventArgs> PictureSave;
        public event EventHandler<EventArgs> RecordingStateChange;

        #endregion

        /// <summary>
        /// Create new camera controller
        /// </summary>
        public Camera()
        {
            this.createFile();
        }

        #region CAMERA

        private bool startRecording = false;
        private bool stopRecording = false;
        private bool splitRecording = false;
        private bool isForceStop = false;

        private DateTime timeStartRecording;
        private FileSink videoSink;


        /// <summary>
        /// Create camera recorder and fill brush
        /// </summary>
        private void createCamera(string name = null)
        {
            source = new CaptureSource();
            source.CaptureImageCompleted += triggerCaptureImage;
            camera = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice();

            if (camera != null)
            {
                var supported = camera.SupportedFormats.Where(item => item.PixelFormat == PixelFormatType.Format32bppArgb).OrderBy(item => item.PixelWidth);
                source.VideoCaptureDevice.DesiredFormat = supported.ElementAt(0);

                if (videoSink != null)
                {
                    videoSink.IsolatedStorageFileName = name != null ? name : null;
                    videoSink.CaptureSource = name != null ? source : null;
                }

                VideoBrush = null;

                VideoBrush = new VideoBrush();
                VideoBrush.SetSource(source);
                VideoBrush.Stretch = Stretch.UniformToFill;

                source.Start();
            }
            OnPropertyChanged("IsSupported");
            OnPropertyChanged("IsCameraEnabled");
            OnPropertyChanged("IsShotEnabled");
        }

        /// <summary>
        /// Create file sink
        /// </summary>
        private void createFile()
        {
            videoSink = new FileSink();
        }

        /// <summary>
        /// Start camera
        /// </summary>
        public void CameraStart()
        {
            this.createCamera();
        }

        /// <summary>
        /// Start of recording
        /// </summary>
        public async void RecordStart()
        {
            if (source == null || source.VideoCaptureDevice == null)
            {
                return;
            }

            this.startRecording = true;
            OnPropertyChanged("IsCameraEnabled");
            OnPropertyChanged("IsShotEnabled");
            await Task.Delay(500);
            this.triggerRecordChange();
        }

        /// <summary>
        /// Stop of recording
        /// </summary>
        public async void RecordStop(bool force = false)
        {
            if (source == null || source.VideoCaptureDevice == null)
            {
                return;
            }
            this.isForceStop = force;

            this.stopRecording = true;
            OnPropertyChanged("IsCameraEnabled");
            OnPropertyChanged("IsShotEnabled");
            await Task.Delay(500);
            this.triggerRecordChange();
        }

        /// <summary>
        /// Take shot
        /// </summary>
        public void TakeShot()
        {
            if (source == null || source.VideoCaptureDevice == null)
            {
                return;
            }

            this.IsShot = true;
            this.source.CaptureImageAsync();
        }

        #endregion

        #region STATE CHANGE

        /// <summary>
        /// Trigger recorde status change
        /// </summary>
        private void triggerRecordChange()
        {
            this.source.Stop();
            if (this.startRecording)
            {
                //start recording
                startRecordingInternal();
            }
            if (this.stopRecording)
            {
                //save recording
                saveRecordingInternal();
                //force stop
                if (isForceStop)
                {
                    this.stopRecording = false;
                    this.IsRecording = false;
                    return;
                }
                //stop recording
                stopRecordingInternal();
            }
            if (this.splitRecording)
            {
                //save recording
                saveRecordingInternal();
                //clear sink
                videoSink.IsolatedStorageFileName = null;
                videoSink.CaptureSource = null;
                //start recording
                startRecordingInternal();
            }
            this.runTimer(this.IsRecording);
        }

        /// <summary>
        /// Stop recording internal
        /// </summary>
        private void stopRecordingInternal()
        {
            //create blank video
            this.createCamera(null);
            //set data
            this.stopRecording = false;
            this.splitRecording = false;
            this.IsRecording = false;
        }

        /// <summary>
        /// Save recording
        /// </summary>
        private void saveRecordingInternal()
        {
            video.Duration = this.RecordDuration;
            this.triggerVideoSave();
        }

        /// <summary>
        /// Start recording
        /// </summary>
        private void startRecordingInternal()
        {
            if (!this.IsAvailableSpaceForVideo)
            {
                stopRecordingInternal();
                return;
            }
            //create video, camera and capture image
            this.createVideoObject();
            this.createCamera(video.Path);
            this.source.CaptureImageAsync();
            //set data
            this.startRecording = false;
            this.splitRecording = false;
            this.IsRecording = true;
        }

        /// <summary>
        /// Create vide object
        /// </summary>
        private void createVideoObject()
        {
            video = new Video();
            video.Path = this.getFileName("mp4");
            video.Time = DateTime.Now;
            video.Rotated = this.Orientation == PageOrientation.LandscapeRight;
            video.Duration = TimeSpan.Zero;
            video.VideoBackup = new BackupInfo();
        }

        /// <summary>
        /// Video save event
        /// </summary>
        private void triggerVideoSave()
        {
            if (this.VideoSave != null)
            {
                VideoSaveEventArgs args = new VideoSaveEventArgs();
                args.Video = video;
                this.VideoSave.Invoke(this, args);
            }
            video = null;
        }

        /// <summary>
        /// Picture save event
        /// </summary>
        private void triggerPictureSave(Picture picture)
        {
            if (this.PictureSave != null)
            {
                PictureSaveEventArgs args = new PictureSaveEventArgs();
                args.Picture = picture;

                this.PictureSave.Invoke(this, args);
            }
        }


        /// <summary>
        /// On image capture complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void triggerCaptureImage(object sender, CaptureImageCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                return;
            }

            var name = this.getFileName("jpg");
            Picture picture = new Picture();
            picture.Path = name;
            picture.Time = DateTime.Now;
            picture.Rotated = this.Orientation == PageOrientation.LandscapeRight;
            picture.Backup = new BackupInfo();

            var wb = e.Result;
            var stream = Storage.CreateFile(picture.Path);
            wb.SaveJpeg(stream, wb.PixelWidth, wb.PixelHeight, 0, 60);
            stream.Close();

            if (this.IsRecording) {
                this.video.Preview = name;
            } else {
                this.triggerPictureSave(picture);
            }
        }

        /// <summary>
        /// Run update timer
        /// </summary>
        /// <param name="run"></param>
        private void runTimer(bool run)
        {
            //stop and return
            if (!run && videoTimer != null)
            {
                videoTimer.Stop();
                return;
            }
            //stop previous
            if (videoTimer != null && videoTimer.IsEnabled)
            {
                videoTimer.Stop();
            }
            //run again
            videoTimer = new DispatcherTimer();
            videoTimer.Interval = TimeSpan.FromMilliseconds(100);
            videoTimer.Tick += delegate
            {
                //change
                OnPropertyChanged("RecordLength");
                OnPropertyChanged("RecordDuration");
                //split
                if (RecordDuration.TotalSeconds > MAX_VIDEO_LENGTH)
                {
                    this.splitRecording = true;
                    this.triggerRecordChange();
                }
            };
            videoTimer.Start();
        }

        /// <summary>
        /// Get file name
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private string getFileName(String extension)
        {
            return DateTime.Now.ToString("'D'yyyy'-'MM'-'dd'T'HH'-'mm'-'ss") + "." + extension;
        }

        #endregion
    }
}