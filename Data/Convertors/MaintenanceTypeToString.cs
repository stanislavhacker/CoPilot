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
    public class MaintenanceTypeToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MaintenanceType val = (MaintenanceType)value;

            switch (val)
            {
                case MaintenanceType.Filters:
                    return AppResources.MaintenanceType_Filters;
                case MaintenanceType.Oil:
                    return AppResources.MaintenanceType_Oil;
                case MaintenanceType.Maintenance:
                    return AppResources.MaintenanceType_Maintenance;
                case MaintenanceType.Insurance:
                    return AppResources.MaintenanceType_Insurance;
                case MaintenanceType.TechnicalInspection:
                    return AppResources.MaintenanceType_TechnicalInspection;
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
