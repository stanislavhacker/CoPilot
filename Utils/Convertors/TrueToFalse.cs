using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CoPilot.Utils.Convertors
{
    public class TrueToFalse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Boolean val = (Boolean)value;
            return !val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Boolean val = (Boolean)value;
            return !val;
        }
    }
}
