namespace Mathematics.Functions;

public static class GammaFunction
{
	public static double Calculate(double x)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(x, 0);

		switch (x)
		{
			case 1.0:
				return 1.0;
			case 0.5:
				return Math.Sqrt(Math.PI);
		}

		if (Math.Abs(x - Math.Round(x)) < 1e-10 && x < 20)
			return FactorialFunction.Calculate((int)x - 1);

		return LanczosGammaFunction.Calculate(x);
	}


}

public static class LanczosGammaFunction
{
	public static double Calculate(double x)
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
			return Math.PI / (Math.Sin(Math.PI * x) * Calculate(1 - x));

		x -= 1;

		var a = p[0];
		var t = x + g + 0.5;

		for (var i = 1; i < p.Length; i++)
			a += p[i] / (x + i);

		return Math.Sqrt(2 * Math.PI) * Math.Pow(t, x + 0.5) * Math.Exp(-t) * a;
	}
}

public static class LogGammaFunction
{
	public static double Calculate(double x)
	{
		// Приближение логарифма гамма-функции
		double[] coef =
		[
			76.18009172947146,
			-86.50532032941677,
			24.01409824083091,
			-1.231739572450155,
			0.1208650973866179e-2,
			-0.5395239384953e-5
		];

		var tmp = x + 5.5;
		tmp -= (x + 0.5) * Math.Log(tmp);
		var ser = 1.000000000190015;

		for (var j = 0; j < 6; j++)
		{
			ser += coef[j] / (x + j + 1);
		}

		return -tmp + Math.Log(2.5066282746310005 * ser / x);	
	}
}