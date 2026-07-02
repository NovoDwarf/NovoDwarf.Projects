using Modeling.Core.Models.Abstracts.Commons.Nodes;

namespace Modeling.Core.Extensions;

public static class RouteNodeExtensions
{
	extension(RouteNode node)
	{
		public RouteNode Connect(RouteNode next)
		{
			node.AddOutput(next);

			return next;
		}
	}
}