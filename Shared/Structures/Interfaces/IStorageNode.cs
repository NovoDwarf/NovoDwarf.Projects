using Structures.Models.Base;

namespace Structures.Interfaces;

public interface IStorageNode : INode
{
	/// <summary>
	/// Gets a value indicating whether the node has unlimited storage capacity.
	/// Returns true when Capacity is less than or equal to zero.
	/// </summary>
	public bool IsUnlimitedCapacity { get; }
    
	/// <summary>
	/// Gets a value indicating whether the node is currently busy and cannot accept new requests.
	/// Returns true when the node has limited capacity and current storage count meets or exceeds capacity.
	/// </summary>
	public bool IsFull { get; }
	
	/// <summary>
	/// Gets or sets the capacity of the node's storage queue.
	/// Values less than or equal to 0 indicate unlimited capacity.
	/// Default value is -1 (unlimited).
	/// </summary>
	public int Capacity { get; }
	
	public Request? Dequeue();
	
	public void Enqueue(Request request);

	public Request? Peek();
}