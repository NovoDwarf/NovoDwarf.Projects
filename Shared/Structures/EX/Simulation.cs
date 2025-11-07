using Logging.Models;
using Structures.Models.Abstracts.Commons.Nodes;
using Structures.Models.Base;

namespace Structures.EX;

public abstract class Simulation
{
	protected readonly Dictionary<Guid, NodeBase> NodesById = new();
	protected readonly List<NodeBase> Nodes = [];

	protected MetricCollector Collector { get; set; } = null!;
	protected SimulationContext Context { get; set; } = null!;
	
	public void AddNodes(params NodeBase[] nodes)
	{
		foreach (var node in nodes)
		{
			if (!NodesById.TryAdd(node.Id, node))
				continue;

			Nodes.Add(node);
		}
	}

	internal void CompleteRequest(Request request)
	{
		//if (Options.ClosedSystem && NodesById.TryGetValue(request.SourceId, out var node) && node is ICompletionAware aware)
		{
			//aware.OnRequestCompleted(request, this);
		}
	}
	
	public abstract void Simulate();
}