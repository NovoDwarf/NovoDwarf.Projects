using Grekov.Core;
using Grekov.Definitions.Entities;
using Grekov.Definitions.Interfaces;
using Grekov.Packaging.Constants;
using Grekov.Packaging.Entities;
using Grekov.Packaging.Interfaces;

namespace Grekov.Definitions.Services;

internal sealed class DefService : IDefCatalog, IPackageContentLoader
{
	private readonly DefIndex _index;
	private readonly DefScanner _scanner;
	private readonly DefReaderRegistry _readers;

	public DefService(DefIndex index, DefScanner scanner, DefReaderRegistry readers)
	{
		_index = index;
		_scanner = scanner;
		_readers = readers;
	}

	public int Stage => PackageStages.Data;

	public void Clear()
	{
		_index.Clear();
	}

	public void LoadPackage(PackageLoadContext context)
	{
		_readers.RefreshTypeMaps();

		var pendingReferences = new List<DefPendingReference>();
		foreach (var (path, reader) in _scanner.Scan(context.Package))
		{
			var fallbackId = Path.GetFileNameWithoutExtension(path);
			var defs = reader.ReadDefs(new DefReadContext(context.Package.Id, path, fallbackId, pendingReferences));

			foreach (var def in defs)
			{
				context.Conflicts.Register($"def:{def.Id}", context.Package.Id);
				_index.Add(def);
			}
		}

		ResolvePendingReferences(pendingReferences);
	}

	public void UnloadPackage(string packageId)
	{
		_index.RemovePackage(packageId);
	}

	public T? Get<T>(string id) where T : Def
	{
		return _index.Get<T>(id);
	}

	public T? GetByPath<T>(string? resourcePath) where T : Def
	{
		return _index.GetByPath<T>(resourcePath);
	}

	public IEnumerable<T> All<T>() where T : Def
	{
		return _index.All<T>();
	}

	public T? FirstOrDefault<T>() where T : Def
	{
		return All<T>().FirstOrDefault();
	}

	public bool TryGetOrigin(Def def, out string packageId, out string resourcePath)
	{
		return _index.TryGetOrigin(def, out packageId, out resourcePath);
	}

	private void ResolvePendingReferences(IEnumerable<DefPendingReference> references)
	{
		foreach (var reference in references)
		{
			var def = _index.Get(reference.ExpectedType, reference.Id);
			if (def == null)
				throw new InvalidOperationException($"Definition reference '{reference.Id}' was not found for '{reference.ResourcePath}'.");

			reference.Apply(def);
		}
	}
}
