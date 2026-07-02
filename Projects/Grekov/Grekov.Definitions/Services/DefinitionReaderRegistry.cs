using Grekov.Definitions.Interfaces;
using NovoDwarf.FS.Interfaces;

namespace Grekov.Definitions.Services;

internal sealed class DefinitionReaderRegistry
{
	private readonly IReadOnlyList<IDefinitionFormatReader> _readers;
	private readonly Dictionary<string, IDefinitionFormatReader> _readersByExtension;
	private readonly IPathService _pathService;

	public DefinitionReaderRegistry(IEnumerable<IDefinitionFormatReader> readers, IPathService pathService)
	{
		_pathService = pathService;
		_readers = readers.ToArray();
		_readersByExtension = new Dictionary<string, IDefinitionFormatReader>(StringComparer.OrdinalIgnoreCase);

		foreach (var reader in _readers)
		foreach (var extension in reader.Extensions)
		{
			var normalized = NormalizeExtension(extension);
			if (!_readersByExtension.TryAdd(normalized, reader))
				throw new InvalidOperationException($"Definition reader for extension [{normalized}] is already registered.");
		}
	}

	public IReadOnlyCollection<string> Extensions => _readersByExtension.Keys;

	public void RefreshTypeMaps()
	{
		foreach (var reader in _readers)
			reader.RefreshTypeMap();
	}

	public bool TryResolve(string path, out IDefinitionFormatReader reader)
	{
		return _readersByExtension.TryGetValue(NormalizeExtension(_pathService.GetExtension(path)), out reader!);
	}

	private static string NormalizeExtension(string extension)
	{
		if (string.IsNullOrWhiteSpace(extension))
			throw new ArgumentException("Definition reader extension is empty.", nameof(extension));

		extension = extension.Trim();
		return extension[0] == '.' ? extension : $".{extension}";
	}
}
