﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CoPilot.Data.Convertors
{
    public class MaintenanceTypeToStyle : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String val = (String)value;
            String param = (String)parameter;
            return val == param ? App.Current.Resources["SelectedPriceButton"] as Style : App.Current.Resources["PriceButton"] as Style;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}