namespace Mathematics.App.Maui.Systems.Application.Constants;

public static class AxisLabelers
{
	public static Func<double, string> Time { get; } =
		v => TimeSpan.FromSeconds(v).ToString(@"mm\:ss");

	public static Func<double, string> Value { get; } =
		v => v.ToString("F1");
}
