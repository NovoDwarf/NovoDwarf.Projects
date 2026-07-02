using Grekov.Core;

namespace Grekov.Definitions.Interfaces;

public interface IDefinitionCatalog
{
	T? Get<T>(string id) where T : Def;

	T? GetByPath<T>(string? resourcePath) where T : Def;

	IEnumerable<T> All<T>() where T : Def;

	T? FirstOrDefault<T>() where T : Def;

	bool TryGetOrigin(Def def, out string packageId, out string resourcePath);
}
