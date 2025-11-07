using Mathematics.Interfaces;
using Mathematics.Models.Distributions.Basic;

namespace Structures.Models.Abstracts.Commons.Options;

public class DistributionOptions : RouteOptions
{
	public IDistribution Distribution { get; set; } = new Exponential(5);
}