namespace Mathematics.Numerical.Simple;

public static class ErrorFunction
{
	// Abramowitz and Stegun. Handbook of Mathematical Functions
	// Page 299. Formula 7.1.26. Error Function
	
	private const double A1 = 0.254829592;
	private const double A2 = -0.284496736;
	private const double A3 = 1.421413741;
	private const double A4 = -1.453152027;
	private const double A5 = 1.061405429;
	private const double P = 0.3275911;
	
	public static double Calculate(double x)
	{
		var sign = 1;
		
		if (x < 0)
			sign = -1;
        
		x = Math.Abs(x);
        
		var t = 1.0 / (1.0 + P * x);
		var y = 1.0 - ((((A5 * t + A4) * t + A3) * t + A2) * t + A1) * t * Math.Exp(-x * x);
        
		return sign * y;
	}
}