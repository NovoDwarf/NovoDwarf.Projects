using System.Numerics;
using AccelerometerData = Mathematics.App.Maui.Systems.Sensors.DTOs.AccelerometerData;

namespace Mathematics.App.Maui.Systems.Sensors.Processing;

public sealed class AccelerometerProcessingService
{
	private double _gx, _gy, _gz;
	private double _linearMagFiltered;

	private const double Alpha = 0.2;
	private const double MagAlpha = 0.3;

	public AccelerometerData Process(Vector3 raw)
	{
		_gx += Alpha * (raw.X - _gx);
		_gy += Alpha * (raw.Y - _gy);
		_gz += Alpha * (raw.Z - _gz);
		
		var lx = raw.X - _gx;
		var ly = raw.Y - _gy;
		var lz = raw.Z - _gz;

		var mag = Math.Sqrt((lx * lx) + (ly * ly) + (lz * lz));
		_linearMagFiltered += MagAlpha * (mag - _linearMagFiltered);

		return new AccelerometerData
		{
			Linear = new Vector3((float)lx, (float)ly, (float)lz),
			Gravity = new Vector3((float)_gx, (float)_gy, (float)_gz),
			LinearMagnitude = _linearMagFiltered
		};
	}
}