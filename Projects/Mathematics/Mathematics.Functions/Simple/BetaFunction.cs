namespace Mathematics.Functions;

public class BetaFunction
{
	public static double Calculate(double a, double b)
	{
		return GammaFunction.Calculate(a) * GammaFunction.Calculate(b) / GammaFunction.Calculate(a + b);
	}	
}