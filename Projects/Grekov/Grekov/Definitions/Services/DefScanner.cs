using Grekov.Definitions.Interfaces;
using Grekov.Packaging.Entities;
using NovoDwarf.FS.Interfaces;

namespace Grekov.Definitions.Services;

internal sealed class DefScanner
{
	private readonly DefReaderRegistry _readers;
	private readonly IFileSystemService _fileSystem;

	public DefScanner(DefReaderRegistry readers, IFileSystemService fileSystem)
	{
		_readers = readers;
		_fileSystem = fileSystem;
	}

	public IReadOnlyList<(string Path, IDefFormatReader Reader)> Scan(PackageInstance package)
	{
		if (!_fileSystem.DirectoryExists(package.DefinitionsPath))
			return [];

		return _fileSystem.EnumerateFiles(package.DefinitionsPath, "*.*", SearchOption.AllDirectories)
			.Select(path => _readers.TryResolve(path, out var reader) ? (Path: path, Reader: reader) : default)
			.Where(static item => item.Reader != null)
			.ToArray()!;
	}
}
