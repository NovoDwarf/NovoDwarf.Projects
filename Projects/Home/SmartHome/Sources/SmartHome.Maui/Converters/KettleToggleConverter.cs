using System.Globalization;

namespace SmartHome.Maui.Converters;

/// <summary>Returns "Выключить" when on, "☕ Вскипятить" when off.</summary>
public sealed class KettleToggleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? "Выключить" : "☕ Вскипятить";

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
