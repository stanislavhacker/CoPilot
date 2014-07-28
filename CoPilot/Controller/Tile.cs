using CoPilot.Data.Convertors;
using CoPilot.Resources;
using CoPilot.Utils.Convertors;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoPilot.CoPilot.Controller
{
    public class Tile : Base
    {
        #region STATIC 

        public static Tile Current = null;

        #endregion

        #region PROPERTY

        /// <summary>
        /// Tile
        /// </summary>
        private ShellTile appTile;
        public ShellTile AppTile
        {
            get
            {
                return appTile;
            }
            set
            {
                appTile = value;
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

        #endregion

        /// <summary>
        /// Tile controller
        /// </summary>
        public Tile()
        {
            Current = this;

            //get first tile
            this.getFirstTile();
        }

        /// <summary>
        /// Update tile
        /// </summary>
        public void Update()
        {
            if (appTile != null)
            {
                FlipTileData data = new FlipTileData();
                DistanceToConsumptionString convertor = new DistanceToConsumptionString();
                String consumprionString = convertor.Convert(DataController.AverageConsumption, typeof(Double), null, null) as String;
                String consumption = consumprionString + " " + RealConsumptionToString.Convert(DataController.Consumption, DataController.Distance);

                data.BackTitle = "Co-Pilot";
                data.BackContent = String.Format(AppResources.Tile_Small, consumption, DataController.Repairs.Count);
                data.WideBackContent = String.Format(AppResources.Tile, consumption, DataController.Repairs.Count);

                data.BackBackgroundImage = new Uri("/Resources/Images/Tiles/BlankIcon.png", UriKind.Relative);
                data.WideBackBackgroundImage = new Uri("/Resources/Images/Tiles/BlankIcon.png", UriKind.Relative);

                appTile.Update(data);
            }
        }

        #region PRIVATE

        /// <summary>
        /// Resolve tile
        /// </summary>
        private void getFirstTile()
        {
            AppTile = ShellTile.ActiveTiles.FirstOrDefault();
        }

        #endregion

    }
}