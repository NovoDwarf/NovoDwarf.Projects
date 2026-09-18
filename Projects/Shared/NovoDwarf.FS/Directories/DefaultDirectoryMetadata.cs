using NovoDwarf.FS.Directories.Interfaces;

namespace NovoDwarf.FS.Directories;

/// <inheritdoc />
public sealed class DefaultDirectoryMetadata : IDirectoryMetadata
{
	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FileSystemInfo CreateSymbolicLink(string path, string pathToTarget) 
		=> Directory.CreateSymbolicLink(path, pathToTarget);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FileSystemInfo? ResolveLinkTarget(string path, bool returnFinalTarget) 
		=> Directory.ResolveLinkTarget(path, returnFinalTarget);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DateTime GetCreationTime(string path) => Directory.GetCreationTime(path);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DateTime GetCreationTimeUtc(string path) => Directory.GetCreationTimeUtc(path);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetCreationTime(string path, DateTime creationTime) => Directory.SetCreationTime(path, creationTime);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetCreationTimeUtc(string path, DateTime creationTimeUtc) => Directory.SetCreationTimeUtc(path, creationTimeUtc);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DateTime GetLastAccessTime(string path) => Directory.GetLastAccessTime(path);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DateTime GetLastAccessTimeUtc(string path) => Directory.GetLastAccessTimeUtc(path);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetLastAccessTime(string path, DateTime lastAccessTime) => Directory.SetLastAccessTime(path, lastAccessTime);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) => Directory.SetLastAccessTimeUtc(path, lastAccessTimeUtc);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DateTime GetLastWriteTime(string path) => Directory.GetLastWriteTime(path);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DateTime GetLastWriteTimeUtc(string path) => Directory.GetLastWriteTimeUtc(path);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetLastWriteTime(string path, DateTime lastWriteTime) => Directory.SetLastWriteTime(path, lastWriteTime);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) => Directory.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
}