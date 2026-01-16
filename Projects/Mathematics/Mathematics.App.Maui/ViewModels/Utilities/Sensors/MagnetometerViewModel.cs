using Mathematics.App.Maui.Systems.Application.Constants;
using Mathematics.App.Maui.Systems.Sensors.Services;
using Mathematics.App.Maui.UI.Base;

namespace Mathematics.App.Maui.UI.ViewModels.Utilities.Sensors;

public class MagnetometerViewModel : BaseViewModel
{
	public MagnetometerViewModel(MagnetometerService magnetometerService)
	{
		
	}

	private void Start()
	{
		if (!MagnetometerService.IsSupported)
		{ 
			Alerts.SensorNotSupported("Accelerometer");
			return;
		}
	}
	
	private void Stop()
	{
		
	}
}