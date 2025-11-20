using Logging.Models;
using Modeling.Core.Models.Abstracts.Commons.Nodes;
using Modeling.Core.Models.Base;

namespace Modeling.Core.EX;

public abstract class Simulation
{
	protected readonly List<NodeBase> Nodes = [];
	protected readonly Dictionary<Guid, NodeBase> NodesById = new();

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