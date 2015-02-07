using CoPilot.Core.Data;
using CoPilot.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CoPilot.Data.Convertors
{
    public class UnitToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Unit val = (Unit)value;
            String info = Convert(val);
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
        /// <param name="unit"></param>
        /// <returns></returns>
        public static string Convert(Unit val)
        {
            String info = "";
            switch (val)
            {
                case Unit.Gallons:
                    info = AppResources.Gallon;
                    break;
                case Unit.Liters:
                    info = AppResources.Liter;
                    break;
                default:
                    break;
            }
            return info;
        }
    }
}
