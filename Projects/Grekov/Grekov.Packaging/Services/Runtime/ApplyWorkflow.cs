using Grekov.Packaging.Entities;
using Grekov.Packaging.Services.Conflicts;
using Grekov.Packaging.Services.Loading;

namespace Grekov.Packaging.Services.Runtime;

internal sealed class ApplyWorkflow
{
	private readonly PackageLoaderPipeline _loaderPipeline;
	private readonly PackageConflictRegistry _packageConflicts;

	public ApplyWorkflow(PackageLoaderPipeline loaderPipeline, PackageConflictRegistry packageConflicts)
	{
		_loaderPipeline = loaderPipeline;
		_packageConflicts = packageConflicts;
	}

	public void Apply(IReadOnlyList<PackageInstance> loadOrder)
	{
		_loaderPipeline.ClearAll(_packageConflicts);
		_loaderPipeline.BeginTransaction();

		try
		{
			_loaderPipeline.LoadBatch(loadOrder, _packageConflicts);
			_loaderPipeline.CommitTransaction();
		}
		catch
		{
			_loaderPipeline.RollbackTransaction();
			throw;
		}
	}
}
