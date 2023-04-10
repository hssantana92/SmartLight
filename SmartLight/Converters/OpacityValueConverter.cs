using System.Diagnostics;
using System.Globalization;
using System.Drawing;

namespace SmartLight.Converters;

public class OpacityValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

        Debug.WriteLine("Opacity Value Converter Fired");

        double opaque = 1.0;
        double transparent = 0.3;


        var hasColour = (bool)value;

        if (hasColour)
        {
            return opaque;
        }
        else
        {
            return transparent;
        }

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        //System.Diagnostics.Debug.WriteLine("Halted");
        throw new NotImplementedException();
        //return "String";
    }
}