namespace Mathematics.Numerical.Simple;

public static class ErrorInverseFunction
{
	private const double A = 0.147;
	
	public static double Calculate(double y)
	{
		if (y is <= -1.0 or >= 1.0)
			throw new ArgumentOutOfRangeException(nameof(y), "y must be in range (-1, 1)");

		var sign = y < 0 ? -1 : 1;
		y = Math.Abs(y);
        
		var term1 = 2.0 / (Math.PI * A) + 0.5 * Math.Log(1.0 - y * y);
		var term2 = 0.5 / A * Math.Log(1.0 - y * y);
		var inner = term1 - term2;
        
		if (inner < 0)
			inner = 0;
        
		var result = sign * Math.Sqrt(Math.Sqrt(inner * inner - 1.0 / A * Math.Log(1.0 - y * y)) - inner);
        
		return result;
	}
}