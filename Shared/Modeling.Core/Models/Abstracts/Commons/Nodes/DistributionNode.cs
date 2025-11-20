using Mathematics.Core.Distributions.Interfaces;
using Modeling.Core.Models.Abstracts.Commons.Options;

namespace Modeling.Core.Models.Abstracts.Commons.Nodes;

public abstract class DistributionNode : RouteNode
{
	private new readonly DistributionOptions _options;

	public DistributionNode(DistributionOptions? options = null) : base(options)
	{
		_options = options ?? new DistributionOptions();
	}

	public bool IsBusy { get; protected set; }
	public IDistribution<double> Distribution => _options.Distribution;
}