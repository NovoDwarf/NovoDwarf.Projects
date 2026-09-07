using System.Text.Json;
using Grekov.Packaging.Entities;
using Grekov.Packaging.Interfaces;

namespace Grekov.Packaging.Services.Discovery;

internal sealed class PackageDiscover
{
	private const string ManifestFileName = "grekov.package.json";

	public IReadOnlyList<PackageInstance> Discover(IPackageCatalog catalog, IReadOnlyDictionary<string, bool> enabledOverrides)
	{
		var packages = new List<PackageInstance>();

		foreach (var root in catalog.GetPackageRoots())
		{
			var manifest = ReadManifest(root);
			var package = new PackageInstance(manifest.Id, manifest.Version, root)
			{
				Enabled = enabledOverrides.GetValueOrDefault(manifest.Id, manifest.Enabled)
			};

			package.Dependencies.AddRange(manifest.Dependencies);
			packages.Add(package);
		}

		return packages;
	}

	private static PackageManifest ReadManifest(string root)
	{
		var manifestPath = Path.Combine(root, ManifestFileName);
		if (!File.Exists(manifestPath))
		{
			return new PackageManifest
			{
				Id = Path.GetFileName(root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
			};
		}

		var manifest = JsonSerializer.Deserialize<PackageManifest>(File.ReadAllText(manifestPath), new JsonSerializerOptions(JsonSerializerDefaults.Web))
		               ?? new PackageManifest();

		if (string.IsNullOrWhiteSpace(manifest.Id))
			manifest.Id = Path.GetFileName(root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

		return manifest;
	}
}
