namespace Mathematics.Numerical.Simple;

public class FactorialApproximationFunction
{
	private static readonly double[] SmallFactorials =
	[
		1, 1, 2, 6, 24, 120, 720, 5040, 40320, 362880,
		3628800, 39916800, 479001600, 6227020800, 87178291200,
		1307674368000, 20922789888000, 355687428096000,
		6402373705728000, 121645100408832000
	];
	
	public static double Calculate(int n)
	{
		return n switch
		{
			< 0 => 0,
			<= 1 => 1,
			>= 20 => Math.Sqrt(2 * Math.PI * n) * Math.Pow(n / Math.E, n) * (1 + 1.0 / (12 * n) + 1.0 / (288 * n * n)),
			_ => SmallFactorials[n]
		};
	}
}