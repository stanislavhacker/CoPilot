using CoPilot.Interfaces;
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
    public class PreferencesToText : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ProgressPreferences pref = (ProgressPreferences)value;

            switch (pref)
            {
                case ProgressPreferences.AllowOnCelluralAndBatery:
                    return AppResources.AllowOnCelluralAndBatery;
                case ProgressPreferences.AllowOnWifiAndBatery:
                    return AppResources.AllowOnWifiAndBatery;
                case ProgressPreferences.AllowOnWifiAndExternalPower:
                    return AppResources.AllowOnWifiAndExternalPower;
                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
