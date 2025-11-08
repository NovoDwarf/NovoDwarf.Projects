using Mathematics.Distributions.Interfaces;
using Mathematics.Distributions.Models.Continuous.SemiInfinite;

namespace Structures.Models.Abstracts.Commons.Options;

public class DistributionOptions : RouteOptions
{
	public IDistribution<double> Distribution { get; set; } = new ExpoDistribution(5);
}