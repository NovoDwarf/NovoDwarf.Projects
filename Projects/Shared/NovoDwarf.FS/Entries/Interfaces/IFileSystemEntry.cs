namespace NovoDwarf.FS.Entries;

/// <summary>A read-only metadata handle for a file or directory, independent of its storage backend.</summary>
/// <remarks>
/// A handle may describe a missing entry. Metadata can be cached; call <see cref="Refresh" />
/// to observe changes. Properties follow the backend's rules for missing or inaccessible entries.
/// Mutating operations are available through the file and directory services.
/// </remarks>
public interface IFileSystemEntry
{
    /// <summary>Gets the entry name, without its parent path.</summary>
    public string Name { get; }

    /// <summary>Gets the absolute path of the entry.</summary>
    public string FullName { get; }

    /// <summary>Gets the extension, including the leading period, or an empty string.</summary>
    public string Extension { get; }

    /// <summary>Gets whether the entry exists.</summary>
    public bool Exists { get; }

    /// <summary>Gets whether this handle describes a directory, even if it does not exist.</summary>
    public bool IsDirectory { get; }

    /// <summary>Gets the entry attributes.</summary>
    public FileAttributes Attributes { get; }

    /// <summary>Gets the creation time in local time.</summary>
    public DateTime CreationTime { get; }

    /// <summary>Gets the creation time in UTC.</summary>
    public DateTime CreationTimeUtc { get; }

    /// <summary>Gets the last access time in local time.</summary>
    public DateTime LastAccessTime { get; }

    /// <summary>Gets the last access time in UTC.</summary>
    public DateTime LastAccessTimeUtc { get; }

    /// <summary>Gets the last write time in local time.</summary>
    public DateTime LastWriteTime { get; }

    /// <summary>Gets the last write time in UTC.</summary>
    public  DateTime LastWriteTimeUtc { get; }

    /// <summary>Reloads cached metadata from the backend without modifying the entry.</summary>
    public void Refresh();
}
