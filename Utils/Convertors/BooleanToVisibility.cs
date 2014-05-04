using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CoPilot.Utils.Convertors
{
    public class BooleanToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Boolean val;
            if (value == null)
            {
                val = false;
            }
            else if (value.GetType() == typeof(Boolean))
            {
                val = (Boolean)value;
            }
            else
            {
                val = true;
            }
            return val ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility vis = (Visibility)value;
            return vis == Visibility.Visible;
        }
    }
}
