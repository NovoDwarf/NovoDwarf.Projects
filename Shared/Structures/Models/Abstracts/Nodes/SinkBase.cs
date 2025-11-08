using Structures.Models.Abstracts.Commons.Nodes;
using Structures.Models.Abstracts.Options;

namespace Structures.Models.Abstracts.Nodes;

public abstract class SinkBase : RouteNode
{
	protected SinkBase(SinkOptions? options = null) : base(options)
	{
	}
}