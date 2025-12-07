using Mathematics.Core.Base;
using Mathematics.Core.Base.Entities;

namespace Mathematics.Tests.Services;

public static class TestsUtils
{
	public static List<double> GenerateSamples(Distribution distribution, int count)
	{
		var results = new List<double>();

		for (var i = 0; i < count; i++)
			results.Add(distribution.Distribute());

		return results;
	}
}