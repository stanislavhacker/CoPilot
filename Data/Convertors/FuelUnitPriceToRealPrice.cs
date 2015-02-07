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
    public class FuelUnitPriceToRealPrice : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Price price = (Price)value;
            Currency currentCurrency = RateExchange.CurrentCurrency;
            Double unitRate = UnitExchange.GetExchangeUnitFor(UnitExchange.CurrentUnit, Unit.Liters); 

            //in same currency
            if (price.Currency == currentCurrency)
            {
                return Math.Round(price.Value * unitRate, 2);
            }

            var rate = RateExchange.GetExchangeRateFor(price.Currency, currentCurrency);
            return Math.Round(rate * price.Value * unitRate, 2);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
