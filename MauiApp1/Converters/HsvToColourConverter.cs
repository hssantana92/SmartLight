using SmartLight.Models;
using System.Globalization;
using Device = SmartLight.Models.Device;

namespace SmartLight.Converters;

public class HsvToColourConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        System.Diagnostics.Debug.WriteLine("HSV To colour Coverter Fired");

        var device = value as Device;

        if (device == null)
        {
            return null;
        }
        // Convert HSV to Color
        Color hsvToColor = Helper.DeviceToColor(device);

        return hsvToColor;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}