namespace Aegis.Packaging.Assemblies.Utilities;

public static class ExceptionUtils
{
	public static void ThrowIfManyEntrypoints(string packageId, int counts, string names) =>
		throw new InvalidOperationException($"Package [{packageId}] must declare exactly one {nameof(IEntrypoint)} implementation, " + $"but found {counts}: {names}.");

	public static void ThrowIfNoEntrypoints(string packageId, string details) =>
		throw new InvalidOperationException($"Package [{packageId}] did not expose any loadable {nameof(IEntrypoint)} implementation. {details}");

	public static void ThrowIfResolveReservedAssembly(string packageId, string assemblyName) =>
		throw new InvalidOperationException(
			$"Package [{packageId}] attempted to resolve reserved host assembly [{assemblyName}] from package-local context. " +
			$"This assembly must be provided by the game.");

	public static void ThrowIfLoadingReservedAssembly(string packageId, string file, string path) =>
		throw new InvalidOperationException(
			$"Package [{packageId}] attempted to load reserved host assembly [{file}] from [{path}].");
}
