using Aegis.Packaging.Core.Enums;

namespace Aegis.Packaging.Core.Entities;

public sealed record PackageReloadResult(
	bool Succeeded,
	bool Changed,
	bool Skipped,
	string Mode,
	string PackageId,
	string? Reason,
	int LoadedCount,
	int TotalCount,
	long ElapsedMilliseconds,
	IReadOnlyList<string> AffectedPackageIds,
	IReadOnlyList<PackageIssue> Issues)
{
	public static PackageReloadResult NotRun(string mode, string packageId, string reason) => new(
		false,
		false,
		true,
		mode,
		packageId,
		reason,
		0,
		0,
		0,
		[],
		[]);

	public static PackageReloadResult FromState(
		bool succeeded,
		bool changed,
		bool skipped,
		string mode,
		string packageId,
		string? reason,
		IEnumerable<PackageInstance> packages,
		IEnumerable<string> affectedPackageIds,
		long elapsedMilliseconds)
	{
		var packageList = packages.ToArray();
		return new PackageReloadResult(
			succeeded,
			changed,
			skipped,
			mode,
			packageId,
			reason,
			packageList.Count(static package => package.State == PackageState.Loaded),
			packageList.Length,
			elapsedMilliseconds,
			affectedPackageIds.OrderBy(static id => id, StringComparer.OrdinalIgnoreCase).ToArray(),
			packageList.SelectMany(static package => package.Issues).ToArray());
	}
}
