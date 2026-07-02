using NovoDwarf.Mathematics.App.Core.ViewModels;
using NovoDwarf.Mathematics.App.Systems.Application.Constants;
using NovoDwarf.Mathematics.App.Systems.Sensors.Services;

namespace NovoDwarf.Mathematics.App.ViewModels.Utilities.Sensors;

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