using NovoDwarf.FS.Paths.Interfaces;

namespace NovoDwarf.FS.Paths;

/// <inheritdoc />
public sealed class DefaultPathParser : IPathParser
{
	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string? GetDirectoryName(string? path) => Path.GetDirectoryName(path);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string? GetFileName(string? path) => Path.GetFileName(path);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string? GetFileNameWithoutExtension(string? path) => Path.GetFileNameWithoutExtension(path);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string? GetExtension(string? path) => Path.GetExtension(path);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string? ChangeExtension(string? path, string? extension) => Path.ChangeExtension(path, extension);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasExtension(string? path) => Path.HasExtension(path);
}