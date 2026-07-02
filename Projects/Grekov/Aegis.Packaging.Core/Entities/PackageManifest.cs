namespace Aegis.Packaging.Core.Entities;

public sealed class PackageManifest
{
	public int ManifestVersion { get; init; } = 1;
	
	public string Id { get; init; } = string.Empty;
	public string Author { get; init; } = "Unknown";
	public string Version { get; init; } = "Unknown";
	
	public object? Icon { get; init; }

	public string Description { get; init; } = string.Empty;
	public string Website { get; init; } = string.Empty;

	public IReadOnlyList<string> Tags { get; init; } = [];
	public IReadOnlyList<string> Locales { get; init; } = [];
	public IReadOnlyList<string> Capabilities { get; init; } = [];
	public IReadOnlyList<string> CompatibilityVersions { get; init; } = [];
	public IReadOnlyList<string> DependenciesRaw { get; init; } = [];
	public IReadOnlyList<string> OptionalDependenciesRaw { get; init; } = [];
	public IReadOnlyList<string> IncompatiblePackages { get; init; } = [];
	public IReadOnlyList<string> LoadAfter { get; init; } = [];
	public IReadOnlyList<string> LoadBefore { get; init; } = [];
}
