using Aegis.Packaging.Assemblies.Entities;
using Aegis.Packaging.Core.Entities;

namespace Aegis.Packaging.Assemblies;

public sealed class PackageAssemblyLocator
{
	private readonly IFileSystemService _fileSystem;

	public PackageAssemblyLocator(IFileSystemService fileSystem)
	{
		_fileSystem = fileSystem;
	}

	public PackageAssemblySet? Locate(PackageInstance package)
	{
		var assembliesRoot = PackageAlias.GetAssemblyFolder(package.RootPath);
		var fsRoot = _fileSystem.GlobalizePath(assembliesRoot);

		if (!_fileSystem.DirectoryExists(fsRoot))
			return null;

		var assemblyPaths = _fileSystem
			.EnumerateFiles(fsRoot, "*.dll", SearchOption.TopDirectoryOnly)
			.OrderBy(static path => path, StringComparer.OrdinalIgnoreCase)
			.ToArray();

		if (assemblyPaths.Length == 0)
			return null;

		var filteredAssemblyPaths = FilterPackageAssemblyPaths(package, assemblyPaths);
		if (filteredAssemblyPaths.Length == 0)
			return null;

		var primaryAssemblyPath = ResolvePrimaryAssemblyPath(package, filteredAssemblyPaths);

		return new PackageAssemblySet(
			assembliesRoot,
			primaryAssemblyPath,
			filteredAssemblyPaths);
	}

	public static string ResolvePrimaryAssemblyPath(PackageInstance package, IReadOnlyList<string> assemblyPaths)
	{
		var preferredName = $"{package.Id.Trim()}.dll";

		return assemblyPaths.FirstOrDefault(path =>
			       string.Equals(Path.GetFileName(path), preferredName, StringComparison.OrdinalIgnoreCase))
		       ?? assemblyPaths[0];
	}

	public static string[] FilterPackageAssemblyPaths(PackageInstance package, IReadOnlyList<string> assemblyPaths)
	{
		var primaryAssemblyPath = ResolvePrimaryAssemblyPath(package, assemblyPaths);
		var primaryAssemblyName = Path.GetFileNameWithoutExtension(primaryAssemblyPath);

		return assemblyPaths
			.Where(path => !AssemblyLoadPolicy.IsReservedHostAssemblyName(
				Path.GetFileNameWithoutExtension(path),
				primaryAssemblyName))
			.ToArray();
	}

	public static IEnumerable<string> OrderAssemblyPaths(PackageInstance package, IReadOnlyList<string> assemblyPaths)
	{
		var primaryAssemblyPath = ResolvePrimaryAssemblyPath(package, assemblyPaths);

		yield return primaryAssemblyPath;

		foreach (var assemblyPath in assemblyPaths)
		{
			if (string.Equals(assemblyPath, primaryAssemblyPath, StringComparison.OrdinalIgnoreCase))
				continue;

			yield return assemblyPath;
		}
	}
}
