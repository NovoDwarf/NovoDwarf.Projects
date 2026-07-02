namespace Mathematics.Numerical.Simple;

public static class GammaFunction
{
	public static double Calculate(double x)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(x, 0);

		switch (x)
		{
			case 1.0: return 1.0;
			case 0.5: return Math.Sqrt(Math.PI);
		}

		if (Math.Abs(x - Math.Round(x)) < 1e-10 && x < 20)
			return FactorialFunction.Calculate((int)x - 1);

		return GammaLanczosFunction.Calculate(x);
	}


}