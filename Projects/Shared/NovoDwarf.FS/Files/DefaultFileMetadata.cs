using NovoDwarf.FS.Files.Interfaces;

namespace NovoDwarf.FS.Files;

/// <inheritdoc cref="IFileMetadata" />
public sealed class DefaultFileMetadata : IFileMetadata
{
    /// <inheritdoc />
    public IFileSystemEntry GetInfo(string path) => new LocalFileSystemEntry(new FileInfo(path));

    /// <inheritdoc />
    public bool Exists(string path) => File.Exists(path);

    /// <inheritdoc />
    public FileAttributes GetAttributes(string path) => File.GetAttributes(path);

    /// <inheritdoc />
    public void SetAttributes(string path, FileAttributes fileAttributes) => File.SetAttributes(path, fileAttributes);

    /// <inheritdoc />
    public DateTime GetCreationTime(string path) => File.GetCreationTime(path);

    /// <inheritdoc />
    public DateTime GetCreationTimeUtc(string path) => File.GetCreationTimeUtc(path);

    /// <inheritdoc />
    public void SetCreationTime(string path, DateTime creationTime) => File.SetCreationTime(path, creationTime);

    /// <inheritdoc />
    public void SetCreationTimeUtc(string path, DateTime creationTimeUtc) => File.SetCreationTimeUtc(path, creationTimeUtc);

    /// <inheritdoc />
    public DateTime GetLastAccessTime(string path) => File.GetLastAccessTime(path);

    /// <inheritdoc />
    public DateTime GetLastAccessTimeUtc(string path) => File.GetLastAccessTimeUtc(path);

    /// <inheritdoc />
    public void SetLastAccessTime(string path, DateTime lastAccessTime) => File.SetLastAccessTime(path, lastAccessTime);

    /// <inheritdoc />
    public void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) => File.SetLastAccessTimeUtc(path, lastAccessTimeUtc);

    /// <inheritdoc />
    public DateTime GetLastWriteTime(string path) => File.GetLastWriteTime(path);

    /// <inheritdoc />
    public DateTime GetLastWriteTimeUtc(string path) => File.GetLastWriteTimeUtc(path);

    /// <inheritdoc />
    public void SetLastWriteTime(string path, DateTime lastWriteTime) => File.SetLastWriteTime(path, lastWriteTime);

    /// <inheritdoc />
    public void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) => File.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
}
