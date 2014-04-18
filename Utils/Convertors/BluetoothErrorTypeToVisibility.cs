using CoPilot.CoPilot.Controller;
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
    public class BluetoothErrorTypeToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BluetoothErrorType error = (BluetoothErrorType)value;
            BluetoothErrorType current = (BluetoothErrorType)Enum.Parse(typeof(BluetoothErrorType), (String)parameter);
            return error == current ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
