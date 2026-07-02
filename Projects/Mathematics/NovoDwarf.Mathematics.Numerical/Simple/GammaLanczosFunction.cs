namespace Mathematics.Numerical.Simple;

public static class GammaLanczosFunction
{
	private const double G = 7;
	private static readonly double[] Coef =
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
	
	public static double Calculate(double x)
	{
		if (x < 0.5)
			return Math.PI / (Math.Sin(Math.PI * x) * Calculate(1 - x));

		x -= 1;

		var a = Coef[0];
		var t = x + G + 0.5;

		for (var i = 1; i < Coef.Length; i++)
			a += Coef[i] / (x + i);

		return Math.Sqrt(2 * Math.PI) * Math.Pow(t, x + 0.5) * Math.Exp(-t) * a;
	}
}