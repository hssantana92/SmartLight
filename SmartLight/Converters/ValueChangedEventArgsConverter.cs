using System.Globalization;

namespace SmartLight.Converters;

public class ValueChangedEventArgsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {


        var valueChangedEventArgs = value as ValueChangedEventArgs;

        if (valueChangedEventArgs == null)
        {
            throw new ArgumentException("Expected value to be of type valueChangedEventArgs", nameof(value));
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}