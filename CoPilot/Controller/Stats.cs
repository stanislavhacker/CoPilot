using CoPilot.CoPilot.View.Popup;
using CoPilot.Core.Utils;
using CoPilot.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;

namespace CoPilot.CoPilot.Controller
{
    public class Stats : Base
    {

        #region COMMAND

        /// <summary>
        /// OpenInterface Command
        /// </summary>
        public ICommand OpenInterfaceCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.aboutInterfaceUrl();
                },
                param => true
               );
            }
        }

        /// <summary>
        /// SpeedWay Command
        /// </summary>
        public ICommand SpeedWayCommand
        {
            get
            {
                return new RelayCommand(async (param) =>
                {
                    await Launcher.LaunchUriAsync(new Uri("copilot-speedway:run"));
                }, param => true);
            }
        }

        /// <summary>
        /// SpeedWay Command
        /// </summary>
        public ICommand SpeedWayBuyCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    MarketplaceDetailTask mdt = new MarketplaceDetailTask();
                    mdt.ContentIdentifier = "9e8721ed-645b-4c0b-a7a4-62faec49e04e";
                    mdt.ContentType = MarketplaceContentType.Applications;
                    mdt.Show();
                }, param => true);
            }
        }


        /// <summary>
        /// AppTap Command
        /// </summary>
        public ICommand AppTap
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.MenuController.Context = MenuContext.App;
                }, param => true);
            }
        }

        /// <summary>
        /// StatsTap Command
        /// </summary>
        public ICommand StatsTap
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.MenuController.Context = MenuContext.Statistics;
                }, param => true);
            }
        }

        /// <summary>
        /// TwitterTap Command
        /// </summary>
        public ICommand TwitterTap
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.MenuController.Context = MenuContext.Browser;
                    this.SocialUrl = new Uri("https://twitter.com/carcopilot");
                }, param => true);
            }
        }

        /// <summary>
        /// FacebookTap Command
        /// </summary>
        public ICommand FacebookTap
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.MenuController.Context = MenuContext.Browser;
                    this.SocialUrl = new Uri("https://www.facebook.com/carcopilot");
                }, param => true);
            }
        }

        /// <summary>
        /// GoogleTap Command
        /// </summary>
        public ICommand GoogleTap
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.MenuController.Context = MenuContext.Browser;
                    this.SocialUrl = new Uri("https://plus.google.com/u/0/115628070739534024707/posts");
                }, param => true);
            }
        }

        /// <summary>
        /// BlogTap Command
        /// </summary>
        public ICommand BlogTap
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.MenuController.Context = MenuContext.Browser;
                    this.SocialUrl = new Uri("http://carcopilot.blogspot.cz/?m=1");
                }, param => true);
            }
        }

        #endregion

        #region PRIVATE

        private Statistics.Statistics baseStats;
        private Statistics.Statistics speedwayStats;

        #endregion

        #region PROPERTY

        /// <summary>
        /// Features utility
        /// </summary>
        private FeaturesUtility featuresUtility = new FeaturesUtility();
        public FeaturesUtility FeaturesUtility
        {
            get
            {
                return featuresUtility;
            }
        }

        /// <summary>
        /// SocialUrl
        /// </summary>
        private Uri socialUrl;
        public Uri SocialUrl
        {
            get
            {
                return socialUrl;
            }
            set
            {
                socialUrl = value;
                RaisePropertyChanged();
            }
        }

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
        /// Is net enabled
        /// </summary>
        private Boolean isNetEnabled = false;
        public Boolean IsNetEnabled
        {
            get
            {
                return isNetEnabled;
            }
            set
            {
                isNetEnabled = value;
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
                return Math.Round(this.baseStats.getStateStats().AvarageSpeed(), 2);
            }
        }

        /// <summary>
        /// Paid for fuel
        /// </summary>
        public Double PaidForFuel
        {
            get
            {
                return Math.Round(this.baseStats.getFuelStats().PaidForFuel(DataController.Currency), 2);
            }
        }

        /// <summary>
        /// Paid for repairs
        /// </summary>
        public Double PaidForRepairs
        {
            get
            {
                return Math.Round(this.baseStats.getRepairStats().PaidForRepairs(DataController.Currency));
            }
        }

        /// <summary>
        /// CountOfLaps
        /// </summary>
        public Int64 CountOfLaps
        {
            get
            {
                var count = this.speedwayStats.getCircuits().Count;
                return count;
            }
        }

        /// <summary>
        /// LengthOfLaps
        /// </summary>
        public Double LengthOfLaps
        {
            get
            {
                var circuits = this.speedwayStats.getCircuits();
                var sum = circuits.Sum((e) => e.getLength(this.DataController.Distance));
                return Math.Round(sum, 2);
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
                var trend = this.baseStats.getFuelStats().TrendFuelPrices(DataController.Currency, DataController.Unit);
                var count = trend.X.Count < 20 ? trend.X.Count : 20;

                for (var i = count - 1; i >= 0; i--)
                {
                    data.Add(new DateTimeModel(trend.X[i], trend.Y[i]));
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
                var trend = this.baseStats.getFuelStats().TrendUnitsPerRefill(DataController.Currency, DataController.Unit);
                var count = trend.X.Count < 20 ? trend.X.Count : 20;

                for (var i = count - 1; i >= 0; i--)
                {
                    data.Add(new DateTimeModel(trend.X[i], trend.Y[i]));
                }

                return data;
            }
        }

        #endregion

        #region PROPERTY MENU

        /// <summary>
        /// Menu controller
        /// </summary>
        private Menu menuController;
        public Menu MenuController
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
        
        /// <summary>
        /// Stats
        /// </summary>
        public Stats(Data dataController, CoPilot.View.CoPilot copilot)
        {
            //data controller
            this.DataController = dataController;
            this.DataController.PropertyChanged += propertyChangedEvent;
            //menu controller
            this.MenuController = new Menu();
            this.MenuController.Context = MenuContext.Statistics;
            //copilot
            this.CoPilot = copilot;
            //stats
            this.baseStats = new Statistics.Statistics(DataController.Records);
            this.speedwayStats = new Statistics.Statistics(DataController.Circuits);
        }

        #region PRIVATE

        /// <summary>
        /// Open interface url
        /// </summary>
        private void aboutInterfaceUrl()
        {
            MessageBox box = MessageBox.Create();
            box.Caption = AppResources.WebInterface_Title;
            box.Message = AppResources.WebInterface_Description;
            box.ShowLeftButton = true;
            box.ShowRightButton = true;
            box.LeftButtonText = AppResources.Understood;
            box.RightButtonText = AppResources.Send;

            box.Dismiss += (sender, e) =>
            {
                switch (e)
                {
                    case MessageBoxResult.LeftButton:
                    case MessageBoxResult.None:
                        break;
                    case MessageBoxResult.RightButton:
                        EmailComposeTask emailComposeTask = new EmailComposeTask();
                        emailComposeTask.Subject = AppResources.WebInterface_Title;
                        emailComposeTask.Body = CoPilot.HttpServerController.Url;
                        emailComposeTask.Show();
                        break;
                    default:
                        break;
                }
            };

            box.IsOpen = true;
        }

        #endregion

        #region CHANGE

        /// <summary>
        /// Property changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void propertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var force = e.PropertyName == "" || e.PropertyName == "Currency" || e.PropertyName == "Distance"  || e.PropertyName == "Unit";

            //create new statsi f its reset
            if (e.PropertyName == "")
            {
                //stats
                this.baseStats = new Statistics.Statistics(DataController.Records);
                this.speedwayStats = new Statistics.Statistics(DataController.Circuits);
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

            if (force || e.PropertyName == "Circuits")
            {
                RaisePropertyChanged("CountOfLaps");
                RaisePropertyChanged("LengthOfLaps");
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
