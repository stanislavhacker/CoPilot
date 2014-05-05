using CoPilot.Core.Data;
using CoPilot.Core.Utils;
using CoPilot.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CoPilot.CoPilot.Controller
{
    public enum RemoveType 
    {
        SoftDelete,
        HardDelete
    }

    public class Data : Base
    {
        public const string DATA_FILE_NAME = "co-pilot.xml";
        public const string DATA_FILE = "/shared/transfers/" + DATA_FILE_NAME;

        //PRIVATE
        private Boolean changed = true;
        private Records data;
        private DispatcherTimer timer;
        private DateTime saved = DateTime.Now;

        #region EVENTS

        public event EventHandler onUnitsChange;

        #endregion

        #region GET

        /// <summary>
        /// Average consumption
        /// </summary>
        public Double AverageConsumption
        {
            get
            {
                return this.getAverageConsumption();
            }
        }

        /// <summary>
        /// Is backup available
        /// </summary>
        public bool IsBackupAvailable
        {
            get
            {
                return data.Backup != null && !String.IsNullOrEmpty(data.Backup.Id);
            }
        }

        #endregion

        #region State

        /// <summary>
        /// Position
        /// </summary>
        private GeoCoordinate position = GeoCoordinate.Unknown;
        public GeoCoordinate Position
        {
            get
            {
                return position;
            }
            set
            {
                if (position != null && value != null)
                {
                    if (position.Latitude == value.Latitude &&
                        position.Longitude == value.Longitude &&
                        position.Altitude == value.Altitude &&
                        position.HorizontalAccuracy == value.HorizontalAccuracy)
                    {
                        return;
                    }
                }
                position = value == null ? GeoCoordinate.Unknown : value;
                RaisePropertyChanged();
                triggerChange();
            }
        }

        /// <summary>
        /// Rpm
        /// </summary>
        private Double rpm = 0;
        public Double Rpm
        {
            get
            {
                return rpm;
            }
            set
            {
                if (rpm == value)
                {
                    return;
                }
                rpm = value;
                RaisePropertyChanged();
                triggerChange();
            }
        }

        /// <summary>
        /// Speed
        /// </summary>
        private Double speed = 0;
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
                speed = value;
                RaisePropertyChanged();
                triggerChange();
            }
        }

        /// <summary>
        /// Temperature
        /// </summary>
        private Double temperature = 0;
        public Double Temperature
        {
            get
            {
                return temperature;
            }
            set
            {
                if (temperature == value)
                {
                    return;
                }
                temperature = value;
                RaisePropertyChanged();
                triggerChange();
            }
        }

        /// <summary>
        /// Engine load
        /// </summary>
        private Double engineLoad = 0;
        public Double EngineLoad
        {
            get
            {
                return engineLoad;
            }
            set
            {
                if (engineLoad == value)
                {
                    return;
                }
                engineLoad = value;
                RaisePropertyChanged();
                triggerChange();
            }
        }

        /// <summary>
        /// Max air flow rate
        /// </summary>
        private Double maxAirFlowRate = 0;
        public Double MaxAirFlowRate
        {
            get
            {
                return maxAirFlowRate;
            }
            set
            {
                if (maxAirFlowRate == value)
                {
                    return;
                }
                maxAirFlowRate = value;
                RaisePropertyChanged();
                triggerChange();
            }
        }

        /// <summary>
        /// Throttle position
        /// </summary>
        private Double throttlePosition = 0;
        public Double ThrottlePosition
        {
            get
            {
                return throttlePosition;
            }
            set
            {
                if (throttlePosition == value)
                {
                    return;
                }
                throttlePosition = value;
                RaisePropertyChanged();
                triggerChange();
            }
        }

        /// <summary>
        /// Uptime
        /// </summary>
        private Double uptime = 0;
        public Double Uptime
        {
            get
            {
                return uptime;
            }
            set
            {
                if (uptime == value)
                {
                    return;
                }
                uptime = value;
                RaisePropertyChanged();
                triggerChange();
            }
        }

        /// <summary>
        /// Accelerator pedal position
        /// </summary>
        private Double acceleratorPedalPosition = 0;
        public Double AcceleratorPedalPosition
        {
            get
            {
                return acceleratorPedalPosition;
            }
            set
            {
                if (acceleratorPedalPosition == value)
                {
                    return;
                }
                acceleratorPedalPosition = value;
                RaisePropertyChanged();
                triggerChange();
            }
        }

        /// <summary>
        /// Engine oil temperature
        /// </summary>
        private Double engineOilTemperature = 0;
        public Double EngineOilTemperature
        {
            get
            {
                return engineOilTemperature;
            }
            set
            {
                if (engineOilTemperature == value)
                {
                    return;
                }
                engineOilTemperature = value;
                RaisePropertyChanged();
                triggerChange();
            }
        }

        /// <summary>
        /// Fuel injection timing
        /// </summary>
        private Double fuelInjectionTiming = 0;
        public Double FuelInjectionTiming
        {
            get
            {
                return fuelInjectionTiming;
            }
            set
            {
                if (fuelInjectionTiming == value)
                {
                    return;
                }
                fuelInjectionTiming = value;
                RaisePropertyChanged();
                triggerChange();
            }
        }

        /// <summary>
        /// Engine reference torgue
        /// </summary>
        private Double engineReferenceTorque = 0;
        public Double EngineReferenceTorque
        {
            get
            {
                return engineReferenceTorque;
            }
            set
            {
                if (engineReferenceTorque == value)
                {
                    return;
                }
                engineReferenceTorque = value;
                RaisePropertyChanged();
                triggerChange();
            }
        }


        #endregion

        #region Setting

        /// <summary>
        /// Is gps Allowed
        /// </summary>
        public bool IsGpsAllowed
        {
            get
            {
                return data.GpsAllowed;
            }
            set
            {
                data.GpsAllowed = value;
                RaisePropertyChanged();
                this.Save();
            }
        }

        /// <summary>
        /// Is obd allowed
        /// </summary>
        public bool IsObdAllowed
        {
            get
            {
                return data.ObdAllowed;
            }
            set
            {
                data.ObdAllowed = value;
                RaisePropertyChanged();
                this.Save();
            }
        }

        /// <summary>
        /// Is drive mode allowed
        /// </summary>
        public bool IsDriveModeAllowed
        {
            get
            {
                return data.DriveModeAllowed;
            }
            set
            {
                data.DriveModeAllowed = value;
                RaisePropertyChanged();
                this.Save();
            }
        }

        /// <summary>
        /// Is backup on start allowed
        /// </summary>
        public bool IsBackupOnStart
        {
            get
            {
                return data.BackupOnStart;
            }
            set
            {
                data.BackupOnStart = value;
                RaisePropertyChanged();
                this.Save();
            }
        }

        /// <summary>
        /// Currency
        /// </summary>
        public Currency Currency
        {
            get
            {
                return data.Currency;
            }
            set
            {
                data.Currency = value;

                if (onUnitsChange != null)
                {
                    onUnitsChange.Invoke(this, EventArgs.Empty);
                }

                RaisePropertyChanged();
                refresh();
                this.Save();
            }
        }

        /// <summary>
        /// Distance
        /// </summary>
        public Distance Distance
        {
            get
            {
                return data.Distance;
            }
            set
            {
                data.Distance = value;

                if (onUnitsChange != null)
                {
                    onUnitsChange.Invoke(this, EventArgs.Empty);
                }

                RaisePropertyChanged();
                refresh();
                this.Save();
            }
        }

        /// <summary>
        /// Consumption
        /// </summary>
        public Consumption Consumption
        {
            get
            {
                return data.Consumption;
            }
            set
            {
                data.Consumption = value;
                RaisePropertyChanged();
                OnPropertyChanged("AverageConsumption");
                this.Save();
            }
        }

        /// <summary>
        /// Obd device
        /// </summary>
        public String ObdDevice
        {
            get
            {
                return data.ObdDevice;
            }
            set
            {
                if (data.ObdDevice == value)
                {
                    return;
                }
                data.ObdDevice = value;
                RaisePropertyChanged();
                this.Save();
            }
        }

        /// <summary>
        /// Id
        /// </summary>
        public String Id
        {
            get
            {
                return data.Id;
            }
        }

        /// <summary>
        /// Backup
        /// </summary>
        public BackupInfo Backup
        {
            get
            {
                return data.Backup;
            }
            set
            {
                if (data.Backup == value)
                {
                    return;
                }
                data.Backup = value;
                RaisePropertyChanged();
                RaisePropertyChanged("IsBackupAvailable");
                this.Save();
            }
        }


        #endregion

        #region Media

        /// <summary>
        /// Videos count
        /// </summary>
        public Double VideosCount
        {
            get
            {
                return data.Videos.Count;
            }
        }

        /// <summary>
        /// Pistures count
        /// </summary>
        public Double PicturesCount
        {
            get
            {
                return data.Pictures.Count;
            }
        }

        /// <summary>
        /// Videos
        /// </summary>
        public ObservableCollection<Video> Videos
        {
            get
            {
                return data.Videos;
            }
        }

        /// <summary>
        /// Pictures
        /// </summary>
        public ObservableCollection<Picture> Pictures
        {
            get
            {
                return data.Pictures;
            }
        }


        /// <summary>
        /// Fills
        /// </summary>
        public ObservableCollection<Fill> Fills
        {
            get
            {
                return data.Fills;
            }
        }

        /// <summary>
        /// Repairs
        /// </summary>
        public ObservableCollection<Repair> Repairs
        {
            get
            {
                return data.Repairs;
            }
        }

        #endregion

        #region Spaces

        /// <summary>
        /// AvailableSpace
        /// </summary>
        public String AvailableSpace
        {
            get
            {
                return Storage.GetAvailableSpace();
            }
        }


        /// <summary>
        /// Size
        /// </summary>
        public Double Size
        {
            get
            {
                return Math.Round(data.Size / 1048576, 3);
            }
        }


        #endregion

        /// <summary>
        /// Data collector
        /// </summary>
        public Data()
        {
            data = Records.Load(DATA_FILE, Storage.Get());
        }

        /// <summary>
        /// Start timer
        /// </summary>
        public void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += delegate
            {
                if (changed)
                {
                    createRecord();
                    changed = false;
                }
            };
            timer.Start();
        }

        /// <summary>
        /// Save
        /// </summary>
        public void Save(Boolean force = false)
        {
            var seconds = DateTime.Now.Subtract(saved).TotalSeconds;
            if (seconds > 15 || force)
            {
                saved = DateTime.Now;
                data.Change = DateTime.Now;
                data.Save(DATA_FILE, Storage.Get());
                OnPropertyChanged("AvailableSpace");
                OnPropertyChanged("Size");
            }
        }

        /// <summary>
        /// From backup
        /// </summary>
        public void FromBackup()
        {
            this.data = Records.Load(DATA_FILE, Storage.Get());
            //set globals
            RateExchange.CurrentCurrency = this.Currency;
            DistanceExchange.CurrentDistance = this.Distance;
            //refresh
            RaisePropertiesChanged();
            //Start timer again
            StartTimer();
        }

        /// <summary>
        /// Stop timer
        /// </summary>
        public void Stop()
        {
            if (this.timer != null && this.timer.IsEnabled)
            {
                this.timer.Stop();
            }
        }

        #region PRIVATES

        /// <summary>
        /// Create record
        /// </summary>
        private void createRecord()
        {
            State state = new State();
            state.Time = DateTime.Now;
            state.Position = Position.ToString();
            state.Rpm = Rpm;
            state.Speed = Speed;
            state.Temperature = Temperature;
            state.EngineLoad = EngineLoad;
            state.MaxAirFlowRate = MaxAirFlowRate;
            state.ThrottlePosition = ThrottlePosition;
            state.AcceleratorPedalPosition = AcceleratorPedalPosition;
            state.EngineOilTemperature = EngineOilTemperature;
            state.FuelInjectionTiming = FuelInjectionTiming;
            state.EngineReferenceTorque = EngineReferenceTorque;
            state.Uptime = Uptime;

            data.States.Add(state);
            this.Save();
        }

        /// <summary>
        /// Trigger change
        /// </summary>
        private void triggerChange()
        {
            changed = true;
        }

        /// <summary>
        /// Refresh
        /// </summary>
        private void refresh()
        {
            //refresh fills
            foreach(Fill fill in this.Fills) 
            {
                fill.CallPropertyChangedOnAll();
            }

            //refresh repairs
            foreach (Repair repair in this.Repairs)
            {
                repair.CallPropertyChangedOnAll();
            }

            OnPropertyChanged("AverageConsumption");
            OnPropertyChanged("Consumption");
        }

        /// <summary>
        /// Get average consuption
        /// </summary>
        /// <returns></returns>
        private Double getAverageConsumption()
        {
            var distanceSum = 0.0;
            var fuelSum = 0.0;

            var distanceSumFull = 0.0;
            var fuelSumFull = 0.0;

            if (this.Fills.Count < 2)
            {
                return 0;
            }

            for (var i = this.Fills.Count - 1; i > 0; i--)
            {
                var current = this.Fills[i];
                var next = this.Fills[i - 1];

                var distance = DistanceExchange.GetOdometerWithRightDistance(next.Odometer) - DistanceExchange.GetOdometerWithRightDistance(current.Odometer);
                var l = current.Refueled;

                if (current.Full)
                {
                    distanceSumFull += distance;
                    fuelSumFull += l;
                }
                else
                {
                    distanceSum += distance;
                    fuelSum += l;
                }
            }

            return Math.Round((fuelSum + fuelSumFull) / (distanceSum + distanceSumFull), 4);
        }

        #endregion

        #region VIDEOS

        /// <summary>
        /// Add video
        /// </summary>
        /// <param name="video"></param>
        public void AddVideo(Video video)
        {
            data.Videos.Add(video);
            this.Save();
            OnPropertyChanged("VideosCount");
            OnPropertyChanged("Videos");
            OnPropertyChanged("AvailableSpace");
        }

        /// <summary>
        /// Remvoe video
        /// </summary>
        /// <param name="video"></param>
        public void RemoveVideo(Video video, RemoveType type)
        {
            //delete files
            Storage.DeleteFile(video.Path);
            Storage.DeleteFile(video.Preview);

            if (type == RemoveType.HardDelete)
            {
                //delete record
                data.Videos.Remove(video);
                this.Save();
                OnPropertyChanged("VideosCount");
                OnPropertyChanged("Videos");
            }

            OnPropertyChanged("AvailableSpace");
        }

        #endregion

        #region PICTURES

        /// <summary>
        /// Add picture
        /// </summary>
        /// <param name="picture"></param>
        public void AddPicture(Picture picture)
        {
            data.Pictures.Add(picture);
            this.Save();
            OnPropertyChanged("PicturesCount");
            OnPropertyChanged("Pictures");
            OnPropertyChanged("AvailableSpace");
        }

        /// <summary>
        /// Remvoe picture
        /// </summary>
        /// <param name="picture"></param>
        public void RemovePicture(Picture picture, RemoveType type)
        {
            //delete files
            Storage.DeleteFile(picture.Path);

            if (type == RemoveType.HardDelete)
            {
                //delete record
                data.Pictures.Remove(picture);
                this.Save();
                OnPropertyChanged("PicturesCount");
                OnPropertyChanged("Pictures");
            }

            OnPropertyChanged("AvailableSpace");
        }

        #endregion

        #region FILLS

        /// <summary>
        /// Add fill
        /// </summary>
        /// <param name="fill"></param>
        public void AddFill(Fill fill)
        {
            data.Fills.Insert(0, fill);
            this.Save();
            OnPropertyChanged("Fills");
            OnPropertyChanged("AverageConsumption");
            OnPropertyChanged("AvailableSpace");
        }

        /// <summary>
        /// Remove fill
        /// </summary>
        /// <param name="fill"></param>
        public void RemoveFill(Fill fill)
        {
            data.Fills.Remove(fill);
            this.Save();
            OnPropertyChanged("Fills");
            OnPropertyChanged("AverageConsumption");
            OnPropertyChanged("AvailableSpace");
        }

        #endregion

        #region REPAIRS

        /// <summary>
        /// Add repair
        /// </summary>
        /// <param name="repair"></param>
        public void AddRepair(Repair repair)
        {
            data.Repairs.Insert(0, repair);
            this.Save();
            OnPropertyChanged("Repairs");
            OnPropertyChanged("AvailableSpace");
        }

        /// <summary>
        /// Remove repair
        /// </summary>
        /// <param name="repair"></param>
        public void RemoveRepair(Repair repair)
        {
            data.Repairs.Remove(repair);
            this.Save();
            OnPropertyChanged("Repairs");
            OnPropertyChanged("AvailableSpace");
        }

        #endregion
    }
}
