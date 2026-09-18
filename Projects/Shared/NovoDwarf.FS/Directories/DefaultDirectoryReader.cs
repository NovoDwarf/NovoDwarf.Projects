using NovoDwarf.FS.Directories.Interfaces;

namespace NovoDwarf.FS.Directories;

/// <inheritdoc />
public sealed class DefaultDirectoryReader : IDirectoryReader
{
	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Exists(string? path) => Directory.Exists(path);
	
	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)] 
	public DirectoryInfo? GetInfo(string path) => new(path);

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string[] GetDirectories(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly) 
	    => Directory.GetDirectories(path, searchPattern, searchOption);
	
	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)] 
    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly) 
	    => Directory.EnumerateDirectories(path, searchPattern, searchOption);

	/// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string[] GetFiles(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly) 
	    => Directory.GetFiles(path, searchPattern, searchOption);

	/// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<string> EnumerateFiles(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly) 
	    => Directory.EnumerateFiles(path, searchPattern, searchOption);

	/// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string[] GetFileSystemEntries(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly) 
	    => Directory.GetFileSystemEntries(path, searchPattern, searchOption);

	/// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly) 
	    => Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);

	/// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string[] GetLogicalDrives() => Directory.GetLogicalDrives();

	/// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DirectoryInfo? GetParent(string path) => Directory.GetParent(path);

	/// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetCurrentDirectory() => Directory.GetCurrentDirectory();
}