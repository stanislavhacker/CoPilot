using GpsCalculation;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CoPilot.CoPilot.Controller
{
    public class Gps : Base
    {
        private const double MIN_ACURRACY = 100;
        private const double MIN_SPEED_ACCURACY = 20;

        #region PRIVATE

        private GeoCoordinateWatcher gpsSenzor = null;
        private GeoPositionStatus gpsStatus = GeoPositionStatus.Disabled;
        private List<GeoPosition> gpsLastPositions = new List<GeoPosition>();
        private DispatcherTimer speedTimer = null;

        #endregion

        #region EVENTS

        public event EventHandler onChange;

        #endregion

        #region PROPERTY

        /// <summary>
        /// Is gps enabled
        /// </summary>
        private bool isGpsEnabled = false;
        public bool IsGpsEnabled
        {
            get
            {
                return isGpsEnabled;
            }
            set
            {
                isGpsEnabled = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Is gps allowed
        /// </summary>
        private bool isGpsAllowed = false;
        public bool IsGpsAllowed
        {
            get
            {
                return isGpsAllowed;
            }
            set
            {
                isGpsAllowed = value;
                RaisePropertyChanged();

                if (value)
                {
                    this.createGps();
                }
                else
                {
                    this.destroyGps();
                }
            }
        }

        /// <summary>
        /// Is gps initializing
        /// </summary>
        private bool isGpsInitializing = false;
        public bool IsGpsInitializing
        {
            get
            {
                return isGpsInitializing;
            }
            set
            {
                isGpsInitializing = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Is gps data available
        /// </summary>
        private bool isGpsData = false;
        public bool IsGpsData
        {
            get
            {
                return isGpsData;
            }
            set
            {
                isGpsData = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// If map accured?
        /// </summary>
        public bool IsAccured
        {
            get
            {
                return this.IsGpsEnabled && this.gpsSenzor.Position.Location.HorizontalAccuracy <= MIN_ACURRACY;
            }
        }

        /// <summary>
        /// Longitude
        /// </summary>
        public Double Longitude
        {
            get
            {
                return this.Current.Longitude;
            }
        }

        /// <summary>
        /// Latitude
        /// </summary>
        public Double Latitude
        {
            get
            {
                return this.Current.Latitude;
            }
        }

        /// <summary>
        /// Accuracy
        /// </summary>
        public Double Accuracy
        {
            get
            {
                return this.Current.HorizontalAccuracy;
            }
        }

        /// <summary>
        /// Speed
        /// </summary>
        private Double speed = Double.NaN;
        public Double Speed
        {
            get
            {
                return speed;
            }
            set
            {
                if (speed == value)
                {
                    return;
                }
                //set
                speed = value;
                RaisePropertyChanged();
                //change
                if (onChange != null)
                {
                    onChange.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// GeoCoordinate
        /// </summary>
        private GeoCoordinate current = GeoCoordinate.Unknown;
        public GeoCoordinate Current
        {
            get
            {
                return current;
            }
            set
            {
                current = value;
                RaisePropertyChanged();
                OnPropertyChanged("Longitude");
                OnPropertyChanged("Latitude");
                OnPropertyChanged("Accuracy");
            }
        }

        #endregion

        /// <summary>
        /// Create new gps controller
        /// </summary>
        /// <param name="parent"></param>
        public Gps()
        {
        }



        /// <summary>
        /// Create GPS
        /// </summary>
        private void createGps()
        {
            this.IsGpsEnabled = false;
            this.IsGpsInitializing = false;
            this.IsGpsData = false;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += delegate
            {
                timer.Stop();
                if (this.isGpsAllowed)
                {
                    this.gpsSenzor = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                    this.gpsSenzor.StatusChanged += triggerGpsStatusChanged;
                    this.gpsSenzor.PositionChanged += triggerGpsPositionChanged;
                    this.gpsSenzor.MovementThreshold = 1;
                    this.gpsSenzor.TryStart(false, TimeSpan.FromMilliseconds(2000));
                }
            };
            timer.Start();

            //speed timer
            this.initSpeedMeasurement();
        }

        /// <summary>
        /// Delete GPS
        /// </summary>
        private void destroyGps()
        {
            if (this.gpsSenzor != null)
            {
                this.gpsSenzor.Stop();
            }
            this.Current = GeoCoordinate.Unknown;
            this.IsGpsEnabled = true;
            this.IsGpsInitializing = false;
            this.IsGpsData = false;

            //speed timer
            if (this.speedTimer != null) {
                this.speedTimer.Stop();
            }
            this.speedTimer = null;
        }

        /// <summary>
        /// Set data
        /// </summary>
        private void setData()
        {
            if (IsGpsEnabled)
            {
                this.Current = gpsSenzor.Position.Location;
                //change
                if (onChange != null)
                {
                    onChange.Invoke(this, EventArgs.Empty);
                }
            }
        }

        #region TRIGGERS

        /// <summary>
        /// Trigger Gps Position change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void triggerGpsPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            this.setData();
        }

        /// <summary>
        /// Trigger Gps Status change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void triggerGpsStatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    this.IsGpsEnabled = false;
                    this.IsGpsInitializing = false;
                    this.IsGpsData = false;
                    break;
                case GeoPositionStatus.Initializing:
                    this.IsGpsEnabled = true;
                    this.IsGpsInitializing = true;
                    this.IsGpsData = false;
                    break;
                case GeoPositionStatus.NoData:
                    this.IsGpsEnabled = true;
                    this.IsGpsInitializing = true;
                    this.IsGpsData = false;
                    break;
                case GeoPositionStatus.Ready:
                    this.IsGpsEnabled = true;
                    this.IsGpsInitializing = false;
                    this.IsGpsData = true;
                    break;
            }
            this.gpsStatus = e.Status;
            this.setData();
        }

        #endregion

        #region SPEED

        /// <summary>
        /// Init
        /// </summary>
        private void initSpeedMeasurement()
        {
            this.speedTimer = new DispatcherTimer();
            this.speedTimer.Interval = TimeSpan.FromMilliseconds(333);
            this.speedTimer.Tick += delegate
            {
                //create geo
                var geo = new GeoPosition(this.Current.Latitude, this.Current.Longitude, this.Current.HorizontalAccuracy);
                //update history
                this.updatePositionHistory(geo);
                //speed
                this.finalSpeedTo();
            };
            this.speedTimer.Start();
        }

        /// <summary>
        /// Update position history
        /// </summary>
        /// <param name="geoPosition"></param>
        private void updatePositionHistory(GeoPosition geoPosition)
        {
            //add position to array
            this.gpsLastPositions.Add(geoPosition);
            if (this.gpsLastPositions.Count > 9)
            {
                this.gpsLastPositions.RemoveAt(0);
            }
        }

        /// <summary>
        /// Calculate speed
        /// </summary>
        /// <param name="geoCoordinate"></param>
        private void finalSpeedTo()
        {
            //previous and is accured
            if (this.gpsLastPositions.Count > 2 && this.IsAccured)
            {
                //set new speed
                this.Speed = Geo.SpeedTo(this.gpsLastPositions);
            }
        }

        #endregion
    }
}
