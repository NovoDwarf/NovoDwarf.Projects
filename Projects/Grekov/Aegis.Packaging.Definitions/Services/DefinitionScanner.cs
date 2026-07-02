using Aegis.Packaging.Definitions.Entities;
using Aegis.Packaging.Definitions.Reader.Services;

namespace Aegis.Packaging.Definitions.Services;

internal sealed class DefinitionScanner
{
	private readonly IFileSystemService _fileSystem;
	private readonly DefinitionXmlReader _reader;

	public DefinitionScanner(IFileSystemService fileSystem)
	{
		_fileSystem = fileSystem;
		_reader = new DefinitionXmlReader(fileSystem);
	}

	public DefinitionBatch LoadPackage(string packageId, string scanPath, string root)
	{
		var entries = new List<DefinitionEntry>();
		var pendingReferences = new List<PendingReference>();

		_reader.RefreshTypeMap();
		LoadRecursive(packageId, scanPath, root, entries, pendingReferences);

		return new DefinitionBatch(entries, pendingReferences);
	}

	private void LoadRecursive(string packageId, string scanPath, string root, List<DefinitionEntry> entries,
		List<PendingReference> pendingReferences)
	{
		foreach (var directoryPath in _fileSystem.EnumerateDirectories(scanPath, "*", SearchOption.TopDirectoryOnly))
		{
			LoadRecursive(
				packageId,
				_fileSystem.LocalizePath(directoryPath),
				root,
				entries,
				pendingReferences);
		}

		foreach (var filePath in _fileSystem.EnumerateFiles(scanPath, "*.xml", SearchOption.TopDirectoryOnly))
		{
			var localizedPath = _fileSystem.LocalizePath(filePath);
			var name = _fileSystem.GetFileName(localizedPath);

			if (string.Equals(name, PackageAlias.Manifest, StringComparison.OrdinalIgnoreCase))
				continue;

			var key = BuildFallbackId(root, localizedPath);
			var def = _reader.ReadDefinition(packageId, localizedPath, key, pendingReferences);

			entries.Add(new DefinitionEntry(def.Id.Value, def, packageId, localizedPath));
		}
	}

	private static string BuildFallbackId(string root, string fullPath)
	{
		var key = fullPath;
		if (key.StartsWith(root + "/", StringComparison.OrdinalIgnoreCase))
			key = key[(root.Length + 1)..];

		key = key.Replace('\\', '/');

		if (key.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
			key = key[..^4];

		return key;
	}
}