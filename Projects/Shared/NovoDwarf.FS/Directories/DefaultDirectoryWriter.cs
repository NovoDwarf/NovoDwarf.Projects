using NovoDwarf.FS.Directories.Interfaces;

namespace NovoDwarf.FS.Directories;

/// <inheritdoc cref="IDirectoryWriter" />
public sealed class DefaultDirectoryWriter : IDirectoryWriter
{
    /// <inheritdoc />
    public IFileSystemEntry CreateDirectory(string path) => new LocalFileSystemEntry(Directory.CreateDirectory(path));

    /// <inheritdoc />
    public IFileSystemEntry CreateSubdirectory(string path, string subdirectoryName) => new LocalFileSystemEntry(new DirectoryInfo(path).CreateSubdirectory(subdirectoryName));

    /// <inheritdoc />
    public void Delete(string path, bool recursive = false) => Directory.Delete(path, recursive);

    /// <inheritdoc />
    public void Move(string sourceDirName, string destDirName) => Directory.Move(sourceDirName, destDirName);

    /// <inheritdoc />
    public void SetCurrentDirectory(string path) => Directory.SetCurrentDirectory(path);

    /// <inheritdoc />
    public IFileSystemEntry CreateTempSubdirectory(string? prefix = null) => new LocalFileSystemEntry(Directory.CreateTempSubdirectory(prefix));
}
