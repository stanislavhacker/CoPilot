using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoPilot.CoPilot.Controller
{
    public enum MenuContext
    {
        Statistics,
        Browser,
        Camera,
        Fuel,
        Repair,
        Other,
        MainBackup,
        PhotoBackup,
        VideoBackup
    }

    public class Menu : Base
    {
        #region PROPERTY

        /// <summary>
        /// Is open
        /// </summary>
        private bool isOpen = false;
        public bool IsOpen
        {
            get
            {
                return this.isOpen;
            }
            private set
            {
                this.isOpen = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Context
        /// </summary>
        private MenuContext context = MenuContext.Camera;
        public MenuContext Context
        {
            get
            {
                return this.context;
            }
            set
            {
                this.context = value;
                RaisePropertyChanged();
            }
        }


        #endregion

        /// <summary>
        /// Menu
        /// </summary>
        public Menu()
        {
        }

        /// <summary>
        /// Open menu
        /// </summary>
        public void open() 
        {
            this.IsOpen = true;
        }

        /// <summary>
        /// Close menu
        /// </summary>
        public void close()
        {
            this.IsOpen = false;
        }

    }
}
