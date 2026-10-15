namespace NovoDwarf.FS.Entries;

internal sealed class LocalFileSystemEntry : IFileSystemEntry
{
    private readonly FileSystemInfo _entry;

    internal LocalFileSystemEntry(FileSystemInfo entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        _entry = entry;
    }

    public string Name => _entry.Name;

    public string FullName => _entry.FullName;

    public string Extension => _entry.Extension;

    public bool Exists => _entry.Exists;

    public bool IsDirectory => _entry is DirectoryInfo;

    public FileAttributes Attributes => _entry.Attributes;

    public DateTime CreationTime => _entry.CreationTime;

    public DateTime CreationTimeUtc => _entry.CreationTimeUtc;

    public DateTime LastAccessTime => _entry.LastAccessTime;

    public DateTime LastAccessTimeUtc => _entry.LastAccessTimeUtc;

    public DateTime LastWriteTime => _entry.LastWriteTime;

    public DateTime LastWriteTimeUtc => _entry.LastWriteTimeUtc;

    public void Refresh()
    {
        _entry.Refresh();
    }
}
