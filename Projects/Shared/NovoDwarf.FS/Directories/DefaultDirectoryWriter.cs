using NovoDwarf.FS.Directories.Interfaces;

namespace NovoDwarf.FS.Directories;

/// <inheritdoc />
public sealed class DefaultDirectoryWriter : IDirectoryWriter
{
	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DirectoryInfo CreateDirectory(string path) => Directory.CreateDirectory(path);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DirectoryInfo CreateSubdirectory(string path, string subdirectoryName) 
		=> new DirectoryInfo(path).CreateSubdirectory(subdirectoryName);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Delete(string path, bool recursive = false) => Directory.Delete(path, recursive);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Move(string sourceDirName, string destDirName) => Directory.Move(sourceDirName, destDirName);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetCurrentDirectory(string path) => Directory.SetCurrentDirectory(path);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DirectoryInfo CreateTempSubdirectory(string? prefix = null) => Directory.CreateTempSubdirectory(prefix);
}