using NovoDwarf.FS.Paths.Interfaces;

namespace NovoDwarf.FS.Paths;

/// <inheritdoc />
public sealed class DefaultPathGenerator : IPathGenerator
{
	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string GetRandomFileName() => Path.GetRandomFileName();

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string GetTempFileName() => Path.GetTempFileName();

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string GetTempPath() => Path.GetTempPath();
}