using Aegis.Packaging.Core.Entities;
using Aegis.Packaging.Core.Enums;
using Aegis.Packaging.Core.Interfaces;
using Aegis.Packaging.Core.Services;
using Microsoft.Extensions.Logging;

namespace Aegis.Packaging.Definitions.Services;

public sealed class DefinitionService : IDefinitionCatalog, IPackageContentLoader
{
	private readonly IFileSystemService _fileSystem;
	private readonly DefinitionIndex _index;
	private readonly DefinitionScanner _scanner;

	public DefinitionService(ILogger<DefinitionService> logger, IFileSystemService fileSystem)
	{
		_fileSystem = fileSystem;
		_index = new DefinitionIndex(logger);
		_scanner = new DefinitionScanner(fileSystem);
	}

	public PackageContentStage Stage => PackageContentStage.Definitions;

	public T? Get<T>(string id) where T : Def
	{
		return _index.Get<T>(id);
	}

	public T? GetByPath<T>(string? resourcePath) where T : Def
	{
		if (string.IsNullOrWhiteSpace(resourcePath))
			return null;
		
		return _index.GetByPath<T>(resourcePath);
	}

	public IEnumerable<T> All<T>() where T : Def
	{
		return _index.All<T>();
	}

	public T? FirstOrDefault<T>() where T : Def
	{
		return _index.FirstOrDefault<T>();
	}
	
	public bool TryGetOrigin(Def def, out string packageId, out string resourcePath)
	{
		return _index.TryGetOrigin(def, out packageId, out resourcePath);
	}

	public void Clear()
	{
		_index.Clear();
	}

	public void LoadPackage(PackageInstance package, PackageConflictRegistry packageConflicts)
	{
		var definitionsRoot = PackageAlias.GetDefinitionsFolder(package.RootPath);

		if (!_fileSystem.DirectoryExists(definitionsRoot))
			definitionsRoot = package.RootPath;

		var batch = _scanner.LoadPackage(package.Id, definitionsRoot, definitionsRoot);
		_index.Register(batch.Entries, packageConflicts);
		_index.Resolve(batch.PendingReferences);
	}

	public void UnloadPackage(string packageId)
	{
		_index.UnloadPackage(packageId);
	}
}
