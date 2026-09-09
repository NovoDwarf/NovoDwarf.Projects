using System.Globalization;

namespace SmartHome.Maui.Converters;

/// <summary>Returns Green when true, Gray when false.</summary>
public sealed class BoolToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? Colors.ForestGreen : Colors.DarkGray;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
