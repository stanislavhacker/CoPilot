using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoPilot.CoPilot.Controller
{
    public class Stats : Base
    {

        #region PRIVATE

        private Statistics.Statistics stats;

        #endregion

        #region PROPERTY

        /// <summary>
        /// Is stats visible
        /// </summary>
        /// <returns></returns>
        private bool isVisible = true;
        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                isVisible = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Data controller
        /// </summary>
        private Data dataController;
        public Data DataController
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
        /// CoPilot
        /// </summary>
        private View.CoPilot coPilot;
        public View.CoPilot CoPilot {
            get
            {
                return coPilot;
            }
            set
            {
                coPilot = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// AverageSpeed
        /// </summary>
        public Double AverageSpeed
        {
            get
            {
                return Math.Round(this.stats.getStateStats().AvarageSpeed(), 2);
            }
        }

        /// <summary>
        /// Paid for fuel
        /// </summary>
        public Double PaidForFuel
        {
            get
            {
                return this.stats.getFuelStats().PaidForFuel(DataController.Currency);
            }
        }

        /// <summary>
        /// Paid for repairs
        /// </summary>
        public Double PaidForRepairs
        {
            get
            {
                return this.stats.getRepairStats().PaidForRepairs(DataController.Currency);
            }
        }

        /// <summary>
        /// TrendFuelPrices
        /// </summary>
        public ObservableCollection<DateTimeModel> TrendFuelPrices
        {
            get
            {
                var data = new ObservableCollection<DateTimeModel>();
                var trend = this.stats.getFuelStats().TrendFuelPrices(DataController.Currency);
                var count = trend.X.Count < 20 ? trend.X.Count : 20;

                for (var i = count - 1; i >= 0; i--)
                {
                    data.Add(new DateTimeModel(DateTime.Parse(trend.X[i]), trend.Y[i]));
                }

                return data;
            }
        }

        /// <summary>
        /// TrendUnitsPerRefill
        /// </summary>
        public ObservableCollection<DateTimeModel> TrendUnitsPerRefill
        {
            get
            {
                var data = new ObservableCollection<DateTimeModel>();
                var trend = this.stats.getFuelStats().TrendUnitsPerRefill(DataController.Currency);
                var count = trend.X.Count < 20 ? trend.X.Count : 20;

                for (var i = count - 1; i >= 0; i--)
                {
                    data.Add(new DateTimeModel(DateTime.Parse(trend.X[i]), trend.Y[i]));
                }

                return data;
            }
        }

        #endregion
        
        /// <summary>
        /// Stats
        /// </summary>
        public Stats(Data dataController, CoPilot.View.CoPilot copilot)
        {
            //data controller
            this.DataController = dataController;
            this.DataController.PropertyChanged += propertyChangedEvent;
            //copilot
            this.CoPilot = copilot;
            //stats
            this.stats = new Statistics.Statistics(DataController.Records);
        }

        #region CHANGE

        /// <summary>
        /// Property changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void propertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var force = e.PropertyName == "" || e.PropertyName == "Currency" || e.PropertyName == "Distance";

            //create new statsi f its reset
            if (e.PropertyName == "")
            {
                //stats
                this.stats = new Statistics.Statistics(DataController.Records);
            }

            if (force || e.PropertyName == "Fills")
            {
                RaisePropertyChanged("PaidForFuel");
            }

            if (force || e.PropertyName == "Repairs")
            {
                RaisePropertyChanged("PaidForRepairs");
            }

            if (force || e.PropertyName == "Speed")
            {
                RaisePropertyChanged("AverageSpeed");
            }

            if (force || e.PropertyName == "Fills")
            {
                RaisePropertyChanged("TrendFuelPrices");
            }

            if (force || e.PropertyName == "Fills")
            {
                RaisePropertyChanged("TrendUnitsPerRefill");
            }
        }

        #endregion
    }

    public class DateTimeModel
    {
        public DateTime X { get; set; }
        public double Y { get; set; }

        public DateTimeModel(DateTime x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
