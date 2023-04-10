using System;
using System.Globalization;
using ConvertT = System.Convert;

namespace SmartLight.Converters;

public class BrightnessValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

        if (value is null)
        {
            return 0;
        }


        var intVal = ConvertT.ToInt32(value);

        
        int uIntBrightness = ConvertT.ToInt32((double)intVal / (double)655.35 ) ;
        string brightness = uIntBrightness.ToString() + "%";


        return brightness;
        //return uIntBrightness;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}