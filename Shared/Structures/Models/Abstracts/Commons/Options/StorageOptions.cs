using Data.Interfaces;
using Data.Models;
using Data.Models.Storages;
using Structures.Models.Base;

namespace Structures.Models.Abstracts.Commons.Options;

public class StorageOptions : NodeOptions
{
	public int Capacity { get; set; } = -1;

	/// <summary>
	///     Gets the storage queue containing requests currently being processed by the node.
	/// </summary>
	public IStorage<Request> Storage { get; set; } = new LifoStorage<Request>();
}