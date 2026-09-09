using System.Globalization;

namespace SmartHome.Maui.Converters;

/// <summary>Returns "🔊 Звук" when muted, "🔇 Тихо" when not muted.</summary>
public sealed class MuteTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? "🔊 Звук" : "🔇 Тихо";

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
