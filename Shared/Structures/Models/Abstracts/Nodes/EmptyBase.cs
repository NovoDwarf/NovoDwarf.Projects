using Structures.Models.Abstracts.Commons.Nodes;
using Structures.Models.Abstracts.Options;

namespace Structures.Models.Abstracts.Nodes;

public abstract class EmptyBase : RouteNode
{
	protected EmptyBase(EmptyOptions? options = null) : base(options)
	{
	}
}