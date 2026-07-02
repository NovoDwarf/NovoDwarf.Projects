using Modeling.Core.Models.Base;
using NovoDwarf.Primitives.Interfaces;
using NovoDwarf.Primitives.Models.Storages;

namespace Modeling.Core.Models.Abstracts.Commons.Options;

public class StorageOptions : NodeOptions
{
	public int Capacity { get; set; } = -1;

	/// <summary>
	///  Gets the storage queue containing requests currently being processed by the node.
	/// </summary>
	public IStorage<Request> Storage { get; set; } = new LifoStorage<Request>();
}