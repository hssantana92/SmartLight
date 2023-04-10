using ColorPicker.Maui;
using System.Globalization;

namespace SmartLight.Converters;

public class PickedColorChangedEventArgsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var pickedColorChangedEventArgs = value as PickedColorChangedEventArgs;
        if (pickedColorChangedEventArgs == null)
        {
            throw new ArgumentException("Expected value to be of type pickedColorChangedEventArgs", nameof(value));
        }
        return pickedColorChangedEventArgs;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        System.Diagnostics.Debug.WriteLine("Halted");
        //throw new NotImplementedException();
        return "String";
    }
}