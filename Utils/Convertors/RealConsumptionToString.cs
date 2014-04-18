using CoPilot.CoPilot.Controller;
using CoPilot.Core.Data;
using CoPilot.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CoPilot.Utils.Convertors
{
    public class RealConsumptionToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Consumption val = (Consumption)value;
            Distance distance = App.DataController.Distance;
            String info = Convert(val, distance);
            return info;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        /// <summary>
        /// Convert
        /// </summary>
        /// <param name="val"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static string Convert(Consumption val, Distance distance)
        {
            String info = "";
            switch (val)
            {
                case Consumption.LitersPer100Distance:
                    info = AppResources.FueledUnit + " " + AppResources.Per + " 100 " + distance;
                    break;
                case Consumption.DistanceOnGallon:
                    info = distance + " " + AppResources.Per + " " + AppResources.Gallon;
                    break;
                default:
                    break;
            }
            return info;
        }
    }
}
