﻿using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Controllers = CoPilot.CoPilot.Controller;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Phone.Info;
using System.Windows.Input;
using CoPilot.Utils;
using CoPilot.Utils.Events;
using CoreData = CoPilot.Core.Data;
using PR = System.Windows.Controls.Primitives;
using CoPilot.CoPilot.Controller;
using OdbCommunicator.OdbEventArg;
using OdbCommunicator;
using Windows.System;
using Microsoft.Phone.Tasks;
using System.Globalization;
using System.Windows.Resources;
using CoPilot.Resources;
using System.Linq;
using Windows.Networking.Connectivity;
using Microsoft.Phone.Net.NetworkInformation;
using System.IO;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Text;
using CoPilot.Core.Utils;
using System.Threading.Tasks;
using OdbCommunicator.OdbCommon;
using CoPilot.Core.Data;
using CoPilot.Utils.Enums;
using CoPilot.Data;
using CoPilot.Utils.Exception;
using Microsoft.Phone.Shell;

namespace CoPilot.CoPilot.View
{
    public partial class CoPilot : PhoneApplicationPage, INotifyPropertyChanged
    {
        #region STATIC

        /// <summary>
        /// Drive mode end
        /// </summary>
        /// <param name="driveMode"></param>
        /// <param name="e"></param>
        public static void DriveModeEnd(DriveMode driveMode, CancelEventArgs e)
        {
            if (driveMode != null && driveMode.IsOpen)
            {
                driveMode.StopDriveMode();
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Tutorial end
        /// </summary>
        /// <param name="e"></param>
        private static void TutorialEnd(CancelEventArgs e)
        {
            var tutorial = Tutorial.Tutorial.Current;
            if (tutorial != null && tutorial.IsTutorial)
            {
                tutorial.Close();
                e.Cancel = true;
            }
        }

        #endregion

        #region PRIVATE

        private PR.Popup popup;
        private SplashScreen screen;
        private BackgroundWorker backroungWorker;
        private Popup.MessageBox messageBox = null;

        #endregion

        #region PROPERTY

        /// <summary>
        /// Device is on external power source
        /// </summary>
        /// <returns></returns>
        public bool IsExternaPowerSource
        {
            get
            {
                return DeviceStatus.PowerSource == PowerSource.External;
            }
        }

        #endregion

        #region COMMANDS

        /// <summary>
        /// Record Command
        /// </summary>
        public ICommand CameraViewCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.closeMenuIfItsNecessary();
                    StatsController.IsVisible = false;
                },
                param => true
               );
            }
        }

        /// <summary>
        /// Tutorial Command
        /// </summary>
        public ICommand TutorialCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    Tutorial.Tutorial.Current.IsTutorial = true;
                    this.closeMenuIfItsNecessary();
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
        /// Camera Command
        /// </summary>
        public ICommand CameraTap
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.openCameraMenu();
                }, param => true);
            }
        }

        /// <summary>
        /// Fuel Command
        /// </summary>
        public ICommand FuelTap
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.openFuelMenu();
                }, param => true);
            }
        }

        /// <summary>
        /// Repair Command
        /// </summary>
        public ICommand RepairTap
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.openRepairMenu();
                }, param => true);
            }
        }

        /// <summary>
        /// More Command
        /// </summary>
        public ICommand MoreTap
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.openMoreMenu();
                }, param => true);
            }
        }

        /// <summary>
        /// Gps toggle
        /// </summary>
        public ICommand GpsToggleCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    DataController.IsGpsAllowed = !DataController.IsGpsAllowed;
                    GpsController.IsGpsAllowed = DataController.IsGpsAllowed;
                }, param => true);
            }
        }

        /// <summary>
        /// Obd toggle
        /// </summary>
        public ICommand ObdToggleCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    DataController.IsObdAllowed = !DataController.IsObdAllowed;
                    BluetoothController.IsObdAllowed = DataController.IsObdAllowed;
                }, param => true);
            }
        }

        /// <summary>
        /// Drive mode toggle
        /// </summary>
        public ICommand DriveModeToggleCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    DataController.IsDriveModeAllowed = !DataController.IsDriveModeAllowed;
                    DriveModeController.IsDriveModeAllowed = DataController.IsDriveModeAllowed;
                }, param => true);
            }
        }

        /// <summary>
        /// Drive mode manually start command
        /// </summary>
        public ICommand DriveModeCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    DriveModeController.StartDriveMode();
                    this.closeMenuIfItsNecessary();
                }, param => true);
            }
        }

        /// <summary>
        /// Bluetooth command
        /// </summary>
        public ICommand BluetoothCommand
        {
            get
            {
                return new RelayCommand(async (param) =>
                {
                    switch (BluetoothController.ErrorType)
                    {
                        case BluetoothErrorType.NotEnabled:
                            await Launcher.LaunchUriAsync(new Uri("ms-settings-bluetooth:"));
                            break;
                        case BluetoothErrorType.NotSelected:
                            NavigationService.Navigate("/CoPilot/View/Bluetooth.xaml", this.GetDefaultDataContainer());
                            break;
                        case BluetoothErrorType.NotFound:
                        case BluetoothErrorType.NoDevice:
                        case BluetoothErrorType.OutOfRange:
                        case BluetoothErrorType.FatalError:
                            BluetoothController.Scan();
                            break;
                        case BluetoothErrorType.None:
                            NavigationService.Navigate("/CoPilot/View/Obd.xaml", this.GetDefaultDataContainer());
                            break;
                        case BluetoothErrorType.NotAllowed:
                        default:
                            break;
                    }
                }, param => true);
            }
        }

        /// <summary>
        /// Pictures Command
        /// </summary>
        public ICommand PicturesCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    NavigationService.Navigate("/CoPilot/View/Pictures.xaml", this.GetPictureDataContainer(param as Picture));
                }, param => true);
            }
        }

        /// <summary>
        /// Videos Command
        /// </summary>
        public ICommand VideosCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    NavigationService.Navigate("/CoPilot/View/Videos.xaml", this.GetVideoDataContainer(param as Video));
                }, param => true);
            }
        }

        /// <summary>
        /// Stats Command
        /// </summary>
        public ICommand StatsCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    StatsController.IsVisible = !StatsController.IsVisible;
                }, param => true);
            }
        }

        /// <summary>
        /// add fuel command
        /// </summary>
        public ICommand AddFuelCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    NavigationService.Navigate("/CoPilot/View/Fuel.xaml", this.GetDefaultDataContainer());
                }, param => true);
            }
        }

        /// <summary>
        /// add repair command
        /// </summary>
        public ICommand AddRepairCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    NavigationService.Navigate("/CoPilot/View/Repair.xaml", this.GetDefaultDataContainer());
                }, param => true);
            }
        }

        /// <summary>
        /// add maintenance command
        /// </summary>
        public ICommand AddMaintenanceCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    NavigationService.Navigate("/CoPilot/View/Maintenance.xaml", this.GetDefaultDataContainer());
                }, param => true);
            }
        }

        /// <summary>
        /// ObdView command
        /// </summary>
        public ICommand ObdViewCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    NavigationService.Navigate("/CoPilot/View/ObdView.xaml", this.GetDefaultDataContainer());
                }, param => true);
            }
        }

        /// <summary>
        /// View fuel command
        /// </summary>
        public ICommand ViewFuelCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    NavigationService.Navigate("/CoPilot/View/FuelView.xaml", this.GetFillDataContainer(param as CoreData.Fill));
                }, param => true);
            }
        }

        /// <summary>
        /// View repair command
        /// </summary>
        public ICommand ViewRepairCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    NavigationService.Navigate("/CoPilot/View/RepairView.xaml", this.GetRepairDataContainer(param as CoreData.Repair));
                }, param => true);
            }
        }

        /// <summary>
        /// Change maintenance type
        /// </summary>
        public ICommand ChangeMaintenanceTypeCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    MaintenanceType = param as String;
                }, param => true);
            }
        }

        /// <summary>
        /// View maintenance command
        /// </summary>
        public ICommand ViewMaintenanceCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    NavigationService.Navigate("/CoPilot/View/MaintenanceView.xaml", this.GetMaintenanceDataContainer(param as CoreData.Maintenance));
                }, param => true);
            }
        }

        /// <summary>
        /// Change currency command
        /// </summary>
        public ICommand ChangeCurrencyCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    CoreData.Currency currency = (CoreData.Currency)Enum.Parse(typeof(CoreData.Currency), (String)param);
                    DataController.Currency = currency;
                }, param => true);
            }
        }

        /// <summary>
        /// Change distance command
        /// </summary>
        public ICommand ChangeDistanceCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    CoreData.Distance currency = (CoreData.Distance)Enum.Parse(typeof(CoreData.Distance), (String)param);
                    DataController.Distance = currency;
                }, param => true);
            }
        }

        /// <summary>
        /// Change unit command
        /// </summary>
        public ICommand ChangeUnitCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    CoreData.Unit unit = (CoreData.Unit)Enum.Parse(typeof(CoreData.Unit), (String)param);
                    DataController.Unit = unit;
                }, param => true);
            }
        }

        /// <summary>
        /// Social command
        /// </summary>
        public ICommand SocialCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.openUrl(param);
                }, param => true);
            }
        }

        /// <summary>
        /// Privacy policy command
        /// </summary>
        public ICommand PrivacyPolicyCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.privacyPolicy();
                }, param => true);
            }
        }

        /// <summary>
        /// Back command
        /// </summary>
        public ICommand BackCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                    }
                }, param => true);
            }
        }

        /// <summary>
        /// Toggle consumption command 
        /// </summary>
        public ICommand ToggleConsumptionCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    var values = Enum.GetValues(typeof(CoreData.Consumption)).Cast<CoreData.Consumption>();
                    var index = values.ToList().IndexOf(DataController.Consumption);
                    index++;
                    if (index >= values.Count())
                    {
                        index = 0;
                    }
                    DataController.Consumption = values.ElementAt(index);
                }, param => true);
            }
        }

        /// <summary>
        /// Backup command
        /// </summary>
        public ICommand BackupCommand
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
        /// Rate Command
        /// </summary>
        public ICommand RateCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    MarketplaceReviewTask review = new MarketplaceReviewTask();
                    review.Show();
                }, param => true);
            }
        }

        /// <summary>
        /// ShowFuelPriceTrend Command
        /// </summary>
        public ICommand ShowFuelPriceTrendCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    NavigationService.Navigate("/CoPilot/View/Graph.xaml", this.GetGraphDataContainer("TrendFuelPrices"));
                }, param => true);
            }
        }

        /// <summary>
        /// ShowTrendUnitsPerRefill Command
        /// </summary>
        public ICommand ShowTrendUnitsPerRefillCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    NavigationService.Navigate("/CoPilot/View/Graph.xaml", this.GetGraphDataContainer("TrendUnitsPerRefill"));
                }, param => true);
            }
        }

        /// <summary>
        /// Came info command
        /// </summary>
        public ICommand CameraInfoCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    //camera info popup
                    this.createCameraInfoMessageBox();
                }, param => true);
            }
        }

        #endregion


        #region PROPERTY STATS

        /// <summary>
        /// Stats controller
        /// </summary>
        private Controllers.Stats statsController;
        public Controllers.Stats StatsController
        {
            get
            {
                return statsController;
            }
            set
            {
                statsController = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region PROPERTY CAMERA

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

        #region PROPERTY DATA

        /// <summary>
        /// Date controller
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
                App.DataController = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Maintenance type
        /// </summary>
        private String maintenanceType = "Repairs";
        public String MaintenanceType
        {
            get
            {
                return maintenanceType;
            }
            set
            {
                maintenanceType = value;
                RaisePropertyChanged();
                RaisePropertyChanged("IsRepairs");
                RaisePropertyChanged("IsMaintenance");

            }
        }

        /// <summary>
        /// Is repairs
        /// </summary>
        public Boolean IsRepairs
        {
            get
            {
                return MaintenanceType == "Repairs";
            }
        }

        /// <summary>
        /// Is maintenance
        /// </summary>
        public Boolean IsMaintenance
        {
            get
            {
                return MaintenanceType == "Maintenance";
            }
        }

        #endregion

        #region PROPERTY GPS

        /// <summary>
        /// Gps controller
        /// </summary>
        private Controllers.Gps gpsController;
        public Controllers.Gps GpsController
        {
            get
            {
                return gpsController;
            }
            set
            {
                gpsController = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region PROPERTY DRIVE MODE

        /// <summary>
        /// Drove Mode controller
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

        #endregion

        #region PROPERTY BLUETOOTH

        /// <summary>
        /// Bluetooth controller
        /// </summary>
        private Controllers.Bluetooth bluetoothController;
        public Controllers.Bluetooth BluetoothController
        {
            get
            {
                return bluetoothController;
            }
            set
            {
                bluetoothController = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region PROPERTY FTP

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
                App.FtpController = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region PROPERTY TILES

        /// <summary>
        /// Tile controller
        /// </summary>
        private Controllers.Tile tileController;
        public Controllers.Tile TileController
        {
            get
            {
                return tileController;
            }
            set
            {
                tileController = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region PROPERTY SCHEDULER

        /// <summary>
        /// Scheduler controller
        /// </summary>
        private Controllers.Scheduler schedulerController;
        public Controllers.Scheduler SchedulerController
        {
            get
            {
                return schedulerController;
            }
            set
            {
                schedulerController = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region PROPERTY HTTP SERVER

        /// <summary>
        /// HttpServer controller
        /// </summary>
        private Controllers.HttpServer httpServerController;
        public Controllers.HttpServer HttpServerController
        {
            get
            {
                return httpServerController;
            }
            set
            {
                httpServerController = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region TRIAL DATA

        /// <summary>
        /// Is ad visible
        /// </summary>
        private Boolean isAdVisible = false;
        public Boolean IsAdVisible
        {
            get
            {
                return isAdVisible && this.IsAddvertismets;
            }
            set
            {
                isAdVisible = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Is Addvertismets
        /// </summary>
        public Boolean IsAddvertismets
        {
            get
            {
                return License.IsAddvertismets;
            }
        }

        /// <summary>
        /// AdRefreshedCommand
        /// </summary>
        public ICommand AdRefreshedCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.IsAdVisible = true;
                }, param => true);
            }
        }

        /// <summary>
        /// AdErrorCommand
        /// </summary>
        public ICommand AdErrorCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.IsAdVisible = false;
                }, param => true);
            }
        }

        /// <summary>
        /// Buy command
        /// </summary>
        public ICommand BuyCommand
        {
            get
            {
                return new RelayCommand(async (param) =>
                {
                    var progress = CoPilot.showSystemProgressIndicator();
                    //buy
                    await License.BuyIsAddvertismets();
                    //resolve
                    ResolveLicense();
                    //hide
                    progress.IsVisible = false;
                    SystemTray.IsVisible = false;
                }, param => true);
            }
        }

        #endregion

        /// <summary>
        /// Create copilot app
        /// </summary>
        public CoPilot()
        {
            InitializeComponent();
            CreateSplashScreen();
            StartLoadingData();
        }

        #region SPLASH SCRENN

        /// <summary>
        /// Create splash screen
        /// </summary>
        private void CreateSplashScreen()
        {
            this.screen = new SplashScreen();
            this.screen.Width = Application.Current.Host.Content.ActualWidth;
            this.screen.Height = Application.Current.Host.Content.ActualHeight;

            this.popup = new PR.Popup();
            this.popup.Child = screen;
            this.popup.IsOpen = true;
        }

        /// <summary>
        /// Start loading data
        /// </summary>
        private void StartLoadingData()
        {
            backroungWorker = new BackgroundWorker();
            backroungWorker.DoWork += startLoading;
            backroungWorker.RunWorkerCompleted += completeLoading;
            backroungWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Loading complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void completeLoading(object sender, RunWorkerCompletedEventArgs e)
        {
            var timeout = 300;

            this.Dispatcher.BeginInvoke(async () => {
                await Task.Delay(timeout * 3);
                this.screen.Animate(timeout);
                await Task.Delay(timeout * 2);
                this.popup.IsOpen = false;
            });
        }

        /// <summary>
        /// Loading start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startLoading(object sender, DoWorkEventArgs e)
        {
            //set tutorial app
            Tutorial.Tutorial.CoPilotApp = this;

            //init
            InitializeEvents();
            InitializeControllers();
            ResolveLicense();

            this.Dispatcher.BeginInvoke(() => {
                this.setupControllers();
                NavigationService.Navigated += triggerNavigated;
                this.DataContext = this;
            });
        }

        /// <summary>
        /// Resolve license
        /// </summary>
        private void ResolveLicense()
        {
            //Resovle license
            License.ResolveLicense();
            OnPropertyChanged("IsAddvertismets");
            OnPropertyChanged("IsAdVisible");
        }

        #endregion

        #region CONTROLLERS

        /// <summary>
        /// Initialize all controllers
        /// </summary>
        private void InitializeControllers()
        {
            //create controllers
            this.createDataController();
            this.createCameraController();
            this.createMenuController();
            this.createGpsController();
            this.createBluetoothController();
            this.createDriveModeController();
            this.createFtpController();
            this.createTileController();
            this.createSchedulerController();
            this.createStatsController();
            this.createHttpServerController();
        }

        /// <summary>
        /// Set up controllers
        /// </summary>
        private void setupControllers()
        {
            //data
            DataController.StartTimer();
            RateExchange.CurrentCurrency = DataController.Currency;
            DistanceExchange.CurrentDistance = DataController.Distance;
            UnitExchange.CurrentUnit = DataController.Unit;

            //camera
            CameraController.Orientation = this.Orientation;
            CameraController.CameraStart();

            //ftp
            FtpController.IsWifiEnabled = DeviceNetworkInformation.IsNetworkAvailable && DeviceNetworkInformation.IsWiFiEnabled;
            FtpController.IsNetEnabled = DeviceNetworkInformation.IsWiFiEnabled || DeviceNetworkInformation.IsCellularDataEnabled;
            FtpController.DataController = this.DataController;
            FtpController.TryLogin();

            //stats
            StatsController.IsNetEnabled = DeviceNetworkInformation.IsWiFiEnabled || DeviceNetworkInformation.IsCellularDataEnabled;

            //gps
            GpsController.IsGpsAllowed = DataController.IsGpsAllowed;

            //bluetooth
            BluetoothController.PreselectedDevice = DataController.ObdDevice;
            BluetoothController.IsObdAllowed = DataController.IsObdAllowed;
            BluetoothController.Scan();

            //drive mode
            DriveModeController.Orientation = this.Orientation;
            DriveModeController.IsDriveModeAllowed = DataController.IsDriveModeAllowed;
            DriveModeController.CoPilot = this;

            //tile controller
            TileController.DataController = this.DataController;
            TileController.Update();

            //scheduler
            SchedulerController.Update();

            //http
            HttpServerController.IdentifyDeviceIp();
            HttpServerController.ResolveData();

            //send error
            ExceptionCollector.SendError(AppResources.ReportTitle, AppResources.ReportDescription);
        }

        /// <summary>
        /// Create tile controller
        /// </summary>
        private void createTileController()
        {
            ///CONTROLLER
            TileController = new Controllers.Tile();
        }

        /// <summary>
        /// Create scheduler controller
        /// </summary>
        private void createSchedulerController()
        {
            ///CONTROLLER
            SchedulerController = new Controllers.Scheduler(this.dataController);
        }

        /// <summary>
        /// Create http server controller
        /// </summary>
        private void createHttpServerController()
        {
            ///CONTROLLER
            HttpServerController = new HttpServer(this.dataController, this);
        }

        /// <summary>
        /// Create camera controller
        /// </summary>
        private void createCameraController()
        {
            ///CONTROLLER
            CameraController = new Controllers.Camera();

            //Events
            CameraController.VideoSave += (object sender, VideoSaveEventArgs e) =>
            {
                DataController.AddVideo(e.Video);
            };
            CameraController.PictureSave += (object sender, PictureSaveEventArgs e) =>
            {
                DataController.AddPicture(e.Picture);
            };
            CameraController.RecordingStateChange += (object sender, EventArgs e) =>
            {
                StatsController.IsVisible = !CameraController.IsRecording;
            };
        }

        /// <summary>
        /// Create menu controller
        /// </summary>
        private void createMenuController()
        {
            ///CONTROLLER
            MenuController = new Controllers.Menu();
        }

        /// <summary>
        /// Create data controller
        /// </summary>
        private void createDataController()
        {
            ///CONTROLLER
            DataController = new Controllers.Data();

            //Events
            DataController.onUnitsChange += (object sender, EventArgs e) =>
            {
                RateExchange.CurrentCurrency = DataController.Currency;
                DistanceExchange.CurrentDistance = DataController.Distance;
                UnitExchange.CurrentUnit = DataController.Unit;
            };
            DataController.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                if (DriveModeController != null && DriveModeController.IsOpen && e.PropertyName == "Speed")
                {
                    DriveModeController.Speak(PredefinnedSpeak.Speed, DataController.Speed);
                }

                //tile update
                if (e.PropertyName == "AverageConsumption" ||
                    e.PropertyName == "Repairs" ||
                    e.PropertyName == "Fills" ||
                    e.PropertyName == "Maintenances" ||
                    e.PropertyName == "Consumption" ||
                    e.PropertyName == "Distance")
                {
                    TileController.Update();
                    SchedulerController.Update();
                }

                if (e.PropertyName == "AvailableSpace")
                {
                    CameraController.OnPropertyChanged("IsSupported");
                    CameraController.OnPropertyChanged("IsShotEnabled");
                }
            };
        }

        /// <summary>
        /// Create bluetooth controller
        /// </summary>
        private void createBluetoothController()
        {
            BluetoothController = new Controller.Bluetooth();

            //Events
            BluetoothController.DataReceive += (object sender, OdbEventArgs e) =>
            {
                if (BluetoothController.IsSuccess)
                {
                    this.getAllResponses(e);
                }
            };
            BluetoothController.SelectedDeviceChange += (object sender, EventArgs e) =>
            {
                DataController.ObdDevice = BluetoothController.PreselectedDevice;
            };
            BluetoothController.ConnectedToDevice += (object sender, EventArgs e) =>
            {
                DriveModeController.IsDeviceConnected = true;
            };
            BluetoothController.Disconnected += (object sender, EventArgs e) =>
            {
                DriveModeController.IsDeviceConnected = false;
            };
        }

        /// <summary>
        /// Create gps controller
        /// </summary>
        private void createGpsController()
        {
            ///CONTROLLER
            GpsController = new Controllers.Gps();

            //Events
            GpsController.onChange += (object sender, EventArgs e) =>
            {
                DataController.Position = gpsController.Current;
                if (!Double.IsNaN(gpsController.Speed) && !BluetoothController.IsSuccess)
                {
                    DataController.Speed = gpsController.Speed;
                }
            };
        }

        /// <summary>
        /// Create drive mode controller
        /// </summary>
        private void createDriveModeController()
        {
            ///CONTROLLER
            DriveModeController = new Controllers.DriveMode();
        }

        /// <summary>
        /// Create ftp controller
        /// </summary>
        private void createFtpController()
        {
            ///CONTROLLER
            FtpController = new Controllers.Ftp();

            //state change
            FtpController.OnStateChange += async (object sender, Interfaces.EventArgs.StateEventArgs e) =>
            {
                if (e.State == Interfaces.EventArgs.ConnectionStatus.Connected)
                {
                    //save data
                    Settings.Add("StorageConnected", (e.State == Interfaces.EventArgs.ConnectionStatus.Connected).ToString());
                    //try backup now
                    await FtpController.BackupNow(DeviceNetworkInformation.IsNetworkAvailable && DataController.IsBackupOnStart);
                    //try download speedway
                    if (await FtpController.SpeedWayNow(DeviceNetworkInformation.IsNetworkAvailable && DataController.IsBackupOnStart))
                    {
                        DataController.RaisePropertyChanged("Circuits");
                    }
                }
            };

            FtpController.Error += (object sender, Interfaces.EventArgs.ErrorEventArgs e) =>
            {
                //save data
                Settings.Add("StorageConnected", false.ToString());
                FtpController.IsLogged = false;
            };
        }


        /// <summary>
        /// Create stats controller
        /// </summary>
        private void createStatsController()
        {
            ///CONTROLLER
            StatsController = new Controllers.Stats(this.dataController, this);
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
            data.BluetoothController = this.BluetoothController;
            data.FtpController = this.FtpController;
            data.CameraController = this.CameraController;
            data.DriveModeController = this.DriveModeController;
            data.StatsController = this.StatsController;
            return data;
        }

        /// <summary>
        /// Get video data container
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        private DataContainer GetVideoDataContainer(CoreData.Video video)
        {
            DataContainer data = this.GetDefaultDataContainer();
            data.Video = video;
            return data;
        }

        /// <summary>
        /// Get picture data container
        /// </summary>
        /// <param name="picture"></param>
        /// <returns></returns>
        private DataContainer GetPictureDataContainer(CoreData.Picture picture)
        {
            DataContainer data = this.GetDefaultDataContainer();
            data.Picture = picture;
            return data;
        }

        /// <summary>
        /// Get fill data container
        /// </summary>
        /// <param name="fill"></param>
        /// <returns></returns>
        private DataContainer GetFillDataContainer(CoreData.Fill fill)
        {
            DataContainer data = this.GetDefaultDataContainer();
            data.Fill = fill;
            return data;
        }

        /// <summary>
        /// Get repair data container
        /// </summary>
        /// <param name="repair"></param>
        /// <returns></returns>
        private DataContainer GetRepairDataContainer(CoreData.Repair repair)
        {
            DataContainer data = this.GetDefaultDataContainer();
            data.Repair = repair;
            return data;
        }

        /// <summary>
        /// Get maintenance data container
        /// </summary>
        /// <param name="repair"></param>
        /// <returns></returns>
        private DataContainer GetMaintenanceDataContainer(CoreData.Maintenance maintenance)
        {
            DataContainer data = this.GetDefaultDataContainer();
            data.Maintenance = maintenance;
            data.SchedulerController = schedulerController;
            return data;
        }

        /// <summary>
        /// Get uri data container
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private DataContainer GetUriDataContainer(Uri uri)
        {
            DataContainer data = this.GetDefaultDataContainer();
            data.Uri = uri;
            return data;
        }

        /// <summary>
        /// Get graph type data container
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        private DataContainer GetGraphDataContainer(String graph)
        {
            DataContainer data = this.GetDefaultDataContainer();
            data.GrapType = graph;
            return data;
        }

        #endregion

        #region PRIVATE

        /// <summary>
        /// Trigger navigated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void triggerNavigated(object sender, NavigationEventArgs e)
        {
            DriveModeController.Page = e.Uri.OriginalString.Replace("/CoPilot/View/", "");
            DriveModeController.Content = e.Content;
        }

        /// <summary>
        /// Get all responses
        /// </summary>
        /// <param name="e"></param>
        private void getAllResponses(OdbEventArgs e)
        {
            this.getResponseFor(e.Client, OdbPids.Mode1_VehicleSpeed, data => DataController.Speed = data);
            this.getResponseFor(e.Client, OdbPids.Mode1_EngineRpm, data => DataController.Rpm = data);
            this.getResponseFor(e.Client, OdbPids.Mode1_EngineCoolantTemperature, data => DataController.Temperature = data);
            this.getResponseFor(e.Client, OdbPids.Mode1_EngineCalculatedLoad, data => DataController.EngineLoad = data);
            this.getResponseFor(e.Client, OdbPids.Mode1_MafAirFlowRate, data => DataController.MaxAirFlowRate = data);
            this.getResponseFor(e.Client, OdbPids.Mode1_ThrottlePosition, data => DataController.ThrottlePosition = data);
            this.getResponseFor(e.Client, OdbPids.Mode1_RunTimeSinceEngineStart, data => DataController.Uptime = data);
            this.getResponseFor(e.Client, OdbPids.Mode1_RelativeAcceleratorPedalPosition, data => DataController.AcceleratorPedalPosition = data);
            this.getResponseFor(e.Client, OdbPids.Mode1_EngineOilTemperature, data => DataController.EngineOilTemperature = data);
            this.getResponseFor(e.Client, OdbPids.Mode1_FuelInjectionTiming, data => DataController.FuelInjectionTiming = data);
            this.getResponseFor(e.Client, OdbPids.Mode1_EngineReferenceTorque, data => DataController.EngineReferenceTorque = data);
        }

        /// <summary>
        /// Get response for
        /// </summary>
        /// <param name="client"></param>
        /// <param name="odbPid"></param>
        /// <param name="action"></param>
        private void getResponseFor(OdbClient client, OdbPid odbPid, Action<Double> action)
        {
            var response = client.RequestFor(odbPid);
            if (response != null)
            {
                action(response.Data);
            }
        }

        /// <summary>
        /// Open url
        /// </summary>
        /// <param name="param"></param>
        private void openUrl(object param)
        {
            String type = (String)param;
            String url;

            switch (type)
            {
                case "Facebook":
                    url = "https://www.facebook.com/carcopilot";
                    break;
                case "Twitter":
                    url = "https://twitter.com/carcopilot";
                    break;
                case "GooglePlus":
                    url = "https://plus.google.com/u/0/b/115628070739534024707/115628070739534024707/posts";
                    break;
                case "Donate":
                    url = "https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=LVCK2P82YFGJ6";
                    break;
                case "Blog":
                    url = "http://carcopilot.blogspot.cz";
                    break;
                default:
                    url = type;
                    break;
            }

            WebBrowserTask www = new WebBrowserTask();
            www.Uri = new Uri(url);
            www.Show();
        }

        /// <summary>
        /// Privacy policy
        /// </summary>
        private void privacyPolicy()
        {
            var culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            StreamResourceInfo resource;
            Uri uri;
            try
            {
                uri = new Uri("Resources/PrivacyPolicy/" + culture + ".html", UriKind.Relative);
                resource = App.GetResourceStream(uri);
            }
            catch
            {
                uri = new Uri("Resources/PrivacyPolicy/en.html", UriKind.Relative);
                resource = App.GetResourceStream(uri);
            }
            NavigationService.Navigate("/CoPilot/View/WebBrowser.xaml", this.GetUriDataContainer(uri));
        }

        /// <summary>
        /// Create camera info message box
        /// </summary>
        private void createCameraInfoMessageBox()
        {
            Popup.MessageBox box = Popup.MessageBox.Create();
            box.Caption = AppResources.CameraInfoTitle;
            box.Message = AppResources.CameraInfoDescription;
            box.ShowLeftButton = false;
            box.ShowRightButton = true;
            box.RightButtonText = AppResources.Ok;
            box.IsOpen = true;
        }

        #endregion

        #region OPEN MENU

        /// <summary>
        /// Open camera menu
        /// </summary>
        /// <returns></returns>
        private void openCameraMenu()
        {
            openMenuIfItsNecessary();
            MenuController.Context = Controllers.MenuContext.Camera;
        }

        /// <summary>
        /// Open fuel menu
        /// </summary>
        /// <returns></returns>
        private void openFuelMenu()
        {
            openMenuIfItsNecessary();
            MenuController.Context = Controllers.MenuContext.Fuel;
        }

        /// <summary>
        /// Open repair menu
        /// </summary>
        /// <returns></returns>
        private void openRepairMenu()
        {
            openMenuIfItsNecessary();
            MenuController.Context = Controllers.MenuContext.Repair;
        }

        /// <summary>
        /// Open more menu
        /// </summary>
        /// <returns></returns>
        private void openMoreMenu()
        {
            openMenuIfItsNecessary();
            MenuController.Context = Controllers.MenuContext.Other;
        }



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
        /// Bind events on page
        /// </summary>
        private void InitializeEvents()
        {
            App.RootFrame.Obscured += onScreenLock;
            App.RootFrame.Unobscured += onScreenUnlock;
            DeviceStatus.PowerSourceChanged += triggerPowerSourceChanged;
            DeviceNetworkInformation.NetworkAvailabilityChanged += triggerNetworkAvailabilityChanged;
            this.OrientationChanged += triggerOrientationChanged;
        }

        /// <summary>
        /// Trigger newtwork change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void triggerNetworkAvailabilityChanged(object sender, NetworkNotificationEventArgs e)
        {
            if (FtpController != null)
            {
                FtpController.IsWifiEnabled = DeviceNetworkInformation.IsNetworkAvailable && DeviceNetworkInformation.IsWiFiEnabled;
                FtpController.IsNetEnabled = DeviceNetworkInformation.IsWiFiEnabled || DeviceNetworkInformation.IsCellularDataEnabled;
            }
            if (HttpServerController != null)
            {
                HttpServerController.IdentifyDeviceIp();
            }
        }

        /// <summary>
        /// Trigger power source change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void triggerPowerSourceChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("IsExternaPowerSource");
        }

        /// <summary>
        /// Orientation change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void triggerOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if (CameraController != null)
            {
                CameraController.Orientation = e.Orientation;
            }
            if (DriveModeController != null)
            {
                DriveModeController.Orientation = e.Orientation;
            }
        }

        /// <summary>
        /// On back key press
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            //popup
            if (this.messageBox == null && Popup.MessageBox.Hide())
            {
                e.Cancel = true;
            }

            //try end tutorial
            CoPilot.TutorialEnd(e);
            //try end drive mode
            CoPilot.DriveModeEnd(this.DriveModeController, e);

            //close menu
            if (e.Cancel == false && MenuController != null && MenuController.IsOpen)
            {
                MenuController.close();
                e.Cancel = true;
            }

            //close stats
            if (e.Cancel == false && StatsController != null && StatsController.IsVisible)
            {
                StatsController.IsVisible = false;
                e.Cancel = true;
            }

            //dialog before exit
            if (e.Cancel == false && this.messageBox == null)
            {
                e.Cancel = true;
                this.exitConfirmMessageBox();
            }

            //stop all
            if (e.Cancel == false)
            {
                StopRecordingNow();
            }
            base.OnBackKeyPress(e);
        }

        /// <summary>
        /// Exit confirm box
        /// </summary>
        private void exitConfirmMessageBox()
        {
            this.messageBox = Popup.MessageBox.Create();
            this.messageBox.Caption = AppResources.AppEndTitle;
            this.messageBox.Message = AppResources.AppEndDescription;
            this.messageBox.ShowLeftButton = false;
            this.messageBox.ShowRightButton = true;
            this.messageBox.RightButtonText = AppResources.Cancel;

            this.messageBox.Dismiss += (sender, e1) =>
            {
                this.messageBox = null;
            };

            this.messageBox.IsOpen = true;
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

        /// <summary>
        /// On navigate on page
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var fromInactive = e.NavigationMode == NavigationMode.New || App.IsInactiveMode;
            if (fromInactive && CameraController != null)
            {
                CameraController.CameraStart();
            }
            if (fromInactive && BluetoothController != null)
            {
                BluetoothController.Scan();
            }
            if (fromInactive)
            {
                ResolveLicense();
            }
            App.IsInactiveMode = false;
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            App.IsInactiveMode = (e.Uri.ToString() == "app://external/");
            if (e.NavigationMode == NavigationMode.Back || App.IsInactiveMode)
            {
                StopRecordingNow();
                DataController.Save(true);
            }
            base.OnNavigatingFrom(e);
        }

        /// <summary>
        /// on screen lock
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onScreenLock(object sender, ObscuredEventArgs e)
        {
            if (DataController != null)
            {
                DataController.Save(true);
            }
        }

        /// <summary>
        /// on screen unlock
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onScreenUnlock(object sender, EventArgs e)
        {
            if (CameraController != null && CameraController.IsNecessaryToStart)
            {
                CameraController.CameraStart();
            }
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

        #region STATIC

        /// <summary>
        /// Show system progress indicator
        /// </summary>
        /// <returns></returns>
        private static ProgressIndicator showSystemProgressIndicator()
        {
            SystemTray.Opacity = 0.8;
            SystemTray.IsVisible = true;
            SystemTray.ProgressIndicator = new ProgressIndicator();
            var progress = SystemTray.ProgressIndicator;
            progress.IsIndeterminate = true;
            progress.IsVisible = true;
            progress.Text = AppResources.Working;
            return progress;
        }

        #endregion
    }
}