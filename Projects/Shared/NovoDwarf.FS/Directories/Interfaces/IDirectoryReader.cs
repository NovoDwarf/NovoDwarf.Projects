namespace NovoDwarf.FS.Directories.Interfaces;

/// <summary>Inspects directories and enumerates directories or file system entries.</summary>
public interface IDirectoryReader
{
    /// <inheritdoc cref="Directory.Exists(string)" />
    public bool Exists(string? path);

    /// <summary>Returns a read-only metadata handle, including for a path that does not exist.</summary>
    public IFileSystemEntry GetInfo(string path);

    /// <inheritdoc cref="Directory.GetDirectories(string, string, SearchOption)" />
    public string[] GetDirectories(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly);

    /// <inheritdoc cref="Directory.EnumerateDirectories(string, string, SearchOption)" />
    public IEnumerable<string> EnumerateDirectories(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly);

    /// <inheritdoc cref="Directory.GetFileSystemEntries(string, string, SearchOption)" />
    public string[] GetFileSystemEntries(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly);

    /// <inheritdoc cref="Directory.EnumerateFileSystemEntries(string, string, SearchOption)" />
    public IEnumerable<string> EnumerateFileSystemEntries(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly);

    /// <inheritdoc cref="Directory.GetLogicalDrives()" />
    public string[] GetLogicalDrives();

    /// <inheritdoc cref="Directory.GetParent(string)" />
    public IFileSystemEntry? GetParent(string path);

    /// <inheritdoc cref="Directory.GetCurrentDirectory()" />
    public string GetCurrentDirectory();
}
