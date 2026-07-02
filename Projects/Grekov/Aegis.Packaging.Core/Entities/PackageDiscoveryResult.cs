namespace Aegis.Packaging.Core.Entities;

public sealed record PackageDiscoveryResult(IReadOnlyList<PackageInstance> Packages, IReadOnlyDictionary<string, PackageInstance> PackagesById);