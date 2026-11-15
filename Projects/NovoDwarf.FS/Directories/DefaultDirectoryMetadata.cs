using NovoDwarf.FS.Directories.Interfaces;

namespace NovoDwarf.FS.Directories;

/// <inheritdoc cref="IDirectoryMetadata" />
public sealed class DefaultDirectoryMetadata : IDirectoryMetadata
{
    /// <inheritdoc />
    public DateTime GetCreationTime(string path) => Directory.GetCreationTime(path);

    /// <inheritdoc />
    public DateTime GetCreationTimeUtc(string path) => Directory.GetCreationTimeUtc(path);

    /// <inheritdoc />
    public void SetCreationTime(string path, DateTime creationTime) => Directory.SetCreationTime(path, creationTime);

    /// <inheritdoc />
    public void SetCreationTimeUtc(string path, DateTime creationTimeUtc) => Directory.SetCreationTimeUtc(path, creationTimeUtc);

    /// <inheritdoc />
    public DateTime GetLastAccessTime(string path) => Directory.GetLastAccessTime(path);

    /// <inheritdoc />
    public DateTime GetLastAccessTimeUtc(string path) => Directory.GetLastAccessTimeUtc(path);

    /// <inheritdoc />
    public void SetLastAccessTime(string path, DateTime lastAccessTime) => Directory.SetLastAccessTime(path, lastAccessTime);

    /// <inheritdoc />
    public void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) => Directory.SetLastAccessTimeUtc(path, lastAccessTimeUtc);

    /// <inheritdoc />
    public DateTime GetLastWriteTime(string path) => Directory.GetLastWriteTime(path);

    /// <inheritdoc />
    public DateTime GetLastWriteTimeUtc(string path) => Directory.GetLastWriteTimeUtc(path);

    /// <inheritdoc />
    public void SetLastWriteTime(string path, DateTime lastWriteTime) => Directory.SetLastWriteTime(path, lastWriteTime);

    /// <inheritdoc />
    public void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) => Directory.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
}
