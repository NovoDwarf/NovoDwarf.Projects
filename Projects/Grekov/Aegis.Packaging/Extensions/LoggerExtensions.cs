using Aegis.Packaging.Services;
using Microsoft.Extensions.Logging;

namespace Aegis.Packaging.Extensions;

public static partial class LoggerExtensions
{
	[LoggerMessage(LogLevel.Information, "Package reload started. Mode={Mode}, PackageId={PackageId}, CurrentCount={CurrentCount}")]
	public static partial void PackageReloadStarted(this ILogger<PackageService> logger, string mode, string packageId, int currentCount);

	[LoggerMessage(LogLevel.Information, "Package reload completed. Mode={Mode}, Loaded={Loaded}, Total={Total}, ElapsedMs={ElapsedMs}")]
	public static partial void PackageReloadCompleted(this ILogger<PackageService> logger, string mode, int loaded, int total, long elapsedMs);

	[LoggerMessage(LogLevel.Debug, "Package enabled override changed. PackageId={PackageId}, Enabled={Enabled}")]
	public static partial void PackageEnabledChanged(this ILogger<PackageService> logger, string packageId, bool enabled);

	[LoggerMessage(LogLevel.Information, "Loaded: [{Loaded}/{Total}]")]
	public static partial void TotalPackages(this ILogger<PackageService> logger, int loaded, int total);

	[LoggerMessage(LogLevel.Debug, "Package reload skipped: package fingerprints and load order are unchanged.")]
	public static partial void SkipReload(this ILogger<PackageService> logger);

	[LoggerMessage(LogLevel.Debug, "Built package load order. Candidates={CandidateCount}, Ordered={OrderedCount}")]
	public static partial void PackageLoadOrderBuilt(this ILogger<PackageLoadExecutionService> logger, int candidateCount, int orderedCount);

	[LoggerMessage(LogLevel.Debug, "Applying package reload. PreviousAffected={PreviousAffected}, NextAffected={NextAffected}")]
	public static partial void PackageReloadApplyStarted(this ILogger<PackageLoadExecutionService> logger, int previousAffected, int nextAffected);

	[LoggerMessage(LogLevel.Debug, "Package reload committed successfully. ElapsedMs={ElapsedMs}")]
	public static partial void PackageReloadCommitted(this ILogger<PackageLoadExecutionService> logger, long elapsedMs);

	[LoggerMessage(LogLevel.Debug, "Package loader started. Loader={Loader}, PackageId={PackageId}, Stage={Stage}")]
	public static partial void PackageLoaderStarted(this ILogger<PackageLoadExecutionService> logger, string loader, string packageId, string stage);

	[LoggerMessage(LogLevel.Debug, "Package loader completed. Loader={Loader}, PackageId={PackageId}, Stage={Stage}, ElapsedMs={ElapsedMs}")]
	public static partial void PackageLoaderCompleted(this ILogger<PackageLoadExecutionService> logger, string loader, string packageId, string stage, long elapsedMs);

	[LoggerMessage(LogLevel.Debug, "Package unload completed. Loader={Loader}, PackageId={PackageId}, Stage={Stage}")]
	public static partial void PackageLoaderUnloaded(this ILogger<PackageLoadExecutionService> logger, string loader, string packageId, string stage);

	[LoggerMessage(LogLevel.Error, "Package reload transaction failed. Rolling back affected packages.")]
	public static partial void PackageReloadTransactionFailed(this ILogger<PackageLoadExecutionService> logger, Exception exception);

	[LoggerMessage(LogLevel.Error, "Package reload rollback failed.")]
	public static partial void PackageReloadRollbackFailed(this ILogger<PackageLoadExecutionService> logger, Exception exception);
}
