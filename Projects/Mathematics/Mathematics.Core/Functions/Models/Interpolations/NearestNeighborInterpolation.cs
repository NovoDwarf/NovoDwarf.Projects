namespace Mathematics.Core.Functions.Models.Interpolations;

public class NearestNeighborInterpolation
{
	public static double Interpolate2D(double[] x, double[] y, double[,] z, double xNew, double yNew)
	{
		var nearestX = FindNearestIndex(x, xNew);
		var nearestY = FindNearestIndex(y, yNew);

		return z[nearestY, nearestX];
	}

	private static int FindNearestIndex(double[] array, double value)
	{
		var nearestIndex = 0;
		var minDistance = double.MaxValue;

		for (var i = 0; i < array.Length; i++)
		{
			var distance = Math.Abs(array[i] - value);

			if (!(distance < minDistance))
				continue;

			minDistance = distance;
			nearestIndex = i;
		}

		return nearestIndex;
	}
}