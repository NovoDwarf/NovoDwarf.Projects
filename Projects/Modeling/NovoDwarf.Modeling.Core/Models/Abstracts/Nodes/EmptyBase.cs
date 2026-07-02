using Modeling.Core.Models.Abstracts.Commons.Nodes;
using Modeling.Core.Models.Abstracts.Options;

namespace Modeling.Core.Models.Abstracts.Nodes;

public abstract class EmptyBase : RouteNode
{
	protected EmptyBase(EmptyOptions? options = null) : base(options)
	{
	}
}