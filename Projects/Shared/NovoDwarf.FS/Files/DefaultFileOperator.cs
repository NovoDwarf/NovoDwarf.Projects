using NovoDwarf.FS.Files.Interfaces;

namespace NovoDwarf.FS.Files;

/// <inheritdoc cref="IFileOperator" />
public sealed class DefaultFileOperator : IFileOperator
{
    /// <inheritdoc />
    public void Copy(string sourceFileName, string destFileName) => File.Copy(sourceFileName, destFileName);

    /// <inheritdoc />
    public void Copy(string sourceFileName, string destFileName, bool overwrite)
        => File.Copy(sourceFileName, destFileName, overwrite);

    /// <inheritdoc />
    public void Move(string sourceFileName, string destFileName) => File.Move(sourceFileName, destFileName);

    /// <inheritdoc />
    public void Move(string sourceFileName, string destFileName, bool overwrite)
        => File.Move(sourceFileName, destFileName, overwrite);

    /// <inheritdoc />
    public void Delete(string path) => File.Delete(path);

    /// <inheritdoc />
    public void Replace(string sourceFileName, string destinationFileName, string? destinationBackupFileName)
        => File.Replace(sourceFileName, destinationFileName, destinationBackupFileName);

    /// <inheritdoc />
    public void Replace(string sourceFileName, string destinationFileName, string? destinationBackupFileName, bool ignoreMetadataErrors)
        => File.Replace(sourceFileName, destinationFileName, destinationBackupFileName, ignoreMetadataErrors);

    /// <inheritdoc />
    public FileSystemStream Create(string path) => new(File.Create(path));

    /// <inheritdoc />
    public FileSystemStream Create(string path, int bufferSize) => new(File.Create(path, bufferSize));

    /// <inheritdoc />
    public FileSystemStream Create(string path, int bufferSize, FileOptions options) => new(File.Create(path, bufferSize, options));
}
