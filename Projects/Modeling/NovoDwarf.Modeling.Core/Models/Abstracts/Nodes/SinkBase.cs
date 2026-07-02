using Modeling.Core.Models.Abstracts.Commons.Nodes;
using Modeling.Core.Models.Abstracts.Options;

namespace Modeling.Core.Models.Abstracts.Nodes;

public abstract class SinkBase : RouteNode
{
	protected SinkBase(SinkOptions? options = null) : base(options)
	{
	}
}