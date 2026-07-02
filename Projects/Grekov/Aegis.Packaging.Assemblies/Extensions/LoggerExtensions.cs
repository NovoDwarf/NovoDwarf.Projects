using Microsoft.Extensions.Logging;

namespace Aegis.Packaging.Assemblies.Extensions;

public static partial class LoggerExtensions
{
	[LoggerMessage(LogLevel.Information, "Loaded package assembly {AssemblyPath} for package {PackageId} without mod entrypoints.")]
	public static partial void LoadedPackageWithoutEntrypoint(this ILogger<AssemblyService> logger, string assemblyPath, string packageId);

	[LoggerMessage(LogLevel.Warning, "Skipping mod entrypoint {Entrypoint} from {AssemblyPath}: no public parameterless constructor.")]
	public static partial void EntrypointHasNoPublicConstructor(this ILogger<AssemblyService> logger, string? entrypoint, string assemblyPath);

	[LoggerMessage(LogLevel.Information, "Loaded {Count} mod entrypoint(s) for package {PackageId} from {AssemblyPath}.")]
	public static partial void LoadedEntrypoint(this ILogger<AssemblyService> logger, int count, string packageId, string assemblyPath);

	[LoggerMessage(LogLevel.Warning, "Failed to load mod assembly dependency from {AssemblyPath}.")]
	public static partial void FailedToLoadAssemblyDependencies(this ILogger<AssemblyService> logger, string assemblyPath, Exception? exception);

	[LoggerMessage(LogLevel.Warning, "Failed to load mod assembly from {AssemblyPath}.")]
	public static partial void FailedToLoadAssembly(this ILogger<AssemblyService> logger, string assemblyPath, Exception exception);

	[LoggerMessage(LogLevel.Warning, "Failed to discover mod entrypoint for package {PackageId} from {AssemblyPath}: {Details}")]
	public static partial void FailedEntrypointDiscovery(this ILogger<AssemblyService> logger, string packageId, string assemblyPath, string details);
}

