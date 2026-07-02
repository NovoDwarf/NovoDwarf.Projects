namespace NovoDwarf.Mathematics.App.Systems.Sensors.ValueObjects;

public sealed class MinMaxTracker
{
	public double Min { get; private set; }
	public double Max { get; private set; }

	public void Reset(double value)
	{
		Min = value;
		Max = value;
	}

	public void Push(double value)
	{
		Min = Math.Min(Min, value);
		Max = Math.Max(Max, value);
	}
}