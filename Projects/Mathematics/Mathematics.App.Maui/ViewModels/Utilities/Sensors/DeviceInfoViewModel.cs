using System.Globalization;
using Mathematics.App.Maui.UI.Base;

namespace Mathematics.App.Maui.UI.ViewModels.Utilities;

public class DeviceInfoViewModel : BaseViewModel
{
	public static string Model => DeviceInfo.Model;
	public static string Manufacturer => DeviceInfo.Manufacturer;
	public static string Platform => DeviceInfo.Platform.ToString();
	public static string OsVersion => DeviceInfo.VersionString;
	public static string DeviceType => DeviceInfo.DeviceType.ToString();
	public static string DeviceIdiom => DeviceInfo.Idiom.ToString();

	public static string AppName => AppInfo.Name;
	public static string AppVersion => AppInfo.VersionString;

	public static string ScreenInfo => $"{DeviceDisplay.MainDisplayInfo.Width} x {DeviceDisplay.MainDisplayInfo.Height}";
	public static string Density => DeviceDisplay.MainDisplayInfo.Density.ToString("0.##");
	public static string Orientation => DeviceDisplay.MainDisplayInfo.Orientation.ToString();
	public static string Rotation => DeviceDisplay.MainDisplayInfo.Rotation.ToString();
	public static string RefreshRate => DeviceDisplay.MainDisplayInfo.RefreshRate.ToString(CultureInfo.CurrentCulture);

	public static string NetworkAccess => Connectivity.Current.NetworkAccess.ToString();
}