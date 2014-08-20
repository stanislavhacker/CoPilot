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
using System.Runtime.CompilerServices;
using Controllers = CoPilot.CoPilot.Controller;
using CoPilot.Utils;
using System.Globalization;
using System.Windows.Input;
using CoreData = CoPilot.Core.Data;
using CoPilot.Core.Utils;
using CoPilot.Data;

namespace CoPilot.CoPilot.View
{
    public partial class Fuel : PhoneApplicationPage, INotifyPropertyChanged
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
        /// Trip distance kilometres
        /// </summary>
        public Double TripDistanceKilometres
        {
            get
            {
                if (DataController != null && DataController.Fills.Count > 0 && !Double.IsNaN(odometer))
                {
                    var lastFill = DataController.Fills.First();
                    var odometerLast = DistanceExchange.GetOdometerWithRightDistance(lastFill.Odometer);
                    if (odometer - odometerLast > 0)
                    {
                        return Math.Round(odometer - odometerLast, 1);
                    }
                }
                return 0;
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
                OnPropertyChanged("TripDistanceKilometres");
            }
        }

        /// <summary>
        /// Fuel price
        /// </summary>
        private Double fuelPrice = Double.NaN;
        public String FuelPrice
        {
            get
            {
                if (Double.IsNaN(fuelPrice))
                {
                    return "";
                }
                return fuelPrice.ToString();
            }
            set
            {
                if (this.TryParseDouble(value, out fuelPrice))
                {
                    calculatePricePerLiter();
                    calculateLiters();
                }
                else
                {
                    fuelPrice = Double.NaN;
                }
                RaisePropertyChanged();
                OnPropertyChanged("IsSaveEnable");
            }
        }

        /// <summary>
        /// Price per liter
        /// </summary>
        private Double pricePerLiter = Double.NaN;
        public String PricePerLiter
        {
            get
            {
                if (Double.IsNaN(pricePerLiter))
                {
                    return "";
                }
                return pricePerLiter.ToString();
            }
            set
            {
                if (this.TryParseDouble(value, out pricePerLiter))
                {
                    calculateFuelPrice();
                    calculateLiters();
                }
                else
                {
                    pricePerLiter = Double.NaN;
                }
                RaisePropertyChanged();
                OnPropertyChanged("IsSaveEnable");
            }
        }

        /// <summary>
        /// Liters
        /// </summary>
        private Double liters = Double.NaN;
        public String Liters
        {
            get
            {
                if (Double.IsNaN(liters))
                {
                    return "";
                }
                return liters.ToString();
            }
            set
            {
                if (this.TryParseDouble(value, out liters))
                {
                    calculatePricePerLiter();
                    calculateFuelPrice();
                }
                else
                {
                    liters = Double.NaN;
                }
                RaisePropertyChanged();
                OnPropertyChanged("IsSaveEnable");
            }
        }

