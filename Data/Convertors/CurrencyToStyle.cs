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
    public class CurrencyToStyle : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Currency val = (Currency)value;
            Currency param = (Currency)Enum.Parse(typeof(Currency), (String)parameter);
            return val == param ? App.Current.Resources["SelectedPriceButton"] as Style : App.Current.Resources["PriceButton"] as Style;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
