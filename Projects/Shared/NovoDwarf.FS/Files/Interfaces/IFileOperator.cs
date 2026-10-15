namespace NovoDwarf.FS.Files.Interfaces;

/// <summary>Creates, copies, moves, replaces, and deletes files.</summary>
public interface IFileOperator
{
    /// <inheritdoc cref="File.Copy(string, string)" />
    public void Copy(string sourceFileName, string destFileName);

    /// <inheritdoc cref="File.Copy(string, string, bool)" />
    public void Copy(string sourceFileName, string destFileName, bool overwrite);

    /// <inheritdoc cref="File.Move(string, string)" />
    public void Move(string sourceFileName, string destFileName);

    /// <inheritdoc cref="File.Move(string, string, bool)" />
    public void Move(string sourceFileName, string destFileName, bool overwrite);

    /// <inheritdoc cref="File.Delete(string)" />
    public void Delete(string path);

    /// <inheritdoc cref="File.Replace(string, string, string)" />
    public void Replace(string sourceFileName, string destinationFileName, string? destinationBackupFileName);

    /// <inheritdoc cref="File.Replace(string, string, string, bool)" />
    public void Replace(
        string sourceFileName,
        string destinationFileName,
        string? destinationBackupFileName,
        bool ignoreMetadataErrors);

    /// <inheritdoc cref="File.Create(string)" />
    public FileSystemStream Create(string path);

    /// <inheritdoc cref="File.Create(string, int)" />
    public FileSystemStream Create(string path, int bufferSize);

    /// <inheritdoc cref="File.Create(string, int, FileOptions)" />
    public FileSystemStream Create(string path, int bufferSize, FileOptions options);
}
