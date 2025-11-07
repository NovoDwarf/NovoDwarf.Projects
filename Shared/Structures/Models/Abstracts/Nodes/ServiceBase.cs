using Structures.Models.Abstracts.Commons.Nodes;
using Structures.Models.Abstracts.Options;

namespace Structures.Models.Abstracts.Nodes;

public abstract class ServiceBase : DistributionNode
{
	protected ServiceBase(ServiceOptions? options = null) : base(options)
	{
		
	}
}