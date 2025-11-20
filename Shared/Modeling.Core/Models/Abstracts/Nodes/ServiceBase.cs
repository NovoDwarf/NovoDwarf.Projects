using Modeling.Core.Models.Abstracts.Commons.Nodes;
using Modeling.Core.Models.Abstracts.Options;

namespace Modeling.Core.Models.Abstracts.Nodes;

public abstract class ServiceBase : DistributionNode
{
	protected ServiceBase(ServiceOptions? options = null) : base(options)
	{
	}
}