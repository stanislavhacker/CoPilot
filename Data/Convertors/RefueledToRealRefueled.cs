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
    public class RefueledToRealRefueled : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Double refueled = (Double)value;
            Double unitRate = UnitExchange.GetExchangeUnitFor(UnitExchange.CurrentUnit, Unit.Liters);
            return Math.Round(refueled / unitRate, 2);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
