using Aegis.Packaging.Core.Entities;

namespace Aegis.Packaging.Entities;

internal sealed class PackageServiceState
{
	public List<PackageInstance> Packages { get; } = [];
	
	public Dictionary<string, PackageInstance> PackagesById { get; } = new(StringComparer.OrdinalIgnoreCase);
	public Dictionary<string, bool> EnabledOverrides { get; } = new(StringComparer.OrdinalIgnoreCase);

	public IReadOnlyList<PackageInstance> LoadOrder { get; set; } = [];
}
