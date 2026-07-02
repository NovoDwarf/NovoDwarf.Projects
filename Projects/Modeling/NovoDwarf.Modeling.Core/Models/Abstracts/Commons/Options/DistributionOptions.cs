using Mathematics.Core.Base.Entities;
using Mathematics.Probability.Distributions.Univariate.Continuous.Semibounded;

namespace Modeling.Core.Models.Abstracts.Commons.Options;

public class DistributionOptions : RouteOptions
{
	public Distribution Distribution { get; set; } = new ExpoDistribution() ;
}