namespace Modeling.Core.Interfaces;

public interface IRouteNode : IStorageNode
{
	/// <summary>
	///     Gets a read-only list of input nodes connected to this node.
	///     These are nodes that can send requests to this node.
	/// </summary>
	public IReadOnlyList<IRouteNode> Inputs { get; }

	/// <summary>
	///     Gets a read-only list of output nodes connected to this node.
	///     These are nodes that can receive requests from this node.
	/// </summary>
	public IReadOnlyList<IRouteNode> Outputs { get; }
}