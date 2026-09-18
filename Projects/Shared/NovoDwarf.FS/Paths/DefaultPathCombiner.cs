using NovoDwarf.FS.Paths.Interfaces;

namespace NovoDwarf.FS.Paths;

/// <inheritdoc />
public sealed class DefaultPathCombiner : IPathCombiner
{
	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string Combine(string path1, string path2) => Path.Combine(path1, path2);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string Combine(string path1, string path2, string path3) => Path.Combine(path1, path2, path3);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string Combine(string path1, string path2, string path3, string path4) => Path.Combine(path1, path2, path3, path4);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string Combine(params string[] paths) => Path.Combine(paths);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string Join(string? path1, string? path2) => Path.Join(path1, path2);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string Join(string? path1, string? path2, string? path3) => Path.Join(path1, path2, path3);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string Join(string? path1, string? path2, string? path3, string? path4) => Path.Join(path1, path2, path3, path4);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string Join(params string?[] paths) => Path.Join(paths);
}