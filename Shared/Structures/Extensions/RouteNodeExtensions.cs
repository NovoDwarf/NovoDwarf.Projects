using Structures.Models.Abstracts.Commons.Nodes;

namespace Structures.Extensions;

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