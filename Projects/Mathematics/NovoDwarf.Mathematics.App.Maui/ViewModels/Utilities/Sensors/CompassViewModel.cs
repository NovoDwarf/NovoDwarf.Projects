using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NovoDwarf.Mathematics.App.Core.ViewModels;
using NovoDwarf.Mathematics.App.Systems.Sensors.Services;

namespace NovoDwarf.Mathematics.App.ViewModels.Utilities.Sensors;

public sealed partial class CompassViewModel : BaseViewModel
{
	private readonly CompassService _compass;

	public CompassViewModel(CompassService compass)
	{
		_compass = compass;
		Start();
	}

	[ObservableProperty]
	public partial double Heading { get; set; }

	[ObservableProperty]
	public partial string HeadingText { get; set; } = "0°";

	[ObservableProperty]
	public partial bool IsActive { get; set; }

	private void Start()
	{
		if (!CompassService.IsSupported)
		{
			Shell.Current.DisplayAlertAsync(
				"Уведомление",
				"Магнитометр не поддерживается данным устройством!",
				"Ок!");
			return;
		}

		_compass.HeadingChanged += OnHeading;
		_compass.Start();
		IsActive = true;
	}

	private void OnHeading(object? sender, double heading)
	{
		MainThread.BeginInvokeOnMainThread(() =>
		{
			Heading = heading;
			HeadingText = $"{Math.Round(heading)}°";
		});
	}

	[RelayCommand]
	private void Stop()
	{
		if (!IsActive)
			return;

		_compass.HeadingChanged -= OnHeading;
		_compass.Stop();
		IsActive = false;
	}
}
