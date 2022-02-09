using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AniGate.WpfClient.FrameworkExtensions;

[ValueConversion(typeof(bool), typeof(Visibility))]
public sealed class BooleanToCustomVisibilityConverter : IValueConverter
{
    public BooleanToCustomVisibilityConverter()
    {
        // set defaults
        TrueValue = Visibility.Visible;
        FalseValue = Visibility.Collapsed;
    }

    public Visibility TrueValue { get; set; }

    public Visibility FalseValue { get; set; }

    public object Convert(object value, Type targetType,
                          object parameter, CultureInfo culture)
    {
        if (value is not bool b)
            return FalseValue;

        return b ? TrueValue : FalseValue;
    }

    public object ConvertBack(object value, Type targetType,
                              object parameter, CultureInfo culture)
    {
        return Equals(value, TrueValue);
    }
}