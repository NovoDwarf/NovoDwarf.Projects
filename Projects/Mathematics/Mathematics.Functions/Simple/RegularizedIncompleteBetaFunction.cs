namespace Mathematics.Functions;

public class RegularizedIncompleteBetaFunction
{
	public static double Calculate(double x, double a, double b)
	{
		switch (x)
		{
			case 0: return 0;
			case 1: return 1;
		}

		const int steps = 1000;
		
		var sum = 0.0;
		var step = x / steps;

		for (var i = 0; i < steps; i++)
		{
			var t = (i + 0.5) * step;
			sum += Math.Pow(t, a - 1) * Math.Pow(1 - t, b - 1);
		}

		return sum * step / (GammaFunction.Calculate(a) * GammaFunction.Calculate(b) / GammaFunction.Calculate(a + b));
	}
}