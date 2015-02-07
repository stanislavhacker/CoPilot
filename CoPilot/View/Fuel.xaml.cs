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
        /// Is tank full
        /// </summary>
        public Boolean IsValidRecord
        {
            get
            {
                var isValid = true;
                if (DataController != null)
                {
                    //previous
                    var previous = DataController.Fills.Where(e => e.Date >= this.Date).ToArray();
                    if (previous.Length > 0)
                    {
                        var odometerPrevious = DistanceExchange.GetOdometerWithRightDistance(previous.Last().Odometer);
                        isValid = odometerPrevious > this.odometer;
                    }
                    //next
                    var next = DataController.Fills.Where(e => e.Date <= this.Date).ToArray();
                    if (next.Length > 0)
                    {
                        var odometerNext = DistanceExchange.GetOdometerWithRightDistance(next.First().Odometer);
                        isValid = isValid && odometerNext < this.odometer;
                    }
                }
                return isValid;
            }
        }

        /// <summary>
        /// Trip distance kilometres
        /// </summary>
        public Double TripDistanceKilometres
        {
            get
            {
                if (DataController != null && !Double.IsNaN(odometer))
                {
                    if (!Double.IsNaN(this.PreviousOdometer))
                    {
                        var odometerLast = this.PreviousOdometer;
                        if (odometer - odometerLast > 0)
                        {
                            return Math.Round(odometer - odometerLast, 1);
                        }
                    }
                    else
                    {
                        return odometer;
                    }
                }
                return 0;
            }
        }

        /// <summary>
        /// Previous odometer
        /// </summary>
        public Double PreviousOdometer
        {
            get
            {
                if (DataController != null)
                {
                    var lastFill = DataController.Fills.Where(e => e.Date <= this.Date).FirstOrDefault();
                    if (lastFill != null)
                    {
                        return DistanceExchange.GetOdometerWithRightDistance(lastFill.Odometer);
                    }
                }
                return Double.NaN;
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
                OnPropertyChanged("IsSaveEnable");
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
                    calculatePricePerUnit();
                    calculateUnits();
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
        /// Price per unit
        /// </summary>
        private Double pricePerUnit = Double.NaN;
        public String PricePerUnit
        {
            get
            {
                if (Double.IsNaN(pricePerUnit))
                {
                    return "";
                }
                return pricePerUnit.ToString();
            }
            set
            {
                if (this.TryParseDouble(value, out pricePerUnit))
                {
                    calculateFuelPrice();
                    calculateUnits();
                }
                else
                {
                    pricePerUnit = Double.NaN;
                }
                RaisePropertyChanged();
                OnPropertyChanged("IsSaveEnable");
            }
        }

        /// <summary>
        /// Units
        /// </summary>
        private Double units = Double.NaN;
        public String Units
        {
            get
            {
                if (Double.IsNaN(units))
                {
                    return "";
                }
                return units.ToString();
            }
            set
            {
                if (this.TryParseDouble(value, out units))
                {
                    calculatePricePerUnit();
                    calculateFuelPrice();
                }
                else
                {
                    units = Double.NaN;
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
        /// Date
        /// </summary>
        private DateTime date = DateTime.MinValue;
        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                if (value >= DateTime.Now)
                {
                    date = DateTime.Now;
                    this.IsChangeDate = false;
                }
                else
                {
                    date = value;
                }
                RaisePropertyChanged();
                RaisePropertyChanged("IsValidRecord");
                RaisePropertyChanged("IsSaveEnable");
                RaisePropertyChanged("TripDistanceKilometres");
                RaisePropertyChanged("PreviousOdometer");
            }
        }

        /// <summary>
        /// Is save enable
        /// </summary>
        public Boolean IsSaveEnable
        {
            get
            {
                return this.IsValidRecord &&
                    !Double.IsNaN(odometer) && 
                    !Double.IsNaN(fuelPrice) && 
                    !Double.IsNaN(pricePerUnit) && 
                    !Double.IsNaN(units) &&
                    (TripDistanceKilometres > 0 || DataController.Fills.Count == 0);
            }
        }

        /// <summary>
        /// Is save enable
        /// </summary>
        private Boolean isChangeDate = false;
        public Boolean IsChangeDate
        {
            get
            {
                return isChangeDate;
            }
            set
            {
                isChangeDate = value;
                RaisePropertyChanged();
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
        /// Price per unit style
        /// </summary>
        private Style pricePerUnitStyle = App.Current.Resources["Value"] as Style;
        public Style PricePerUnitStyle
        {
            get
            {
                return pricePerUnitStyle;
            }
            set
            {
                pricePerUnitStyle = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Units style
        /// </summary>
        private Style unitsStyle = App.Current.Resources["Value"] as Style;
        public Style UnitsStyle
        {
            get
            {
                return unitsStyle;
            }
            set
            {
                unitsStyle = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region COMMANDS

        /// <summary>
        /// ChangeDate command
        /// </summary>
        public ICommand ChangeDateCommand
        {
            get
            {
                return new RelayCommand(
                    param =>
                    {
                        this.IsChangeDate = true;
                    },
                    param => true
                );
            }
        }

        /// <summary>
        /// ChangeDateCancel command
        /// </summary>
        public ICommand ChangeDateCancelCommand
        {
            get
            {
                return new RelayCommand(
                    param =>
                    {
                        this.IsChangeDate = false;
                        this.Date = DateTime.Now;
                    },
                    param => true
                );
            }
        }

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
                            case "PricePerUnit":
                                if (PricePerUnitStyle == emptyValueStyle)
                                {
                                    PricePerUnitStyle = defaultValueStyle;
                                    PricePerUnit = "";
                                }
                                break;
                            case "Units":
                                if (UnitsStyle == emptyValueStyle)
                                {
                                    UnitsStyle = defaultValueStyle;
                                    Units = "";
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
                            case "PricePerUnit":
                                if (PricePerUnit == "")
                                {
                                    calculatePricePerUnit();
                                }
                                break;
                            case "Units":
                                if (Units == "")
                                {

                                    calculateUnits();
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
                    PricePerUnit = value;
                    break;
                case "Fueled":
                    Units = value;
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
        private void calculateUnits()
        {

            //price and price per liter
            if (!Double.IsNaN(pricePerUnit) && pricePerUnit > 0 && !Double.IsNaN(fuelPrice) && fuelPrice > 0)
            {
                if (Double.IsNaN(units) || UnitsStyle == emptyValueStyle)
                {
                    Units = Math.Round(fuelPrice / pricePerUnit, 2).ToString();
                    UnitsStyle = emptyValueStyle;
                }
                else
                {
                    UnitsStyle = defaultValueStyle;
                }
            }
        }

        /// <summary>
        /// Calculate price per unit and units
        /// </summary>
        private void calculateFuelPrice()
        {
            //price per unit and units
            if (!Double.IsNaN(pricePerUnit) && pricePerUnit > 0 && !Double.IsNaN(units) && units > 0)
            {
                if (Double.IsNaN(fuelPrice) || FuelPriceStyle == emptyValueStyle)
                {
                    FuelPrice = Math.Round(pricePerUnit * units, 2).ToString();
                    FuelPriceStyle = emptyValueStyle;
                }
                else
                {
                    FuelPriceStyle = defaultValueStyle;
                }
            }
        }

        /// <summary>
        /// Calculate price per unit
        /// </summary>
        private void calculatePricePerUnit()
        {
            //fuel price and units
            if (!Double.IsNaN(fuelPrice) && fuelPrice > 0 && !Double.IsNaN(units) && units > 0)
            {
                if (Double.IsNaN(pricePerUnit) || PricePerUnitStyle == emptyValueStyle)
                {
                    PricePerUnit = Math.Round(fuelPrice / units, 2).ToString();
                    PricePerUnitStyle = emptyValueStyle;
                }
                else
                {
                    PricePerUnitStyle = defaultValueStyle;
                }
            }
        }

        /// <summary>
        /// Save fuel
        /// </summary>
        private void saveFuel()
        {
            //get exchange
            var rate = UnitExchange.GetExchangeUnitFor(UnitExchange.CurrentUnit, CoreData.Unit.Liters);
            //create fill, all fills are in liters
            CoreData.Fill fill = new CoreData.Fill();
            fill.Odometer = new CoreData.Odometer(this.odometer, DataController.Distance);
            fill.Date = this.Date;
            fill.Full = this.isTankFull;
            fill.Price = new CoreData.Price(this.fuelPrice, DataController.Currency); 
            fill.Refueled = this.units * rate;
            fill.UnitPrice = new CoreData.Price(this.pricePerUnit / rate, DataController.Currency);
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
            //set new current date
            if (e.NavigationMode == NavigationMode.New || e.NavigationMode == NavigationMode.Refresh || e.NavigationMode == NavigationMode.Reset)
            {
                this.Date = DateTime.Now;
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