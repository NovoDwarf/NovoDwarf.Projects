using System.Globalization;

namespace SmartHome.Maui.Converters;

/// <summary>Returns "Выключить" when true, "Включить" when false.</summary>
public sealed class BoolToOnOffConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? "Выключить" : "Включить";

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
