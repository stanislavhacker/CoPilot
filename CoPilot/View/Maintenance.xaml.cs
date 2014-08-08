using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Controllers = CoPilot.CoPilot.Controller;
using CoreData = CoPilot.Core.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Globalization;
using CoPilot.Data;
using CoPilot.Core.Data;
using CoPilot.Resources;
using System.Windows.Input;
using CoPilot.Core.Utils;

namespace CoPilot.CoPilot.View
{
    public partial class Maintenance : PhoneApplicationPage, INotifyPropertyChanged
    {
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
        /// Type
        /// </summary>
        private List<MaintenanceTypeItem> types = new List<MaintenanceTypeItem>()
        {
            new MaintenanceTypeItem() { Name =  AppResources.MaintenanceType_Filters, Type = MaintenanceType.Filters },
            new MaintenanceTypeItem() { Name =  AppResources.MaintenanceType_Oil, Type = MaintenanceType.Oil },
            new MaintenanceTypeItem() { Name =  AppResources.MaintenanceType_Maintenance, Type = MaintenanceType.Maintenance },
            new MaintenanceTypeItem() { Name =  AppResources.MaintenanceType_Insurance, Type = MaintenanceType.Insurance },
            new MaintenanceTypeItem() { Name =  AppResources.MaintenanceType_TechnicalInspection, Type = MaintenanceType.TechnicalInspection }
        };
        public List<MaintenanceTypeItem> Types
        {
            get
            {
                return types;
            }
        }

        /// <summary>
        /// Type
        /// </summary>
        private MaintenanceTypeItem type = null;
        public MaintenanceTypeItem Type
        {
            get
            {
                if (type == null)
                {
                    type = Types[0];
                }
                return type;
            }
            set
            {
                type = value;
                RaisePropertyChanged();
                RaisePropertyChanged("IsOdometer");
            }
        }

        /// <summary>
        /// Is odometer
        /// </summary>
        public Boolean IsOdometer
        {
            get
            {   var maintenance = new CoreData.Maintenance();
                maintenance.Type = Type.Type;
                return maintenance.IsOdometer;
            }
        }





        /// <summary>
        /// Odometer
        /// </summary>
        private Double odometer = Double.NaN;
        public String Odometer
        {
            get
            {
                if (Double.IsNaN(odometer))
                {
                    return "";
                }
                return odometer.ToString();
            }
            set
            {
                if (!this.TryParseDouble(value, out odometer))
                {
                    odometer = Double.NaN;
                }
                RaisePropertyChanged();
                OnPropertyChanged("IsSaveEnable");
            }
        }

        /// <summary>
        /// Date
        /// </summary>
        private DateTime date = DateTime.Now;
        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                date = value;
                RaisePropertyChanged();
                OnPropertyChanged("IsSaveEnable");
            }
        }

        /// <summary>
        /// Description
        /// </summary>
        private String description = "";
        public String Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                RaisePropertyChanged();
                OnPropertyChanged("IsSaveEnable");
            }
        }

        /// <summary>
        /// WarningDistance
        /// </summary>
        private Int16 warningDistance = 0;
        public String WarningDistance
        {
            get
            {
                return warningDistance.ToString();
            }
            set
            {
                if (!this.TryParseInt(value, out warningDistance))
                {
                    warningDistance = 0;
                }
                RaisePropertyChanged();
                OnPropertyChanged("IsSaveEnable");
            }
        }

        /// <summary>
        /// WarningDays
        /// </summary>
        private Int16 warningDays = 0;
        public String WarningDays
        {
            get
            {
                return warningDays.ToString();
            }
            set
            {
                if (!this.TryParseInt(value, out warningDays))
                {
                    warningDays = 0;
                }
                RaisePropertyChanged();
                OnPropertyChanged("IsSaveEnable");
            }
        }


        /// <summary>
        /// Is save enable
        /// </summary>
        public Boolean IsSaveEnable
        {
            get
            {
                if (IsOdometer)
                {
                    return !Double.IsNaN(odometer) && this.description.Length > 0;
                }
                else
                {
                    return date > DateTime.Now && this.description.Length > 0;
                }
            }
        }

        #endregion

        #region COMMANDS

        /// <summary>
        /// Ok command
        /// </summary>
        public ICommand OkCommand
        {
            get
            {
                return new RelayCommand(
                    param =>
                    {
                        this.saveMaintenance();
                        NavigationService.GoBack();
                    },
                    param => true
                );
            }
        }

        /// <summary>
        /// Close command
        /// </summary>
        public ICommand CancelCommand
        {
            get
            {
                return new RelayCommand(
                    param =>
                    {
                        NavigationService.GoBack();
                    },
                    param => true
                );
            }
        }

        #endregion

        /// <summary>
        /// Maintenance
        /// </summary>
        public Maintenance()
        {
            InitializeComponent();

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

        #region PRIVATE FCE

        /// <summary>
        /// Try parse double
        /// </summary>
        /// <param name="value"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        private Boolean TryParseDouble(string value, out double outValue)
        {
            var style = NumberStyles.AllowDecimalPoint;
            var culture = CultureInfo.CurrentCulture;
            var result = Double.TryParse(value.Replace(",", "."), style, culture, out outValue);
            return result;
        }

        /// <summary>
        /// Try parse int
        /// </summary>
        /// <param name="value"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        private Boolean TryParseInt(string value, out Int16 outValue)
        {
            var style = NumberStyles.Integer;
            var culture = CultureInfo.CurrentCulture;
            var result = Int16.TryParse(value, style, culture, out outValue);
            return result;
        }

        /// <summary>
        /// Save maintenance
        /// </summary>
        private void saveMaintenance()
        {
            CoreData.Maintenance maintenance = new CoreData.Maintenance();
            maintenance.Type = this.Type.Type;
            maintenance.Description = this.description;

            if (this.IsOdometer)
            {
                //odometer type
                maintenance.Odometer = new CoreData.Odometer(this.odometer, DataController.Distance);
                maintenance.WarningDistance = this.warningDistance;
                maintenance.Date = DateTime.Now;
                maintenance.WarningDays = this.warningDays;
            }
            else
            {
                //date type
                maintenance.Date = DateTime.Now;
                maintenance.WarningDays = this.warningDays;
                maintenance.Odometer = new CoreData.Odometer(0, DataController.Distance);
                maintenance.WarningDistance = this.warningDistance;
            }

            DataController.AddMaintenance(maintenance);
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

        public class MaintenanceTypeItem
        {
            public String Name { get; set; }
            public MaintenanceType Type { get; set; }
        }
    }
}