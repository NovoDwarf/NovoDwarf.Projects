using NovoDwarf.FS.Directories.Interfaces;
using NovoDwarf.FS.Entries.Interfaces;

namespace NovoDwarf.FS.Directories;

/// <inheritdoc cref="IDirectoryReader" />
public sealed class DefaultDirectoryReader : IDirectoryReader
{
    /// <inheritdoc />
    public bool Exists(string? path) => Directory.Exists(path);

    /// <inheritdoc />
    public IFileSystemEntry GetInfo(string path) => new LocalFileSystemEntry(new DirectoryInfo(path));

    /// <inheritdoc />
    public string[] GetDirectories(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        => Directory.GetDirectories(path, searchPattern, searchOption);

    /// <inheritdoc />
    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        => Directory.EnumerateDirectories(path, searchPattern, searchOption);

    /// <inheritdoc />
    public string[] GetFileSystemEntries(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        => Directory.GetFileSystemEntries(path, searchPattern, searchOption);

    /// <inheritdoc />
    public IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        => Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);

    /// <inheritdoc />
    public string[] GetLogicalDrives() => Directory.GetLogicalDrives();

    /// <inheritdoc />
    public IFileSystemEntry? GetParent(string path)
    {
        var parent = Directory.GetParent(path);
        return parent is null ? null : new LocalFileSystemEntry(parent);
    }

    /// <inheritdoc />
    public string GetCurrentDirectory() => Directory.GetCurrentDirectory();
}
