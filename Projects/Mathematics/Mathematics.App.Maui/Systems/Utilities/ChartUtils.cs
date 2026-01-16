using LiveChartsCore;

namespace Mathematics.App.Maui.Systems.Utilities;

public class ChartUtils
{
	public static void Push<T>(ISeries series, T value, int maxPoints = 100)
	{
		var values = (List<T>)series.Values!;
		values.Add(value);

		if (values.Count > maxPoints)
			values.RemoveAt(0);
	}
}