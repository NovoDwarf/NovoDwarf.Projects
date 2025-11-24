namespace Mathematics.Functions;

public static class ComplementaryErrorFunction
{
	public static double Calculate(double x)
	{
		return 1 - ErrorFunction.Calculate(x);
	}
}