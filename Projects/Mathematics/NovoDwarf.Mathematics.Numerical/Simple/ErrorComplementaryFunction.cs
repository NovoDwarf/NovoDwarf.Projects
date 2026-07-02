namespace Mathematics.Numerical.Simple;

public static class ErrorComplementaryFunction
{
	public static double Calculate(double x)
	{
		return 1 - ErrorFunction.Calculate(x);
	}
}