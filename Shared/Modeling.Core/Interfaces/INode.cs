namespace Modeling.Core.Interfaces;

public interface INode
{
	/// <summary>
	///     Gets the unique identifier of the node.
	///     This ID is automatically generated upon node creation.
	/// </summary>
	public Guid Id { get; }
}