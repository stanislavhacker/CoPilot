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
using CoPilot.Utils;
using Controllers = CoPilot.CoPilot.Controller;
using CoreData = CoPilot.Core.Data;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Globalization;
using CoPilot.Core.Utils;
using CoPilot.Data;

namespace CoPilot.CoPilot.View
{
    public partial class Repair : PhoneApplicationPage, INotifyPropertyChanged
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
        /// Serivce name
        /// </summary>
        private String serviceName = "";
        public String ServiceName
        {
            get
            {
                return serviceName;
            }
            set
            {
                serviceName = value;
                RaisePropertyChanged();
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
            }
        }

        /// <summary>
        /// Repair price
        /// </summary>
        private Double repairPrice = Double.NaN;
        public String RepairPrice
        {
            get
            {
                if (Double.IsNaN(repairPrice))
                {
                    return "";
                }
                return repairPrice.ToString();
            }
            set
            {
                if (!this.TryParseDouble(value, out repairPrice))
                {
                    repairPrice = Double.NaN;
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
                return !Double.IsNaN(odometer) && !Double.IsNaN(repairPrice);
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
                        this.saveRepair();
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
        /// Repair
        /// </summary>
        public Repair()
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
        /// Save fuel
        /// </summary>
        private void saveRepair()
        {
            CoreData.Repair repair = new CoreData.Repair();
            repair.Odometer = new CoreData.Odometer(this.odometer, DataController.Distance);
            repair.Date = DateTime.Now;
            repair.ServiceName = this.serviceName;
            repair.Price = new CoreData.Price(this.repairPrice, DataController.Currency);
            repair.Description = this.description;
            DataController.AddRepair(repair);
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
            //popup
            if (Popup.MessageBox.Hide())
            {
                e.Cancel = true;
            }

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
    }
}