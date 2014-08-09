using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CoPilot.Utils;
using CoPilot.CoPilot.Controller;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CoPilot.Core.Utils;
using CoPilot.Resources;

namespace CoPilot.CoPilot.View.Tutorial
{
    public partial class Tutorial : UserControl, INotifyPropertyChanged
    {
        #region STATIC

        public static CoPilot CoPilotApp;
        public static Tutorial Current; 

        #endregion

        #region COMMANDS

        /// <summary>
        /// NExt Command
        /// </summary>
        public ICommand NextCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    Step++;
                    if (Step > 15)
                    {
                        IsTutorial = false;
                    }
                }, param => true);
            }
        }

        /// <summary>
        /// NExt Command
        /// </summary>
        public ICommand PreviousCommand

        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (Step > 1)
                    {
                        Step--;
                    }
                }, param => true);
            }
        }

        #endregion

        #region PROPERTY

        /// <summary>
        /// Is tutorial
        /// </summary>
        public Boolean IsTutorial
        {
            get
            {
                var isIt = Settings.Get("IsTutorial");
                return isIt == null ? true : Boolean.Parse(Settings.Get("IsTutorial"));
            }
            set
            {
                if (value)
                {
                    Step = 1;
                }
                Settings.Add("IsTutorial", value.ToString());
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Step
        /// </summary>
        private int step = 1;
        public int Step
        {
            get
            {
                return step;
            }
            set
            {
                step = value;
                RaisePropertyChanged();
            }
        }

        #endregion


        /// <summary>
        /// Tutorial
        /// </summary>
        public Tutorial()
        {
            InitializeComponent();
            Current = this;
            this.DataContext = this;
        }

        /// <summary>
        /// Close
        /// </summary>
        public void Close()
        {
            var close = MessageBox.Show(AppResources.Tutorial_End_Description, AppResources.Tutorial_End, MessageBoxButton.OKCancel);
            if (close == MessageBoxResult.OK)
            {
                this.IsTutorial = false;
            }
        }

        #region NOTIFY

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

        /// <summary>
        /// Call propertis change on all
        /// </summary>
        public void RaisePropertiesChanged()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
            }
        }

        #endregion
    }
}
