namespace Mathematics.Numerical.Simple;

public class FactorialFunction
{
	public static double Calculate(int n)
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