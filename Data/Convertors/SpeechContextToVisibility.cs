using CoPilot.CoPilot.Controller;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CoPilot.Data.Convertors
{
    public class SpeechContextToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SpeechContext context = (SpeechContext)value;
            SpeechContext current = (SpeechContext)Enum.Parse(typeof(SpeechContext), (String)parameter);
            return context == current ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
