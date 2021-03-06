﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using Controllers = CoPilot.CoPilot.Controller;
using CoPilot.Data;
using System.Runtime.CompilerServices;

namespace CoPilot.CoPilot.View
{
    public partial class Graph : PhoneApplicationPage, INotifyPropertyChanged
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

        /// <summary>
        /// isTrendFuelPrices controller
        /// </summary>
        private Boolean isTrendFuelPrices;
        public Boolean IsTrendFuelPrices
        {
            get
            {
                return isTrendFuelPrices;
            }
            set
            {
                isTrendFuelPrices = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// isTrendUnitsPerRefill controller
        /// </summary>
        private Boolean isTrendUnitsPerRefill;
        public Boolean IsTrendUnitsPerRefill
        {
            get
            {
                return isTrendUnitsPerRefill;
            }
            set
            {
                isTrendUnitsPerRefill = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        /// <summary>
        /// Graph
        /// </summary>
        public Graph()
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
                this.CameraController = container.CameraController;
                this.DataController = container.DataController;
                this.DriveModeController = container.DriveModeController;
                this.StatsController = container.StatsController;

                this.IsTrendFuelPrices = container.GrapType == "TrendFuelPrices";
                this.IsTrendUnitsPerRefill = container.GrapType == "TrendUnitsPerRefill";
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
                DataController.Save(true);
                StopRecordingNow();
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