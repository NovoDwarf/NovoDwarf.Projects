namespace Grekov.Packaging.Interfaces;

public interface IPackageConflictSink
{
	void Register(string key, string packageId);
}
