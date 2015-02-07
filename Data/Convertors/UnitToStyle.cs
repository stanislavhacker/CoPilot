﻿using CoPilot.Core.Data;
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
    public class UnitToStyle : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Unit val = (Unit)value;
            Unit param = (Unit)Enum.Parse(typeof(Unit), (String)parameter);
            return val == param ? App.Current.Resources["SelectedPriceButton"] as Style : App.Current.Resources["PriceButton"] as Style;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