        /// <summary>
        /// Is tank full
        /// </summary>
        private Boolean isTankFull = false;
        public Boolean IsTankFull
        {
            get
            {
                return isTankFull;
            }
            set
            {
                isTankFull = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Is save enable
        /// </summary>
        public Boolean IsSaveEnable
        {
            get
            {
                return !Double.IsNaN(odometer) && 
                    !Double.IsNaN(fuelPrice) && 
                    !Double.IsNaN(pricePerLiter) && 
                    !Double.IsNaN(liters) &&
                    (TripDistanceKilometres > 0 || DataController.Fills.Count == 0);
            }
        }

        #endregion

        #region STYLE PROPERTY

        /// <summary>
        /// Fuel price style
        /// </summary>
        private Style fuelPriceStyle = App.Current.Resources["Value"] as Style; 
        public Style FuelPriceStyle
        {
            get
            {
                return fuelPriceStyle;
            }
            set
            {
                fuelPriceStyle = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Price per liter style
        /// </summary>
        private Style pricePerLiterStyle = App.Current.Resources["Value"] as Style;
        public Style PricePerLiterStyle
        {
            get
            {
                return pricePerLiterStyle;
            }
            set
            {
                pricePerLiterStyle = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Liters style
        /// </summary>
        private Style litersStyle = App.Current.Resources["Value"] as Style;
        public Style LitersStyle
        {
            get
            {
                return litersStyle;
            }
            set
            {
                litersStyle = value;
                RaisePropertyChanged();
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
                        this.saveFuel();
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

        /// <summary>
        /// Focus command
        /// </summary>
        public ICommand FocusCommand
        {
            get
            {
                return new RelayCommand(
                    (param) => {
                        String name = param as String;
                        switch (name)
                        {
                            case "FuelPrice":
                                if (FuelPriceStyle == emptyValueStyle) 
                                {
                                    FuelPriceStyle = defaultValueStyle;
                                    FuelPrice = "";
                                }
                                break;
                            case "PricePerLiter":
                                if (PricePerLiterStyle == emptyValueStyle)
                                {
                                    PricePerLiterStyle = defaultValueStyle;
                                    PricePerLiter = "";
                                }
                                break;
                            case "Liters":
                                if (LitersStyle == emptyValueStyle)
                                {
                                    LitersStyle = defaultValueStyle;
                                    Liters = "";
                                }
                                break;
                            default:
                                break;
                        }
                    },
                    param => true
                );
            }
        }

        /// <summary>
        /// Blur command
        /// </summary>
        public ICommand BlurCommand
        {
            get
            {
                return new RelayCommand(
                    (param) =>
                    {
                        String name = param as String;
                        switch (name)
                        {
                            case "FuelPrice":
                                if (FuelPrice == "")
                                {
                                    calculateFuelPrice();
                                }
                                break;
                            case "PricePerLiter":
                                if (PricePerLiter == "")
                                {
                                    calculatePricePerLiter();
                                }
                                break;
                            case "Liters":
                                if (Liters == "")
                                {

                                    calculateLiters();
                                }
                                break;
                            default:
                                break;
                        }
                    },
                    param => true
                );
            }
        }

        #endregion

        #region PRIVATE

        private Style defaultValueStyle = App.Current.Resources["Value"] as Style;
        private Style emptyValueStyle = App.Current.Resources["ValueEmpty"] as Style;

        #endregion

        /// <summary>
        /// Fuel
        /// </summary>
        public Fuel()
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

        /// <summary>
        /// Focus input
        /// </summary>
        /// <param name="where"></param>
        public void EnterValue(string where, string value)
        {
            switch (where)
            {
                case "Odometer":
                    Odometer = value;
                    break;
                case "FuelPrice":
                    FuelPrice = value;
                    break;
                case "PricePerUnit":
                    PricePerLiter = value;
                    break;
                case "Fueled":
                    Liters = value;
                    break;
            }
        }

        /// <summary>
        /// Toggle check box
        /// </summary>
        /// <param name="what"></param>
        public void ToggleCheckBox(string what)
        {
            switch (what)
            {
                case "FullTank":
                    IsTankFull = !IsTankFull;
                    break;
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
            var culture = CultureInfo.InvariantCulture;
            var result = Double.TryParse(value.Replace(",", "."), style, culture, out outValue);
            return result;
        }

        /// <summary>
        /// Calculate price and price per liter
        /// </summary>
        private void calculateLiters()
        {

            //price and price per liter
            if (!Double.IsNaN(pricePerLiter) && pricePerLiter > 0 && !Double.IsNaN(fuelPrice) && fuelPrice > 0)
            {
                if (Double.IsNaN(liters) || LitersStyle == emptyValueStyle)
                {
                    Liters = Math.Round(fuelPrice / pricePerLiter, 2).ToString();
                    LitersStyle = emptyValueStyle;
                }
                else
                {
                    LitersStyle = defaultValueStyle;
                }
            }
        }

        /// <summary>
        /// Calculate price per liter and liters
        /// </summary>
        private void calculateFuelPrice()
        {
            //price per liter and liters
            if (!Double.IsNaN(pricePerLiter) && pricePerLiter > 0 && !Double.IsNaN(liters) && liters > 0)
            {
                if (Double.IsNaN(fuelPrice) || FuelPriceStyle == emptyValueStyle)
                {
                    FuelPrice = Math.Round(pricePerLiter * liters, 2).ToString();
                    FuelPriceStyle = emptyValueStyle;
                }
                else
                {
                    FuelPriceStyle = defaultValueStyle;
                }
            }
        }

        /// <summary>
        /// Calculate price per liter
        /// </summary>
        private void calculatePricePerLiter()
        {
            //fuel price and liters
            if (!Double.IsNaN(fuelPrice) && fuelPrice > 0 && !Double.IsNaN(liters) && liters > 0)
            {
                if (Double.IsNaN(pricePerLiter) || PricePerLiterStyle == emptyValueStyle)
                {
                    PricePerLiter = Math.Round(fuelPrice / liters, 2).ToString();
                    PricePerLiterStyle = emptyValueStyle;
                }
                else
                {
                    PricePerLiterStyle = defaultValueStyle;
                }
            }
        }

        /// <summary>
        /// Save fuel
        /// </summary>
        private void saveFuel()
        {
            CoreData.Fill fill = new CoreData.Fill();
            fill.Odometer = new CoreData.Odometer(this.odometer, DataController.Distance);
            fill.Date = DateTime.Now;
            fill.Full = this.isTankFull;
            fill.Price = new CoreData.Price(this.fuelPrice, DataController.Currency); 
            fill.Refueled = this.liters;
            fill.UnitPrice = new CoreData.Price(this.pricePerLiter, DataController.Currency);
            DataController.AddFill(fill);
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

    }
}