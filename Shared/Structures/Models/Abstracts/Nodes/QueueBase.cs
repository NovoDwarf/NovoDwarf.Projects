using Structures.Models.Abstracts.Commons.Nodes;
using Structures.Models.Abstracts.Options;

namespace Structures.Models.Abstracts.Nodes;

public abstract class QueueBase : RouteNode
{
	private protected readonly QueueOptions _options;
	
	protected QueueBase(QueueOptions? options = null) : base(options)
	{
		_options = options ?? new QueueOptions();
	}
}