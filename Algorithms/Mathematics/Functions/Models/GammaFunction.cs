namespace Mathematics.Functions.Models;

public class GammaFunction
{
	public static double Calculate(double x)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(x, 0);

		if (x == 1.0)
			return 1.0;

		if (x == 0.5)
			return Math.Sqrt(Math.PI);

		if (Math.Abs(x - Math.Round(x)) < 1e-10 && x < 20)
			return Factorial((int)x - 1);

		return LanczosGamma(x);
	}

	public static double LogGamma(double x)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(x, 0);

		return x > 15
			? StirlingLogGamma(x)
			: Math.Log(Calculate(x));
	}

	private static double LanczosGamma(double x)
	{
		double[] p =
		[
			676.5203681218851,
			-1259.1392167224028,
			771.32342877765313,
			-176.61502916214059,
			12.507343278686905,
			-0.13857109526572012,
			9.9843695780195716e-6,
			1.5056327351493116e-7
		];

		const double g = 7;

		if (x < 0.5)
			return Math.PI / (Math.Sin(Math.PI * x) * LanczosGamma(1 - x));

		x -= 1;

		var a = p[0];
		var t = x + g + 0.5;

		for (var i = 1; i < p.Length; i++)
			a += p[i] / (x + i);

		return Math.Sqrt(2 * Math.PI) * Math.Pow(t, x + 0.5) * Math.Exp(-t) * a;
	}

	private static double StirlingLogGamma(double x)
	{
		return 0.5 * Math.Log(2 * Math.PI) - Math.Log(x) +
			x * (Math.Log(x) - 1) + 1.0 / (12 * x) - 1.0 / (360 * x * x * x);
	}

	private static double Factorial(int n)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(n, 0);

		if (n is 0 or 1)
			return 1.0;

		var result = 1.0;

		for (var i = 2; i <= n; i++)
			result *= i;

		return result;
	}
}