namespace Aegis.Packaging.Core.Interfaces;

public interface IPackageLoadOrderStore
{
	public IReadOnlyDictionary<string, int> BuildIndexMap();
}