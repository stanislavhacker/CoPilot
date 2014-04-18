using CoPilot.Core.Data;
using CoPilot.Core.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CoPilot.Utils.Convertors
{
    public class OdometerToRealOdometer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Odometer odometer = (Odometer)value;
            Distance currentDistance = DistanceExchange.CurrentDistance;

            //in same distance
            if (odometer.Distance == currentDistance)
            {
                return odometer.Value;
            }
            return DistanceExchange.GetOdometerWithRightDistance(odometer);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
