using Modeling.Core.Models.Abstracts.Commons.Nodes;
using Modeling.Core.Models.Abstracts.Options;

namespace Modeling.Core.Models.Abstracts.Nodes;

public abstract class QueueBase : RouteNode
{
	private protected readonly QueueOptions _options;

	protected QueueBase(QueueOptions? options = null) : base(options)
	{
		_options = options ?? new QueueOptions();
	}
}