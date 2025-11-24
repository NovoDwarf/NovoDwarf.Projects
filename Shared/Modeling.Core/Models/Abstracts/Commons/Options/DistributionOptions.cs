using Mathematics.Core.Base;
using Mathematics.Distributions.Univariate.Continuous.Semibounded;

namespace Modeling.Core.Models.Abstracts.Commons.Options;

public class DistributionOptions : RouteOptions
{
	public Distribution Distribution { get; set; } = new ExpoDistribution(5);
}