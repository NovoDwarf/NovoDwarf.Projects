using Mathematics.Core.Distributions.Interfaces;
using Mathematics.Core.Distributions.Models.Univariate.Continuous.SemiInfinite;

namespace Modeling.Core.Models.Abstracts.Commons.Options;

public class DistributionOptions : RouteOptions
{
	public IDistribution<double> Distribution { get; set; } = new ExpoDistribution(5);
}