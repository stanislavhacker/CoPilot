using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CoPilot.Utils.Convertors
{
    public class RecordingToImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Boolean val = (Boolean)value;
            BitmapImage image = new BitmapImage();
            if (val)
            {
                image.UriSource = new Uri("/Resources/Images/Buttons/stop.png", UriKind.Relative);
            }
            else
            {
                image.UriSource = new Uri("/Resources/Images/Buttons/record.png", UriKind.Relative);
            }

            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
