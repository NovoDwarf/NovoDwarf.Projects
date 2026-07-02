using Modeling.Core.Interfaces;
using Modeling.Core.Models.Abstracts.Commons.Options;
using Modeling.Core.Models.Base;

namespace Modeling.Core.Models.Abstracts.Commons.Nodes;

public abstract class RouteNode : StorageNode, IRouteNode
{
	private readonly RouteOptions _options;

	private int _roundRobinIndex;

	protected RouteNode(RouteOptions? options = null) : base(options)
	{
		_options = options ?? new RouteOptions();
	}

	/// <inheritdoc cref="RouteOptions.InputMaxCount" />
	public int InputMaxCount => _options.InputMaxCount;

	/// <inheritdoc cref="RouteOptions.OutputMaxCount" />

	public int OutputMaxCount => _options.OutputMaxCount;

	/// <summary>
	///     Gets the list of input nodes that can send requests to this node.
	///     This collection is protected and should be modified through the AddInput method.
	/// </summary>
	protected List<RouteNode> InputNodes { get; } = [];

	/// <summary>
	///     Gets the list of output nodes that can receive requests from this node.
	///     This collection is protected and should be modified through the AddOutput method.
	/// </summary>
	protected List<RouteNode> OutputNodes { get; } = [];

	/// <inheritdoc cref="IRouteNode.Inputs" />
	public IReadOnlyList<IRouteNode> Inputs => InputNodes.AsReadOnly();

	/// <inheritdoc cref="IRouteNode.Outputs" />
	public IReadOnlyList<IRouteNode> Outputs => OutputNodes.AsReadOnly();

	public override void Update(double deltaTime)
	{
		if (IsEmpty)
			return;

		var next = GetAvailableExit();

		if (next == null)
			return;

		var req = Dequeue();
		//var wait = currentTime - req.QueueEnter;

		//req.QueueTime += wait;

		if (req != null)
			next.Process(req);
	}

	public override void Process(Request request)
	{
		if (IsFull)
			return;

		Enqueue(request);
	}

	/// <summary>
	///     Adds an input node to this node's input connections.
	///     Validates against the InputMaxCount limit before adding.
	/// </summary>
	/// <param name="node">The node to add as input. Null values are ignored.</param>
	/// <exception cref="InvalidOperationException">
	///     Thrown when attempting to exceed the maximum allowed input count.
	/// </exception>
	protected void AddInput(RouteNode? node)
	{
		if (InputMaxCount > 0 && InputNodes.Count >= InputMaxCount)
			throw new InvalidOperationException($"Node [{Id}]: input links exceed limit [{InputMaxCount}].");

		if (node != null && !InputNodes.Contains(node))
			InputNodes.Add(node);
	}

	/// <summary>
	///     Adds an output node to this node's output connections.
	///     Validates against the OutputMaxCount limit before adding.
	/// </summary>
	/// <param name="node">The node to add as output. Null values are ignored.</param>
	/// <exception cref="InvalidOperationException">
	///     Thrown when attempting to exceed the maximum allowed output count.
	/// </exception>
	protected internal void AddOutput(RouteNode? node)
	{
		if (OutputMaxCount > 0 && OutputNodes.Count >= OutputMaxCount)
			throw new InvalidOperationException($"Node [{Id}]: output links exceed limit [{OutputMaxCount}].");

		if (node != null && !OutputNodes.Contains(node))
		{
			OutputNodes.Add(node);
			node.AddInput(this); // двунаправленно фиксируем связь для доступа к Inputs
		}
	}

	/// <summary>
	///     Selects an available output node using round-robin load balancing algorithm.
	///     Skips nodes that are currently busy and cycles through available nodes in sequence.
	/// </summary>
	/// <returns>
	///     The next available output node that is not busy, or null if no available nodes are found.
	/// </returns>
	protected RouteNode? GetAvailableExit()
	{
		if (OutputNodes.Count == 0)
			return null;

		for (var i = 0; i < OutputNodes.Count; i++)
		{
			var idx = (_roundRobinIndex + i) % OutputNodes.Count;
			var candidate = OutputNodes[idx];

			if (candidate is not { IsFull: false })
				continue;

			_roundRobinIndex = (idx + 1) % OutputNodes.Count;

			return candidate;
		}

		return null;
	}
}