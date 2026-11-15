using NovoDwarf.FS.Entries.Interfaces;

namespace NovoDwarf.FS.Files.Interfaces;

/// <summary>Reads and changes file metadata.</summary>
public interface IFileMetadata
{
    /// <summary>Returns a read-only metadata handle, including for a path that does not exist.</summary>
    public IFileSystemEntry GetInfo(string path);

    /// <inheritdoc cref="File.Exists(string)" />
    public bool Exists(string path);

    /// <inheritdoc cref="File.GetAttributes(string)" />
    public FileAttributes GetAttributes(string path);

    /// <inheritdoc cref="File.SetAttributes(string, FileAttributes)" />
    public void SetAttributes(string path, FileAttributes fileAttributes);

    /// <inheritdoc cref="File.GetCreationTime(string)" />
    public DateTime GetCreationTime(string path);

    /// <inheritdoc cref="File.GetCreationTimeUtc(string)" />
    public DateTime GetCreationTimeUtc(string path);

    /// <inheritdoc cref="File.SetCreationTime(string, DateTime)" />
    public void SetCreationTime(string path, DateTime creationTime);

    /// <inheritdoc cref="File.SetCreationTimeUtc(string, DateTime)" />
    public void SetCreationTimeUtc(string path, DateTime creationTimeUtc);

    /// <inheritdoc cref="File.GetLastAccessTime(string)" />
    public DateTime GetLastAccessTime(string path);

    /// <inheritdoc cref="File.GetLastAccessTimeUtc(string)" />
    public DateTime GetLastAccessTimeUtc(string path);

    /// <inheritdoc cref="File.SetLastAccessTime(string, DateTime)" />
    public void SetLastAccessTime(string path, DateTime lastAccessTime);

    /// <inheritdoc cref="File.SetLastAccessTimeUtc(string, DateTime)" />
    public void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc);

    /// <inheritdoc cref="File.GetLastWriteTime(string)" />
    public DateTime GetLastWriteTime(string path);

    /// <inheritdoc cref="File.GetLastWriteTimeUtc(string)" />
    public DateTime GetLastWriteTimeUtc(string path);

    /// <inheritdoc cref="File.SetLastWriteTime(string, DateTime)" />
    public void SetLastWriteTime(string path, DateTime lastWriteTime);

    /// <inheritdoc cref="File.SetLastWriteTimeUtc(string, DateTime)" />
    public void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc);
}
