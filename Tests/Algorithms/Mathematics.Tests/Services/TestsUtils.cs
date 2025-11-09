using Mathematics.Distributions.Interfaces;

namespace Mathematics.Tests.Services;

public class TestsUtils
{
	public static List<double> GenerateSamples(IDistribution<double> distribution, int count)
	{
		var results = new List<double>();

		for (var i = 0; i < count; i++)
			results.Add(distribution.Calculate());

		return results;
	}
}