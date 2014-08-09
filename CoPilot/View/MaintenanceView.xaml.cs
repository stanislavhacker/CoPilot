using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Input;
using Controllers = CoPilot.CoPilot.Controller;
using CoPilot.Core.Utils;
using CoreData = CoPilot.Core.Data;
using System.ComponentModel;
using CoPilot.Data;
using System.Runtime.CompilerServices;

namespace CoPilot.CoPilot.View
{
    public partial class MaintenanceView : PhoneApplicationPage, INotifyPropertyChanged
    {
        #region COMMANDS

        /// <summary>
        /// Next Command
        /// </summary>
        public ICommand NextCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.getNextMaintenance();
                }, param => true);
            }
        }

        /// <summary>
        /// Previous Command
        /// </summary>
        public ICommand PreviousCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.getPreviousMaintenance();
                }, param => true);
            }
        }

        /// <summary>
        /// Delete Command
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.deleteMaintenance();
                    if (this.Max == 0)
                    {
                        NavigationService.GoBack();
                    }
                    else
                    {
                        this.closeMenuIfItsNecessary();
                        this.getPreviousMaintenance();
                    }
                }, param => true);
            }
        }

        /// <summary>
        /// Cancel Command
        /// </summary>
        public ICommand CancelCommand
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
        /// Confirm delete Command
        /// </summary>
        public ICommand ConfirmDeleteCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.openMenuIfItsNecessary();
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
                this.Max = dataController.Maintenances.Count;
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

        /// <summary>
        /// Maintenance
        /// </summary>
        private CoreData.Maintenance maintenance;
        public CoreData.Maintenance Maintenance
        {
            get
            {
                return maintenance;
            }
            set
            {
                maintenance = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Position
        /// </summary>
        private int position = -1;
        public int Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Max
        /// </summary>
        private int max = 0;
        public int Max
        {
            get
            {
                return max;
            }
            set
            {
                max = value;
                OnPropertyChanged("IsButtonsEnabled");
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Enable buttons
        /// </summary>
        public Boolean IsButtonsEnabled
        {
            get
            {
                return dataController != null && dataController.Maintenances.Count > 1;
            }
        }

        #endregion

        /// <summary>
        /// Maintenance view controller
        /// </summary>
        public MaintenanceView()
        {
            InitializeComponent();
            InitializeControllers();

            this.DataContext = this;
        }

        #region REPAIRS

        /// <summary>
        /// Get next maintenance
        /// </summary>
        private void getNextMaintenance()
        {
            var maintenances = dataController.Maintenances;

            Position++;
            if (Position > maintenances.Count)
            {
                Position = 1;
            }
            this.Maintenance = maintenances.ElementAt(Position - 1);
        }

        /// <summary>
        /// Get previous maintenance
        /// </summary>
        private void getPreviousMaintenance()
        {
            var maintenances = dataController.Maintenances;
            Position--;
            if (Position < 1)
            {
                Position = maintenances.Count;
            }
            this.Maintenance = maintenances.ElementAt(Position - 1);
        }

        /// <summary>
        /// Get current maintenances
        /// </summary>
        private void getCurrentMaintenance()
        {
            var maintenances = dataController.Maintenances;
            if (Position <= maintenances.Count && Position > 0)
            {
                this.Maintenance = maintenances.ElementAt(Position - 1);
            }
        }

        /// <summary>
        /// Delete maintenances
        /// </summary>
        private void deleteMaintenance()
        {
            var maintenance = dataController.Maintenances.ElementAt(Position - 1);
            SchedulerController.Remove(maintenance);
            dataController.RemoveMaintenance(maintenance);
            this.Max = dataController.Maintenances.Count;
        }

        #endregion

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



        #region CONTROLLERS

        /// <summary>
        /// Initialize all controllers
        /// </summary>
        private void InitializeControllers()
        {
            //create controllers
            this.createMenuController();
        }

        /// <summary>
        /// Create menu controller
        /// </summary>
        private void createMenuController()
        {
            ///CONTROLLER
            MenuController = new Controllers.Menu();
        }

        #endregion

        #region OPEN MENU

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
                this.SchedulerController = container.SchedulerController;

                this.Maintenance = container.Maintenance;
                this.Position = this.DataController.Maintenances.IndexOf(this.Maintenance) + 1;
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

            if (e.Cancel == false && MenuController.IsOpen)
            {
                MenuController.close();
                e.Cancel = true;
            }
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