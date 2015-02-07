using CoPilot.CoPilot.Controller;
using CoPilot.Core.Data;
using CoPilot.Core.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CoPilot.Data.Convertors
{
    public class DistanceToConsumptionString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //liters per 1 distance
            Double consumption = (Double)value;
            Double unitRate;
            Consumption current = App.DataController.Consumption;
            String val = "";

            switch (current)
            {
                case Consumption.LitersPer100Distance:
                    unitRate = UnitExchange.GetExchangeUnitFor(App.DataController.Unit, Unit.Liters);
                    val = Math.Round((consumption * 100) / unitRate, 2).ToString();
                    break;
                case Consumption.DistanceOnGallon:
                    unitRate = UnitExchange.GetExchangeUnitFor(Unit.Gallons, Unit.Liters);
                    var gallonsPerDistance = consumption / unitRate;
                    if (gallonsPerDistance > 0)
                    {
                        val = Math.Round(1 / gallonsPerDistance, 2).ToString();
                    }
                    else
                    {
                        val = 0.ToString();
                    }
                    break;
                default:
                    break;
            }

            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
