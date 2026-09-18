using NovoDwarf.FS.Paths.Interfaces;

namespace NovoDwarf.FS.Paths;

public sealed class DefaultPathResolver : IPathResolver
{	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string GetFullPath(string path) => Path.GetFullPath(path);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string GetFullPath(string path, string basePath) => Path.GetFullPath(path, basePath);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string GetRelativePath(string relativeTo, string path) => Path.GetRelativePath(relativeTo, path);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string? GetPathRoot(string? path) => Path.GetPathRoot(path);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsPathFullyQualified(string path) => Path.IsPathFullyQualified(path);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsPathRooted(string? path) => Path.IsPathRooted(path);
}