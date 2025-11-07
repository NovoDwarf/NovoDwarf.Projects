using Mathematics.Interfaces;

namespace Mathematics.Tests.Services;

public class TestsUtils
{
	public static List<double> GenerateSamples<T>(IDistribution distribution, int count) where T : IDistribution
	{
		var results = new List<double>();
		
		for (var i = 0; i < count; i++) 
			results.Add(distribution.Calculate());
		
		return results;
	}
}