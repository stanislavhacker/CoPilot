using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CoPilot.CoPilot.Controller;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PR = System.Windows.Controls.Primitives;
using System.Windows.Input;
using CoPilot.Core.Utils;

namespace CoPilot.CoPilot.View.Popup
{
    public enum MessageBoxResult
    {
        LeftButton,
        RightButton,
        None
    }

    public partial class MessageBox : UserControl, INotifyPropertyChanged
    {
        #region STATIC

        private static MessageBox current;

        /// <summary>
        /// Create box
        /// </summary>
        /// <returns></returns>
        public static MessageBox Create() 
        {
            if (current != null) {
                current.IsOpen = false;
            }
            current = new MessageBox();
            return current;
        }

        /// <summary>
        /// Hide box
        /// </summary>
        /// <returns></returns>
        public static Boolean Hide()
        {
            if (current != null)
            {
                current.IsOpen = false;
                current = null;
                return true;
            }
            return false;
        }

        #endregion

        #region EVENT

        public event EventHandler<MessageBoxResult> Dismiss;

        #endregion

        #region COMMAND

        /// <summary>
        /// LeftComand Command
        /// </summary>
        public ICommand LeftComand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.dismiss(MessageBoxResult.LeftButton);
                },
                param => true
               );
            }
        }

        /// <summary>
        /// RightComand Command
        /// </summary>
        public ICommand RightComand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.dismiss(MessageBoxResult.RightButton);
                },
                param => true
               );
            }
        }

        #endregion

        #region PROPERTY

        /// <summary>
        /// Caption
        /// </summary>
        private String caption = "";
        public String Caption 
        {
            get
            {
                return caption;
            }
            set
            {
                caption = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Message
        /// </summary>
        private String message = "";
        public String Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// LeftButtonText
        /// </summary>
        private String leftButtonText = "";
        public String LeftButtonText
        {
            get
            {
                return leftButtonText;
            }
            set
            {
                leftButtonText = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// RightButtonText
        /// </summary>
        private String rightButtonText = "";
        public String RightButtonText
        {
            get
            {
                return rightButtonText;
            }
            set
            {
                rightButtonText = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// ShowLeftButton
        /// </summary>
        private Boolean showLeftButton = false;
        public Boolean ShowLeftButton
        {
            get
            {
                return showLeftButton;
            }
            set
            {
                showLeftButton = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// ShowRightButton
        /// </summary>
        private Boolean showRightButton = false;
        public Boolean ShowRightButton
        {
            get
            {
                return showRightButton;
            }
            set
            {
                showRightButton = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// IsOpen
        /// </summary>
        private Boolean isOpen = false;
        public Boolean IsOpen
        {
            get
            {
                return isOpen;
            }
            set
            {
                if (value == false)
                {
                    this.dismiss(MessageBoxResult.None);
                }
                else
                {
                    this.parent.Children.Add(this);
                    isOpen = true;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region PRIVATE

        private Grid parent;

        #endregion


        /// <summary>
        /// Message box
        /// </summary>
        private MessageBox()
        {
            InitializeComponent();

            //parent
            this.parent = (App.RootFrame.Content as PhoneApplicationPage).Content as Grid;

            //context
            this.DataContext = this;
        }

        /// <summary>
        /// Dismiss
        /// </summary>
        /// <param name="messageBoxResult"></param>
        private void dismiss(MessageBoxResult messageBoxResult)
        {
            if (Dismiss != null)
            {
                Dismiss.Invoke(this, messageBoxResult);
            }
            MessageBox.current = null;
            isOpen = false;
            this.parent.Children.Remove(this);
            RaisePropertyChanged("IsOpen");
        }

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
