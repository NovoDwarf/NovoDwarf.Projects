using Mathematics.Core.Distributions.Interfaces;

namespace Mathematics.Tests.Services;

public static class TestsUtils
{
	public static List<double> GenerateSamples(IDistribution<double> distribution, int count)
	{
		var results = new List<double>();

		for (var i = 0; i < count; i++)
			results.Add(distribution.Distribute());

		return results;
	}
}