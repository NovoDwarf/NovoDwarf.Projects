using Aegis.Packaging.Core.Entities;

namespace Aegis.Packaging.Entities;

internal sealed record PackageReloadPlan(
	string Mode,
	string PackageId,
	PackageDiscoveryResult Discovery,
	IReadOnlyList<PackageInstance> NextLoadOrder,
	IReadOnlyList<PackageInstance> CurrentLoadOrder,
	IReadOnlyDictionary<string, PackageInstance> CurrentPackages,
	HashSet<string> AffectedIds,
	IReadOnlyDictionary<string, string> Fingerprints,
	bool CanReuseSnapshot);
