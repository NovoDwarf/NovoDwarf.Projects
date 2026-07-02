using System.Globalization;
using NovoDwarf.Mathematics.App.Core.ViewModels;

namespace NovoDwarf.Mathematics.App.ViewModels.Utilities.Sensors;

public class DeviceInfoViewModel : BaseViewModel
{
	public string Model => DeviceInfo.Model;
	public string Manufacturer => DeviceInfo.Manufacturer;
	public string Platform => DeviceInfo.Platform.ToString();
	public string OsVersion => DeviceInfo.VersionString;
	public string DeviceType => DeviceInfo.DeviceType.ToString();
	public string DeviceIdiom => DeviceInfo.Idiom.ToString();

	public string AppName => AppInfo.Name;
	public string AppVersion => AppInfo.VersionString;

	public string ScreenInfo => $"{DeviceDisplay.MainDisplayInfo.Width} x {DeviceDisplay.MainDisplayInfo.Height}";
	public string Density => DeviceDisplay.MainDisplayInfo.Density.ToString("0.##");
	public string Orientation => DeviceDisplay.MainDisplayInfo.Orientation.ToString();
	public string Rotation => DeviceDisplay.MainDisplayInfo.Rotation.ToString();
	public string RefreshRate => DeviceDisplay.MainDisplayInfo.RefreshRate.ToString(CultureInfo.CurrentCulture);

	public string NetworkAccess => Connectivity.Current.NetworkAccess.ToString();
}
