namespace Mathematics.Numerical.Simple;

public static class GammaLogFunction
{
	private static readonly double[] Coef =
	[
		76.18009172947146,
		-86.50532032941677,
		24.01409824083091,
		-1.231739572450155,
		0.1208650973866179e-2,
		-0.5395239384953e-5
	];
	
	public static double Calculate(double x)
	{


		var tmp = x + 5.5;
		tmp -= (x + 0.5) * Math.Log(tmp);
		var ser = 1.000000000190015;

		for (var j = 0; j < 6; j++) 
			ser += Coef[j] / (x + j + 1);

		return -tmp + Math.Log(2.5066282746310005 * ser / x);	
	}
}